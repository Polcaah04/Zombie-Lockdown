using System.Runtime.InteropServices;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player; 
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    Camera m_Camera;

    private void Start()
    {
        m_Camera = GetComponent<Camera>();
        GameManager.GetGameManager().SetCamera(this);
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    public void ReduceFOV()
    {
        m_Camera.orthographicSize = 5;
    }

    public void IncreaseFOV()
    {
        m_Camera.orthographicSize = 7;
    }
}
