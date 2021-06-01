using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipSignal : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    public bool isOn;
    public float timer = 0;
    public float totalTime = 1;
    [SerializeField] private Image progressBar;
    System.Action OnReady;
    public bool pressed;

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
        print("OnSkipOff");
        pressed = false;
        isOn = false;
        panel.SetActive(false);
        Reset();
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
        pressed = false;
    }
    private void Reset()
    {
        pressed = false;
        timer = 0;
        panel.GetComponent<Animation>().Play("skipSignal_off");
    }
    void SetProgress()
    {
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
        OnSkipOff();
        OnReady();        
    }
}
