/*******************************************
 * Filename:            CEnemyController.cs
 * Date:                08/15/2021
 * Mod. Date:           10/15/2021
 * Mod. Initials:       MRHA
 * Author:              Trevor Cook
 * Purpose:             This controls enemy movement and AI
 ******************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CEnemyController : CController
{
    /***************************************
        * m_fLookRadius:        This is the enemy's awareness zone; it will chase the player once entered.
        * m_fAttackRadius:      This is the enemy's attack range.
        * m_fStoppingRadius:    This is the enemy's maximum range for maintaining aggression.
        * 
        * m_cPlayer:            This is a reference to the player instance
        * m_cEnemy:             This is a reference to the enemy instance
        * 
        * m_fDropChance         The probability that the enemy will drop an item
        * m_LootDrop            The item the enemy can drop
        * m_bEnemyAlerted       Bool for tracking enemy alertness
        * m_bIsAlive            Bool for disabling chase functionality after enemy dies
        * 
        * m_cEnemyAnim          Reference to the Unity animator
        **************************************/

    [SerializeField] private float m_fLookRadius = 10f;
    [SerializeField] private float m_fAttackRadius = 3f;
    [SerializeField] private float m_fStoppingDistance = 15f;

    private bool m_bEnemyAlerted = false;
    public bool m_bIsAttacking = false;

    private Transform m_cPlayer;
    private NavMeshAgent m_cEnemy;
    public CPickUp m_LootDrop;
    public float m_fDropChance;

    public Animator m_cEnemyAnim;

    public AudioSource m_cEnemyRoar;
    public AudioSource m_cEnemyMovement;
    public AudioSource m_cEnemyAttack;
    public AudioSource m_cEnemyDeath;
    public AudioSource m_cEnemyDeathSound;

    /***************************************
     * Start()          Sets references to instances at game start
     *                  
     * 
     * Mod date:        08/15/2021
     * Mod initials:    TCC
    **************************************/
    public void Start()
    {
        m_cEnemy = GetComponent<NavMeshAgent>();
        m_cPlayer = CPlayerManager.instance.player.transform;
    }

    /***************************************
     * Update()         Checks to see if the player has entered within range
     *                  and pursues player character. Calls FacePlayer()
     *                  
     * Mod date:        10/06/2021
     * Mod initials:    MRHA
    **************************************/
    public void Update()
    {
        Knockback();
        if (!m_bIsAlive || !m_cPlayer.GetComponent<CPlayerController>().m_bIsAlive)
            return;

        float m_fDistance = Vector3.Distance(m_cPlayer.position, transform.position); // Distance between the player and the enemy

        if (!m_bEnemyAlerted) // If the enemy is not alerted:
        {
            if (m_fDistance <= m_fLookRadius) // If the player is within the enemy's vision sphere:
            {
                FacePlayer(); // Turn them to the player
                Roar(); // Alert and roar
            }
        }
        else // If the enemy is alerted...
        {
            if (m_bIsAlive && !m_bIsAttacking)
                FacePlayer(); // Face the player

            if (m_fDistance <= m_fAttackRadius || m_bIsAttacking) // If the player is close enough to attack or still attacking
            {
                Attack(); // Start the attack animation
            }
            else if (m_fDistance >= m_fStoppingDistance)   // If the player is too far away:
            {
                m_cEnemy.isStopped = true;
                m_cEnemyAnim.SetBool("isWalking", false);   // Stop the walk animation
                m_bEnemyAlerted = false;
            }
            else // If the player is not in attack range
            {
                Pursue();
            }
        }
    }

    /***************************************
    * Roar()           Alerts the enemy and makes it roar
    * 
    * Mod date:        10/04/2021
    * Mod initials:    MRHA
    **************************************/
    public void Roar()
    {
        m_bEnemyAlerted = true; // Set them to alert
        m_cEnemyAnim.SetTrigger("doTheRoar"); // Start the roar animation

        if (!m_cEnemyRoar.isPlaying)
        {
            m_cEnemyRoar.Play();
        }
    }

    /***************************************
    * Pursue()         Makes enemy follow player
    * 
    * Mod date:        10/04/2021
    * Mod initials:    MRHA
    **************************************/
    private void Pursue()
    {
        m_cEnemy.SetDestination(m_cPlayer.position);    // Sets enemy to pursue player

        m_cEnemyAnim.SetBool("isWalking", true); // Walk towards them

        if (!m_cEnemyMovement.isPlaying)
        {
            m_cEnemyMovement.Play();
        }
    }

    /***************************************
    * FacePlayer()         Makes sure enemy is facing player at all times once player is detected
    *
    *
    * 
    * Mod date:        08/15/2021
    * Mod initials:    TCC
    **************************************/
    void FacePlayer()
    {
        Vector3 tDirection = (m_cPlayer.position - transform.position).normalized; // Get the player's direction
        Quaternion qLookRotation = Quaternion.LookRotation(new Vector3(tDirection.x, 0, tDirection.z)); // Calculates the rotation needed to face player
        transform.rotation = Quaternion.Slerp(transform.rotation, qLookRotation, Time.deltaTime * 15f); // Performs the rotation with interpolation
    }

    /***************************************
    * Attack()         Attacks the player, dealing damage
    * 
    * Mod date:        10/07/2021
    * Mod initials:    MRHA
    **************************************/
    void Attack()
    {
        if (!m_bIsAttacking)
        {
            FacePlayer();
            m_bIsAttacking = true;
            m_cEnemyAnim.SetTrigger("targetInRange"); // Tells animator to trigger attack animation

            //if (!m_cEnemyAttack.isPlaying)
            //{
            //    m_cEnemyAttack.Play();
            //}
        }
    }

    /***************************************
    * PlayAttackSound() Plays the attack sound
    * 
    * Mod date:         10/05/2021
    * Mod initials:     MRHA
    ***************************************/
    public void PlayAttackSound()
    {
        if (!m_cEnemyAttack.isPlaying)
        {
            m_cEnemyAttack.Play();
        }
    }

    /***************************************
    * Die()         Triggers death animation
    *
    *
    * 
    * Mod date:        11/01/2021
    * Mod initials:    ARV
    **************************************/
    public override void Die()
    {
        m_bIsAttacking = false;
        m_cEnemyAnim.SetTrigger("timeToDie"); // Tells animator to trigger death animation

        // Why are there two of these?
        //because they add more immersion to the skeleton but they are different clips

        if (!m_cEnemyDeath.isPlaying)
        {
            m_cEnemyDeath.Play();
        }
        if (!m_cEnemyDeathSound.isPlaying)
        {
            m_cEnemyDeathSound.Play();
        }

        gameObject.GetComponentInChildren<Canvas>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;

        // Drop loot item for player
        if (m_LootDrop)
        {
            float randVal = Random.Range(0, 100);
            if (randVal <= m_fDropChance)
                GameObject.Instantiate(m_LootDrop, transform.position, Quaternion.identity);
        }
    }

    /***************************************
    * Respawn()        Resets enemy values
    * 
    * Mod date:        09/09/2021
    * Mod initials:    MRHA
    **************************************/
    public override void Respawn()
    {
        //m_cEnemyAnim.SetTrigger("Respawn"); // Tells animator to trigger respawn animation
        m_bIsAlive = true; // Re-enables enemy functionality
        m_bIsAttacking = false;

        // TODO: Ensure UI is only re-enabled if options allow it
        gameObject.GetComponentInChildren<Canvas>().enabled = true; // Turn on the health bar
        gameObject.GetComponent<Collider>().enabled = true; // Turn on the enemy's collider
        gameObject.GetComponent<CHealth>().FillHealth(); // Fill the enemy's HP
    }

    /***************************************
    * OnDrawGizmosSelected()         Allows visual debugging of enemy detection radius
    *
    *
    * 
    * Mod date:        08/17/2021
    * Mod initials:    TCC
    **************************************/
    private void OnDrawGizmosSelected()
    {
        // Draw the enemy's awareness sphere for debugging
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_fLookRadius);

        // Draw the enemy's ranged attack sphere for debugging
        Gizmos.color = Color.blue;
        // Gizmos.DrawWireSphere(transform.position, mf_ProjectileRadius);

        // Draw the enemy's melee range for debugging
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_fAttackRadius);
    }
}
