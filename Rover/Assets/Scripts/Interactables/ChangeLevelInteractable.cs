using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLevelInteractable : Interactable
{

    public delegate void OnSceneChange();
    public static event OnSceneChange OnSceneChanged;// change to gamemanager load later

    public delegate void BeforeSceneChange();
    public static event BeforeSceneChange BeforeSceneChanged;// change to gamemanager load later

    public void InvokeOnSceneChange()
    {
        OnSceneChanged?.Invoke();
    }

    public void InvokeBeforeSceneChange()
    {
        BeforeSceneChanged?.Invoke();
    }
}
