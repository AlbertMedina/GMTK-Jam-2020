using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HUDLogic : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counter;
    [SerializeField] private ResultsUILogic _resultsUiLogic;

    private MatchController _matchController;

    private void Awake()
    {
        _matchController = FindObjectOfType<MatchController>();
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
        _matchController.StopMatch();
    }
}
