using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maleability : MonoBehaviour{
    #region "Private Serialized Fields"
    [SerializeField] private Rigidbody objRB;
    [SerializeField] private float areaofImpact;
    [SerializeField] private Vector3 initVelocity;
    
    #endregion
    // Start is called before the first frame update
    void Start(){
        objRB = transform.parent.gameObject.GetComponent<Rigidbody>();
        initVelocity = objRB.velocity;
        //Deriving surface area of for impact(s)
        areaofImpact = Mathf.PI * Mathf.Pow(gameObject.GetComponent<CircleCollider2D>().radius, 2.0f);
        initVelocity = objRB.velocity;
    }
    void Update(){
        initVelocity = objRB.velocity;
    }
    void OnTriggerEnter(Collider other){
        if(other.CompareTag("Metal Bars"))
            other.GetComponent<ShapeController>().ChangeShape(DeriveStress(), transform.forward);
    }
    public float DeriveStress(){
        //strain = external force of hammer/area of impact
        return (objRB.mass*((objRB.velocity-initVelocity)/Time.deltaTime).magnitude)/areaofImpact;
    }
}