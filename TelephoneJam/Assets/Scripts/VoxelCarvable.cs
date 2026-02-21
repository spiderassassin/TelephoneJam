using System.Collections.Generic;
using UnityEngine;

// Maintains a cache of carveable chunk colliders and removes nearby chunks when laser carving occurs.
// This class intentionally targets DestructibleChunk instances only, so non-building geometry is ignored.
public class VoxelCarvable : MonoBehaviour
{
    private static readonly string[] DefaultExcludedNamePrefixes = { "Floor", "PolyShape", "CAR", "TREE" };

    [SerializeField] private string[]    _excludedNamePrefixes = { "Floor", "PolyShape", "CAR", "TREE" };      // Prefix-based exclusion fallback for known non-target groups.
    [SerializeField] private Transform   _carvableRoot;                        // Root searched for carve targets; defaults to this transform.
    [SerializeField] private Transform[] _excludedRoots;                       // Explicit roots excluded from carving.
    [SerializeField] private int         _maxBlocksPerCarve = 1;               // Maximum chunk removals per carve call; <= 0 means unlimited.
    [SerializeField] private float       _minTimeBetweenCarves = 0.05f;        // Global carve cooldown to prevent over-removal per frame.

    private readonly List<Collider>  _targetColliders = new List<Collider>();  // Parallel list of registered chunk colliders.
    private readonly List<GameObject> _targetObjects = new List<GameObject>(); // Parallel list of collider GameObjects.
    private readonly HashSet<int>    _targetColliderIds = new HashSet<int>();  // Fast de-duplication by collider instance id.
    private float _nextCarveTime;                                              // Earliest Time.time at which carving is allowed again.

    private Transform CarvableRoot => _carvableRoot ? _carvableRoot : transform;

    private void Awake()
    {
        // Ensure exclusion defaults remain safe if prefab serialization is missing.
        if (_excludedNamePrefixes == null || _excludedNamePrefixes.Length == 0)
        {
            _excludedNamePrefixes = (string[])DefaultExcludedNamePrefixes.Clone();
        }

        // Build the initial carve target cache from scene/prefab content.
        RebuildCache();
    }

    [ContextMenu("Rebuild Carve Cache")]
    public void RebuildCache()
    {
        // Full cache rebuild is used for initialization and manual debugging.
        _targetColliders.Clear();
        _targetObjects.Clear();
        _targetColliderIds.Clear();

        Transform root = CarvableRoot;
        if (!root)
        {
            return;
        }

        var seenObjectIds = new HashSet<int>();
        Collider[] colliders = root.GetComponentsInChildren<Collider>(true);
        foreach (Collider col in colliders)
        {
            if (!col)
            {
                continue;
            }

            Transform colTransform = col.transform;
            if (IsExcluded(colTransform))
            {
                continue;
            }

            GameObject go = col.gameObject;
            if (!go.GetComponent<Renderer>())
            {
                continue;
            }

            if (!col.GetComponent<DestructibleChunk>())
            {
                continue;
            }

            int objectId = go.GetInstanceID();
            if (!seenObjectIds.Add(objectId))
            {
                continue;
            }

            _targetColliders.Add(col);
            _targetObjects.Add(go);
            _targetColliderIds.Add(col.GetInstanceID());
        }
    }

    // Adds one runtime-spawned chunk collider to the carve cache without full rebuild.
    public bool RegisterChunk(Collider chunkCollider)
    {
        if (!chunkCollider)
        {
            return false;
        }

        if (!chunkCollider.GetComponent<DestructibleChunk>())
        {
            return false;
        }

        int colliderId = chunkCollider.GetInstanceID();
        if (!_targetColliderIds.Add(colliderId))
        {
            return false;
        }

        _targetColliders.Add(chunkCollider);
        _targetObjects.Add(chunkCollider.gameObject);
        return true;
    }

    // Rate-limited carve entry point used by gameplay systems.
    public int CarveSphere(Vector3 worldPosition, float radius)
    {
        if (radius <= 0f)
        {
            return 0;
        }

        if (Time.time < _nextCarveTime)
        {
            return 0;
        }

        _nextCarveTime = Time.time + Mathf.Max(0f, _minTimeBetweenCarves);
        return CarveSphereImmediate(worldPosition, radius);
    }

