using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManagerX : MonoBehaviour
{
    public GameObject[] ballPrefabs;

    private float spawnLimitXLeft = -22;
    private float spawnLimitXRight = 7;
    private float spawnPosY = 30;

    public float spawnDelay;
    private float spawnTimer;
    public float spawnMinTime;
    public float spawnMaxTime;
    public float spawnInterval=0;
    private bool intervalRemain =true ;

    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = -spawnDelay;
    }

    // Spawn random ball at random x position at top of play area

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if(intervalRemain == false)
        {
            spawnInterval = RandomSpawnInterval(spawnMinTime, spawnMaxTime);
            intervalRemain = true;
        }
        else if (spawnTimer >= spawnInterval)
        {
            SpawnRandomBall();
            spawnTimer = 0;
            intervalRemain = false;
        }
        
    }
    void SpawnRandomBall ()
    {
        // Generate random ball index and random spawn position
        Vector3 spawnPos = new Vector3(Random.Range(spawnLimitXLeft, spawnLimitXRight), spawnPosY, 0);

        int ballIndex = Random.Range(0, ballPrefabs.Length);
        // instantiate ball at random spawn location
        Instantiate(ballPrefabs[ballIndex], spawnPos, ballPrefabs[ballIndex].transform.rotation);
    }
    float RandomSpawnInterval(float a,float b)
    {
        float interval = Random.Range(a, b);
        return interval;
    }


}
