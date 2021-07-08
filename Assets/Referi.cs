using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referi : Character
{
    Vector3 initialPos;
    Ball ball;
    public bool isInsideGame = true;

    public override void Start()
    {        
        initialPos = transform.position;
        base.Start();
        Events.OnGameStatusChanged += OnGameStatusChanged;
        Events.OnRestartGame += OnRestartGame;
    }
    public void OnDestroy()
    {
        Events.OnRestartGame -= OnRestartGame;
        Events.OnGameStatusChanged -= OnGameStatusChanged;
    }
    void OnRestartGame()
    {
        transform.position = initialPos;
        actions.Idle();
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
        ball = Game.Instance.ball;
        scaleFactor = Data.Instance.settings.scaleFactor;
        this.charactersManager = charactersManager;
        speed = Data.Instance.settings.referiSpeed;
        stats.speed = speed;
        GameObject asset = Instantiate(asset_to_instantiate);
        asset.transform.SetParent(characterContainer);
        asset.transform.localEulerAngles = asset.transform.localPosition = Vector3.zero;
        asset.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        actions.Init(asset, 0);
        Invoke("ChangeZ", Random.Range(4, 10));
        data = Data.Instance.textsData.GetReferisData(CharactersData.Instance.referiId);
        dataSources = CharactersData.Instance.all_referis[CharactersData.Instance.referiId-1];

        SetLimits();
    }
    public override void SetPosition(float _x, float _y)
    {
        if (!isInsideGame) return;
        MoveTo(_x, _y);
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
    float timer;
    void Update()
    {
        if (!isInsideGame) return;
        if (ball == null)
            return;
        if (Game.Instance.state != Game.states.GOAL && actions.state != CharacterActions.states.SPECIAL_ACTION)
        {
            timer += Time.deltaTime;
            if (timer < 1)  return;

            int _z = 0;
            int _x = 0;
            float dest_x = ball.transform.position.x/1.5f;
            if (Mathf.Abs(transform.position.x - dest_x) < 2)
            {
                actions.Idle();
                timer = 0;
            }
            else
            {
                if (transform.position.z < destZ)
                    _z = 1;
                else
                    _z = -1;
                if (Mathf.Abs(transform.position.x - dest_x) > 1)
                {
                    if (transform.position.x < dest_x)
                        _x = 1;
                    else
                        _x = -1;
                    MoveTo(_x, _z);
                }
                
            }
        }
    }
 
}
