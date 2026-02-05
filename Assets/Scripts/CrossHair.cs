using UnityEngine;

public class CrossHair : MonoBehaviour
{
    void Update()
    {
        Vector3 l_Position = Input.mousePosition;
        l_Position.z = 1f;

        Vector3 l_WorldPos = Camera.main.ScreenToWorldPoint(l_Position);
        transform.position = l_WorldPos;
    }
}
