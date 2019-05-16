using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class egg : MonoBehaviour
{
    public GameObject life;

    float timeToLife = 5;
    float runningTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        runningTime += Time.deltaTime;

        if (runningTime >= timeToLife)
        {
            Instantiate(life, this.transform.position, Quaternion.identity);
            this.gameObject.SetActive(false);
        }
    }
}
