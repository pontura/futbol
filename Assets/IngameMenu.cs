using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class IngameMenu : MonoBehaviour
{
    public GameObject panel;
    public GameObject arcadePanel;
    public GameObject mobilePanel;
    bool isOn;
    bool isMobile;
    void Start()
    {
        isMobile = Data.Instance.isMobile;
        mobilePanel.SetActive(false);
        arcadePanel.SetActive(false);
        Reset();
    }
    public void Reset()
    {
        isOn = false;
        Time.timeScale = Data.Instance.settings.timeScale;
        panel.SetActive(false);
    }
    public void Open()
    {
        isOn = true;
        Time.timeScale = 0.001f;
        panel.SetActive(true);
        if (isMobile)
            mobilePanel.SetActive(true);
        else
            arcadePanel.SetActive(true);
    }
    private void Update()
    {
        if (isMobile) return;
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
    public void MainMenu()
    {
        Time.timeScale = 1;
        Data.Instance.LoadLevel("1_MainMenu");
    }
}
