using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MetalBarController : MonoBehaviour{
    #region "Private Serialized Fields"
    [Header("Used to represent incendecent color transition.")]
    [SerializeField] private Gradient incanGradient;
    [Tooltip("Data structure w/ nedded attributes for particular instance attached to this script.")]
    [SerializeField] private MetalStruct metalBarStruct;
    [SerializeField] private Rigidbody metalBarRB;
    [Tooltip("Following booleans for evaluating possible states current metal bar instantiation could be in.")]
    [SerializeField] private bool isHeating, inAir, inWater, isEmitting;
    [Tooltip("Following floats are used in blocks where metal bar is changing temperature")]
    [SerializeField] private float heatingTimer, initTemp, envTemp, glowTransitionTimer;
    [Tooltip("Script attached to forge, to access attributes needed for heating/cooling")]
    [SerializeField] private ForgeController forgeController;
    [Tooltip("Script attached to Forge Cooler.")]
    [SerializeField] private QuenchingController quenchingController;
    [Tooltip("Material of the metal bars shader, used to change emmisive color to give us incendecent effect.")]
    [SerializeField] private Material mBMaterial;
    [Header("Reference to global timer all metal bars reference when heating/cooling over time.")]
    [SerializeField] private GlobalTimer mGlobalTimer;
    #endregion
    #region "Public Fields +  Members"
    public MetalStruct MetalBarStruct{get{return metalBarStruct;}}
    public bool InWater{get{return inWater;}set{inWater = value;}}
    public float HeatingTimer{get{return heatingTimer;}set{heatingTimer = value;}}
    public bool IsHeating{get{return isHeating;}set{isHeating = value;}}
    #endregion
    #region "Private Fields + Members"
    private float curCoolingConst;
    private static float gradientTempToTime(float metalTemp){
        //The temp range is within 750.0C to 1300C
        //Value returned lies between 0-1
        return ((metalTemp - 550.0f)/750.0f);
    }
    ///<summary>
    /// This following variable is a Gradient to represent the color scale of incandecence
    /// The values represent the color shift experienced by metals when being heated.
    /// The current temperature of the metal bar is passed into a local func.
    /// The local func then returns a val that lies between 0 -1
    /// 0 = Temperature is less then min temp for incandecence
    /// 1 = Maximum Temperature such that any temperature higher does not change the color of icandecence
    ///</summary>
    private GradientColorKey[] incanColorKeys = {new GradientColorKey(new Color(0.12743768f, 0.0f, 0.0f, 1.00f), 0.0f), 
    new GradientColorKey(new Color(0.30946895f, 0.0f, 0.0f, 1.00f),gradientTempToTime(680.0f)),new GradientColorKey(new Color(0.61720675f, 0.0f, 0.0f, 1.0f),gradientTempToTime(770.0f)),
    new GradientColorKey(new Color(0.8631574f, 0.0f, 0.0f, 1.00f),gradientTempToTime(850.0f)),new GradientColorKey(new Color(1.0f, 0.32314324f, 0.03071345f, 1.0f), gradientTempToTime(950.0f)),
    new GradientColorKey(new Color(1.0f, 0.5840786f, 0.0f, 1.0f), gradientTempToTime(1000.0f)),
    new GradientColorKey(new Color(1.0f, 1.0f, 0.03071345f, 1.0f), gradientTempToTime(1100.0f)),new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), gradientTempToTime(1300.0f))};
    private GradientAlphaKey[] incanAlphaKeys = new GradientAlphaKey[8];
    #endregion
    // Start is called before the first frame update
    void Start(){
        //Making sure the metal Obj at a starting temp below emissive temps doesnt suddenly become incandescent
        metalBarStruct.metalTemp = GameStats.ambientTemp;
        isHeating = false;
        inWater = false;
        incanGradient = new Gradient();
        mBMaterial.DisableKeyword("_EMISSION");
        isEmitting = false;
        for(int i = 0;i < 8;i++){
            incanAlphaKeys[i].alpha = 1.00f;
            incanAlphaKeys[i].time = incanColorKeys[i].time;
        }
        incanGradient.SetKeys(incanColorKeys, incanAlphaKeys);
        //(Thermal conductivity * surface area)/(mass * specific heat * thickness)
        curCoolingConst = (metalBarStruct.thermConduct*metalBarStruct.surfaceArea)*(metalBarRB.mass*metalBarStruct.specificHeat*metalBarStruct.normalDepth);
    }
    void FixedUpdate(){
        inAir = (!isHeating && !inWater);
        if(metalBarStruct.metalTemp < 550.0f){
            mBMaterial.DisableKeyword("_EMISSION");
            isEmitting = false;
            }
        else if(metalBarStruct.metalTemp >= 550.0f && !isEmitting){
                isEmitting = true;
            }
        if(isEmitting){
            mBMaterial.EnableKeyword("_EMISSION");
            mBMaterial.SetColor("_EmissionColor", Color.Lerp(mBMaterial.color,incanGradient.Evaluate(gradientTempToTime(metalBarStruct.metalTemp)), 1.0f));
        };
        //In the act of heating metal bar
        if(isHeating){
            if(heatingTimer == 0.0f)
                initTemp = metalBarStruct.metalTemp;
            heatingTimer += Time.deltaTime;
            if (mGlobalTimer.OneSecPassed){     
                //Newtons Cooling Law for heating
                //T(t)= Ambient Temp - (Ambient Temp - Objects init temp) * e^(-k*t)
                metalBarStruct.metalTemp = NewtonsCoolingEquation.heatObj(forgeController.ForgeTemp, initTemp, curCoolingConst, heatingTimer, metalBarStruct.metalType);          
            }
        }
        else if((inAir || inWater) && metalBarStruct.metalTemp > GameStats.ambientTemp){
            //Debug.Log($"The metal bar is being cooled is {metalBarStruct.metalTemp} at {heatingTimer}.");
            envTemp = (inWater)?quenchingController.LiquidTemp:GameStats.ambientTemp;
            if(heatingTimer == 0.0f)
                initTemp = metalBarStruct.metalTemp;
            heatingTimer += Time.deltaTime;
            if (mGlobalTimer.OneSecPassed){
                //Newtons Cooling Law
                //T(t)= Ambient Temp + (Objects init temp - Ambient Temp ) * e^(-k*t)
                 metalBarStruct.metalTemp = NewtonsCoolingEquation.coolObj(envTemp, initTemp, curCoolingConst, heatingTimer, metalBarStruct.metalType);
            }
        }
    }
}

