using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBar : MonoBehaviour
{
    public Text playerName;
    Character character;
    public Vector3 offset;

    void Awake()
    {
        Events.CharacterCatchBall += CharacterCatchBall;
        Events.OnBallKicked += OnBallKicked;
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        Events.CharacterCatchBall -= CharacterCatchBall;
        Events.OnBallKicked -= OnBallKicked;
    }
    private void Update()
    {
        if (character == null)
            return;
        transform.position = Game.Instance.cameraInGame.cam.WorldToScreenPoint(character.transform.position) + offset;        
    }
   
    void OnBallKicked()
    {
        character = null;
        Invoke("Reset", 1);
    }
    void CharacterCatchBall(Character character)
    {
        gameObject.SetActive(true);
        this.character = character;
        CancelInvoke();
        playerName.text = character.data.avatarName.ToUpper();
    }
    void Reset()
    {       
        playerName.text = "";
        if (character == null)
            gameObject.SetActive(false);
    }
}
