using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class IngameMenu : MonoBehaviour
{
    public GameObject arcadePanel;
    bool isOn;

    void Start()
    {
        Reset();
    }
    void Reset()
    {
        isOn = false;
        Time.timeScale = Data.Instance.settings.timeScale;
        arcadePanel.SetActive(false);
    }
    public void Open()
    {
        isOn = true;
        Time.timeScale = 0.001f;
        arcadePanel.SetActive(true);
    }
    private void Update()
    {
        if (!isOn)
            return;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            SetAction(false);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            SetAction(true);
    }
    void SetAction( bool isDone)
    {
        if (isDone)
            MainMenu();
        else
            Reset();
    }
    void MainMenu()
    {
        Time.timeScale = 1;
        Data.Instance.LoadLevel("1_MainMenu");
    }
}
