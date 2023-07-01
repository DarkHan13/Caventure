using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public bool isGrounded;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer is 3 or 9) isGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 3) isGrounded = false;
    }
}
