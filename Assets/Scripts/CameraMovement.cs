using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public PlayerController _player;
    
    

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(_player.transform.position.x , _player.transform.position.y,transform.position.z);
    }
}
