using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DoorWay : Interactable
{
    void Start()
    {
        
    }

    public override void Interact()
    {
        SceneManager.LoadScene("RoverGame");
    }
}
