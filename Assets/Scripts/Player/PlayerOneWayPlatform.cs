using System.Collections;
using UnityEngine;

public class PlayerOneWayPlatform : MonoBehaviour
{
    private GameObject _currentOneWayPlatform;
    public bool _isReady;

    [SerializeField] private Collider2D myCollider;
    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<Collider2D>();
    }



    public void JumpDown()
    {
        if (_currentOneWayPlatform != null)
        {
            StartCoroutine(DisableCollider());
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("OneWayPlatform"))
        {
            _isReady = true;
            _currentOneWayPlatform = col.gameObject;
        }
    }
    
    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("OneWayPlatform"))
        {
            _isReady = false;
            _currentOneWayPlatform = null;
        }
    }

    private IEnumerator DisableCollider()
    {
        Collider2D platformCollider = _currentOneWayPlatform.GetComponent<Collider2D>();
        
        Physics2D.IgnoreCollision(myCollider, platformCollider);
        yield return new WaitForSeconds(0.25f);
        Physics2D.IgnoreCollision(myCollider, platformCollider, false);
    }
}
