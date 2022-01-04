using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BlackSmithController : MonoBehaviour{
    #region "Private Serialized Fields"
    [SerializeField] private Camera playerCam;
    [Header("Tongs player will use to pick up metal bars")]
    [SerializeField] private  GameObject tool;
    [Header("Reference to the object the player is currently holding, DNI: tools.")]
    [SerializeField] private GameObject holdingObj;
    [Header("TextBox that holds the message of prompting user interaction.")]
    [SerializeField] private Text interactText;
    [Header("Script attached player's UI, informs the player on the properties on metal bar in their LOS.")]
    [SerializeField] private MetalInfoController metalInfoContr;
    [Header("The minimum distance the player has to be in order to interact with an obj.")]
    [SerializeField] private float minDist;
    [Header("The transform we want the obj being 'held' to take relative to its new parent the player.")]
    [SerializeField] private Transform holdingPos;
    #endregion
    #region "Private Fields + Methods
    //Reference to info about the obj that is hit by the raycast down player's LOS
    private RaycastHit hit;
    //calculated dist from player to the object in its LOS
    private float dist;
    //Rigidbody of obj being held
    private Rigidbody objRB;
    //script attached to the metal bar we are holding
    private MetalBarController mBContr;
    //bool to tell if we are carrying an obj at the moment
    private bool isCarrying;
    #endregion
    //This function makes sure that the object we are holding, maintains the same position and rotation relatiev to the position of the blacksmiths tool
    private void holdingObject(){
        holdingObj.transform.position = holdingPos.position;
        holdingObj.transform.SetParent(holdingPos);
        objRB.constraints = RigidbodyConstraints.FreezeAll;
    }
    //fun the update the users UI so that they are given stats about the metal bar they are looking at and close enough too
    public void updateMetalInfo(){
        metalInfoContr.metalTemp = decimal.Round((decimal)mBContr.MetalBarStruct.metalTemp,2).ToString();
        metalInfoContr.metalType = mBContr.MetalBarStruct.metalType;
    }
    // Start is called before the first frame update
    void Start(){
        interactText.enabled = false;
        isCarrying = false;
    }
    // Update is called once per frame
    void Update(){
        //Reference the ray that is once per frame being fired through the center of the lense
        //Did the ray hit anything? if so store it in hit
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        if(Physics.Raycast(ray, out hit)){
            //Debug.Log($"{hit.transform.name} is on your LOS.");
            //Objects in our line of sight, but are we close enough to it?
            dist = Vector3.Distance(tool.transform.position, hit.transform.position);
            interactText.enabled = dist <= minDist && hit.collider.tag == "Metal Bars";
        }
        //picking up
        if(interactText.enabled && !isCarrying){
            //Obj to hold + components attached to it are referenced for future use
            //References are gathered here so that when player is looking at the metal bar they are given info about in in their UI
            holdingObj = hit.collider.gameObject;
            objRB = holdingObj.GetComponent<Rigidbody>();
            mBContr = holdingObj.GetComponent<MetalBarController>();
            updateMetalInfo();
            if(Input.GetButtonDown("Interact") && dist <= minDist) {
                isCarrying = true;
                holdingObject();
                //If we dont ignore the collision between the two objects right the metal bar will prevent us from moving forward 
                Physics.IgnoreLayerCollision(gameObject.layer, holdingObj.layer, true);
            }
        }
        //dropping
        else if(isCarrying && Input.GetButtonDown("Interact") && !mBContr.InWater){
            isCarrying = false;
            holdingObj.transform.SetParent(null);
            objRB.constraints = RigidbodyConstraints.None;
            metalInfoContr.Reset();
            Physics.IgnoreLayerCollision(gameObject.layer, holdingObj.layer, false);
        }
        //carrying obj
        else if(isCarrying){   
            //Make sure the metal bar stays in the same pos relative to the tongs
            updateMetalInfo();
        }
    }
}
