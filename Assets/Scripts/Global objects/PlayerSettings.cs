using UnityEngine;

public class PlayerSettings
{
    public float Speed = 5f;
    public float JumpHeight = 2f;
    public GameObject WeaponObject;

    public PlayerSettings(float speed, float jumpHeight, GameObject weaponObject)
    {
        Speed = speed;
        JumpHeight = jumpHeight;
        WeaponObject = weaponObject;
    }
}
