using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionsUIManager : MonoBehaviour
{
    public Transform container;
    public PositionsUIThumb thumb;
    public Vector2 scaleFactor = new Vector2(1, 1);
    public List<PositionsUIThumb> all;

    public void Init(CharactersPositions.PositionsData data, List<int> characters)
    {
        Utils.RemoveAllChildsIn(container);
        int id = 0;
        foreach(int characterID in characters)
        {
            PositionsUIThumb newThumb= Instantiate(thumb);            
            newThumb.transform.SetParent(container);
            newThumb.transform.localScale = Vector2.one;
            newThumb.Init(this, data.posData[id], characterID);
            id++;
        }
    }
}
