using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GP_JumpAssistantTest : MonoBehaviour
{
    [HideInInspector] public bool isPlayerInJumpableZone = false;

    void Update()
    {
        //Debug.Log("is Player in jumpable zone = " + isPlayerInJumpableZone);
    }

    private void OnTriggerEnter(Collider other)
    {
        isPlayerInJumpableZone = true;
    }

    private void OnTriggerStay(Collider other)
    {
        //isPlayerInJumpableZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isPlayerInJumpableZone = false;
    }
}
