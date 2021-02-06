using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PositionsUIThumb : MonoBehaviour
{
    public Image image;
    PositionsUIManager manager;

    public void Init(PositionsUIManager manager, CharactersPositions.CharacterPositionData characterPositionData, int characterID)
    {
        this.manager = manager;
        print(characterPositionData.type + " characterID: " + characterID); 

        if (characterPositionData.type == Character.types.GOALKEEPER)
            image.sprite = CharactersData.Instance.all_goalkeepers[characterID-1].thumb;
        else
            image.sprite = CharactersData.Instance.all[characterID-1].thumb;

        float _x = characterPositionData.pos[1] * manager.scaleFactor.x;
        float _y = (2*(characterPositionData.pos[0] * manager.scaleFactor.y)) - manager.scaleFactor.y;
        transform.localPosition = new Vector2(_x , _y );
    }

}
