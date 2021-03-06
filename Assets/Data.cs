﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Data : MonoBehaviour
{
    const string PREFAB_PATH = "Data";
    static Data mInstance = null;
    public bool DEBUG;
    public string lastScene;
    public string newScene;
    private float time_ViewingMap = 7.5f;
    public Settings settings;
    public StadiumsData stadiumData;
    public CharactersPositions charactersPositions;
    public TextsData textsData;
    public bool isMobile;
    [HideInInspector] public MatchData matchData;
    [HideInInspector] public InputManagerUI inputManagerUI;
    public GameObject rewiredInputManager;
    public SpreadsheetLoader spreadsheetLoader;
    public MyTeam myTeam;

    public static Data Instance
    {
        get
        {
            return mInstance;
        }
    }
    public void LoadLevel(string aLevelName)
    {
        this.newScene = aLevelName;
         SceneManager.LoadScene(newScene);
    }
    void Awake()
    {
        if (!mInstance)
            mInstance = this;

        else
        {
            Destroy(this.gameObject);
            return;
        }
        //isMobile = false;
#if UNITY_EDITOR
        // isMobile = false;
#elif UNITY_STANDALONE
        DEBUG = false;
#elif UNITY_ANDROID || UNITY_IOS
        isMobile = true;
        DEBUG = false;
#endif
        matchData = GetComponent<MatchData>();
        inputManagerUI = GetComponent<InputManagerUI>();
        stadiumData = GetComponent<StadiumsData>();
        spreadsheetLoader = GetComponent<SpreadsheetLoader>();
        charactersPositions = GetComponent<CharactersPositions>();
        DontDestroyOnLoad(this);

    }
    public void SettingsLoaded()
    {
        if (!settings.mainSettings.isArcade && !isMobile)
            Instantiate(rewiredInputManager);
    }
}
