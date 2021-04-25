using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultSelector : MonoBehaviour
{
    public GameObject panel;
    bool isOn;

    private void Start()
    {
        Reset();
        Events.OnButtonClick += OnButtonClick;
    }
    private void OnDestroy()
    {
        Events.OnButtonClick -= OnButtonClick;
    }
    void OnButtonClick(int a, int b)
    {
        if(isOn)
            OnSelect(0);
    }
    private void Reset()
    {
        panel.SetActive(false);
        isOn = false;
    }
    public void Init()
    {
        Invoke("Delayed", 0.5f);
        panel.SetActive(true);
    }
    void Delayed()
    {
        isOn = true;
    }
    public void OnSelect(int id)
    {
        isOn = false;
        //if (id == 0)
        //    Data.Instance.settings.fileName = "FulboStars - Easy";
        //else if (id == 1)
        //    Data.Instance.settings.fileName = "FulboStars - Medium";
        //else
        //    Data.Instance.settings.fileName = "FulboStars - Hard";

        GetComponent<FieldSelector>().Init();
    }
}
