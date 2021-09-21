using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DoorWay : ChangeLevelInteractable
{
    void Start()
    {
        
    }

    public override void Interact()
    {
        InvokeBeforeSceneChange();
        SceneManager.LoadScene("RoverGame");
        InvokeOnSceneChange();
    }
}
