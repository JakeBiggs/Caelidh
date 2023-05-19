using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHoldersScript : MonoBehaviour
{
    private void Awake()
    {
        //Destroys its mesh filter so it cannot be seen as it is only visual in editor so you can see where you are placing the placeholder properly
        Destroy(GetComponent<MeshFilter>());
    }
}
