using System.Collections.Generic;
using UnityEngine;

// Converts city buildings into runtime-spawned chunk cubes the first time they are hit.
// The laser then carves those cubes via VoxelCarvable instead of deleting entire building objects.
public class RuntimeBuildingChunker : MonoBehaviour
{
    private static readonly string[] DefaultExcludedNamePrefixes = { "Floor", "PolyShape", "CAR", "TREE", "RuntimeChunks" };

    [SerializeField] private string[]      _excludedNamePrefixes = { "Floor", "PolyShape", "CAR", "TREE", "RuntimeChunks" }; // Name-based exclusion fallback.
    [SerializeField] private Transform     _cityRoot;                                     // Root whose direct children are treated as buildings; defaults to this transform.
    [SerializeField] private VoxelCarvable _voxelCarvable;                                // Carve system cache; spawned chunk colliders are registered here.
    [SerializeField] private Transform[]   _excludedRoots;                                // Explicit roots that are never chunked (roads, cars, trees, etc).
    [SerializeField] private string        _runtimeChunksContainerName = "RuntimeChunks"; // Parent object name for runtime chunk containers.
    [SerializeField] private float         _chunkSize = 1.25f;                            // Target chunk cube edge length in world units before budget scaling.
    [SerializeField] private int           _maxChunksPerBuilding = 2000;                  // Per-building chunk cap; <= 0 disables cap.
    [SerializeField] private bool          _autoScaleChunkSizeToChunkBudget = true;       // Grows chunk size if needed to stay within max chunk budget.
    [SerializeField] private bool          _fillCombinedBounds = true;                    // True: fill full combined bounds volume. False: surface-style occupancy fill.
    [SerializeField] private float         _occupancyPadding = 0.2f;                      // Surface-fill proximity padding when testing source collider occupancy.
    [SerializeField] private bool          _disableOriginalBuildingRoot = true;           // Disables original building after successful chunk replacement.

    // Tracks which top-level building roots have already been converted once.
    private readonly HashSet<int> _chunkedBuildingIds = new HashSet<int>();
    private Transform _runtimeChunksRoot;
    private static Mesh _sharedCubeMesh;
    private static Material _sharedDefaultMaterial;

    private Transform CityRoot => _cityRoot ? _cityRoot : transform;

    private void Awake()
    {
        // Ensure exclusion list remains safe if prefab data is missing/cleared.
        if (_excludedNamePrefixes == null || _excludedNamePrefixes.Length == 0)
        {
            _excludedNamePrefixes = (string[])DefaultExcludedNamePrefixes.Clone();
        }

        // Auto-wire local carvable if not explicitly assigned in inspector.
        if (_voxelCarvable == null)
        {
            _voxelCarvable = GetComponent<VoxelCarvable>();
        }
    }

    // Guarantees the hit building is chunked and returns a chunk when available.
    // Returns true when the hit is already chunk geometry or chunking succeeded.
    public bool EnsureChunkedForHit(Collider hitCollider, out DestructibleChunk nearestChunk)
    {
        nearestChunk = null;
        if (!hitCollider)
        {
            return false;
        }

        // Already chunked geometry can be carved immediately.
        if (hitCollider.TryGetComponent(out nearestChunk))
        {
            return true;
        }

        // Resolve to a top-level building unit and avoid re-chunking.
        Transform buildingRoot = ResolveBuildingRoot(hitCollider.transform);
        if (!buildingRoot)
        {
            return false;
        }

        int buildingId = buildingRoot.GetInstanceID();
        if (_chunkedBuildingIds.Contains(buildingId))
        {
            return false;
        }

        // Build replacement chunk geometry and remember at least one spawned chunk.
        int spawnedChunks = ChunkBuilding(buildingRoot, out nearestChunk);
        if (spawnedChunks <= 0)
        {
            return false;
        }

        _chunkedBuildingIds.Add(buildingId);
        if (_disableOriginalBuildingRoot)
        {
            // The original render/collider stays out of gameplay after conversion.
            buildingRoot.gameObject.SetActive(false);
        }

        return true;
    }

