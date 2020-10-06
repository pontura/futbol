using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour
{
    void Start()
    {
        Invoke("GotoGame", 3);
    }
    public void GotoGame()
    {
        Data.Instance.LoadLevel("1_MainMenu");
    }
}
