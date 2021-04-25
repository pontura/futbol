using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMain : MonoBehaviour
{
    public GameObject canvasEventSystem;
    static UIMain mInstance = null;
    public TeamUI team1;
    public TeamUI team2;
    public GameObject all;
    public UIForce uIForce;
    public IngameMenu ingameMenu;

    public static UIMain Instance
    {
        get  {  return mInstance;   }
    }
    void Awake()
    {
        if (!mInstance)
            mInstance = this;
    }
    void Start()
    {
        if (Data.Instance.isMobile)
            canvasEventSystem.SetActive(true);
        else
            canvasEventSystem.SetActive(false);
        Events.OnGoal += OnGoal;
    }
    void OnDestroy()
    {
        Events.OnGoal -= OnGoal;
    }
    public void OnShow()
    {
        all.SetActive(true);
    }
    void OnGoal(int i, Character c)
    {
        all.SetActive(false);
    }
    public Vector3 GetScore()
    {
        return new Vector2(Data.Instance.matchData.score.x, Data.Instance.matchData.score.y);
    }
}
