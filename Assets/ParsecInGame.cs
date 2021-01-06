using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParsecInGame : MonoBehaviour
{
    public void GotoGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("1_MainMenu");
    }
}
