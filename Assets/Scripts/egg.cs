using UnityEngine;

public class egg : MonoBehaviour
{
    public GameObject Life;

    internal life mother;
    internal life father;

    private Color color;

    private float age = 0;
    private readonly float hatchingTime = 5;
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
        life.color = color;

        gameObject.SetActive(false);
    }

    private void SetColor()
    {
        var sprite = GetComponent<SpriteRenderer>();

        var r = (mother.color.r + father.color.r) / 2;
        var g = (mother.color.g + father.color.g) / 2;
        var b = (mother.color.b + father.color.b) / 2;

        color = new Color(
            Mathf.Clamp(r + Random.Range(-colorDrift, colorDrift), 0, 1),
            Mathf.Clamp(g + Random.Range(-colorDrift, colorDrift), 0, 1),
            Mathf.Clamp(b + Random.Range(-colorDrift, colorDrift), 0, 1)
        );

        sprite.color = color;

    }
}
