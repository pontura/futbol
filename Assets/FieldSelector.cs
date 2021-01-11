using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldSelector : MonoBehaviour
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
        Data.Instance.settings.mainSettings.stadium_id = id;
        Data.Instance.stadiumData.SetActiveStadium(id);
        print(id + "_________" + Data.Instance.stadiumData.active.totalPlayers);
        Data.Instance.LoadLevel("Selector");
    }
}
