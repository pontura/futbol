using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBar : MonoBehaviour
{
    public Text playerName;
    Character character;
    public Vector3 offset;
    public GameObject progressBar;

    void Awake()
    {
        Events.CharacterCatchBall += CharacterCatchBall;
        Events.OnBallKicked += OnBallKicked;
        Events.OnGoal += OnGoal;
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        Events.CharacterCatchBall -= CharacterCatchBall;
        Events.OnBallKicked -= OnBallKicked;
        Events.OnGoal -= OnGoal;
    }
    void OnGoal(int id, Character ch)
    {
        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (character == null)
            return;
        transform.position = Game.Instance.cameraInGame.cam.WorldToScreenPoint(character.transform.position) + offset;        
    }
   
    void OnBallKicked(CharacterActions.kickTypes kickType, float forceForce, Character character)
    {
        character = null;
        Invoke("Reset", 1);
    }
    void CharacterCatchBall(Character character)
    {     
        gameObject.SetActive(true);
        //if (!character.isBeingControlled)
        //    progressBar.SetActive(false);
        //else
        //    progressBar.SetActive(true);

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
