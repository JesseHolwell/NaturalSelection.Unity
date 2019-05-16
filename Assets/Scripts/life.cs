using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class life : MonoBehaviour
{
    private Dictionary<int, double> CancerRates = new Dictionary<int, double>()
    {
        { 20, 1 },
        { 34, 2.7 },
        { 44, 5.2 },
        { 54, 14.1 },
        { 64, 24.1 },
        { 74, 25.4 },
        { 84, 19.6 },
        { 100, 7.8 }
    };

    private Rigidbody2D rb2d;
    private SpriteRenderer sprite;

    public GameObject child;
    public GameObject egg;

    private float TimeStep = 1;

    private Vector3 Waypoint;

    private float Age = 0;
    private float Energy = 50;
    private float BaseSpeed = 0.15f;
    private readonly int Metabolism = 1;
    private int Size = 1;

    private readonly int FoodEnergy = 50;
    private readonly int MinEnergyToReproduce = 85;
    private readonly int MinEnergyToSearchForFood = 75;

    private int VisionDistance = 2;

    private bool IsMale;
    private bool IsPregnant = false;

    private float AgeImpregnated = 0;
    private readonly int GestationTime = 1;

    private float LastReproduced = 0;
    private readonly int ReproductionDelay = 0;
    private readonly int ReproductionCost = 50;

    private readonly int MinFertilityAge = MaxAge * 25 / 100;
    private readonly int MaxFertilityAge = MaxAge * 75 / 100;

    private readonly float spawnBoundaryX = 15.0f;
    private readonly float spawnBoundaryY = 10.0f;

    private static readonly int MaxAge = 20;

    private float growthRate = 0.002f;



    //distance to food
    //distance to mate

    //Things I care about:
    //color
    //state
    //vision
    //clock
    //pheromones
    //movement
    //wants
    //biological

    protected bool IsFertile
    {
        get
        {
            return Age > MinFertilityAge
                && Age < MaxFertilityAge
                && !IsPregnant
                && Age > LastReproduced + ReproductionDelay;
        }
    }

    private float Speed
    {
        get
        {
            return (1 + BaseSpeed * Age) * TimeStep;
        }
    }

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        Age = Random.Range(0, 8);
        IsMale = Random.Range(0, 2) == 1;
    }

    private void Update()
    {
        //priorities:
        //Die
        //Lay Egg
        //Seek Mate
        //Seek Food
        //Wander


        //wants to lay

        if (Age >= MaxAge)
        {
            gameObject.SetActive(false); //todo: better destroy
        }
        else if (IsPregnant
            && Energy > MinEnergyToReproduce
            && Age > AgeImpregnated + GestationTime)
        {
            GiveBirth();
        }

        var foundTarget = false;
        if (Energy > MinEnergyToSearchForFood
            && IsFertile
            && !IsPregnant)
        {
            sprite.color = Color.yellow;
            foundTarget = SeekMate();
        }
        else if (Energy < MinEnergyToSearchForFood)
        {
            sprite.color = Color.red;
            foundTarget = SeekFood();
        }
        
        if (!foundTarget)
        {
            sprite.color = Color.green;
            Wander();
        }

        MoveTowards(Waypoint);
        Aging();

        if (IsPregnant)
        {
            sprite.color = Color.white;
        }

        if (Age > MaxAge - 1)
        {
            sprite.color = Color.black;
        }
    }

    private bool SeekMate()
    {
        var closest = GetClosestMate("Life");

        if (closest != null)
        {
            var mate = closest.transform.position;
            Waypoint = mate;
        }

        return closest != null;
    }

    private bool SeekFood()
    {
        var closest = GetClosestObjectByTag("Food");

        if (closest != null)
        {
            var foodTarget = closest.transform.position;
            Waypoint = foodTarget;
        }
        return closest != null;
    }

    private GameObject GetClosestObjectByTag(string tag)
    {
        var objList = GameObject.FindGameObjectsWithTag(tag);

        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in objList)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (distance < VisionDistance && curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }

        return closest;
    }

    private GameObject GetClosestMate(string tag)
    {
        var objList = GameObject.FindGameObjectsWithTag(tag);

        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in objList)
        {
            var critter = go.GetComponent<life>();
            if (critter.IsFertile && critter.IsMale != this.IsMale)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (distance < VisionDistance && curDistance < distance)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
        }

        objList.Where(x => x.transform).Select(x=>x.tag);

        return closest;
    }

    private void Wander()
    {
        if ((this.transform.position - Waypoint).magnitude < 1)
        {
            //get a random point within view
            //ensure its within the bounds
            var x = Random.Range(this.transform.position.x - VisionDistance, this.transform.position.x + VisionDistance);
            //if (x < -spawnBoundaryX)
            //    x = -spawnBoundaryX;
            //else if (x > spawnBoundaryX)
            //    x = spawnBoundaryX;

            var y = Random.Range(this.transform.position.y - VisionDistance, this.transform.position.y + VisionDistance);
            //if (y < -spawnBoundaryY)
            //    y = -spawnBoundaryY;
            //else if (y > spawnBoundaryY) 
            //    y = spawnBoundaryY;

            Waypoint = new Vector3(x, y, 0);
        }
    }

    private void MoveTowards(Vector3 point)
    {
        var heading = Waypoint - this.transform.position;
        transform.position += heading.normalized * Speed * Time.deltaTime;

        sprite.flipX = heading.x > 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            if (Energy < 75)
            {
                collision.gameObject.SetActive(false); //todo: better destroy
                Energy = 100; //Energy += FoodEnergy;
                Wander();
            }
        }
        else if (collision.gameObject.CompareTag("Life"))
        {
            if (!IsMale && IsFertile
                && collision.gameObject.GetComponent<life>().IsMale)
            {
                IsPregnant = true;
                AgeImpregnated = Age;
                Wander();
            }
        }
    }

    private void Aging()
    {
        Energy -= (float)Metabolism / 15 * TimeStep;
        Age += Time.deltaTime * TimeStep;

        //grow bigger
        sprite.transform.localScale
            = new Vector3(sprite.transform.localScale.x + Time.deltaTime * growthRate * TimeStep,
                          sprite.transform.localScale.y + Time.deltaTime * growthRate * TimeStep,
                          sprite.transform.localScale.z);

    }

    private void CheckForCancer()
    {
        //var cancerChance = cancerRates.FirstOrDefault(x => Age < x.Key).Value;
        //var diceRoll = Random.Range(0, 100);

        //if (diceRoll < cancerChance)
        //    gameObject.SetActive(false);

    }

    private void Destroy()
    {
        //Create death symbol
    }

    private void GiveBirth()
    {
        LastReproduced = Age;
        Energy -= ReproductionCost;
        Instantiate(egg, transform.position, Quaternion.identity);
        IsPregnant = false;
    }

    public string GetHashCode()
    {
        return this.AgeImpregnated.ToString("D5")
            + this.Size.ToString("D5")
            + this.GestationTime.ToString("D5")
            + this.ReproductionDelay.ToString("D5");
    }

}
