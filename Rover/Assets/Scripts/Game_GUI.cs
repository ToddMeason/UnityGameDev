using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_GUI : MonoBehaviour
{
    #region Variables
    public Image expBar;
    public Text levelText;
    #endregion

    #region Builtin Methods
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    #endregion

    #region Custom Methods
    public void SetPlayerExp(float percentToLevel, int playerLevel)
    {
        levelText.text = "Level: " + playerLevel;
        expBar.rectTransform.localScale = new Vector3(percentToLevel, 1, 1);
    }

    private void UpdateExpBar()
    {

    }

    #endregion
}
