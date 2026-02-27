using UnityEngine;

public class Item : MonoBehaviour
{
    public virtual void Pick(PlayerController player)
    {
        GameObject.Destroy(gameObject);
    }
}
