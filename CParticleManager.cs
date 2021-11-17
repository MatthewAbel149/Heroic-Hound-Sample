using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*******************************************
 * Filename:            CParticleManager.cs
 * Date:                11/15/2021
 * Mod. Date:           11/16/2021
 * Mod. Initials:       MRHA
 * Author:              Matthew Hoekstra-Abel
 * Purpose:             This contains a pool of particles that can be dynamically called
 ******************************************/
public class CParticleManager : MonoBehaviour
{

    /***************************************
     * TParticeSystem       A struct to reduce GetComponent 
     *                      calls and track active flags
     *                      
     * m_bIsActive          Boolean for tracking usage
     * m_cParticleSystem    Reference to particle system
     * m_cGameObject        Reference to the game object
    ***************************************/
    public struct TParticleInfo
    {
        public bool m_bIsActive;
        public ParticleSystem m_cParticleSystem;
        public GameObject m_cGameObject;

        public TParticleInfo(GameObject _cGameObject)
        {
            m_cParticleSystem = _cGameObject.GetComponent<ParticleSystem>();
            m_cGameObject = _cGameObject;
            m_bIsActive = false;
        }
    }

    /***************************************
     * g_cParticleManagerInstance       Singleton variable
     * m_tParticlePrefab                Prefab for instantiation
     * m_tParticlePool                  List of particle objects
     * m_nPoolSize                      Maximum number of particles
     * m_fParticleCutoffTime            Lifetime of particles before they are freed (in seconds)
    ***************************************/
    public static CParticleManager g_cParticleManagerInstance;
    public GameObject m_tParticlePrefab;
    private List<TParticleInfo> m_tParticlePool;
    public int m_nPoolSize = 8;
    [SerializeField] private float m_fParticleCutoffTime = .25f;

    void Start()
    {
        m_tParticlePool = new List<TParticleInfo>();
        if (g_cParticleManagerInstance)
        {
            Debug.LogWarning("Duplicate particle manager found.");
            return;
        }
        g_cParticleManagerInstance = this;

        FillPool();
    }
    /**********************************************************
     * FillPool()       Fills the pool with particles to its maximum
     * 
     * Mod date:        11/16/2021
     * Mod initials:    MRHA
    **********************************************************/
    void FillPool()
    {
        int nCount = m_nPoolSize;
        while (nCount > 0)
        {
            AllocateParticle();
            nCount -= 1;
        }
    }
    /**********************************************************
     * AllocateParticle()       Will create a new particle and add it to the pool
     * 
     * Mod date:        11/16/2021
     * Mod initials:    MRHA
    **********************************************************/
    void AllocateParticle()
    {
        TParticleInfo cNewParticle = new TParticleInfo(GameObject.Instantiate(m_tParticlePrefab, new Vector3(0, 0, 0), Quaternion.identity));
        //cNewParticle.m_cGameObject.SetActive(false);
        cNewParticle.m_cParticleSystem.Stop();

        m_tParticlePool.Add(cNewParticle);
    }
    /**********************************************************
     * PlayParticleEffect()     Plays a particle effect at the given transform
     * 
     * Note:                    If there is no free particle, it makes one
     * 
     * Mod date:                11/16/2021
     * Mod initials:            MRHA
    **********************************************************/
    public void PlayParticleEffect(Transform tPos)
    {
        int nIndex = NextAvailable();
        if (nIndex == -1)
        {
            AllocateParticle();
            nIndex += m_tParticlePool.Count;
        }

        TParticleInfo tParticle = m_tParticlePool[nIndex];

        tParticle.m_cGameObject.transform.position = tPos.position;
        tParticle.m_cParticleSystem.Play();
        tParticle.m_bIsActive = true;

        m_tParticlePool[nIndex] = tParticle;
    }
    /**********************************************************
     * NextAvailable()  Return the index of the next free particle
     * 
     * Mod date:        11/16/2021
     * Mod initials:    MRHA
    **********************************************************/
    private int NextAvailable()
    {
        for (int i = 0; i < m_tParticlePool.Count; ++i)
        {
            if (!m_tParticlePool[i].m_bIsActive)
                return i;
        }
        return -1;
    }

    /**********************************************************
     * Update()         Update will check all particles in the list and reset
     *                  any particles that have reached the end of their life
     * 
     * Mod date:        11/16/2021
     * Mod initials:    MRHA
    **********************************************************/
    void Update()
    {
        TParticleInfo tTemporaryParticleData;
        for (int i = 0; i < m_tParticlePool.Count; ++i)
        {
            tTemporaryParticleData = m_tParticlePool[i];
            if (tTemporaryParticleData.m_bIsActive)
            {
                if (tTemporaryParticleData.m_cParticleSystem.time >= m_fParticleCutoffTime)
                {
                    tTemporaryParticleData.m_cParticleSystem.time = 0;
                    tTemporaryParticleData.m_cParticleSystem.Stop();
                    tTemporaryParticleData.m_bIsActive = false;

                    m_tParticlePool[i] = tTemporaryParticleData;
                }
            }
        }
    }
}
