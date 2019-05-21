using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class time : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var slider = this.GetComponent<Slider>();
        Time.timeScale = slider.value;
        Time.fixedDeltaTime = Time.timeScale;
    }
}
