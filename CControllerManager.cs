using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*******************************************
 * Filename:            CControllerManager.cs
 * Date:                09/1/2021
 * Mod. Date:           09/19/2021
 * Mod. Initials:       MRHA
 * Author:              Matthew Hoekstra-Abel
 * Purpose:             Pool for enemies and enemy management.
 ******************************************/
public class CControllerManager : MonoBehaviour
{
    /***************************************
    * TEnemyInformation:    A struct for holding enemy gameobjects and their controllers
    * 
    * m_nEnemyPoolSize:     The desired number of enemies to store 
    * m_tcEnemyPool:        A pool for storing all enemies (including future variants)
    * m_cEnemyPrefab:       The prefab you wish to instantiate
    * m_sEnemyCount:        A short denoting the number of enemies still alive
    ***************************************/
    public struct TEnemyInformation
    {
        public GameObject m_cEnemyObject;
        public CController m_cEnemyController;
        public bool m_bIsEnabled;

        public void Initialize(GameObject _cEnemyObject, CController _cEnemyController)
        {
            m_cEnemyObject = _cEnemyObject;
            m_cEnemyController = _cEnemyController;
            m_bIsEnabled = false;
        }
    };

    [SerializeField] private int m_nEnemyPoolSize = 10;
    public List<TEnemyInformation> m_tcEnemyPool;
    [SerializeField] private GameObject m_cEnemyPrefab;
    public bool m_bEnemyUIShowing = true;
    public short m_sEnemyCount = 0;

    public delegate void EnemySlain();
    public static event EnemySlain OnEnemySlain;

    /*******************************************
     * Start():      Instantiates the pool of enemies
     * 
     * Mod date:        09/1/2021
     * Mod initials:    MRHA
    *******************************************/
    private void Start()
    {
        m_tcEnemyPool = new List<TEnemyInformation>();
        FillPool();
    }
    /*******************************************
     * SetEnemyHealthBar():      Updates the enemy healthbar visibility
     * 
     * Mod date:        09/07/2021
     * Mod initials:    ARV
    *******************************************/
    public void SetEnemyHealthBar(bool bValue)
    {
        if (m_bEnemyUIShowing == bValue)
        {
            return;
        }
        m_bEnemyUIShowing = bValue;
        for (int i = 0; i < m_nEnemyPoolSize; i++)
        {
            // if(m_tcEnemyPool[i].m_cEnemyObject.GetComponentInChildren<Canvas>()!= null)
            m_tcEnemyPool[i].m_cEnemyObject.GetComponentInChildren<Canvas>().enabled = bValue;
        }
    }
    /*******************************************
     * FillPool():      Adds TEnemyInformation to the pool until it reaches the set maximum
     * 
     * Mod date:        09/1/2021
     * Mod initials:    MRHA
    *******************************************/
    private void FillPool()
    {
        for (int i = m_tcEnemyPool.Count; i < m_nEnemyPoolSize; i++)
        {
            AllocateNewEnemy();
        }
    }

    /*******************************************
     * SpawnEnemy():    Reactivates a dead enemy and places them at tPosition
     * 
     * Ins:
     *                  Spawn position of enemy
     * 
     * Mod date:        10/07/2021
     * Mod initials:    MRHA
    *******************************************/
    public void SpawnEnemy(Vector3 tPosition)
    {
        int nIndex = FindNextAvailable();
        if (nIndex < 0)
        {
            nIndex = m_nEnemyPoolSize;
            m_nEnemyPoolSize += 1;
            AllocateNewEnemy();
        }
        {
            TEnemyInformation tEnemy = m_tcEnemyPool[nIndex];
            tEnemy.m_cEnemyObject.transform.position = tPosition;
            tEnemy.m_bIsEnabled = true;
            tEnemy.m_cEnemyObject.SetActive(true);

            if (!tEnemy.m_cEnemyController.m_bIsAlive)
                tEnemy.m_cEnemyController.Respawn();

            m_tcEnemyPool[nIndex] = tEnemy;
        }
        m_sEnemyCount += 1;
    }
    /*******************************************
     * FindNextAvailable(): Returns the index of the next free enemy
     * 
     * Mod date:            09/1/2021
     * Mod initials:        MRHA
    *******************************************/
    private int FindNextAvailable()
    {
        for (int i = 0; i < m_tcEnemyPool.Count; ++i)
        {
            if (m_tcEnemyPool[i].m_bIsEnabled)
                continue;
            return i;
        }
        return -1;
    }
    /*******************************************
     * AllocateNewEnemy(): Instantiates a new prefab for the list
     * 
     * Mod date:            09/07/2021
     * Mod initials:        ARV
    *******************************************/
    public void AllocateNewEnemy()
    {
        TEnemyInformation tNewEnemy = new TEnemyInformation();
        GameObject cEnemyObject = Instantiate(m_cEnemyPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        cEnemyObject.GetComponentInChildren<Canvas>().gameObject.SetActive(m_bEnemyUIShowing);
        cEnemyObject.SetActive(false);

        tNewEnemy.Initialize(cEnemyObject, cEnemyObject.GetComponent<CController>());

        m_tcEnemyPool.Add(tNewEnemy);
    }
    /*******************************************
     * DeactivateEnemy():   Turns off an enemy after it dies
     * 
     * Mod date:            10/07/2021
     * Mod initials:        MRHA
    *******************************************/
    public void DeactivateEnemy(int nIndex)
    {
        TEnemyInformation tEnemy = m_tcEnemyPool[nIndex];
        tEnemy.m_bIsEnabled = false;
        tEnemy.m_cEnemyObject.SetActive(false);

        m_tcEnemyPool[nIndex] = tEnemy;

        m_sEnemyCount -= 1;

        if (OnEnemySlain != null)
            OnEnemySlain();
    }


    private void Update()
    {
        for (int i = 0; i < m_tcEnemyPool.Count; ++i)
        {
            TEnemyInformation tEnemy = m_tcEnemyPool[i];
            if (tEnemy.m_cEnemyController == null)
                continue;

            if (tEnemy.m_cEnemyController.m_bIsAlive)
                continue;

            if (tEnemy.m_cEnemyObject.activeSelf)
                DeactivateEnemy(i);
        }
    }

    ~CControllerManager()
    {
        m_tcEnemyPool.Clear();
    }
}
