using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultsUILogic : MonoBehaviour
{
    private Animator anim;
    
    [SerializeField] private TextMeshProUGUI playerScore;
    [SerializeField] private TextMeshProUGUI cpuScore;
    private MatchController _matchController;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        _matchController = FindObjectOfType<MatchController>();
    }

    private void OnEnable()
    {
        playerScore.text = _matchController.playerScore.ToString();
        cpuScore.text = _matchController.cpuScore.ToString();
    }

    public void PlayerWins()
    {
        anim.SetTrigger("PlayerWins");
        AudioManager.Instance.Play("Win");
        StartCoroutine(Timer());
    }

    public void CPUWins()
    {
        anim.SetTrigger("CPUWins");
        AudioManager.Instance.Play("Lose");
        StartCoroutine(Timer());
    }

    private IEnumerator Timer()
    {
        yield return new WaitForSecondsRealtime(3);
        anim.SetTrigger("Exit");
        _matchController.InitMatch();
        gameObject.SetActive(false);
    }
}
