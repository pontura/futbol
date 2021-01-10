using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpreadsheetLoader : MonoBehaviour
{
    [Serializable]
    public class Line
    {
        public string[] data;
    }
    //public void LoadFromTo(string googleURL, System.Action<List<Line>> onDone)
    //{
    //    StartCoroutine(GetData(googleURL, onDone));
    //}
    //IEnumerator GetData(string url, System.Action<List<Line>> onDone)
    //{
    //    using (WWW www = new WWW(url))
    //    {
    //        yield return www;
           
    //        CreateListFromFile(www.text, onDone);
    //    }
    //}
    public void CreateListFromFile(string text, System.Action<List<Line>> onDone)
    {
        string[] lines = text.Split("\n"[0]);
        List<Line> arr = new List<Line>();
        foreach (string line in lines)
            arr.Add(ParseLine(line));

        onDone(arr);
    }
    public Line ParseLine(string lineData)
    {
        Line line = new Line();
        line.data = lineData.Split(","[0]);
        return line;
    }
}
