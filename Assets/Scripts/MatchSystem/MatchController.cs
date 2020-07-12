using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    [Header("Config Options")] 
    
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private float slowMoTime;
    [SerializeField] private float fastMoTime;
    [SerializeField] private GameObject hud;
    
    
    private Match _match;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InitMatch();
        }
    }

    public void InitMatch() //Choose match rules
    {
        _match = new Match();
        _match.Generate();
        Config();
    }

    private void Config()
    {
        ConfigUIController.Instance.Reset();
        
        ConfigBullets();
        ConfigMovement();
        ConfigWinCon();
        
        StartCoroutine(Co_StartMatch());
    }

    private void StartMatch() //After countdown
    {
        hud.SetActive(true);
    }

    public void StopMatch()
    {
        ResetMatch();
        hud.SetActive(false);
    }

    private void ResetMatch()
    {
        Time.timeScale = 1;
    }

    private void ConfigBullets()
    {
        switch (_match.bulletRules)
        {
            case 0: //Bouncy Bullets
                ConfigUIController.Instance.bouncyBullets.SetActive(true);
                break;
            case 1: //Heavy Bullets
                ConfigUIController.Instance.heavyBullets.SetActive(true);
                break;
            case 2: //Inverted Axis
                ConfigUIController.Instance.invertedMouse.SetActive(true);
                break;
            case 3: //One Shot
                ConfigUIController.Instance.oneBullet.SetActive(true);
                break;
        }
    }

    private void ConfigMovement()
    {
        switch (_match.movementRules)
        {
            case 0: //SlowMo
                ConfigUIController.Instance.slowMo.SetActive(true);
                break;
            case 1: //FastRound
                ConfigUIController.Instance.quickMovement.SetActive(true);
                break;
            case 2: //Inverted Controls
                ConfigUIController.Instance.invertedControls.SetActive(true);
                break;
            case 3: //No movement
                ConfigUIController.Instance.cantMove.SetActive(true);
                break;
        }
    }

    private void ConfigWinCon()
    {
        switch (_match.winConRules)
        {
            case 0: //Die
                ConfigUIController.Instance.dieToWin.SetActive(true);
                break;
            case 1: //Catch the flag
                ConfigUIController.Instance.catchTheFlag.SetActive(true);
                break;
            case 2: //Only HeadShots
                ConfigUIController.Instance.onlyHeadShots.SetActive(true);
                break;
            case 3: //Only Melee
                ConfigUIController.Instance.onlyMelee.SetActive(true);
                break;
        }
    }

    private IEnumerator Co_StartMatch()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        ConfigUIController.Instance.Play();
        yield return new WaitForSecondsRealtime(3.5f);
        ConfigUIController.Instance.Remove();
        yield return new WaitForSecondsRealtime(0.5f);
        StartCoroutine(Co_Countdown());
    }

    private IEnumerator Co_Countdown()
    {
        var counter = ConfigUIController.Instance.counter;
        counter.gameObject.SetActive(true);
        counter.text = "3";
        yield return new WaitForSecondsRealtime(1);
        counter.text = "2";
        yield return new WaitForSecondsRealtime(1);
        counter.text = "1";
        yield return new WaitForSecondsRealtime(1);
        counter.text = "GO!";
        StartMatch();
        yield return new WaitForSecondsRealtime(0.75f);
        counter.gameObject.SetActive(false);
    }
}
