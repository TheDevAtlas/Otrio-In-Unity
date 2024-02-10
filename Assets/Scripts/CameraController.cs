using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cam;
    public float rotateSpeed;
    public OtrioController otrio;
    public float smoothSpeed = 0.125f;
    public Quaternion gameRotation;

    private void FixedUpdate()
    {
        if (!otrio.isPlay)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.fixedDeltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, gameRotation, smoothSpeed);

            gameRotation = Quaternion.identity;
        }
        
    }
}
