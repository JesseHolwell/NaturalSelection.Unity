using UnityEngine;

public class world : MonoBehaviour
{
    public GameObject Food;
    public GameObject Life;

    //timer for food spawn
    private float nextFoodSpawn, foodDelay = 1f; //in seconds

    private readonly float spawnBoundaryX = 15.0f;
    private readonly float spawnBoundaryY = 9f;

    private readonly int lifeSpawnCount = 1;
    private readonly int foodSpawnCount = 10;

    private readonly int concurrentLifeTarget = 50;

    private void Start()
    {
        SeedWorld();
    }

    private void Update()
    {
        FoodSpawner();
    }

    private void SeedWorld()
    {
        for (var v = 0; v < lifeSpawnCount; v++)
            CreateLife();

        for (var v = 0; v < foodSpawnCount; v++)
            SpawnFood();
    }

    private void FoodSpawner()
    {
        if (Time.time > nextFoodSpawn)
        {
            nextFoodSpawn = Time.time + foodDelay;
            SpawnFood();
        }
    }

    private void CreateLife()
    {
        Vector3 position = new Vector3(Random.Range(-spawnBoundaryX, spawnBoundaryX), Random.Range(-spawnBoundaryY, spawnBoundaryY), 0);
        Instantiate(Life, position, Quaternion.identity);
    }

    private void SpawnFood()
    {
        Vector3 position = new Vector3(Random.Range(-spawnBoundaryX, spawnBoundaryX), spawnBoundaryY, 0);
        Instantiate(Food, position, Quaternion.identity);
    }
}
