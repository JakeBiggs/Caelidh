using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubDoorScript : MonoBehaviour
{
    private GameControllerScript gc;
    // Start is called before the first frame update
    void Start()
    {
        gc = GameObject.Find("GameController").GetComponent<GameControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Player")
        {
            gc.currentLevel++;
            gc.usedDoorPosition = GameControllerScript.DoorPositions.South;
            SceneManager.LoadScene("LevelScene");
            GameObject.Find("Player").transform.position = new Vector3(0, 0, -7.95f);
        }
    }
}
