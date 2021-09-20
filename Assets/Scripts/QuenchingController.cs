using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuenchingController : MonoBehaviour{
    public bool isQuenching;
    private bool isRoomTemp;
    public Rigidbody waterRB;
    private float waterTemp, initTemp;
    private float waterCoolingConst, timer, quenchingTimer;
    public float  thermalConductivity, surfaceArea, thickness, specificHeat;
    public MetalBarController mBController;
    public MetalInfoController metalInfoContr;
    public SphereCollider quenchingCollider;
    public List<GameObject> metalBars = new List<GameObject>();
    private Transform quenchingMBPos;
    public float getWaterTemp(){
        return waterTemp;
    }
    public void resetTimer(){
        quenchingTimer = 0.0f;
    }
    // Start is called before the first frame update
    void Start(){
        waterCoolingConst = (thermalConductivity*surfaceArea)/(waterRB.mass*specificHeat*thickness);
        waterTemp = 21.40f;
        resetTimer();
        isQuenching = false;
    }
    // Update is called once per frame
    void Update(){
        timer += Time.deltaTime;
        if(!isQuenching){
            foreach(GameObject metalBar in metalBars){
                isQuenching = quenchingCollider.bounds.Contains(metalBar.transform.position);
                if(isQuenching){
                    mBController = metalBar.GetComponent<MetalBarController>();
                    quenchingMBPos = metalBar.transform;
                    return;
                }
            }
        }
        //Water is heated by quenching metal
        if(isQuenching){
            Debug.Log("We are quenching!");
            if(quenchingTimer == 0.0f)
                initTemp = waterTemp;
            quenchingTimer += Time.deltaTime;
            if(timer >= 1.00f){
                timer -= 1.0f;
                //call heating law for water, we are cooling a hot object and warming the water
                waterTemp = NewtonsCoolingEquation.heatObj(mBController.metalBarStruct.metalTemp, initTemp, waterCoolingConst, quenchingTimer, "Water");
                //Debug.Log(waterTemp);
            }
            if(!quenchingCollider.bounds.Contains(quenchingMBPos.position)){
                resetTimer();
                isQuenching = false;
            }
        }
        //water is cooling
        else if(!isQuenching&&waterTemp>GameStats.ambientTemp){
            if(quenchingTimer == 0.0f)
                initTemp = waterTemp;
            quenchingTimer += Time.deltaTime;
            if(timer >= 1.00f){
                timer -= 1.0f;
                //call heating law for water, we are cooling a hot object and warming the water
                waterTemp = NewtonsCoolingEquation.coolObj(GameStats.ambientTemp, initTemp, waterCoolingConst, quenchingTimer, "Water");
                //Debug.Log(waterTemp);
            }
        }
        //if water is colder than the ambient temp then its tke heat to match it 
        else if(!isQuenching&&waterTemp<GameStats.ambientTemp){
            if(quenchingTimer == 0.0f)
                initTemp = waterTemp;
            quenchingTimer += Time.deltaTime;
            if(timer >= 1.00f){
                timer -= 1.00f;
                //call heating law for water, we are cooling a hot object and warming the water
                waterTemp = NewtonsCoolingEquation.heatObj(GameStats.ambientTemp, initTemp, waterCoolingConst, quenchingTimer, "Water");
                //Debug.Log(waterTemp);
            }
        }          
        if(Mathf.Round(waterTemp) == GameStats.ambientTemp && !isQuenching){
            Debug.Log("The water is now room temp!");
            resetTimer();
            waterTemp  = GameStats.ambientTemp;
        }
        //if water is colder than the ambient temp then its gonna have to match it at some point
        metalInfoContr.waterTemp = decimal.Round((decimal)waterTemp,2).ToString();
    }

}
