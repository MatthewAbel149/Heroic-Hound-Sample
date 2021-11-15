using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
/*******************************************
 * Filename:            CPlayerController.cs
 * Date:                08/12/2021
 * Mod. Date:           11/02/2021
 * Mod. Initials:       TCC
 * Author:              Matthew Hoekstra-Abel
 * Purpose:             This controls player movement
 ******************************************/

public class CPlayerController : CController
{
    /***************************************
    * m_cController:        This is the CharacterController component for the gameobject. 
    * m_tCam:               This is the transform of the camera; basis of player movement
    * 
    * m_fSpeed:             The speed of the player
    * m_fSprintSpeed        The speed variable for the increase when a player runs
    * m_fJumpSpeed          The speed of the player's jump
    * m_fJumpTime           The base duration of a player's jump
    * m_fJumpCD             The cooldown variable for the jump
    * m_cJumpTimer          The timer for the jump effect
    * m_cJumpCDTimer        The cooldown timer for the jump
    * m_fGravity            The speed at which the player falls. May be redundant if a project gravity variable is used.
    * 
    * m_bShiftIsHeld        A bool for tracking whether sprint is currently pressed
    * m_bBlockIsHeld        A bool for tracking whether block is currently pressed
    * m_bJumpIsStarted      A bool for tracking when jump is pressed
    * m_bIsAlive            A bool indicating whether the player is alive. Used for enemies.
    * 
    * m_fTurnSmoothTime:    Determines the time it takes for the player to turn to the movement direction
    * m_fTurnSmoothVelocity:    Used in SmoothDampAngle to turn the player
    * m_tFallDirection      Used internally to cause the player to fall. Updated every frame.
    * m_tInputVector        Used to hold the WASD input from the player.
    * m_tJumpVector         Used to hold the jump direction.
    *   
    * m_cAnimator           Holds a reference to the animator
    **************************************/
    [SerializeField] private CharacterController m_cController;
    [SerializeField] private Transform m_tCam;

    [SerializeField] private float m_fSpeed = 3.0f;
    [SerializeField] private float m_fSprintSpeed = 2.0f;
    [SerializeField] private float m_fJumpSpeed = 5.0f;
    [SerializeField] private float m_fJumpTime = 0.2f;
    [SerializeField] private float m_fJumpCD = 3.0f;
    [SerializeField] private CTimer m_cJumpTimer;
    [SerializeField] private CTimer m_cJumpCDTimer;
    private float m_fGravity = 9.8f;

    private bool m_bShiftIsHeld = false;
    private bool m_bBlockIsHeld = false;
    private bool m_bJumpIsStarted = false;
    public bool m_bInteracting = false;
    public bool m_bInventoryVisible = false;

    [SerializeField] private float m_fTurnSmoothTime = 0.1f;
    private float m_fTurnSmoothVelocity = 0.1f;
    private Vector3 m_tFallDirection = new Vector3();
    private Vector3 m_tInputVector = new Vector3();
    private Vector3 m_tJumpVector = new Vector3();

    [SerializeField] private Animator m_cAnimator;
    private GameObject playerInventory;
    private CAudioManager m_cAudioManager;

    public static CPlayerController instance;

    private void Start()
    {
        if (instance)
        {
            Debug.LogWarning("Duplicate player controller found.");
            //return;
        }

        instance = this;

        m_cJumpTimer.m_fMaximum = m_fJumpTime;
        m_cJumpTimer.StartTimer();
        m_cJumpTimer.SetTime(m_fJumpTime);

        m_cJumpCDTimer.m_fMaximum = m_fJumpCD;
        m_cJumpCDTimer.StartTimer();
        m_cJumpCDTimer.SetTime(m_fJumpCD);

        playerInventory = GameObject.FindGameObjectWithTag("PlayerInventory");
        m_cAudioManager = FindObjectOfType<CAudioManager>();
    }

    /*******************************************
     * Move()           Changes the movement vector based on the 
     *                  input value (taken from active input handling).
     * 
     * ins:
     *                  cValue
     * 
     * Mod date:        08/20/2021
     * Mod initials:    MRHA
    *******************************************/
    public void Move(InputAction.CallbackContext cValue)
    {
        if (m_bIsAlive)
            m_tInputVector = cValue.ReadValue<Vector2>();
    }

