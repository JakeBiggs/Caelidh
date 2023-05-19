using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class billboardingFAKE : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.eulerAngles = new Vector3(90, 0, 0);
        this.transform.position = new Vector3(this.transform.position.x, 0.5f, this.transform.position.z);
    }
}
