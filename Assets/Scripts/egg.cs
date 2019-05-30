using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class egg : MonoBehaviour
{
    public GameObject life;

    internal life mother;
    internal life father;

    private SpriteRenderer sprite;

    float timeToLife = 5;
    float runningTime = 0;

    Color color;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        SetColor();
    }

    // Update is called once per frame
    void Update()
    {
        runningTime += Time.deltaTime;

        if (runningTime >= timeToLife)
        {
            var baby = Instantiate(life, this.transform.position, Quaternion.identity);
            baby.GetComponent<life>().mother = mother;
            baby.GetComponent<life>().father = father;
            baby.GetComponent<life>().color = color;
            this.gameObject.SetActive(false);
        }
    }

    private void SetColor()
    {
        var r = (mother.color.r + mother.color.r) / 2;
        var g = (mother.color.g + mother.color.g) / 2;
        var b = (mother.color.b + mother.color.b) / 2;

        color = new Color(
            Mathf.Clamp(r + Random.Range(-0.1f, 0.1f), 0, 1),
            Mathf.Clamp(g + Random.Range(-0.1f, 0.1f), 0, 1),
            Mathf.Clamp(b + Random.Range(-0.1f, 0.1f), 0, 1)
        );

        sprite.color = color;

    }
}
