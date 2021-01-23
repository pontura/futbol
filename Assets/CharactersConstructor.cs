using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersConstructor : MonoBehaviour
{
    [SerializeField] private Character character;
    [SerializeField] private Character goalKeeper;

    public void AddCharacters()
    {
        AddCharacters(1);
        AddCharacters(2);
    }
    public void AddCharacters(int teamID)
    {
        Transform container;
        if(teamID == 1)
            container = Game.Instance.charactersManager.containerTeam1.transform;
        else container = Game.Instance.charactersManager.containerTeam2.transform;

        StadiumsData.StadiumData stadiumData = Data.Instance.stadiumData.active;
        CharactersPositions.PositionsData positionsData = Data.Instance.charactersPositions.GetPositionsData(stadiumData.charactersPositions_id);
        foreach (CharactersPositions.CharacterPositionData d in positionsData.posData)
        {
            Character thisCharacter;
           
            if (d.type == Character.types.GOALKEEPER)
                thisCharacter = Instantiate(goalKeeper);
            else thisCharacter = Instantiate(character);

            thisCharacter.type = d.type;
            thisCharacter.transform.SetParent(container);
            thisCharacter.transform.localScale = Vector3.one;
            float _x = d.pos.x * stadiumData.size_x / 2;
            float _z = d.pos.y * stadiumData.size_y / 2;

            if (teamID == 2)
                _x *= -1;

            if (_z > 1)
                thisCharacter.fieldPosition = Character.fieldPositions.UP;
            else if (_z < -1)
                thisCharacter.fieldPosition = Character.fieldPositions.DOWN;
            else
                thisCharacter.fieldPosition = Character.fieldPositions.CENTER;

            thisCharacter.transform.localPosition = new Vector3(_x, 0.54f, _z);          

        }

    }
}
