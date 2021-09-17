using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DoorWay : Interactable
{
    public delegate void OnSceneChange();
    public static event OnSceneChange OnSceneChanged;// change to gamemanager load later

    public delegate void BeforeSceneChange();
    public static event BeforeSceneChange BeforeSceneChanged;// change to gamemanager load later

    void Start()
    {
        
    }

    public override void Interact()
    {
        BeforeSceneChanged?.Invoke();
        SceneManager.LoadScene("RoverGame");
        OnSceneChanged?.Invoke();
    }
}
