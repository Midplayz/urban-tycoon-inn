using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; 
    public Vector3 positionOffset;
    public Vector3 rotationOffset; 

    private void LateUpdate()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        transform.position = player.position + positionOffset;

        transform.rotation = Quaternion.Euler(rotationOffset);
    }
}
