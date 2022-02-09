using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeController : MonoBehaviour
{
    [SerializeField] private float forgeTemp;
    public float ForgeTemp{get{return forgeTemp;}set{forgeTemp = value;}}
    [SerializeField] private SphereCollider forgeCollider;
    public SphereCollider ForgeCollider{get{return forgeCollider;}}
    [SerializeField] private MetalBarController otherMB;

    void OnTriggerExit(Collider other){
        if(other.CompareTag("Metal Bars")){
            otherMB.ThermalTimer = 0.0f;
            otherMB.IsHeating = false;
            otherMB = null;
        }

    }
    void OnTriggerEnter(Collider other){
        if(other.CompareTag("Metal Bars")){
            otherMB = other.GetComponent<MetalBarController>();
            otherMB.ThermalTimer = 0.0f;
            otherMB.IsHeating = true;
            
        }
    }
}
