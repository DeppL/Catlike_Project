using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NucleonSpawner : MonoBehaviour
{
    public float timeBetweenSpawns;
    public float spawnDistance;
    public Nucleu[] nucleonPrefabs;

    float timeSinceLastSpawn;
    private void FixedUpdate() 
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn > timeBetweenSpawns) {
            timeSinceLastSpawn -= timeBetweenSpawns;
            SpawnNucleon();
        }
    }

    void SpawnNucleon() {
        Nucleu prefab = nucleonPrefabs[Random.Range(0, nucleonPrefabs.Length)];
        Nucleu spawn = Instantiate<Nucleu>(prefab);
        spawn.transform.localPosition = Random.onUnitSphere * spawnDistance;
    }
}
