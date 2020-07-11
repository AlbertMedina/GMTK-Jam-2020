using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    [Header("Config Options")] 
    
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private float slowMoTime;
    [SerializeField] private float fastMoTime;
    
    
    private Match _match;
    
    public void InitMatch() //Choose match rules
    {
        _match = new Match();
        _match.Generate();
        Config();
    }

    private void Config()
    {
        ConfigBullets();
        ConfigMovement();
        ConfigWinCon();
    }

    public void StartMatch() //After countdown
    {
        
    }

    public void StopMatch()
    {
        ResetMatch();
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
                break;
            case 1: //Heavy Bullets
                break;
            case 2: //Inverted Axis
                break;
            case 3: //One Shot
                break;
        }
    }

    private void ConfigMovement()
    {
        switch (_match.movementRules)
        {
            case 0: //SlowMo
                Time.timeScale = slowMoTime;
                break;
            case 1: //FastRound
                Time.timeScale = fastMoTime;
                break;
            case 2: //Inverted Controls
                break;
            case 3: //No movement
                break;
        }
    }

    private void ConfigWinCon()
    {
        switch (_match.winConRules)
        {
            case 0: //Die
                break;
            case 1: //Catch the flag
                break;
            case 2: //Only HeadShots
                break;
            case 3: //Only Melee
                break;
        }
    }
}
