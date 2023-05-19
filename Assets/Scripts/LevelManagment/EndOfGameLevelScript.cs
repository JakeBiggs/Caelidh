using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfGameLevelScript : MonoBehaviour
{
    GameControllerScript gc;
    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.Find("GameController").GetComponent<GameControllerScript>();
        foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            DoorScript script = go.GetComponent<DoorScript>();
            if (script != null)
            {
                //print("Found: " + script.doorPosition + " Needs: " + oppositeDoor);
                if (script.doorPosition != gc.usedDoorPosition)
                {
                    Destroy(go);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
