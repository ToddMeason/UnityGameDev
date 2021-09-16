using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DoorWay : Interactable
{
    public delegate void OnSceneChange();
    public static event OnSceneChange OnSceneChanged;// change to gamemanager load later //change to delegates 

    void Start()
    {
        
    }

    public override void Interact()
    {
        SceneManager.LoadScene("RoverGame");
        OnSceneChanged?.Invoke();
    }
}
