using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class life : MonoBehaviour
{
    public GameObject Child;
    public GameObject Egg;

    public Sprite MaleSprite;
    public Sprite FemaleSprite;

    private Rigidbody2D rb2d;
    private SpriteRenderer sprite;

    private float TimeStep = 1;

    internal LifeState State;
    internal Vector3 Waypoint;

    internal float Age = 0;
    internal float Energy = 50;
    internal float Size = 1f;

    private float BaseSpeed = 0.15f;
    private int VisionDistance = 10;
    private float growthRate = 0.002f;
    private readonly float Metabolism = 0.01f;
    private static readonly int MaxAge = 50;

    private readonly int FoodEnergy = 50;
    private readonly int MinEnergyToReproduce = 85;
    private readonly int MinEnergyToSearchForFood = 75;

    internal bool IsMale;
    internal bool IsPregnant = false;
    private float AgeImpregnated = 0;
    private float LastReproduced = 0;
    private readonly int ReproductionDelay = 1;
    private readonly int ReproductionCost = 50;
    private readonly int GestationTime = 2;
    private readonly int MinFertilityAge = MaxAge * 25 / 100;
    private readonly int MaxFertilityAge = MaxAge * 75 / 100;

    private readonly float spawnBoundaryX = 15.0f;
    private readonly float spawnBoundaryY = 10.0f;

    internal Color color;

    internal life mother;
    internal life father;
    internal life babyDaddy;

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

    internal enum LifeState
    {
        Dying,
        Satisfied,
        Hungry,
        Horny,
    }



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

    internal bool IsFertile
    {
        get
        {
            return Energy > MinEnergyToSearchForFood
                && Age > MinFertilityAge
                && Age < MaxFertilityAge
                && !IsPregnant
                && Age > LastReproduced + ReproductionDelay;
        }
    }

    internal bool IsNewLife
    {
        get
        {
            return mother == null && father == null;
        }
    }

    internal float Speed
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

        if (IsNewLife)
            Age = Random.Range(0, 8);

        SetGender();
        SetColor();
    }

    private void Update()
    {
        //priorities:
        //Die
        //Lay Egg
        //Seek Mate
        //Seek Food
        //Wander

        LifeAndDeath();

        GetNewWaypoint();

        MoveTowards(Waypoint);
        Aging();

        if (Age > MaxAge - 1)
        {
            State = LifeState.Dying;
        }
    }

    private void GetNewWaypoint()
    {
        var foundTarget = false;
        if (IsFertile && !IsPregnant)
        {
            State = LifeState.Horny;
            foundTarget = SeekMate();
        }
        else if (Energy < MinEnergyToSearchForFood)
        {
            State = LifeState.Hungry;
            foundTarget = SeekFood();
        }
        else
        {
            State = LifeState.Satisfied;
        }

        if (!foundTarget)
        {
            State = LifeState.Satisfied;
            Wander();
        }
    }

    private void LifeAndDeath()
    {
        if (Age >= MaxAge || Energy <= 0)
        {
            Destroy(gameObject);
        }
        else if (IsPregnant
            && Energy > MinEnergyToReproduce
            && Age > AgeImpregnated + GestationTime)
        {
            GiveBirth();
        }
    }

    private void SetGender()
    {
        IsMale = Random.Range(0, 2) == 1;
        if (IsMale)
        {
            sprite.sprite = MaleSprite;
        }
        else
        {
            sprite.sprite = FemaleSprite;
        }
    }

    private bool SeekMate()
    {
        var closest = GetClosestMate();

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
            if (curDistance < VisionDistance * 10 && curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }

        return closest;
    }

    private GameObject GetClosestMate()
    {
        var objList = GameObject.FindGameObjectsWithTag("Life");

        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in objList)
        {
            var critter = go.GetComponent<life>();
            if (critter && critter.IsFertile && critter.IsMale != IsMale)
            {
                if (Math.Abs(critter.color.r - color.r) < .25f
                    && Math.Abs(critter.color.g - color.g) < .25f
                    && Math.Abs(critter.color.b - color.b) < .25f
                    )
                {
                    Vector3 diff = go.transform.position - position;
                    float curDistance = diff.sqrMagnitude;
                    if (curDistance < VisionDistance * 10 && curDistance < distance)
                    {
                        closest = go;
                        distance = curDistance;
                    }
                }
            }
        }

        return closest;
    }

    private void Wander()
    {
        if ((transform.position - Waypoint).magnitude < 1)
        {
            //get a random point within view
            //ensure its within the bounds
            var x = Random.Range(transform.position.x - VisionDistance, transform.position.x + VisionDistance);
            if (x < -spawnBoundaryX)
                x = -spawnBoundaryX;
            else if (x > spawnBoundaryX)
                x = spawnBoundaryX;

            var y = Random.Range(transform.position.y - VisionDistance, transform.position.y + VisionDistance);
            if (y < -spawnBoundaryY)
                y = -spawnBoundaryY;
            else if (y > spawnBoundaryY)
                y = spawnBoundaryY;

            Waypoint = new Vector3(x, y, 0);
        }
    }

    private void MoveTowards(Vector3 point)
    {
        var heading = Waypoint - transform.position;

        ///Too slow
        //var position = transform.position + heading.normalized * Speed * Time.deltaTime;
        //rb2d.MovePosition(position);

        transform.position = Vector3.MoveTowards(transform.position, Waypoint, Speed * Time.deltaTime);

        //transform.Translate(transform.position + heading * Speed * Time.deltaTime);

        sprite.flipX = heading.x > 0;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("enter");
        if (collision.gameObject.CompareTag("Food"))
        {
            if (Energy < 75)
            {
                Destroy(collision.gameObject);
                Energy = 100; //Energy += FoodEnergy;
                Wander();
            }
        }
        else if (collision.gameObject.CompareTag("Life"))
        {
            if (!IsMale && IsFertile
                && collision.gameObject.GetComponent<life>().IsMale)
            {
                babyDaddy = collision.gameObject.GetComponent<life>();
                IsPregnant = true;
                AgeImpregnated = Age;
                Wander();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.Log("exit");
    }

    private void Aging()
    {
        Energy -= Metabolism * Size * 500 * Time.deltaTime; //(float)(Metabolism) * ( Size^3 + VisionDistance) * TimeStep;
        Age += Time.deltaTime * TimeStep;

        Size += Time.deltaTime * growthRate;
        transform.localScale += new Vector3(Time.deltaTime * growthRate, Time.deltaTime * growthRate, 0);

    }

    private void CheckForCancer()
    {
        //var cancerChance = cancerRates.FirstOrDefault(x => Age < x.Key).Value;
        //var diceRoll = Random.Range(0, 100);

        //if (diceRoll < cancerChance)
        //    gameObject.SetActive(false);

    }

    //private void Destroy(GameObject obj = null)
    //{
    //    if (obj == null)
    //        obj = this.gameObject;

    //    obj.SetActive(false);
    //}

    private void GiveBirth()
    {
        LastReproduced = Age;
        Energy -= ReproductionCost;
        var baby = Instantiate(Egg, transform.position, Quaternion.identity);
        var eggDna = baby.GetComponent<egg>();
        eggDna.mother = this;
        eggDna.father = babyDaddy;
        IsPregnant = false;
    }

    private void SetColor()
    {
        if (IsNewLife)
        {
            color = new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)
            );

        }
        //else
        //{
        //    var r = (mother.color.r + mother.color.r) / 2;
        //    var g = (mother.color.g + mother.color.g) / 2;
        //    var b = (mother.color.b + mother.color.b) / 2;

        //    color = new Color(
        //        r + Random.Range(-0.1f, 0.1f),
        //        g + Random.Range(-0.1f, 0.1f),
        //        b + Random.Range(-0.1f, 0.1f)
        //    );
        //}

        sprite.color = color;

    }

    //private Color MapRainbowColor(float value)
    //{
    //    var color = new Color(value, value, value);
    //    return color;
    //}

    public string GetHashCode()
    {
        return AgeImpregnated.ToString("D5")
            + Size.ToString("D5")
            + GestationTime.ToString("D5")
            + ReproductionDelay.ToString("D5");
    }

}
