using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDLogic : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private TextMeshProUGUI playerScore;
    [SerializeField] private TextMeshProUGUI cpuScore;
    [SerializeField] private ResultsUILogic _resultsUiLogic;
    [SerializeField] private GameObject _timeOut;

    private MatchController _matchController;

    private void Awake()
    {
        _matchController = FindObjectOfType<MatchController>();
    }

    private void OnEnable()
    {
        playerScore.text = _matchController.playerScore.ToString();
        cpuScore.text = _matchController.cpuScore.ToString();
    }

    public void PlayerWins()
    {
        _resultsUiLogic.gameObject.SetActive(true);
        _resultsUiLogic.PlayerWins();
        StopAllCoroutines();
    }

    public void CPUWins()
    {
        _resultsUiLogic.gameObject.SetActive(true);
        _resultsUiLogic.CPUWins();
        StopAllCoroutines();
    }

    private void TimeOut()
    {
        StopAllCoroutines();
        StartCoroutine(Co_TimeOut());
    }

    public IEnumerator Co_Counter()
    {
        int seconds = _matchController.roundLenght;
        counter.text = seconds.ToString();
        while (seconds > 0)
        {
            yield return new WaitForSecondsRealtime(1);
            seconds--;
            counter.text = seconds.ToString();
        }
        TimeOut();
    }

    private IEnumerator Co_TimeOut()
    {
        _matchController.StopMatch(true);
        _timeOut.SetActive(true);
        yield return new WaitForSecondsRealtime(3);
        _timeOut.SetActive(false);
        gameObject.SetActive(false);
        _matchController.InitMatch();
    }
}
