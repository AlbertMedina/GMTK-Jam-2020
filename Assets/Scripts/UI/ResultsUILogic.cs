using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsUILogic : MonoBehaviour
{
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void PlayerWins()
    {
        anim.SetTrigger("PlayerWins");
        StartCoroutine(Timer());
    }

    public void CPUWins()
    {
        anim.SetTrigger("CPUWins");
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSecondsRealtime(3);
        anim.SetTrigger("Exit");
        gameObject.SetActive(false);
    }
}
