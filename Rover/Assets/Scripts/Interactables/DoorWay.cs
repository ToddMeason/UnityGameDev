using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DoorWay : Interactable
{
    public event System.Action OnSceneChange;// change tot gamemanager load later

    void Start()
    {
        
    }

    public override void Interact()
    {
        SceneManager.LoadScene("RoverGame");
        OnSceneChange?.Invoke();
    }
}
