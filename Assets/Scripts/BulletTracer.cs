using UnityEngine;
using System.Collections;

public class BulletTracer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public float duration = 0.1f; 

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Init(Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        StartCoroutine(DestroyAfter());
    }

    IEnumerator DestroyAfter()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }
}
