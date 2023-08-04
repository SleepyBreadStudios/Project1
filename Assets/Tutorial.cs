/******************************************************************************
 * Script for tutorial setup
 * 
 * Authors: Alicia T, Jason N, Jino C
 *****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Tutorial : MonoBehaviour
{
    public GameObject tutorial;

    private bool isActive = true;
    private bool isTabKeyDown = false;
    private bool isTutorialToggled = false;
    private float toggleDelay = 0.5f;
    private float timeSinceLastToggle = 0f;

    // Start is called before the first frame update
    void Start()
    {
        tutorial.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            SetTutorialActive(false);
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            isTabKeyDown = true;
        }
        else
        {
            isTabKeyDown = false;
            isTutorialToggled = false;
        }

        // Toggle tutorial only when Tab key is held and not yet toggled
        if (isTabKeyDown && !isTutorialToggled)
        {
            timeSinceLastToggle += Time.deltaTime;
            if (timeSinceLastToggle >= toggleDelay)
            {
                ToggleTutorial();
                timeSinceLastToggle = 0f;
                isTutorialToggled = true;
            }
        }
    }

    private void SetTutorialActive(bool activeState)
    {
        isActive = activeState;
        tutorial.SetActive(isActive);
    }

    private void ToggleTutorial()
    {
        isActive = !isActive;
        tutorial.SetActive(isActive);
    }
}

// public class Tutorial : NetworkBehaviour
// {
//     public GameObject tutorial;

//     [SerializeField]
//     private bool isActive = true;

//     private Coroutine toggleCoroutine;

//     private void Start()
//     {
//         if (!IsLocalPlayer)
//         {
//             // Disable the tutorial for non-local players (clients)
//             tutorial.SetActive(false);
//         }
//     }

//     private void Update()
//     {
//         if (!IsLocalPlayer) return;

//         if (Input.GetKey(KeyCode.Return))
//         {
//             SetTutorialActive(false);
//         }

//         if (Input.GetKeyDown(KeyCode.Tab))
//         {
//             if (toggleCoroutine != null)
//                 StopCoroutine(toggleCoroutine);

//             toggleCoroutine = StartCoroutine(ToggleTutorialWithDelay());
//         }
//         else if (Input.GetKeyUp(KeyCode.Tab))
//         {
//             if (toggleCoroutine != null)
//                 StopCoroutine(toggleCoroutine);
//         }
//     }

//     private void SetTutorialActive(bool activeState)
//     {
//         isActive = activeState;
//         tutorial.SetActive(isActive);
//         if (IsServer)
//         {
//             RpcUpdateTutorialStateClientRpc(activeState);
//         }
//         else
//         {
//             SubmitTutorialStateServerRpc(activeState);
//         }
//     }

//     private IEnumerator ToggleTutorialWithDelay()
//     {
//         yield return new WaitForSeconds(0.5f); // Adjust the delay time as needed
//         isActive = !isActive;
//         tutorial.SetActive(isActive);
//         if (IsServer)
//         {
//             RpcUpdateTutorialStateClientRpc(isActive);
//         }
//         else
//         {
//             SubmitTutorialStateServerRpc(isActive);
//         }
//     }

//     [ClientRpc]
//     private void RpcUpdateTutorialStateClientRpc(bool activeState)
//     {
//         if (!IsLocalPlayer)
//         {
//             isActive = activeState;
//             tutorial.SetActive(isActive);
//         }
//     }

//     private void SubmitTutorialState(bool activeState)
//     {
//         if (IsLocalPlayer)
//         {
//             // Submit the tutorial state to the server
//             SubmitTutorialStateServerRpc(activeState);
//         }
//     }

//     [ServerRpc]
//     private void SubmitTutorialStateServerRpc(bool activeState)
//     {
//         isActive = activeState;
//         tutorial.SetActive(isActive);
//         RpcUpdateTutorialStateClientRpc(activeState);
//     }
// }