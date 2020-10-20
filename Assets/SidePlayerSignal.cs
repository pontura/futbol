using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class SidePlayerSignal : MonoBehaviour
{
    Character character;
    public GameObject asset;
    CameraInGame cameraInGame;
    public float offsetZ = 2;

    void Start()
    {
        cameraInGame = Game.Instance.cameraInGame;
        Events.CharacterCatchBall += CharacterCatchBall;
        Events.OnGoal += OnGoal;
        Events.SwapCharacter += SwapCharacter;
        Loop();
        asset.transform.localPosition = new Vector3(360, 0, 0);
    }
    void OnDestroy()
    {
        Events.CharacterCatchBall -= CharacterCatchBall;
        Events.OnGoal -= OnGoal;
        Events.SwapCharacter -= SwapCharacter;
    }
    void OnGoal(int id, Character ch)
    {
        asset.SetActive(false);
    }
    void CharacterCatchBall(Character character)
    {
        if (character.isBeingControlled)
            this.character = character;
    }
    void SwapCharacter(Character character)
    {
        if (character.isBeingControlled)
            this.character = character;
    }
    void Loop()
    {
        Invoke("Loop", 0.05f);
        if(character != null)
        {        
            Vector3 character_pos = cameraInGame.cam.WorldToScreenPoint(character.transform.localPosition);
            float fromCenter = character_pos.x - (Screen.width / 2);
            if (fromCenter < 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);
            if (Mathf.Abs(fromCenter) > Screen.width / 2)
            {
                asset.SetActive(true);
                Vector3 p = asset.transform.position;
                p.y = character_pos.y;
                asset.transform.position = p;
            }      
            else asset.SetActive(false);
        }
    }
}
