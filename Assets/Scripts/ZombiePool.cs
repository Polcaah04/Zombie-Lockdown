using System.Collections.Generic;
using UnityEngine;

public class ZombiePool : MonoBehaviour
{
    public static ZombiePool Instance;

    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private int maxZombies = 30;

    private List<GameObject> pool = new List<GameObject>();
    private int activeZombies = 0;

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < maxZombies; i++)
        {
            GameObject zombie = Instantiate(zombiePrefab);
            zombie.SetActive(false);
            pool.Add(zombie);
        }
    }

    public GameObject GetZombie()
    {
        if (activeZombies >= maxZombies)
            return null;

        foreach (var z in pool)
        {
            if (!z.activeInHierarchy)
            {
                z.SetActive(true);
                activeZombies++;
                return z;
            }
        }

        return null;
    }

    public void ReturnZombie(GameObject zombie)
    {
        zombie.SetActive(false);
        activeZombies--;
    }

    public int GetActiveCount()
    {
        return activeZombies;
    }
}