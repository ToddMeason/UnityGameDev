using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()//need to add a UIloaded event to call things after the UI is loaded not before
    {
        if (SceneManager.GetSceneByName("UI").isLoaded == false)
        {
            SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
        }
        else
        {
            SceneManager.UnloadSceneAsync("UI");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
