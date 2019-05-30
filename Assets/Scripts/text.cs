using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class text : MonoBehaviour
{
    public Text objectText;
    internal life selected;
    //internal life;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateUIText();

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Debug.Log("Pressed primary button.");
        //    Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        //    RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);

        //    if (hit)
        //    {
        //        selected = hit.transform.gameObject.GetComponent<life>();

        //        Debug.Log(hit.transform.name);
        //        //return ;
        //    }
        //    else
        //    {
        //        //selected = null;
        //    }

        //}
    }

    private void UpdateUIText()
    {
        if (selected != null)
        {
            var uiString = string.Empty;

            uiString += $"Age:\t{selected.Age}\n";
            uiString += $"Energy:\t{selected.Energy}\n";
            uiString += $"Waypoint:\t{selected.Waypoint.ToString()}\n";
            uiString += $"Size:\t{selected.Size}\n";
            uiString += $"Speed:\t{selected.Speed}\n";
            uiString += $"Is Male:\t{selected.IsMale}\n";
            uiString += $"Is Fertile:\t{selected.IsFertile}\n";
            uiString += $"Is Pregnant:\t{selected.IsPregnant}\n";
            uiString += $"State:\t{selected.State}\n";
            uiString += $"Color:\t{selected.color}\n";

            objectText.text = uiString;
        }
        else
            objectText.text = "";
    }
}
