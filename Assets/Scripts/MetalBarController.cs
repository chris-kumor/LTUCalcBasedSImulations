using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MetalBarController : MonoBehaviour{
    private Gradient incanGradient;
    public MetalStruct metalBarStruct;
    public Rigidbody metalBarRB;
    //is the metal bar in the embers?
    public bool isHeating, inAir, inWater, isEmitting;
    //"dT/dt"
    private float instantTempChange, heatingTimer, initTemp, time, envTemp;
    //Reference to script that contains needed info on the forge such as temperature
    public ForgeController forgeController;
    //Reference to script that contains needed info on the forge cooler
    public QuenchingController quenchingController;
    //cooling Constant that is calculated upon instatiation and can be passed to cooling/heating equation parameters
    private float curCoolingConst;
    private static float gradientTempToTime(float metalTemp){
        return ((metalTemp - 550.0f)/750.0f);
    }
    private GradientColorKey[] incanColorKeys = {new GradientColorKey(new Color(0.12743768f, 0.0f, 0.0f, 1.00f), 0.0f), 
    new GradientColorKey(new Color(0.30946895f, 0.0f, 0.0f, 1.00f),gradientTempToTime(680.0f)),new GradientColorKey(new Color(0.61720675f, 0.0f, 0.0f, 1.0f),gradientTempToTime(770.0f)),
    new GradientColorKey(new Color(0.8631574f, 0.0f, 0.0f, 1.00f),gradientTempToTime(850.0f)),new GradientColorKey(new Color(1.0f, 0.32314324f, 0.03071345f, 1.0f), gradientTempToTime(950.0f)),
    new GradientColorKey(new Color(1.0f, 0.5840786f, 0.0f, 1.0f), gradientTempToTime(1000.0f)),
    new GradientColorKey(new Color(1.0f, 1.0f, 0.03071345f, 1.0f), gradientTempToTime(1100.0f)),new GradientColorKey(new Color(1.0f, 1.0f, 1.0f, 1.0f), gradientTempToTime(1300.0f))};
    private GradientAlphaKey[] incanAlphaKeys = new GradientAlphaKey[8];
    public Material mBMaterial;
    //when object is not being heated or cooled
    public void resetTimer(){
        heatingTimer = 0.0f;
    }
    // Start is called before the first frame update
    void Start(){
        incanGradient = new Gradient();
        //GameStats.ambientTemp = 27.0f;
        //resetTimer();
        GameStats.ambientTemp = 22.0f;
        metalBarStruct.metalTemp = GameStats.ambientTemp;
        //(Thermal conductivity * surface area)/(mass * specific heat * thickness)
        curCoolingConst = (metalBarStruct.thermConduct*metalBarStruct.surfaceArea)*(metalBarRB.mass*metalBarStruct.specificHeat*metalBarStruct.normalDepth);
        //Making sure the metal Obj at a starting temp below emissive temps doesnt suddenly become incandescent
        mBMaterial.DisableKeyword("_EMISSION");
        isEmitting = false;
        for(int i = 0;i < 8;i++){
            incanAlphaKeys[i].alpha = 1.00f;
            incanAlphaKeys[i].time = incanColorKeys[i].time;
        }
        incanGradient.SetKeys(incanColorKeys, incanAlphaKeys);
        quenchingController.metalBars.Add(this.gameObject);
    }
    void FixedUpdate(){
        time += Time.deltaTime;
        isHeating = forgeController.forgeCollider.bounds.Contains(gameObject.transform.position);
        inWater = quenchingController.quenchingCollider.bounds.Contains(gameObject.transform.position);
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
            mBMaterial.SetColor("_EmissionColor", incanGradient.Evaluate(gradientTempToTime(metalBarStruct.metalTemp)));
        }
        else if(!isEmitting)
            mBMaterial.DisableKeyword("_EMISSION");
        //In the act of heating metal bar
        if(isHeating){
            envTemp = forgeController.forgeTemp;
            if(heatingTimer == 0.0f)
                initTemp = metalBarStruct.metalTemp;
            //Debug.Log(initTemp - GameStats.ambientTemp)
            heatingTimer += Time.deltaTime;
            if (time >= 1.00f){
                time = time - 1.00f;
                //Newtons Cooling Law for heating
                //T(t)= Ambient Temp - (Ambient Temp - Objects init temp) * e^(-k*t)
                metalBarStruct.metalTemp = NewtonsCoolingEquation.heatObj(envTemp, initTemp, curCoolingConst, heatingTimer, metalBarStruct.metalType);          
            }
            //Debug.Log(metalBarStruct.metalTemp);
        }
        else if(!isHeating&&metalBarStruct.metalTemp>GameStats.ambientTemp){
            if(!inWater)
                envTemp = GameStats.ambientTemp;
            if(heatingTimer == 0.0f)
                initTemp = metalBarStruct.metalTemp;
            heatingTimer += Time.deltaTime;
            if (time >= 1.00f){
                time = time - 1.00f;
                //Newtons Cooling Law
                //T(t)= Ambient Temp + (Objects init temp - Ambient Temp ) * e^(-k*t)
                 metalBarStruct.metalTemp = NewtonsCoolingEquation.coolObj(envTemp, initTemp, curCoolingConst, heatingTimer, metalBarStruct.metalType);
            }
            //Debug.Log(metalBarStruct.metalTemp);
        }
        if(inAir){
            if(heatingTimer != 0.0f)
                resetTimer();
        }
    }
}

