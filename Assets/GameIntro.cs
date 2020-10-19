using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIntro : MonoBehaviour
{
    public float speed = 2;

    public CharactersManager charactersManager;
    void Start()
    {
        Events.ChangeVolume("croud", 0.25f);
        
        
        charactersManager.Init(0);
        charactersManager.referi.gameObject.SetActive(false);
        foreach (Character ch in charactersManager.team1)
            ch.gameObject.SetActive(false);
        foreach (Character ch in charactersManager.team2)
            ch.gameObject.SetActive(false);

        StartCoroutine(Init());
    }
    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();
        Events.OnIntroSound(1, null);
        Events.PlaySound("crowd", "crowd_quiet", true);
        
        yield return new WaitForSeconds(4);

        Events.OnIntroSound(2, charactersManager.referi);
        charactersManager.referi.gameObject.SetActive(true);
        charactersManager.referi.actions.EnterCancha();

        yield return new WaitForSeconds(2);
        Events.SetDialogue(charactersManager.referi, Data.Instance.textsData.GetRandomReferiDialogue("random"));

        float vol = 0.5f;
        Events.ChangeVolume("croud", vol);
        Events.PlaySound("crowd", "crowd_gol", true);
        Character character;
        for (int id = 0; id<5; id++)
        {
            character = charactersManager.team1[id];
            StartCoroutine(SetCharacterOn(character));
            yield return new WaitForSeconds(1.5f);
            character = charactersManager.team2[id];
            StartCoroutine(SetCharacterOn(character));
            yield return new WaitForSeconds(1.5f);
            vol -= 0.05f;
            Events.ChangeVolume("croud", vol);
        }
        yield return new WaitForSeconds(4);
        Data.Instance.LoadLevel("Game");
    }
    IEnumerator SetCharacterOn(Character character)
    {
        character.gameObject.SetActive(true);
        character.actions.Run();
        yield return new WaitForSeconds(1.6f);
        character.actions.EnterCancha();
        Events.OnIntroSound(0, character);
        // if(Random.Range(0,10)>3)
        Events.SetDialogue(character, Data.Instance.textsData.GetRandomDialogue("random", character.characterID, character.isGoldKeeper));
    }
    void Update()
    {
        if (charactersManager.referi.gameObject.activeSelf)
            Move(charactersManager.referi);
        foreach (Character ch in charactersManager.team1)
            if (ch.gameObject.activeSelf)
                Move(ch);
        foreach (Character ch in charactersManager.team2)
            if (ch.gameObject.activeSelf)
                Move(ch);
    }
    private void Move(Character ch)
    {
        Vector3 pos = ch.transform.localPosition;
        pos.z -= speed * Time.deltaTime;
        ch.transform.localPosition = pos;
        
    }
}
