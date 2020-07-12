using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBg : MonoBehaviour
{
    private PlayerController playerController;
    private Animator anim;

    private bool currentState = true;
    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController.waitToStart != currentState)
        {
            currentState = playerController.waitToStart;
            if (currentState) anim.SetTrigger("in");
            else anim.SetTrigger("out");
        }
    }
}
