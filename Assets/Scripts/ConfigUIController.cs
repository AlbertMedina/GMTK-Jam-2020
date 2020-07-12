using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConfigUIController : MonoBehaviour
{
    private static ConfigUIController instance;

    public static ConfigUIController Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<ConfigUIController>();
            return instance;
        }
    }

    public GameObject bouncyBullets;
    public GameObject heavyBullets;
    public GameObject invertedMouse;
    public GameObject oneBullet;
    public GameObject slowMo;
    public GameObject quickMovement;
    public GameObject invertedControls;
    public GameObject cantMove;
    public GameObject dieToWin;
    public GameObject catchTheFlag;
    public GameObject onlyHeadShots;
    public GameObject onlyMelee;

    private Animator anim;
    public TextMeshProUGUI counter;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Reset()
    {
        bouncyBullets.SetActive(false);
        heavyBullets.SetActive(false);
        invertedMouse.SetActive(false);
        oneBullet.SetActive(false);
        slowMo.SetActive(false);
        quickMovement.SetActive(false);
        invertedControls.SetActive(false);
        cantMove.SetActive(false);
        dieToWin.SetActive(false);
        catchTheFlag.SetActive(false);
        onlyHeadShots.SetActive(false);
        onlyMelee.SetActive(false);
    }

    public void Play()
    {
        anim.SetTrigger("ShowConfig");
    }

    public void Remove()
    {
        anim.SetTrigger("RemoveConfig");
    }

    public void PlaySound()
    {
        AudioManager.Instance.Play("ConfigSpawn");
    }
}
