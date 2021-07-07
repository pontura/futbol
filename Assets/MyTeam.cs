using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTeam : MonoBehaviour
{
    public List<int> characters;
    public List<int> goalkeepers;

    public void SetCharacter(int id, bool add)
    {
        if (add)
            characters.Add(id);
        else
            characters.Remove(id);
    }
    public void SetCGeoalkeeper(int id, bool add)
    {
        if (add)
            goalkeepers.Add(id);
        else
            goalkeepers.Remove(id);
    }
}
