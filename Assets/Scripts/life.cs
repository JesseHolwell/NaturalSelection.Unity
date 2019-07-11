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
    private static readonly int MaxAge = 50;
    internal float Energy = 50;
    private float MaxEnergy = 100;
    internal float Size = 1f;

    private float BaseSpeed = 1f;
    private bool IsRunning = false;
    private int VisionDistance = 10;
    private float growthRate = 0.002f;
    private readonly float Metabolism = 0.01f;

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
    private readonly float spawnBoundaryY = 9f;
    private readonly float spawnZeroX = 15.0f;
    private readonly float spawnZeroY = 10.0f;

    internal Color color;
    internal float colorValue;
    internal float colorSaturation = 0.75f;
    internal float colorLightness = 0.7f;

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
            return (BaseSpeed + BaseSpeed * Age / 20) * TimeStep;
        }
    }

    internal float RunSpeed
    {
        get
        {
            return Speed / TimeStep * 2f * TimeStep;
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

        swimDelay = Random.Range(0.75f, 1.25f);
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
        bool urgent = true;
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
            urgent = false;
            State = LifeState.Satisfied;
            Wander();
        }

        IsRunning = urgent;
    }

    private void LifeAndDeath()
    {
        if (isDead || Age >= MaxAge || Energy <= 0)
        {
            Die();
        }
        //else
        //{
        //    //Live();
        //    //Incase they replenish their energy while dying
        //ehh now I handle this mechanic through saturation
        //    sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
        //}

        if (IsPregnant
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
            if (go.GetComponent<food>().eatable)
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
                //if (Math.Abs(critter.color.r - color.r) < .25f
                //    && Math.Abs(critter.color.g - color.g) < .25f
                //    && Math.Abs(critter.color.b - color.b) < .25f
                //    )
                if (Math.Abs(critter.colorValue - colorValue) < 0.5f)
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

    private float timeToSwim = 1;
    private float nextSwim = 0;
    private float swimDelay;

    private void MoveTowards(Vector3 point)
    {
        if (Time.time > nextSwim)
        {
            nextSwim = Time.time + swimDelay;
        }
        float modifier = nextSwim - Time.time + 0.25f;

        var heading = Waypoint - transform.position;

        ///Too slow
        //var position = transform.position + heading.normalized * Speed * Time.deltaTime;
        //rb2d.MovePosition(position);

        var speed = IsRunning ? RunSpeed : Speed;

        transform.position = Vector3.MoveTowards(transform.position, Waypoint, speed * modifier * Time.deltaTime);

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
                //collision.GetComponent<food>().eatable = false;
                Destroy(collision.gameObject);
                Energy = MaxEnergy; //Energy += FoodEnergy;
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
        if (!isDead)
        {
            Energy -= Metabolism * Size * 500 * Time.deltaTime; //(float)(Metabolism) * ( Size^3 + VisionDistance) * TimeStep;
            Age += Time.deltaTime * TimeStep;

            Size += Time.deltaTime * growthRate;
            transform.localScale += new Vector3(Time.deltaTime * growthRate, Time.deltaTime * growthRate, 0);

            var energyPercentage = Energy / MaxEnergy;
            var energyNormal = energyPercentage * colorSaturation;

            //sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b);
            sprite.color = Color.HSVToRGB(colorValue, energyNormal, colorLightness);
        }

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

    private bool isDead = false;
    private void Die()
    {
        isDead = true;
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - 0.1f);
        if (sprite.color.a < 0)
            Destroy(this.gameObject);
    }

    private void SetColor()
    {
        if (IsNewLife)
        {
            colorValue = Random.Range(0f, 1f);
            //Debug.Log(colorValue);
            //color = new Color(
            //    Random.Range(0f, 1f),
            //    Random.Range(0f, 1f),
            //    Random.Range(0f, 1f)
            //);

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

        color = Color.HSVToRGB(colorValue, colorSaturation, colorLightness);
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