    // Generates chunk cubes for one building and returns the number spawned.
    private int ChunkBuilding(Transform buildingRoot, out DestructibleChunk nearestChunk)
    {
        nearestChunk = null;

        // Source colliders define occupancy and fallback bounds.
        Collider[] sourceColliders = buildingRoot.GetComponentsInChildren<Collider>(true);
        if (sourceColliders == null || sourceColliders.Length == 0)
        {
            return 0;
        }

        Bounds combinedBounds;
        Material sourceMaterial;
        if (!TryBuildBounds(buildingRoot, sourceColliders, out combinedBounds, out sourceMaterial))
        {
            return 0;
        }

        Transform chunkContainer = CreateChunkContainerFor(buildingRoot.name);
        float chunkSize = Mathf.Max(0.25f, _chunkSize);
        int maxChunks = _maxChunksPerBuilding <= 0 ? int.MaxValue : Mathf.Max(1, _maxChunksPerBuilding);
        int layer = buildingRoot.gameObject.layer;

        // Build a regular 3D grid over the building bounds.
        Vector3 min = combinedBounds.min;
        Vector3 max = combinedBounds.max;
        int xCount = Mathf.Max(1, Mathf.CeilToInt((max.x - min.x) / chunkSize));
        int yCount = Mathf.Max(1, Mathf.CeilToInt((max.y - min.y) / chunkSize));
        int zCount = Mathf.Max(1, Mathf.CeilToInt((max.z - min.z) / chunkSize));

        if (_autoScaleChunkSizeToChunkBudget && maxChunks != int.MaxValue)
        {
            // If this building would exceed the chunk budget, increase chunk size
            // so we still cover full volume instead of stopping after a thin slice.
            float estimatedCells = (float)xCount * yCount * zCount;
            if (estimatedCells > maxChunks)
            {
                float scaleUp = Mathf.Pow(estimatedCells / maxChunks, 1f / 3f);
                chunkSize *= scaleUp;
                xCount = Mathf.Max(1, Mathf.CeilToInt((max.x - min.x) / chunkSize));
                yCount = Mathf.Max(1, Mathf.CeilToInt((max.y - min.y) / chunkSize));
                zCount = Mathf.Max(1, Mathf.CeilToInt((max.z - min.z) / chunkSize));
            }
        }

        float halfChunkSize = chunkSize * 0.5f;
        float occupancyDistance = halfChunkSize + Mathf.Max(0f, _occupancyPadding);

        int chunkIndex = 0;
        // Iterate each potential grid cell and spawn one cube per accepted cell.
        for (int x = 0; x < xCount; x++)
        {
            float worldX = min.x + halfChunkSize + x * chunkSize;
            for (int y = 0; y < yCount; y++)
            {
                float worldY = min.y + halfChunkSize + y * chunkSize;
                for (int z = 0; z < zCount; z++)
                {
                    if (chunkIndex >= maxChunks)
                    {
                        return chunkIndex;
                    }

                    float worldZ = min.z + halfChunkSize + z * chunkSize;
                    Vector3 chunkCenter = new Vector3(worldX, worldY, worldZ);
                    // Fill mode uses the whole combined bounds volume.
                    // Surface mode keeps only cells close to source colliders.
                    if (!_fillCombinedBounds && !IsCellOccupied(chunkCenter, occupancyDistance, sourceColliders))
                    {
                        continue;
                    }

                    // Spawn a chunk cube and track the first one for caller convenience.
                    DestructibleChunk chunk = SpawnChunk(chunkContainer, chunkCenter, chunkSize, sourceMaterial, layer, chunkIndex);
                    if (chunk == null)
                    {
                        continue;
                    }

                    if (nearestChunk == null)
                    {
                        nearestChunk = chunk;
                    }

                    chunkIndex++;
                }
            }
        }

        return chunkIndex;
    }

