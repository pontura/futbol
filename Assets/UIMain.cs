using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMain : MonoBehaviour
{
    static UIMain mInstance = null;
    public TeamUI team1;
    public TeamUI team2;

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
        
    }
    public Vector3 GetScore()
    {
        return new Vector2(Data.Instance.matchData.score.x, Data.Instance.matchData.score.y);
    }
}
