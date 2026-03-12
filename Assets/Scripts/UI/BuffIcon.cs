using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuffIcon : MonoBehaviour
{
    [SerializeField] private Image m_Image;

    public void Init(Sprite icon, float duration)
    {
        m_Image.sprite = icon;
        StartCoroutine(Remove(duration));
    }

    IEnumerator Remove(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}