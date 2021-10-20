using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Game_GUI : MonoBehaviour
{
    #region Variables
    public Image expBar;
    public Image healthBar;
    public Image boostBar;
    public Text levelText;
    public Image fadePlane;
    public TextMeshProUGUI ammo;
    public TextMeshProUGUI currency;
    public TextMeshProUGUI objectiveTimer;
    public GameObject gameOverUI;
    public Player player;
    #endregion

    #region Builtin Methods
    private void Awake()
    {
        player = FindObjectOfType<Player>().GetComponent<Player>();
    }

    void Start()
    {
        player.OnDeath += OnGameOver;
        objectiveTimer.enabled = false;
    }

    void Update()
    {

    }

    #endregion

    #region Custom Methods

    public void ShowHealth(float healthPercent)
    {         
        healthBar.rectTransform.localScale = new Vector3(player.healthPercent, 1, 1);
    }

    public void ShowAmmo(float ammoCount)
    {
        ammo.text = ammoCount.ToString();
    }

    public void ShowCurrency(float currentCurrency)
    {
        currency.text = currentCurrency.ToString();
    }

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

    public void ShowBoost(bool boostAvailable)
    {
        if (boostAvailable)
        {
            boostBar.enabled = true;
        }
        else
        {
            boostBar.enabled = false;
        }
    }

    public void ShowObjectiveTimer(float time)
    {
        objectiveTimer.enabled = true;
        objectiveTimer.text = "Objective Charging: " + time.ToString("F2");
    }

    #endregion

    #region Input
    public void StartNewGame()//Added to the button ui object
    {
        SceneManager.LoadScene("RoverGame");
    }

    #endregion

    #region Events
    public void OnEnable()
    {
        player.OnHealthChanged += ShowHealth;
        Gun.OnAmmoChanged += ShowAmmo;
        Player.OnExpChanged += SetPlayerExp;
        Player.OnCurrencyChanged += ShowCurrency;
        Rover.Basic.Rover_Controller.OnBoostUsed += ShowBoost;
        MainObjective.OnTimerChange += ShowObjectiveTimer;
    }

    public void OnDisable()
    {
        player.OnHealthChanged -= ShowHealth;
        Gun.OnAmmoChanged -= ShowAmmo;
        Player.OnExpChanged -= SetPlayerExp;
        Player.OnCurrencyChanged -= ShowCurrency;
        Rover.Basic.Rover_Controller.OnBoostUsed -= ShowBoost;
        MainObjective.OnTimerChange -= ShowObjectiveTimer;
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
