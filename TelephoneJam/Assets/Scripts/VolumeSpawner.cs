using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class VolumeSpawner : MonoBehaviour
{
    [Header("Collider and positions")]
    [SerializeField] private BoxCollider spawnVolume;//el espacio
    [SerializeField] Transform playerRoot;
    [SerializeField] float forwardOffset = 12f;

    [Header("Objects to spawn (birdos hehe)")]
    [SerializeField] private GameObject bird;
    [SerializeField] private int spawnCount = 3;

    [SerializeField] private float timerChangeLocation = 3f;
    [SerializeField] private bool rotation = false; // was trying to do something here but it didn't work....


    private readonly List<Transform> spawned = new();


    private void Start()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 pos = RandomPointInBox();
            Quaternion rot = rotation ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) : Quaternion.identity;

            GameObject go = Instantiate(bird, pos, rot);
            go.SetActive(true);
            spawned.Add(go.transform);
        }
        StartCoroutine(RepositionLoop());

    }

    private IEnumerator RepositionLoop()
    {
        var wait = new WaitForSeconds(timerChangeLocation);

        while (true)
        {
            yield return wait;
            for (int i = 0; i < spawned.Count; i++)
            {
                Transform t = spawned[i];
                if (!t) continue;

                t.position = RandomPointInBox();
                Vector3 worldCenter = spawnVolume.transform.TransformPoint(spawnVolume.center);


                if (rotation)
                    t.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            }
        }
    }

    Vector3 RandomPointInBox()
    {

        Vector3 he = spawnVolume.size * 0.5f;


        Vector3 localOffset = new Vector3(
            Random.Range(-he.x, he.x),
            Random.Range(-he.y, he.y),
            Random.Range(-he.z, he.z)
        );

        Vector3 worldCenter =
            playerRoot.position +
            playerRoot.forward * forwardOffset +
            playerRoot.TransformVector(spawnVolume.center);


        Vector3 worldPos =
            worldCenter +
            playerRoot.right * localOffset.x +
            playerRoot.up * localOffset.y +
            playerRoot.forward * localOffset.z;

        return worldPos;
    }





}
