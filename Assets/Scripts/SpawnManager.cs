using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnRangeX;
    public float spawnRangeZ;
    public float spawnPosX;
    public float spawnPosZ;
    public float spawnPosY = 1.25f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            spawnEnemy();
        }
    }
    void spawnEnemy()
    {
        spawnPosX = Random.Range(-spawnPosX, spawnPosX);
        spawnPosZ = Random.Range(-spawnPosZ, spawnPosZ);
        Instantiate(enemyPrefab, new Vector3(spawnPosX, spawnPosY, spawnPosZ),default);
    }
}
