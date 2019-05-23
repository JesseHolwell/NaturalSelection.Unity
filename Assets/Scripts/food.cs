using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class food : MonoBehaviour
{
    public float gravity;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * gravity * Time.deltaTime;

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Background")
        {
            Destroy(this);
        }

    }
}
