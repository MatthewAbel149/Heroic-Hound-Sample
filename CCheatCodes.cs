using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
/*******************************************
 * Filename:            CCheatCodes.cs
 * Date:                08/26/2021
 * Mod. Date:           09/03/2021
 * Mod. Initials:       MRHA
 * Author:              Matthew Hoekstra-Abel
 * Purpose:             This controls cheat input
 ******************************************/

public class CCheatCodes : MonoBehaviour
{
    // Treat these as defines: Unity's #define system leaves much to be desired
    private int NUMOFCHEATS = 3;
    private int DOGMODE = 0; // God mode
    private int ZOOMIES = 1; // Speed increase
    private int BONEMAN = 2;


    /***************************************
     * m_cInputField        A reference to the input field used for entering cheats
     * 
     * m_tsCheatList        An array of strings: each of which is a cheat code
     * m_bCheats            The flags indicating whether or not enumerated cheats are active
     * 
     * m_cPlayerHealth      A reference to the player's health script
    ***************************************/
    public GameObject m_cInputField;

    [SerializeField] private string[] m_tsCheatList;
    [SerializeField] private bool[] m_bCheats = { false };

    [SerializeField] private CPlayerController m_cPlayer;

    // Initialize cheats in the first frame
    private void Start()
    {
        m_cPlayer = FindObjectOfType<CPlayerController>();
        m_tsCheatList = new string[NUMOFCHEATS];
        m_bCheats = new bool[NUMOFCHEATS];

        m_tsCheatList[DOGMODE] = "DOGMODE";
        m_tsCheatList[ZOOMIES] = "ZOOMIES";
        m_tsCheatList[BONEMAN] = "BONEMAN";
    }
    private void Update()
    {
        if (m_bCheats[DOGMODE])
            m_cPlayer.GetComponent<CHealth>().FillHealth();
    }

    /****************************************
     * InputLog()       Accepts player inputs
     * 
     * Ins:
     * Enter/Return     Checks cheat input
     * 
     * Mod date:        09/03/21
     * Mod initials:    MRHA
    ****************************************/
    public void InputLog(InputAction.CallbackContext cValue)
    {
        if (cValue.started)
        {
            string sPlayerInputLog = m_cInputField.GetComponent<TMP_InputField>().text; // Feed input in field to log
            sPlayerInputLog = sPlayerInputLog.ToUpper(); // Convert string to uppercase

            int iterator = 0; // Iterate through the list of cheats and compare the log to them
            while (iterator < NUMOFCHEATS)
            {
                if (sPlayerInputLog == m_tsCheatList[iterator]) // On a successful comparison, activate the cheat and clear the log
                {
                    m_bCheats[iterator] = !m_bCheats[iterator]; // Toggle the cheat
                    ///////////////////////////////////////////
                    // Toggleable cheats can be altered here //
                    ///////////////////////////////////////////
                    //I would love to use a switch case statement here

                    //if (iterator == DOGMODE)
                    //{
                    //    m_cPlayer.gameObject.GetComponent<CHealth>().ToggleDamage();
                    //}
                    if (iterator == BONEMAN)
                    {
                        gameObject.GetComponent<CControllerManager>().SpawnEnemy( new Vector3(
                                m_cPlayer.transform.position.x + 3,
                                m_cPlayer.transform.position.y, 
                                m_cPlayer.transform.position.z + 3));
                    }
                    if (iterator == ZOOMIES)
                    {
                        if (m_bCheats[ZOOMIES])
                            m_cPlayer.SetSpeed(m_cPlayer.GetSpeed() + 7); // Increase speed if cheat is on
                        else
                            m_cPlayer.SetSpeed(m_cPlayer.GetSpeed() - 7); // Decrease speed if it was toggled off
                    }
                }
                iterator += 1; // Check the next cheat
            }
            m_cInputField.GetComponent<TMP_InputField>().text = ""; // Reset the text panel
        }
    }

    /****************************************
     * ShowCheatMenu()  Shows player input panel
     * 
     * Ins:
     * F11              Toggles cheat input listening
     * 
     * Mod date:        08/26/21
     * Mod initials:    MRHA
    ****************************************/
    public void ShowCheatMenu(InputAction.CallbackContext cValue)
    {
        if (cValue.started)
        {
            m_cInputField.SetActive(!m_cInputField.activeSelf);
            m_cInputField.GetComponent<TMP_InputField>().text = "";
        }
    }
}
