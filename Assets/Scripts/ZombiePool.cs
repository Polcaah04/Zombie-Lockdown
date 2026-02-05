using System.Collections.Generic;
using UnityEngine;

public class ZombiePool : MonoBehaviour 
{
    List<Zombie> m_ZombieList = new List<Zombie>();
    int m_MaxZombiesOnMap = 30;

    void Update()
    {
        if (m_ZombieList.Count < m_MaxZombiesOnMap)
        {

        }
    }

}
