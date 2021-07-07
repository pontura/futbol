using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharactersSelector : MonoBehaviour
{
    public Text field;
    public Transform container;
    public CharacterButton button;
    public List<CharacterButton> all;
    public List<Button> tabs;
    int tabID = 0;

    void Start()
    {          
        LoadCharacters();
        SetTotals();
    }
    void LoadCharacters()
    {
        Utils.RemoveAllChildsIn(container);
        
        List<CharactersData.CharacterData> allCharacters = new List<CharactersData.CharacterData>();
        List<int> myTeam = new List<int>();
        if(tabID == 0)
        {
            allCharacters = CharactersData.Instance.all;
            myTeam = Data.Instance.myTeam.characters;
        }
        else if (tabID == 1)
        {
            allCharacters = CharactersData.Instance.all_goalkeepers;
            myTeam = Data.Instance.myTeam.goalkeepers;
        }
        int id = 1;
        foreach (CharactersData.CharacterData data in allCharacters)
        {
            bool characterInTeam = false;
            foreach (int myteamID in myTeam)
                if (id == myteamID)
                    characterInTeam = true;
            CharacterButton b = Instantiate(button, container);
            b.Init(this, data, id, characterInTeam);
            all.Add(b);
            id++;
        }
    }
   
    public void OnTabClicked(int _tabID)
    {
        if (_tabID == tabID) return;
        tabID = _tabID;
        LoadCharacters();
    }
    public void Clicked(CharacterButton button)
    {
        if (tabID == 2)
            return;

        if (tabID == 0)
            Data.Instance.myTeam.SetCharacter(button.id, button.selected);
        else if (tabID == 1)
            Data.Instance.myTeam.SetCGeoalkeeper(button.id, button.selected);
        SetTotals();
    }
    void SetTotals()
    {
        tabs[0].GetComponentInChildren<Text>().text = "JUGADORES (" + Data.Instance.myTeam.characters.Count + ")";
        tabs[1].GetComponentInChildren<Text>().text = "ARQUEROS (" + Data.Instance.myTeam.goalkeepers.Count + ")";
        tabs[2].GetComponentInChildren<Text>().text = "EQUIPO (" + (Data.Instance.myTeam.characters.Count + Data.Instance.myTeam.goalkeepers.Count) + ")";
    }
    public void Ready()
    {
        Data.Instance.LoadLevel("1_MainMenu");
    }
}
