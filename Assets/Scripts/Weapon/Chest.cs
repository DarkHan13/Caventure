using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private List<GameObject> _items;
    void Start()
    {
        _animator = GetComponent<Animator>();
        if (_items == null) _items = new List<GameObject>();
        List<GameObject> updateItems = new List<GameObject>();
        foreach (GameObject obj in _items)
        {
            GameObject gm = Instantiate(obj, transform);
            Item item = gm.GetComponent<Item>();
            item.Init();
            item.SetActivePhysics(false);
            item.transform.position = transform.position;
            updateItems.Add(gm);
        }
        _items = updateItems;
    }

    public void Open()
    {
        _animator.SetTrigger("open");
        foreach (GameObject obj in _items)
        {
            Item item = obj.GetComponent<Item>();
            item.SetActivePhysics(true);
            item.transform.parent = null;
            item.transform.position = transform.position;
        }
    }
}