    // Computes aggregate bounds and chooses a representative material for chunk meshes.
    // Falls back to collider bounds and default material when renderer data is unavailable.
    private bool TryBuildBounds(Transform buildingRoot, Collider[] sourceColliders, out Bounds combinedBounds, out Material sourceMaterial)
    {
        sourceMaterial = null;
        combinedBounds = default;
        bool hasBounds = false;

        // Prefer renderer bounds/materials for visual parity in spawned chunks.
        Renderer[] renderers = buildingRoot.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < renderers.Length; i++)
        {
            Renderer renderer = renderers[i];
            if (!renderer || !renderer.enabled)
            {
                continue;
            }

            if (!hasBounds)
            {
                combinedBounds = renderer.bounds;
                hasBounds = true;
            }
            else
            {
                combinedBounds.Encapsulate(renderer.bounds);
            }

            if (sourceMaterial == null)
            {
                sourceMaterial = renderer.sharedMaterial;
            }
        }

        if (!hasBounds)
        {
            // Renderer-less building data can still be chunked from collider extents.
            for (int i = 0; i < sourceColliders.Length; i++)
            {
                Collider sourceCollider = sourceColliders[i];
                if (!sourceCollider || !sourceCollider.enabled)
                {
                    continue;
                }

                if (!hasBounds)
                {
                    combinedBounds = sourceCollider.bounds;
                    hasBounds = true;
                }
                else
                {
                    combinedBounds.Encapsulate(sourceCollider.bounds);
                }
            }
        }

        if (sourceMaterial == null)
        {
            // Keep chunks visible even when source renderer material is absent.
            sourceMaterial = GetDefaultMaterial();
        }

