using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StadiumsManager : MonoBehaviour
{
    private void Start()
    {
        Init();
    }
    public void Init()
    {
        GameObject stadiumAsset = Instantiate(Data.Instance.stadiumData.active.stadiumAsset);
        stadiumAsset.transform.SetParent(transform);
        stadiumAsset.transform.localPosition = Vector3.zero;
    }
}
