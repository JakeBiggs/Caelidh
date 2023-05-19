using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{

    private float timeAtStart;
    public float lifetime;
    // Start is called before the first frame update
    void Start()
    {
        timeAtStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > timeAtStart + lifetime)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.transform.parent.GetComponent<HealthManagement>().Damage(((int)GameObject.Find("GameController").GetComponent<GameControllerScript>().levelDifficulty + 1) * 0.5f);
            Destroy(gameObject);
        }
    }
}