    /*******************************************
     * Jump()           Uses the m_fJumpSpeed parameter to 
     *                  increase the player's vertical velocity.
     * 
     * Mod date:        08/23/2021
     * Mod initials:    MRHA
    *******************************************/
    public void Jump(InputAction.CallbackContext cValue)
    {
        //if (m_cController.isGrounded) // Checks to see if playercontroller is currently on the ground before adding upward velocity
        if (cValue.started && m_cJumpCDTimer.Expired()) // Checks to see if player just pressed dodge
        {
            //Start timer
            m_cJumpTimer.StartTimer(); // Begin jump timer
            m_cJumpCDTimer.StartTimer(); // Begin cooldown

            ////////////////////////////////////
            m_bJumpIsStarted = true;
            m_cAudioManager.Play("PlayerDash");
        }
    }

    public void ToggleInventory(InputAction.CallbackContext cValue)
    {
        if (cValue.started && !m_bInventoryVisible)
        {
            playerInventory.SetActive(!playerInventory.activeSelf);
            m_bInventoryVisible = true;
        }

        else if (cValue.started && m_bInventoryVisible)
        {
            playerInventory.SetActive(!playerInventory.activeSelf);
            m_bInventoryVisible = true;
        }
    }

    /*******************************************
     * Interact()       Allows the player to interact with objects and NPCs in the game world
     * 
     * 
     * Mod date:        10/03/2021
     * Mod initials:    TCC
    *******************************************/
    public void Interact(InputAction.CallbackContext cValue)
    {
        if (cValue.started)
        {
            // Player interacts with object
            m_bInteracting = true;
            m_cAudioManager.Play("Interact");
            Debug.Log("Player has interacted with object.");
        }

        if (cValue.canceled)
        {
            m_bInteracting = false;
        }
    }

    /*******************************************
     * Block()          Allows the player to block all incoming damage, 
     *                  but blocks running.
     * 
     * Mod date:        09/03/2021
     * Mod initials:    MRHA
    *******************************************/
    public void Block(InputAction.CallbackContext cValue)
    {
        if (cValue.started)
        {
            m_bBlockIsHeld = true;

            m_cAnimator.SetBool("isBlocking", true); // indicate the player is running for the animator
            gameObject.GetComponent<CHealth>().ToggleDamage();
            m_cAudioManager.Play("PlayerBlock");
        }

        if (cValue.canceled)
        {
            m_cAnimator.SetBool("isBlocking", false); // indicate the player is no longer running for the animator
            m_bBlockIsHeld = false;
            gameObject.GetComponent<CHealth>().ToggleDamage();
        }
    }
    /*******************************************
     * Die()            Tells the animator to kill the player.
     *                  Performs any necessary death actions and shuts off input
     *                  
     * Mod date:        09/09/2021
     * Mod initials:    ARV
    *******************************************/
    public override void Die()
    {
        gameObject.GetComponentInChildren<Collider>().enabled = false; //move it here so the player doesn't accidentally get trigger by something else first before he dies
        m_bIsAlive = false;
        m_cAnimator.SetTrigger("Die");
        m_cAudioManager.Play("PlayerDie");
        FindObjectOfType<CUiManager>().Lose();
    }
    /*******************************************
     * Respawn()        Resets player values
     *                  
     * Mod date:        08/23/2021
     * Mod initials:    MRHA
    *******************************************/
    public override void Respawn()
    {
        m_cAnimator.SetTrigger("Respawn");
        m_bIsAlive = true;

        gameObject.GetComponentInChildren<Collider>().enabled = true;
        gameObject.GetComponent<CHealth>().FillHealth();

        // Find a good way to implement respawn locations
    }

    /*******************************************
     * Attack1()        Tells the animator to perform a light attack.
     * 
     * ins:
     *                  cValue
     *                  
     * Mod date:        08/18/2021
     * Mod initials:    MRHA
    *******************************************/
    public void Attack1(InputAction.CallbackContext cValue)
    {
        if (cValue.started)
        {
            //ToggleWeaponHitbox();
            //m_cWeapon.GetComponent<CWeapon>().m_fDamageMultiplier = 1;
            m_cAnimator.SetTrigger("Attack1"); // Tell the animator to run the attack animation
            m_cAudioManager.Play("PlayerAttack1");
        }
    }

