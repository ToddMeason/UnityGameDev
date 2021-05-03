using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{
    #region Variables
    private int level;
    private float currentLevelExp;
    private float expToLevelUp;

    private Game_GUI gui;

    #endregion

    #region Builtin Methods
    public override void Start()
    {
        base.Start();
        gui = GameObject.Find("ExpBarBackground").GetComponent<Game_GUI>();//Finds direct gameobject not ideal but works
        LevelUp();
    }
    #endregion

    #region Custom Methods
    public void AddExp(float exp)
    {
        currentLevelExp += exp;
        if (currentLevelExp >= expToLevelUp)
        {
            currentLevelExp -= expToLevelUp;
            LevelUp();
        }

        gui.SetPlayerExp(currentLevelExp / expToLevelUp, level);
    }

    private void LevelUp()
    {
        level++;
        expToLevelUp = level * 50 + Mathf.Pow(level * 2, 2);

        AddExp(0);
    }

    #endregion
}