    // Immediate carve pass. Finds nearest valid chunk in radius and disables up to max removals.
    public int CarveSphereImmediate(Vector3 worldPosition, float radius)
    {
        if (radius <= 0f)
        {
            return 0;
        }

        // Work entirely in squared distances to avoid repeated square roots.
        float radiusSqr = radius * radius;
        int maxRemovals = _maxBlocksPerCarve <= 0 // Optional multi-remove mode: values <= 0 are treated as unlimited.
                        ? int.MaxValue : _maxBlocksPerCarve;
        int removedCount = 0;

        // Guard against list size drift; we only iterate the valid parallel range.
        int targetCount = Mathf.Min(_targetColliders.Count, _targetObjects.Count);
        int closestTargetIndex = -1;
        float closestTargetDistanceSqr = float.MaxValue;

        // First pass: find the closest active chunk within carve radius.
        for (int i = 0; i < targetCount; i++)
        {
            GameObject go = _targetObjects[i];
            Collider col = _targetColliders[i];
            if (!go || !go.activeSelf || !col || !col.enabled)
            {
                continue;
            }

            Vector3 closestPoint = col.ClosestPoint(worldPosition);
            // Fast broad-phase reject using collider bounds first.
            if (col.bounds.SqrDistance(worldPosition) > radiusSqr)
            {
                continue;
            }

            // Narrow-phase reject against collider surface distance.
            if ((closestPoint - worldPosition).sqrMagnitude > radiusSqr)
            {
                continue;
            }

            // Keep only the nearest candidate so carve feels local/precise.
            float candidateDistanceSqr = (closestPoint - worldPosition).sqrMagnitude;
            if (candidateDistanceSqr < closestTargetDistanceSqr)
            {
                closestTargetDistanceSqr = candidateDistanceSqr;
                closestTargetIndex = i;
            }
        }

        // No valid targets in radius.
        if (closestTargetIndex < 0)
        {
            return 0;
        }

        // Remove the nearest target, then optionally continue with next-nearest targets.
        int removalsRemaining = maxRemovals;
        while (removalsRemaining > 0 && closestTargetIndex >= 0)
        {
            GameObject targetObject = _targetObjects[closestTargetIndex];
            if (targetObject && targetObject.activeSelf)
            {
                // Deactivating preserves object data and avoids immediate destroy churn.
                targetObject.SetActive(false);
                removedCount++;
            }

            removalsRemaining--;
            if (removalsRemaining <= 0)
            {
                break;
            }

            // Re-scan for the next closest remaining target for multi-remove configurations.
            closestTargetIndex = -1;
            closestTargetDistanceSqr = float.MaxValue;
            for (int i = 0; i < targetCount; i++)
            {
                GameObject go = _targetObjects[i];
                Collider col = _targetColliders[i];
                if (!go || !go.activeSelf || !col || !col.enabled)
                {
                    continue;
                }

                if (col.bounds.SqrDistance(worldPosition) > radiusSqr)
                {
                    continue;
                }

                // Same nearest-in-radius selection used in first pass.
                Vector3 closestPoint = col.ClosestPoint(worldPosition);
                float candidateDistanceSqr = (closestPoint - worldPosition).sqrMagnitude;
                if (candidateDistanceSqr > radiusSqr || candidateDistanceSqr >= closestTargetDistanceSqr)
                {
                    continue;
                }

                closestTargetDistanceSqr = candidateDistanceSqr;
                closestTargetIndex = i;
            }
        }

        return removedCount;
    }

    // Excludes by ancestor names first, then explicit root ancestry.
    private bool IsExcluded(Transform t)
    {
        if (!t)
        {
            return true;
        }

        Transform current = t;
        while (current)
        {
            if (IsExcludedByName(current.name))
            {
                return true;
            }

            current = current.parent;
        }

        if (_excludedRoots == null || _excludedRoots.Length == 0)
        {
            return false;
        }

        foreach (Transform excludedRoot in _excludedRoots)
        {
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

    // Simple prefix matching keeps exclusions resilient to suffix variants like "(1)".
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
}
