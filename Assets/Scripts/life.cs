using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
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

    public GameObject child;
    public GameObject egg;
    //public GameObject uiText;

    //private text textScript;

    private Rigidbody2D rb2d;
    private SpriteRenderer sprite;

    private float TimeStep = 1;

    internal Vector3 Waypoint;

    internal float Age = 0;
    internal float Energy = 50;
    private float BaseSpeed = 0.15f;
    private readonly float Metabolism = 0.01f;
    internal int Size = 1;

    private readonly int FoodEnergy = 50;
    private readonly int MinEnergyToReproduce = 85;
    private readonly int MinEnergyToSearchForFood = 75;

    private int VisionDistance = 100;

    internal bool IsMale;
    internal bool IsPregnant = false;

    private float AgeImpregnated = 0;
    private readonly int GestationTime = 2;

    private float LastReproduced = 0;
    private readonly int ReproductionDelay = 1;
    private readonly int ReproductionCost = 50;

    private readonly int MinFertilityAge = MaxAge * 25 / 100;
    private readonly int MaxFertilityAge = MaxAge * 75 / 100;

    private readonly float spawnBoundaryX = 15.0f;
    private readonly float spawnBoundaryY = 10.0f;

    private static readonly int MaxAge = 50;

    private float growthRate = 0.002f;

    internal LifeState State;

    internal Color color;
    internal life parent;

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

        //uiText = GameObject.Find("text"); // FindGameObjectsWithTag("Text").FirstOrDefault().GetComponent<text>();
        //textScript = uiText.gameObject.find <text>();

        if (parent == null)
            Age = Random.Range(0, 8);

        IsMale = Random.Range(0, 2) == 1;

        SetColor();

    }

    float scale = 0;

    private void Update()
    {

        //priorities:
        //Die
        //Lay Egg
        //Seek Mate
        //Seek Food
        //Wander


        //wants to lay

        if (Age >= MaxAge || Energy <= 0)
        {
            Destroy(this.gameObject);
        }
        else if (IsPregnant
            && Energy > MinEnergyToReproduce
            && Age > AgeImpregnated + GestationTime)
        {
            GiveBirth();
        }

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

        MoveTowards(Waypoint);
        Aging();

        if (Age > MaxAge - 1)
        {
            State = LifeState.Dying;
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
            if (curDistance < VisionDistance && curDistance < distance)
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
            if (critter && critter.IsFertile && critter.IsMale != this.IsMale)
            {
                if (Math.Abs(critter.color.r - this.color.r) < .25f
                    && Math.Abs(critter.color.g - this.color.g) < .25f
                    && Math.Abs(critter.color.b - this.color.b) < .25f
                    )
                {
                    Vector3 diff = go.transform.position - position;
                    float curDistance = diff.sqrMagnitude;
                    if (curDistance < VisionDistance && curDistance < distance)
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
        if ((this.transform.position - Waypoint).magnitude < 1)
        {
            //get a random point within view
            //ensure its within the bounds
            var x = Random.Range(this.transform.position.x - VisionDistance, this.transform.position.x + VisionDistance);
            if (x < -spawnBoundaryX)
                x = -spawnBoundaryX;
            else if (x > spawnBoundaryX)
                x = spawnBoundaryX;

            var y = Random.Range(this.transform.position.y - VisionDistance, this.transform.position.y + VisionDistance);
            if (y < -spawnBoundaryY)
                y = -spawnBoundaryY;
            else if (y > spawnBoundaryY)
                y = spawnBoundaryY;

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
        //Debug.Log(Energy);
        Age += Time.deltaTime * TimeStep;

        //grow bigger
        //sprite.transform.localScale
        //    = new Vector3(sprite.transform.localScale.x + Time.deltaTime * growthRate * TimeStep,
        //                  sprite.transform.localScale.y + Time.deltaTime * growthRate * TimeStep,
        //                  sprite.transform.localScale.z);

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
        var baby = Instantiate(egg, transform.position, Quaternion.identity);
        baby.GetComponent<egg>().parent = this;
        IsPregnant = false;
    }

    private void SetColor()
    {
        if (parent == null)
        {
            scale = Random.Range(0, 256);
        }
        else
        {
            scale += Random.Range(-65, 64);
            //tempColor.r += Random.Range(-0.2f, 0.2f);
            //tempColor.g += Random.Range(-0.2f, 0.2f);
            //tempColor.b += Random.Range(-0.2f, 0.2f);
        }
        //color = MapRainbowColor(scale);
        color = MapRainbowColor(scale);
        sprite.color = color;

    }

    private Color MapRainbowColor(float value)
    {
        var color = new Color(value, value, value);
        return color;
    }

    public string GetHashCode()
    {
        return this.AgeImpregnated.ToString("D5")
            + this.Size.ToString("D5")
            + this.GestationTime.ToString("D5")
            + this.ReproductionDelay.ToString("D5");
    }

}
