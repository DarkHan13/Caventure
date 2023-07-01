using System;
using UnityEngine;

public class GlobalObject : MonoBehaviour
{
    public PlayerSettings PlayerSettings;
    public GameObject tmpWeapon;
    public static GlobalObject Instance;
    

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
            PlayerSettings = new PlayerSettings(5f, 3f, null);
        } else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerSettings(PlayerSettings playerSettings)
    {
        PlayerSettings = playerSettings;
        if (playerSettings.WeaponObject != null)
            playerSettings.WeaponObject.transform.parent = transform;
    }

    public GameObject GetWeapon()
    {
        if (tmpWeapon != null) return tmpWeapon;
        return PlayerSettings.WeaponObject;
    }
}
