using UnityEngine;
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
    public TextsData textsData;
    public bool isMobile;
    [HideInInspector] public MatchData matchData;

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
        isMobile = false;
#if UNITY_EDITOR
        isMobile = false;
#elif  UNITY_ANDROID || UNITY_IOS
        isMobile = true;
#endif
        matchData = GetComponent<MatchData>();
        DontDestroyOnLoad(this);
    }
}
