using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetalInfoController : MonoBehaviour
{
    public string metalType,metalTemp,waterTemp;
    private string envDefault = $"Ambient Temperature:{decimal.Round((decimal)GameStats.ambientTemp,2)}C\nWater Temp:";
    private string statDefault = $"Metal Type:\nMetal Temperature:\n";
    public Text EnvStats, ObjStats;
    public void Reset(){
        EnvStats.text = envDefault;
        ObjStats.text = statDefault;
        metalType = null;
        metalTemp = null;
    }
        // Start is called before the first frame update
    void Start(){
        EnvStats.text = envDefault;
        ObjStats.text = statDefault;
        GameStats.ambientTemp = 22.00f;
    }
    // Update is called once per frame
    void Update(){
        EnvStats.text = $"Ambient Temperature:{decimal.Round((decimal)GameStats.ambientTemp,2)}C\nWater Temp:{waterTemp}C";
        ObjStats.text = $"Metal Type:{metalType}\nMetal Temperature:{metalTemp}C\n";
    }
}
