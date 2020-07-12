using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    [Header("Config Options")]

    public int roundLenght;

    private PlayerController player;
    private EnemyController enemy;
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private float slowMoTime;
    [SerializeField] private float fastMoTime;
    [SerializeField] private GameObject hud;
    [SerializeField] private HUDLogic _hudLogic;

    private Match _match;
    private bool _matchStarted = false;

    [HideInInspector] public int playerScore;
    [HideInInspector] public int cpuScore;

    private PlayerController.ShootingRules currentShootingRule;
    private PlayerController.MovementRules currentMovementRule;
    private PlayerController.WinningRules currentWinningRule;

    [HideInInspector] public GameObject flag;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        enemy = FindObjectOfType<EnemyController>();
        
        currentShootingRule = PlayerController.ShootingRules.NONE;
        currentMovementRule = PlayerController.MovementRules.NONE;
        currentWinningRule = PlayerController.WinningRules.NONE;

        flag = GameObject.FindGameObjectWithTag("Flag");
        flag.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_matchStarted)
        {
            InitMatch();
            _matchStarted = true;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerWins();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            CPUWins();
        }
    }

    public void InitMatch() //Choose match rules
    {
        _match = new Match();
        _match.Generate();

        FindObjectOfType<PlayerController>().ResetRound();

        Config();
        SetPositions();

        player.SetRoundRules(currentShootingRule, currentMovementRule, currentWinningRule);
    }

    private void Config()
    {
        ConfigUIController.Instance.Reset();

        ConfigBullets();
        ConfigMovement();
        ConfigWinCon();

        StartCoroutine(Co_StartMatch());
    }

    private void SetPositions()
    {
        List<GameObject> spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPosition").ToList();

        int idx = UnityEngine.Random.Range(0, spawnPoints.Count);
        player.transform.position = spawnPoints[idx].transform.position;
        player.transform.rotation = spawnPoints[idx].transform.rotation;
        spawnPoints.RemoveAt(idx);
        
        idx = UnityEngine.Random.Range(0, spawnPoints.Count);
        enemy.transform.position = spawnPoints[idx].transform.position;
    }

    private void StartMatch() //After countdown
    {
        hud.SetActive(true);
        FindObjectOfType<EnemyController>().SetInitialState();
        FindObjectOfType<PlayerController>().StartRound();
        _hudLogic.StartCounter();
    }

    public void StopMatch(bool timeOut)
    {
        FindObjectOfType<PlayerController>().FreezePlayer();
        if (!timeOut) hud.SetActive(false);
    }

    public void PlayerWins()
    {
        playerScore++;
        _hudLogic.PlayerWins();
        StopMatch(false);
    }

    public void CPUWins()
    {
        cpuScore++;
        _hudLogic.CPUWins();
        StopMatch(false);
    }

    private void ConfigBullets()
    {
        switch (_match.bulletRules)
        {
            case 0: //Bouncy Bullets
                ConfigUIController.Instance.bouncyBullets.SetActive(true);
                currentShootingRule = PlayerController.ShootingRules.BOUNCING_BULLETS;
                break;
            case 1: //Heavy Bullets
                ConfigUIController.Instance.heavyBullets.SetActive(true);
                currentShootingRule = PlayerController.ShootingRules.GRAVITY_BULLETS;
                break;
            case 2: //Inverted Axis
                ConfigUIController.Instance.invertedMouse.SetActive(true);
                currentShootingRule = PlayerController.ShootingRules.INVERTED_MOUSE;
                break;
            case 3: //One Shot
                ConfigUIController.Instance.oneBullet.SetActive(true);
                currentShootingRule = PlayerController.ShootingRules.ONLY_ONE_BULLET;
                break;
            default:
                currentShootingRule = PlayerController.ShootingRules.NONE;
                break;
        }
    }

    private void ConfigMovement()
    {
        switch (_match.movementRules)
        {
            case 0: //SlowMo
                ConfigUIController.Instance.slowMo.SetActive(true);
                currentMovementRule = PlayerController.MovementRules.SLOW_MOTION;
                break;
            case 1: //FastRound
                ConfigUIController.Instance.quickMovement.SetActive(true);
                currentMovementRule = PlayerController.MovementRules.HYPERFAST;
                break;
            case 2: //Inverted Controls
                ConfigUIController.Instance.invertedControls.SetActive(true);
                currentMovementRule = PlayerController.MovementRules.INVERTED_CONTROLS;
                break;
            default:
                currentMovementRule = PlayerController.MovementRules.NONE;
                break;
        }
    }

    private void ConfigWinCon()
    {
        switch (_match.winConRules)
        {
            case 0: //Die
                ConfigUIController.Instance.dieToWin.SetActive(true);
                currentWinningRule = PlayerController.WinningRules.WIN_BY_DYING;
                break;
            case 1: //Catch the flag
                ConfigUIController.Instance.catchTheFlag.SetActive(true);
                currentWinningRule = PlayerController.WinningRules.CATCH_THE_FLAG;
                break;
            case 2: //Only HeadShots
                ConfigUIController.Instance.onlyHeadShots.SetActive(true);
                currentWinningRule = PlayerController.WinningRules.ONLY_HEADSHOTS;
                break;
            default:
                currentWinningRule = PlayerController.WinningRules.NONE;
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
        AudioManager.Instance.Play("Countdown");
        yield return new WaitForSecondsRealtime(1);
        counter.text = "2";
        AudioManager.Instance.Play("Countdown");
        yield return new WaitForSecondsRealtime(1);
        counter.text = "1";
        AudioManager.Instance.Play("Countdown");
        yield return new WaitForSecondsRealtime(1);
        counter.text = "GO!";
        AudioManager.Instance.Play("GO");
        StartMatch();
        yield return new WaitForSecondsRealtime(0.75f);
        counter.gameObject.SetActive(false);
    }
}