    /*******************************************
     * Attack2()        Tells the animator to perform a heavy attack.
     * 
     * ins:
     *                  cValue
     *                  
     * Mod date:        08/18/2021
     * Mod initials:    MRHA
    *******************************************/
    public void Attack2(InputAction.CallbackContext cValue)
    {
        if (cValue.started)
        {
            // IF SPAMMED, THE PLAYER WILL JUMP AROUND AT MACH 7
            //IncreaseKnockbackVector(transform.forward.normalized * 15); //TODO: THIS IS A PLACEHOLDER! TIME WITH ANIMATION PROPERLY
            
            //ToggleWeaponHitbox();
            //m_cWeapon.GetComponent<CWeapon>().m_fDamageMultiplier = 2;
            m_cAnimator.SetTrigger("Attack2"); // Tell the animator to run the attack animation
            m_cAudioManager.Play("PlayerAttack2");
        }
    }

    /*******************************************
     * Run()            Increases the player's current speed.
     * 
     * ins:
     *                  cValue
     *                  
     * Mod date:        08/13/2021
     * Mod initials:    MRHA
    *******************************************/
    public void Run(InputAction.CallbackContext cValue)
    {
        if (cValue.started)
        {
            m_bShiftIsHeld = true;

            m_cAnimator.SetBool("isRunning", true); // indicate the player is running for the animator
            m_cAudioManager.Play("PlayerRunning");
        }

        if (cValue.canceled)
        {
            m_cAnimator.SetBool("isRunning", false); // indicate the player is no longer running for the animator
            m_bShiftIsHeld = false;
        }
    }

    /*******************************************
     * Update()         Calculates the player's velocity.
     * 
     * Mod date:        08/23/2021
     * Mod initials:    MRHA
    *******************************************/
    void Update()
    {
        float fCurrSpeed = m_fSpeed; // Increases m_fCurrSpeed variable by base speed

        if (m_bBlockIsHeld)
        {
            //fCurrSpeed -= m_fSprintSpeed;
        }
        else if (m_bShiftIsHeld && m_cController.isGrounded) // Checks to see if playercontroller is currently on the ground before increasing speed)
        {
            fCurrSpeed += m_fSprintSpeed;
        }
        Vector3 tDirection = new Vector3(m_tInputVector.x, 0, m_tInputVector.y).normalized; // Create player's move vector

        if (tDirection.magnitude >= 0.1f) // If the move vector is large enough to calculate (accounting for tiny micromovements)
        {
            m_cAnimator.SetBool("isWalking", true); // indicate the player is walking for the animator
            m_cAudioManager.Play("PlayerWalking");
            float targetAngle = Mathf.Atan2(tDirection.x, tDirection.z) * Mathf.Rad2Deg + m_tCam.eulerAngles.y; //The direction the player should move
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref m_fTurnSmoothVelocity, m_fTurnSmoothTime); // Calculate rotational angle to smooth player turning
            transform.rotation = Quaternion.Euler(0f, angle, 0f); // Turn player

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward; // Calculate moveDir from targetAngle

            m_cController.Move(moveDir * fCurrSpeed * Time.deltaTime); // Move player in move direction

            if (m_bJumpIsStarted) //Player dodge
            {
                m_bJumpIsStarted = false;
                m_tJumpVector = new Vector3(moveDir.x, 0, moveDir.z);
            }
        }
        else // indicate the player is no longer walking for the animator
        {
            m_cAnimator.SetBool("isWalking", false);
        }

        m_tFallDirection.y -= m_fGravity * Time.deltaTime; // Set fall direction
        m_cController.Move(m_tFallDirection * Time.deltaTime); // Move player down every frame
        Knockback();    // Knocks the player back properly

        if (!m_cJumpTimer.Expired()) // If the jump timer is running
            m_cController.Move(m_tJumpVector * m_fJumpSpeed * Time.deltaTime);
    }

    // ACCESSORS

    public float GetSpeed()
    {
        return m_fSpeed;
    }
    public void SetSpeed(float speed)
    {
        m_fSpeed = speed;
    }

}
