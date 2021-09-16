using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NewtonsCoolingEquation{
    
    public static float heatObj(float ambientTemp, float initTemp,float k, float t, string objName){

        //Debug.Log($"The {objName} is heating for {t} seconds and is {finalTemp} C.");
        return ambientTemp-(ambientTemp-initTemp)*Mathf.Exp(-k*t);          
    }
    public static float coolObj(float ambientTemp, float initTemp,float k, float t, string objName){
        //Debug.Log($"The {objName} is cooling for {t} seconds and is {finalTemp} C.");
        return (initTemp-ambientTemp)*Mathf.Exp(-k*t)+ambientTemp;          
    }
}
