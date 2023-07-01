using UnityEngine;

public class Hand : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public bool isPickUp;
    public Weapon _weapon;
    void Awake()
    {
        Weapon tmpWeapon = GetComponentInChildren<Weapon>();
        if (tmpWeapon != null)
        {
            _weapon = tmpWeapon;
            isPickUp = true;
        }
    }

    public void SetWeapon(GameObject thing, bool isRight)
    {
        Weapon newWeapon = thing?.GetComponent<Weapon>();
        if (newWeapon == null) return;
        if (!isRight) thing.transform.localScale = new Vector3(-1, 1, 1);
        thing.transform.position = transform.position;
        thing.transform.parent = transform;
        Debug.Log(newWeapon);
        _weapon = newWeapon;
        _weapon.SetActivePhysics(false);
        isPickUp = true;
    }

    public void SpriteEnable(bool enable)
    {
       _weapon.spriteRenderer.enabled = enable;
    }
    public void ColliderEnable(bool enable)
    {
        _weapon.collider.enabled = enable;
        _weapon.collider.isTrigger = enable;
    }

    public GameObject GetWeaponGameObject()
    {
        if (_weapon == null) return null;
        return _weapon.gameObject;
    }
}
