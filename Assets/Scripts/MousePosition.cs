using UnityEngine;

public class MousePosition : MonoBehaviour
{
    private static Vector3 mouse;
    private static Camera _camera;
    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static Vector3 GetMousePosition()
    {
        mouse = _camera.ScreenToWorldPoint(Input.mousePosition);
        return mouse;
    }
}
