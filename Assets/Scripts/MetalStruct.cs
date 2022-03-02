using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MetalStruct", order = 1)]
public class MetalStruct : ScriptableObject{
    public float metalTemp;
    public string metalType;
    public float specificHeat;
    public float surfaceArea;
    public float normalDepth;
    public float thermConduct;
    public float youngsModulus;


}

