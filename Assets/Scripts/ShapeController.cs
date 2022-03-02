using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(MeshFilter))]
public class ShapeController : MonoBehaviour
{
    ///<summary>
    /// Folowing code based on Mesh Deformer script created following blog in following link
    /// https://catlikecoding.com/unity/tutorials/mesh-deformation/
    /// [SerializeField] private Mesh mbMesh;
    /// mbMesh = gameObject.GetComponent<MeshFilter>().mesh;
    /// Above Link and comented out components are there for tools in future implement of how objs will change shape
    ///</summary>

    #region "Private serialized fields"
    [SerializeField] private MetalStruct mbStruct;
    #endregion
    //Changing shape of obj if deemed appropriate
    public void ChangeShape(float stress, Vector3 compressionDir){
        Debug.Log($"{transform.name} is changing shape.");
        // dl = stress * original length / Modulus of Elasticity
        float elongScale1, elongScale2, compScale;  
        int isParallel = (Vector3.Cross(transform.forward, compressionDir) == Vector3.zero)?1:(Vector3.Cross(transform.right, compressionDir) == Vector3.zero)?2:(Vector3.Cross(transform.up, compressionDir) == Vector3.zero)?3:0;
        switch(isParallel){
            //The local Z axis of the metal bar is || to z axis of the hammer head surface
            case 1:
                elongScale1 = transform.localScale.x;
                elongScale2 = transform.localScale.y;
                compScale = transform.localScale.z;
                break;
            //local x axis || to z axis of hammer head
            case 2:
                elongScale1 = transform.localScale.z;
                elongScale2 = transform.localScale.y;
                compScale = transform.localScale.x;
                break;
            //local y axis || z axis on hammer head
            case 3:
                elongScale1 = transform.localScale.z;
                elongScale2 = transform.localScale.x;
                compScale = transform.localScale.y;
                break;
            //Incase || axis of hammer head normal and metal surface normal cant be found or if false trigger occurs
            default:
                elongScale1 = 0.0f;
                elongScale2 = 0.0f;
                compScale = 0.0f;
                break;
        }
        //In future adding temp of obj to take into consideration
        elongScale1 += (stress*elongScale1)/mbStruct.youngsModulus;
        elongScale2 += (stress*elongScale2)/mbStruct.youngsModulus;
        compScale -= (stress*compScale)/mbStruct.youngsModulus;
    }
}
