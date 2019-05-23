using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class world : MonoBehaviour
{
    public GameObject food;
    public GameObject lifeSeed;

    //timer for food spawn
    private float nextActionTime, period = 0.5f; //in seconds

    private float spawnBoundaryX = 15.0f;
    private float spawnBoundaryY = 10.0f;

    private int LifeSpawnCount = 10;
    private int FoodSpawnCount = 10;

    private int ConcurrentLifeTarget = 50;

    // Start is called before the first frame update
    void Start()
    {
        for (var v = 0; v < LifeSpawnCount; v++)
            CreateLife();

        for (var v = 0; v < FoodSpawnCount; v++)
            SpawnFood();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime = Time.time + period;
            SpawnFood();
        }
    }

    void CreateLife()
    {
        Vector3 position = new Vector3(Random.Range(-spawnBoundaryX, spawnBoundaryX), Random.Range(-spawnBoundaryY, spawnBoundaryY), 0);
        Instantiate(lifeSeed, position, Quaternion.identity);
    }

    void SpawnFood()
    {
        //Vector3 position = new Vector3(Random.Range(-spawnBoundaryX, spawnBoundaryX), Random.Range(-spawnBoundaryY, spawnBoundaryY), 0);
        Vector3 position = new Vector3(Random.Range(-spawnBoundaryX, spawnBoundaryX), spawnBoundaryY, 0);
        Instantiate(food, position, Quaternion.identity);
    }
}
