using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referi : Character
{
    public override void Start()
    {
        base.Start();
        Events.OnGameStatusChanged += OnGameStatusChanged;
        actions.Pita();
    }
    public void OnDestroy()
    {
        Events.OnGameStatusChanged -= OnGameStatusChanged;
    }
    void OnGameStatusChanged(Game.states state)
    {
        switch(state)
        { 
        case Game.states.GOAL:
            actions.Pita();break;
        case Game.states.PLAYING:
            actions.Pita(); break;
        }
    }
    public void InitReferi(CharactersManager charactersManager, GameObject asset_to_instantiate)
    {
        this.charactersManager = charactersManager;
        speed = Data.Instance.settings.referiSpeed;
        GameObject asset = Instantiate(asset_to_instantiate);
        asset.transform.SetParent(characterContainer);
        asset.transform.localEulerAngles = asset.transform.localPosition = Vector3.zero;
        asset.transform.localScale = Vector3.one;
        actions.Init(asset, 0);
        Invoke("ChangeZ", Random.Range(4, 10));
        ball = Game.Instance.ball;
    }
    public override void SetPosition(int _x, int _y)
    {
       // MoveTo(_x, _y);
    }
    int destZ;
    void ChangeZ()
    {
        if (Game.Instance.state != Game.states.GOAL)
        {
            if (Random.Range(0, 10) > 5)
                destZ = Random.Range(-8, 8);
        }
        Invoke("ChangeZ", 1);
    }    
    void Update()
    {
        if (Game.Instance.state != Game.states.GOAL && actions.state != CharacterActions.states.SPECIAL_ACTION)
        {
            int _z = 0;
            int _x = 0;

            if (Mathf.Abs(transform.position.x - ball.transform.position.x) < 2)
                actions.Idle();
            else
            {
                if (transform.position.z < destZ)
                    _z = 1;
                else
                    _z = -1;
                if (Mathf.Abs(transform.position.x - ball.transform.position.x) > 1)
                {

                    if (transform.position.x < ball.transform.position.x)
                        _x = 1;
                    else
                        _x = -1;
                }
                MoveTo(_x, _z);

            }
        }
    }
 
}
