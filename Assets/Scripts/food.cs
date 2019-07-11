using UnityEngine;

public class food : MonoBehaviour
{
    private readonly float Gravity = 1;
    private bool floor = false;
    private float floorHit;
    private float destroyDelay = 3;

    private float nextOccurance = 1f; 
    private float minDelay = 1f; 
    private float maxDelay = 3f; 
    private bool leftDrift = true;
    private float drift = 4f; //works inverse

    internal bool eatable = true;

    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = this.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!floor)
        {
            var vector = leftDrift ? Vector3.left : Vector3.right;

            transform.position += Vector3.down * Gravity * Time.deltaTime;
            transform.position += vector * Time.deltaTime / drift;

            //TODO: drift should be calcualted as a parabola?
            //slower if nextOccurance is close to Time.time, or close to last occurance
            //faster if inbetween the two points

            if (Time.time > nextOccurance)
            {
                nextOccurance = Time.time + Random.Range(minDelay, maxDelay);
                leftDrift = !leftDrift;
            }
        }
        else
        {
            //TODO: fade out is not attached to destroy timer
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - 0.05f);

            if (Time.time > floorHit + destroyDelay)
                Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //TODO: this doesnt work, istrigger?
        if (collision.tag == "Background")
        {
            //Debug.Log("floor");
            floor = true;
            floorHit = Time.time;
        }

    }

}
