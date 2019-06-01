using UnityEngine;
using UnityEngine.UI;

public class text : MonoBehaviour
{
    public Text TestObj;
    internal life selected;

    private void Start()
    {

    }

    private void Update()
    {
        UpdateUIText();
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

            TestObj.text = uiString;
        }
        else
            TestObj.text = "";
    }
}
