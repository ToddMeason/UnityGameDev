using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveAndLoad : MonoBehaviour
{
    public Player player;
    public Gun[] guns;

    void Start()
    {
        player = GetComponent<Player>();
    }

    #region Saving/Loading 
    //Should probably move to seperate class later
    [ContextMenu("Save")]
    private void Save()
    {
        PlayerPrefs.SetInt("level", player.level);
        PlayerPrefs.SetFloat("currentExp", player.currentLevelExp);//may need exp to level

        PlayerPrefs.SetFloat("currentCurrency", player.currentCurrency);

        PlayerPrefs.SetInt("gunId", player.gun.ID);


        //Saveplayer stats somehow later once they are setup properly
    }

    [ContextMenu("Load")]
    private void Load()
    {
        player.level = PlayerPrefs.GetInt("level");
        player.currentLevelExp = PlayerPrefs.GetFloat("currentExp");
        player.currentCurrency = PlayerPrefs.GetFloat("currentCurrency");

        for (int i = 0; i < guns.Length; i++)
        {
            if (guns[i].ID == PlayerPrefs.GetInt("gunId"))
            {
                GetComponent<Rover.Basic.Rover_GunController>().EquipGun(guns[i]);
                return;
            }
        }

        player.OnLoad();
    }

    [ContextMenu("Clear")]
    private void Clear()
    {
        PlayerPrefs.SetInt("level", 0);
        PlayerPrefs.SetFloat("currentExp", 0);
        PlayerPrefs.SetFloat("currentCurrency", 0);
    }

    #endregion
}
