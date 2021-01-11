using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultSelector : MonoBehaviour
{
    public GameObject panel;

    private void Start()
    {
        Reset();
    }
    private void Reset()
    {
        panel.SetActive(false);
    }
    public void Init()
    {
        panel.SetActive(true);
    }
    public void OnSelect(int id)
    {
        if(id == 0)
            Data.Instance.settings.fileName = "FulboStars - Easy";
        else if (id == 1)
            Data.Instance.settings.fileName = "FulboStars - Medium";
        else
            Data.Instance.settings.fileName = "FulboStars - Hard";

        GetComponent<FieldSelector>().Init();
    }
}
