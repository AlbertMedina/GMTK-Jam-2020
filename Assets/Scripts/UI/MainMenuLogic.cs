using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuLogic : MonoBehaviour
{
    private MatchController _matchController;

    private void Awake()
    {
        _matchController = FindObjectOfType<MatchController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _matchController.InitMatch();
            gameObject.SetActive(false);
        }
    }
}
