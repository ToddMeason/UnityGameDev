using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game_GUI : MonoBehaviour
{
    #region Variables
    public Image expBar;
    public Text levelText;
    public Image fadePlane;
    public GameObject gameOverUI;
    #endregion

    #region Builtin Methods
    void Start()
    {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
    }

    void Update()
    {
        
    }

    #endregion

    #region Custom Methods

    private void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear, Color.black, 1));
        gameOverUI.SetActive(true);
    }

    public void SetPlayerExp(float percentToLevel, int playerLevel)
    {
        levelText.text = "Level: " + playerLevel;
        expBar.rectTransform.localScale = new Vector3(percentToLevel, 1, 1);
    }

    private void UpdateExpBar()
    {

    }

    #endregion

    #region Input
    public void StartNewGame()//Added to the button ui object
    {
        SceneManager.LoadScene("RoverGame");
    }

    #endregion

    #region Coroutines
    IEnumerator Fade(Color from, Color to, float time)//Fades the fade screen ui element when you die
    {
        float speed = 1 / time;
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    #endregion
}
