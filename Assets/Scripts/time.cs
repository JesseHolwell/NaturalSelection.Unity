using UnityEngine;
using UnityEngine.UI;

public class time : MonoBehaviour
{
    private Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    private void Update()
    {
        Time.timeScale = slider.value;
        Time.fixedDeltaTime = Time.timeScale;

        //TODO: keyboard controls shouldnt affect the UI element
    }
}
