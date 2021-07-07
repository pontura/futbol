using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButton : MonoBehaviour
{
    public Text field;
    public int id;
    CharactersSelector ui;

    public Image thumb;
    public bool selected;

    public GameObject normalBG;
    public GameObject selectedBG;

    public void Init(CharactersSelector ui, CharactersData.CharacterData data, int id, bool selected)
    {
        this.id = id;
        this.ui = ui;
        this.selected = selected;
        SetSelected();
        thumb.sprite = data.thumb;
        field.text = Data.Instance.textsData.GetCharactersData(id).avatarName;
    }
    public void SetSelected()
    {
        normalBG.SetActive(false);
        selectedBG.SetActive(false);

        if (selected)
            selectedBG.SetActive(true);
        else
            normalBG.SetActive(true);
    }
    public void Clicked()
    {
        selected = !selected;
        SetSelected();
        ui.Clicked(this);
    }
}
