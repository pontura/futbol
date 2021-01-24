using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIntro : MonoBehaviour
{
    public float speed = 2;
    int totalCharacters;
    public Character character_to_instantiate;
    public GameObject ui;

    public CharactersManager charactersManager;
    public Transform container_team1;
    public Transform container_team2;

    void Start()
    {

        totalCharacters = Data.Instance.stadiumData.active.totalPlayers;

        for (int a = 0; a < totalCharacters; a++)
        {
            Character character1 = Instantiate(character_to_instantiate, Vector3.zero, Quaternion.identity, container_team1.transform);
            Character character2 = Instantiate(character_to_instantiate, Vector3.zero, Quaternion.identity, container_team2.transform);

            character1.transform.localPosition = new Vector3(0.5f, 0.54f, 16);
            character2.transform.localPosition = new Vector3(-0.5f, 0.54f, 16);

            if (a == 0)
            {
                character1.type = Character.types.GOALKEEPER;
                character2.type = Character.types.GOALKEEPER;
            }
            else
            {
                character1.type = Character.types.CENTRAL;
                character2.type = Character.types.CENTRAL;
            }

        }
        GetComponent<DialoguesManager>().Init();
        Events.ChangeVolume("croud", 0.25f);

        charactersManager.Init(0);
        charactersManager.referi.gameObject.SetActive(false);

        foreach (Character ch in charactersManager.team1)
            ch.gameObject.SetActive(false);
        foreach (Character ch in charactersManager.team2)
            ch.gameObject.SetActive(false);

        StartCoroutine(Init());

        Events.OnSkipOn(OnSkip);
    }
    void OnSkip()
    {
        Ready();
    }
    void Ready()
    {
        StopAllCoroutines();
        Data.Instance.LoadLevel("Game");
        Events.OnSkipOff();
    }
    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();
        Events.OnIntroSound(3, null);
        Events.PlaySound("crowd", "crowd_quiet", true);
        yield return new WaitForSeconds(8);

        Events.OnIntroSound(1, null);

        yield return new WaitForSeconds(4);

        Events.OnIntroSound(2, charactersManager.referi);
        charactersManager.referi.gameObject.SetActive(true);
        charactersManager.referi.actions.EnterCancha();
        Events.PlaySound("crowd", "crowd_intro", true);

        yield return new WaitForSeconds(2);
        ui.SetActive(false);
        Events.SetDialogue(charactersManager.referi, Data.Instance.textsData.GetRandomReferiDialogue("random"));
        float vol = 0.5f;
        Events.ChangeVolume("croud", vol);
        Events.PlaySound("common", "pito_enter", false);

        Character character;
        for (int id = 0; id< totalCharacters; id++)
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
        yield return new WaitForSeconds(1);
        Events.PlaySound("crowd", "crowd_quiet", true);
        yield return new WaitForSeconds(3);
        Ready();
    }
    IEnumerator SetCharacterOn(Character character)
    {
        character.gameObject.SetActive(true);
        character.actions.Run();
        yield return new WaitForSeconds(1.6f);
        character.actions.EnterCancha();
        Events.OnIntroSound(0, character);
        // if(Random.Range(0,10)>3)
        Events.SetDialogue(character, Data.Instance.textsData.GetRandomDialogue("random", character.data.id, character.type == Character.types.GOALKEEPER));
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
