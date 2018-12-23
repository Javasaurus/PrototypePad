using UnityEngine;

/// <summary>
/// A very...VERY simple and primitive camera to allow for minimal movement during
/// debugging. This is by no means a fully operational game camera
/// </summary>
public class CameraController : MonoBehaviour
{
    public float panningSpeed = 5f;

    // Update is called once per frame
    private void Update()
    {
        Vector3 movement = Vector3.zero;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movement.x += panningSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement.x -= panningSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            movement.z += panningSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            movement.z -= panningSpeed * Time.deltaTime;
        }
        transform.position += movement;
    }

}
