using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipSignal : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    bool isOn;
    float timer = 0;
    float totalTime = 1;
    [SerializeField] private Image progressBar;
    System.Action OnReady;
    bool pressed;

    void Start()
    {
        panel.SetActive(false);
        Events.OnSkipOn += OnSkipOn;
        Events.OnSkipOff += OnSkipOff;
        Events.OnButtonDown += OnButtonDown;
        Events.OnButtonClick += OnButtonClick;
    }
    void OnSkipOff()
    {
        pressed = false;
        isOn = false;
        panel.SetActive(false);
    }
    private void Update()
    {
        if (!isOn || !pressed) return;

        SetProgress();
    }
    void OnButtonDown(int playerID, int buttonId)
    {
        if (!isOn) return;
        panel.SetActive(true);
        panel.GetComponent<Animation>().Play("skipSignal_on");
        pressed = true;
    }
    void OnButtonClick(int playerID, int buttonId)
    {
        if (!isOn) return;
        Reset();
    }
    void OnSkipOn(System.Action OnReady)
    {
        isOn = true;
        this.OnReady = OnReady;
        Reset();
    }
    private void Reset()
    {
        pressed = false;
        timer = 0;
        panel.GetComponent<Animation>().Play("skipSignal_off");
    }
    void SetProgress()
    {
        print(timer);
        timer += Time.deltaTime;
        progressBar.fillAmount = timer / totalTime;
        if(timer>=totalTime)
        {
            Done();
        }
    }
    void Done()
    {
        isOn = false;
        OnReady();
        OnSkipOff();
    }
}
