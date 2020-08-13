using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactersSignals : MonoBehaviour
{
    public CharacterSignal signal_to_add;

    public void Add(Character character)
    {
        CharacterSignal s = Instantiate(signal_to_add);
        character.SetSignal(s);
        s.Init(Data.Instance.settings.colors[character.id - 1]);
    }
    public void ChangeSignal(Character from, Character to)
    {
        to.SetSignal(from.characterSignal);
        from.characterSignal = null;
    }
}
