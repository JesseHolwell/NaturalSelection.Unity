using UnityEngine;

public class egg : MonoBehaviour
{
    public GameObject Life;

    internal life mother;
    internal life father;

    private Color color;
    private float colorValue;

    private float age = 0;
    private readonly float hatchingTime = 7;
    private readonly float colorDrift = 0.2f;

    private void Start()
    {
        SetColor();
    }

    private void Update()
    {
        age += Time.deltaTime;

        if (age >= hatchingTime)
        {
            Hatch();
        }
    }

    private void Hatch()
    {
        var baby = Instantiate(Life, transform.position, Quaternion.identity);
        var life = baby.GetComponent<life>();
        life.mother = mother;
        life.father = father;
        life.colorValue = colorValue;

        Destroy(gameObject);
    }

    private void SetColor()
    {
        var sprite = GetComponent<SpriteRenderer>();

        var mvalue = mother.colorValue;
        var fvalue = father.colorValue;

        var min = mvalue < fvalue ? mvalue : fvalue;
        var max = mvalue > fvalue ? mvalue : fvalue;

        min -= colorDrift;
        max += colorDrift;

        if (min < 0)
            min = 0;
        if (max > 1)
            max = 1;

        //TODO: bell curve distribution
        colorValue = Random.Range(min, max);

        //todo: loop numbers around.
        //so 0.9 is closer to 0.1 than it is to 0.6

        //do math on number
        //then on number + maxNumber/2
        //whichever has a smaller value is closer?

        color = Color.HSVToRGB(colorValue, father.colorSaturation, father.colorLightness);

        sprite.color = color;

    }
}
