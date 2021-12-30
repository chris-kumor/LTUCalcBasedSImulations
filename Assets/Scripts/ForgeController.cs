using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeController : MonoBehaviour
{
    [SerializeField] private float forgeTemp;
    public float ForgeTemp{get{return forgeTemp;}set{forgeTemp = value;}}
    [SerializeField] private SphereCollider forgeCollider;
    public SphereCollider ForgeCollider{get{return forgeCollider;}}
}
