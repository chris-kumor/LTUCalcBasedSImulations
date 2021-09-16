using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetalInfoController : MonoBehaviour
{
    public string metalType;
    public string metalTemp;
    public string waterTemp;
    private string defaultStatus = $"Metal Type:\nTemperature:\nAmbient Temperature:{decimal.Round((decimal)GameStats.ambientTemp,2)}C\nWater Temp:";
    public Text statsText;
    // Start is called before the first frame update
    public void Reset(){
        statsText.text = defaultStatus;
        metalType = null;
        metalTemp = null;
    }
    void Start(){
        statsText.text = defaultStatus;
        GameStats.ambientTemp = 22.00f;
    }
    // Update is called once per frame
    void Update(){
        statsText.text = $"Metal Type:{metalType}\nTemperature:{metalTemp}C\nAmbient Temperature:{decimal.Round((decimal)GameStats.ambientTemp,2)}C\nWater Temp:{waterTemp}C";
    }
}
