using UnityEngine;

public class food : MonoBehaviour
{
    private readonly float Gravity = 1;

    private void Start()
    {

    }

    private void Update()
    {
        transform.position += Vector3.down * Gravity * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //TODO: this doesnt work, istrigger?
        if (collision.tag == "Background")
        {
            Destroy(this);
        }

    }
}