        return hasBounds;
    }

    // Surface occupancy test: checks whether a cell center is near any source collider.
    private bool IsCellOccupied(Vector3 worldPosition, float occupancyDistance, Collider[] sourceColliders)
    {
        float occupancyDistanceSqr = occupancyDistance * occupancyDistance;
        for (int i = 0; i < sourceColliders.Length; i++)
        {
            Collider sourceCollider = sourceColliders[i];
            if (!sourceCollider || !sourceCollider.enabled)
            {
                continue;
            }

            if (sourceCollider.bounds.SqrDistance(worldPosition) > occupancyDistanceSqr)
            {
                continue;
            }

            Vector3 closestPoint = sourceCollider.ClosestPoint(worldPosition);
            if ((closestPoint - worldPosition).sqrMagnitude <= occupancyDistanceSqr)
            {
                return true;
            }
        }

        return false;
    }

    // Creates one chunk GameObject at the given world position and wires render/collision state.
    private DestructibleChunk SpawnChunk(Transform parent, Vector3 worldPosition, float chunkSize, Material sourceMaterial, int layer, int index)
    {
        Mesh cubeMesh = GetSharedCubeMesh();
        if (!cubeMesh)
        {
            return null;
        }

        GameObject chunkObject = new GameObject("Chunk_" + index);
        chunkObject.layer = layer;
        Transform chunkTransform = chunkObject.transform;
        chunkTransform.SetParent(parent, true);
        chunkTransform.position = worldPosition;
        chunkTransform.rotation = Quaternion.identity;
        chunkTransform.localScale = Vector3.one * chunkSize;

        MeshFilter meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = cubeMesh;

        MeshRenderer meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = sourceMaterial;

        BoxCollider boxCollider = chunkObject.AddComponent<BoxCollider>();
        DestructibleChunk destructibleChunk = chunkObject.AddComponent<DestructibleChunk>();

        if (_voxelCarvable != null)
        {
            // Register immediately so freshly spawned chunks are carveable this frame.
            _voxelCarvable.RegisterChunk(boxCollider);
        }

        return destructibleChunk;
    }

    // Returns/creates a per-building child container under RuntimeChunks.
    private Transform CreateChunkContainerFor(string buildingName)
    {
        Transform runtimeRoot = GetOrCreateRuntimeChunksRoot();
        string containerName = string.IsNullOrEmpty(buildingName) ? "BuildingChunks" : buildingName + "_Chunks";
        Transform container = runtimeRoot.Find(containerName);
        if (!container)
        {
            GameObject containerObject = new GameObject(containerName);
            container = containerObject.transform;
            container.SetParent(runtimeRoot, false);
        }

        return container;
    }

    // Returns/creates the global runtime chunk parent under city root.
    private Transform GetOrCreateRuntimeChunksRoot()
    {
        if (_runtimeChunksRoot)
        {
            return _runtimeChunksRoot;
        }

        Transform cityRoot = CityRoot;
        string rootName = string.IsNullOrEmpty(_runtimeChunksContainerName) ? "RuntimeChunks" : _runtimeChunksContainerName;
        Transform existing = cityRoot.Find(rootName);
        if (existing)
        {
            _runtimeChunksRoot = existing;
            return _runtimeChunksRoot;
        }

        GameObject rootObject = new GameObject(rootName);
        _runtimeChunksRoot = rootObject.transform;
        _runtimeChunksRoot.SetParent(cityRoot, false);
        _runtimeChunksRoot.localPosition = Vector3.zero;
        _runtimeChunksRoot.localRotation = Quaternion.identity;
        _runtimeChunksRoot.localScale = Vector3.one;
        return _runtimeChunksRoot;
    }

    // Converts an arbitrary hit transform into a direct city child building root.
    private Transform ResolveBuildingRoot(Transform hitTransform)
    {
        if (!hitTransform)
        {
            return null;
        }

        Transform cityRoot = CityRoot;
        if (!cityRoot)
        {
            return null;
        }

        if (hitTransform != cityRoot && !hitTransform.IsChildOf(cityRoot))
        {
            return null;
        }

        // Resolve to a direct child of city root; this is our building unit.
        Transform current = hitTransform;
        while (current && current != cityRoot && current.parent != cityRoot)
        {
            current = current.parent;
        }

        if (!current || current == cityRoot)
        {
            return null;
        }

        // Skip excluded groups even when a child collider was hit.
        if (IsExcluded(current))
        {
            return null;
        }

        return current;
    }

    // Checks both name-based and explicit-root exclusions.
    private bool IsExcluded(Transform t)
    {
        if (!t)
        {
            return true;
        }

        if (IsExcludedByName(t.name))
        {
            return true;
        }

        if (_excludedRoots == null || _excludedRoots.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < _excludedRoots.Length; i++)
        {
            Transform excludedRoot = _excludedRoots[i];
            if (!excludedRoot)
            {
                continue;
            }

            if (t == excludedRoot || t.IsChildOf(excludedRoot))
            {
                return true;
            }
        }

        return false;
    }

    // Prefix match keeps exclusion setup simple and resilient to generated suffixes "(1)", etc.
    private bool IsExcludedByName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName) || _excludedNamePrefixes == null || _excludedNamePrefixes.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < _excludedNamePrefixes.Length; i++)
        {
            string prefix = _excludedNamePrefixes[i];
            if (string.IsNullOrEmpty(prefix))
            {
                continue;
            }

            if (objectName.StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    // Lazily caches Unity's built-in cube mesh for all runtime chunks.
    private static Mesh GetSharedCubeMesh()
    {
        if (_sharedCubeMesh)
        {
            return _sharedCubeMesh;
        }

        GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
        MeshFilter meshFilter = primitive.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            _sharedCubeMesh = meshFilter.sharedMesh;
        }

        if (Application.isPlaying)
        {
            Destroy(primitive);
        }
        else
        {
            DestroyImmediate(primitive);
        }

        return _sharedCubeMesh;
    }

    // Provides a shared fallback material when source renderers do not expose one.
    private static Material GetDefaultMaterial()
    {
        if (_sharedDefaultMaterial)
        {
            return _sharedDefaultMaterial;
        }

        Shader shader = Shader.Find("Standard");
        if (!shader)
        {
            return null;
        }

        _sharedDefaultMaterial = new Material(shader);
        return _sharedDefaultMaterial;
    }
}
