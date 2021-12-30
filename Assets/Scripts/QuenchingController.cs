using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuenchingController : MonoBehaviour{
    #region "Private Serialized Fields"
    [Tooltip("Is the cooler in use?")]
    [SerializeField] private bool isQuenching;
    [Tooltip("Rigidbody component attached to forge cooler.")]
    [SerializeField] private Rigidbody liquidRB;
    [Header("Variables used to calculate heating/cooling water in forge cooler.")]
    [SerializeField] private float liquidTemp, initTemp, liquidCoolingConst, timer, quenchingTimer, thermalConductivity, surfaceArea, thickness, specificHeat, boilingTemp;
    [Tooltip("Component attached to metal bar in quencher, gives us metal bars cur temp.")]
    [SerializeField] private MetalBarController mBController;
    [Tooltip("Component Attached to player's UI where current water temperature is shown")]
    [SerializeField] private MetalInfoController metalInfoContr;
    [Tooltip("Collider used to trigger quelching process.")]
    [SerializeField] private SphereCollider quenchingCollider;
    [SerializeField] private ParticleSystem steamPartSys;
    #endregion
    #region "Public Getter/Setters"
    public bool IsQuenching{get{return isQuenching;}set{isQuenching=value;}}
    public float LiquidTemp{get{return liquidTemp;}set{liquidTemp=value;}}
    public SphereCollider QuenchingCollider{get{return quenchingCollider;}}

    #endregion

    //public List<GameObject> metalBars = new List<GameObject>();
    private Transform quenchingMBPos;
     public void resetTimer(){
        quenchingTimer = 0.0f;
    }
    public bool gatherMBInfo(Collider other){
        mBController = other.gameObject.GetComponent<MetalBarController>();
        quenchingMBPos = other.transform;
        return true;
    }
    // Start is called before the first frame update
    void Start(){
        liquidCoolingConst = (thermalConductivity*surfaceArea)/(liquidRB.mass*specificHeat*thickness);
        //Left as arbitrary val < GameStats.ambientTemp just to see @ runtime the water temp acclamate to room temp
        liquidTemp = 21.40f;
        resetTimer();
        isQuenching = false;
        Physics.IgnoreLayerCollision(GameObject.FindWithTag("Player").layer, gameObject.layer, true);
    }
    // Update is called once per frame
    void Update(){
        timer += Time.deltaTime;
        //Water is heated by quenching metal
        if(isQuenching){
            if(quenchingTimer == 0.0f)
                initTemp = liquidTemp;
            quenchingTimer += Time.deltaTime;
            if(timer >= 1.00f){
                timer -= 1.0f;
                //call heating law for water, we are cooling a hot object and warming the water
               liquidTemp = NewtonsCoolingEquation.heatObj(mBController.MetalBarStruct.metalTemp, initTemp, liquidCoolingConst, quenchingTimer, "Water");
            }
        }
        //water is cooling
        else if(!isQuenching && liquidTemp > GameStats.ambientTemp){
            if(quenchingTimer == 0.0f)
                initTemp = liquidTemp;
            quenchingTimer += Time.deltaTime;
            if(timer >= 1.00f){
                timer -= 1.0f;
                //call heating law for water, we are cooling a hot object and warming the water
                liquidTemp = NewtonsCoolingEquation.coolObj(GameStats.ambientTemp, initTemp, liquidCoolingConst, quenchingTimer, "Water");
            }
        }
        //if water is colder than the ambient temp then environment will heat it
        else if(!isQuenching && liquidTemp<GameStats.ambientTemp){
            if(quenchingTimer == 0.0f)
                initTemp = liquidTemp;
            quenchingTimer += Time.deltaTime;
            if(timer >= 1.00f){
                timer -= 1.00f;
                //call heating law for water, we are cooling a hot object and warming the water
                liquidTemp = NewtonsCoolingEquation.heatObj(GameStats.ambientTemp, initTemp, liquidCoolingConst, quenchingTimer, "Water");
            }
        }          
        if(Mathf.Round(liquidTemp) == GameStats.ambientTemp && !isQuenching){
            Debug.Log("The water is now room temp!");
            resetTimer();
            liquidTemp  = GameStats.ambientTemp;
        }
        //if water is colder than the ambient temp then its gonna have to match it at some point
        metalInfoContr.waterTemp = decimal.Round((decimal)liquidTemp,2).ToString();

        //Should the liquid in the forge cooling be steaming?
        if(liquidTemp >= boilingTemp && !steamPartSys.isPlaying)
            steamPartSys.Play();
        else if(liquidTemp < boilingTemp && steamPartSys.isPlaying)
            steamPartSys.Stop();
    }

    void OnTriggerEnter(Collider other){
        Debug.Log($"{other.name} is being submerged.");
        isQuenching = other.CompareTag("Metal Bars")?gatherMBInfo(other):false;
    }
    void OnTriggerExit(){
        resetTimer();
        mBController = null;
        quenchingMBPos = null;
        isQuenching = false;
    }

}
