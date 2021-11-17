using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu, controlsMenu, objectiveMenu, pauseMenu;
    public GameObject mainFirstButton, controlsFirstButton, objectiveFirstbutton, pauseFirstbutton;

    [SerializeField] private Player player;
    [SerializeField] private SaveAndLoad saveAndLoad;


    void Start()
    {
        OpenMainMenu();
        player = FindObjectOfType<Player>().GetComponent<Player>();
        saveAndLoad = FindObjectOfType<SaveAndLoad>().GetComponent<SaveAndLoad>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            PauseUnpause();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("RoverGame");
    }

    public void PauseUnpause()
    {
        if (!pauseMenu.activeInHierarchy)
        {
            player.roverController.stunned = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(pauseFirstbutton);
        }
        else
        {
            player.roverController.stunned = false;
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
            controlsMenu.SetActive(false);
        }
    }

    public void OpenMainMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(mainFirstButton);
    }

    public void OpenControls()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controlsFirstButton);
    }

    public void OpenObjective()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(objectiveFirstbutton);
    }

    public void OpenPause()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseFirstbutton);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        saveAndLoad.Clear();
        Application.Quit();
    }
}
