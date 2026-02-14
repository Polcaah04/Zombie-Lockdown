using System.Collections;
using System.Net.NetworkInformation;
using UnityEditor;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    private GameManager.TState m_State;
    [SerializeField] private GameObject m_Zombie;
    private Vector2[] m_SpawnPositionsList = new Vector2[4];

    void Start()
    {
        m_SpawnPositionsList[0] = new Vector2 (transform.position.x + 1, transform.position.z + 1);
        m_SpawnPositionsList[1] = new Vector2(transform.position.x - 1, transform.position.z - 1);
        m_SpawnPositionsList[2] = new Vector2(transform.position.x + 1, transform.position.z - 1);
        m_SpawnPositionsList[3] = new Vector2(transform.position.x - 1, transform.position.z + 1);
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (m_State == GameManager.TState.PLAYINGROUNDS)
        {
            if (GameManager.GetGameManager().GetRoundTime() < 30 && GameManager.GetGameManager().GetRoundTime() > 3)
            {
                yield return new WaitForSeconds(Random.Range(1f, 5f));

                Vector2 spawnPos = m_SpawnPositionsList[Random.Range(0, m_SpawnPositionsList.Length)];
                Instantiate(m_Zombie, spawnPos, Quaternion.identity);
            }
            else
            {
                yield return null;
            }
        }
    }
}
