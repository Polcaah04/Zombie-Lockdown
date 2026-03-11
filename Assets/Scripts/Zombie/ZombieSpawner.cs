using System.Collections;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [SerializeField] private GameObject m_Zombie;
    [HideInInspector] public float m_MaxSpawnRateTime = 5f;
    

    private Vector2[] m_SpawnPositionsList = new Vector2[4];

    void Start()
    {
        GameManager.GetGameManager().RegisterSpawner(this);

        m_SpawnPositionsList[0] = new Vector2(transform.position.x + 1, transform.position.y + 1);
        m_SpawnPositionsList[1] = new Vector2(transform.position.x - 1, transform.position.y - 1);
        m_SpawnPositionsList[2] = new Vector2(transform.position.x + 1, transform.position.y - 1);
        m_SpawnPositionsList[3] = new Vector2(transform.position.x - 1, transform.position.y + 1);

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (GameManager.GetGameManager().GetState() == GameManager.TState.PLAYINGROUNDS)
            {
                if (GameManager.GetGameManager().CanSpawnZombie())
                {
                    GameManager.GetGameManager().RegisterZombieSpawn();
                    yield return new WaitForSeconds(Random.Range(1f, m_MaxSpawnRateTime));

                    Vector2 spawnPos = m_SpawnPositionsList[Random.Range(0, m_SpawnPositionsList.Length)];

                    Instantiate(m_Zombie, spawnPos, Quaternion.identity);
                }
                else
                {
                    yield return new WaitForSeconds(1f);
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    public void IncreaseSpawnRate(float value)
    {
        m_MaxSpawnRateTime *= value;
    }

    public GameObject AddSpawnerToList()
    {
        return this.gameObject;
    }

}
