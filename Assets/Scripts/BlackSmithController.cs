using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BlackSmithController : MonoBehaviour{
    //Sorting reference to camera that is players vision 
    public Camera playerCam;
    //Tongs player will use to pick up metal bars
    public GameObject tool;
    //minimum distance we want the player to be before they can interact with a particular obj
    private GameObject holdingObj;
    public float minDist;
    //Reference the the object that is in the players LOS
    //Invisible ray is casted from the camera down the players LOS
    //When this ray hits an object and some info about the object is stored in hit
    private RaycastHit hit;
    //calculated dist from player to the object in its LOS
    private float dist;
    //TextBox that holds the message of prompting user interaction
    public Text interactText;
    public MetalInfoController metalInfoContr;
    //location we want the metal bars to be when it is held by the tongs
    public Transform holdingPos;
    //Rigidbody of obj being held
    private Rigidbody objRB;
    private MetalBarController mBContr;
    public QuenchingController quenchCont;
    private bool isCarrying;
    private void holdingObject(){
        holdingObj.transform.position = Vector3.Lerp(holdingObj.transform.position,holdingPos.position,Time.time);
        holdingObj.transform.rotation = Quaternion.Lerp(holdingObj.transform.rotation,holdingPos.rotation,Time.time);
    }
    public void updateMetalInfo(){
        metalInfoContr.metalTemp = decimal.Round((decimal)mBContr.metalBarStruct.metalTemp,2).ToString();
        metalInfoContr.metalType = mBContr.metalBarStruct.metalType;
    }
    // Start is called before the first frame update
    void Start(){
        interactText.enabled = false;
        isCarrying = false;
        GameStats.ambientTemp = 22.0f; 
    }
    // Update is called once per frame
    void Update(){
        //Reference the ray that is once per frame is being fired through the camera
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.yellow);
        //tool.transform.rotation = Quaternion.Euler(ray.direction.x, ray.direction.y, ray.direction.z);
        //Did the ray hit anything? if so store it in hit
        if(Physics.Raycast(ray, out hit)){
            //Objects in our line of sight, but are we close enough to it?
            dist = Vector3.Distance(tool.transform.position, hit.transform.position);
            interactText.enabled = dist <= minDist && hit.collider.tag == "Metal Bars";
        }
        //picking up
        if(interactText.enabled && !isCarrying){
            holdingObj = hit.collider.gameObject;
            objRB = holdingObj.GetComponent<Rigidbody>();
            mBContr = holdingObj.GetComponent<MetalBarController>();
            updateMetalInfo();
            if(Input.GetButtonDown("Interact") && dist <= minDist) {
                isCarrying = true;
                mBContr.isHeating = false;
                if(mBContr.inWater)
                    quenchCont.isQuenching = false;
                mBContr.inWater = false;
                mBContr.inAir = true;
                mBContr.resetTimer();
                //If we dont ignore the collision between the two objects right the metal bar will prevent us from moving forward 
                Physics.IgnoreLayerCollision(gameObject.layer, holdingObj.layer, true);
                objRB.constraints = RigidbodyConstraints.FreezeRotation;
            }
        }
        //dropping
        else if(isCarrying && Input.GetButtonDown("Interact") && !quenchCont.isQuenching){
            isCarrying = false;
            objRB.constraints = RigidbodyConstraints.None;
            metalInfoContr.Reset();
            mBContr.resetTimer();
            //Since we are not carrying the metal bar anymore we dont want to just simply phase through it or vice versa5
            Physics.IgnoreLayerCollision(gameObject.layer, holdingObj.layer, false);
            
        }
        //carrying obj
        else if(isCarrying){   
            //Make sure the metal bar stays in the same pos relative to the tongs
            holdingObject();
            objRB.constraints = RigidbodyConstraints.FreezePosition;
            updateMetalInfo();
        }
    }
}
