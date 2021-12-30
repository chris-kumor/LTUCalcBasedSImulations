using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalTimer: MonoBehaviour
{
    [SerializeField] private float globalTimer;
    [SerializeField] private bool oneSecPassed;
    public bool OneSecPassed{get{return oneSecPassed;}}
    private void TriggerOneSecPassed(){
        oneSecPassed = true;
        globalTimer -= 1.00f;
    }
    void Start(){
        oneSecPassed = false;
        globalTimer = 0.0f;
    }
    void Update(){
        if(globalTimer >= 1.0f)
            TriggerOneSecPassed();
        else if(globalTimer < 0.0f && oneSecPassed)
            oneSecPassed = false;
        globalTimer += Time.deltaTime;
    }
}
