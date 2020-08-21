using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referi : Character
{    

    public void InitReferi(CharactersManager charactersManager, GameObject asset_to_instantiate)
    {
        this.charactersManager = charactersManager;
        speed = Data.Instance.settings.referiSpeed;
        GameObject asset = Instantiate(asset_to_instantiate);
        asset.transform.SetParent(characterContainer);
        asset.transform.localEulerAngles = asset.transform.localPosition = Vector3.zero;
        asset.transform.localScale = Vector3.one;
        print("InitReferi " + characterContainer.name);
        actions.Init(asset, 0);
        Invoke("ChangeZ", Random.Range(4, 10));
        ball = Game.Instance.ball;
    }




    int _z;
    void ChangeZ()
    {
        if (Game.Instance.state != Game.states.GOAL)
        {
            if (Random.Range(0, 10) > 5)
                _z = Random.Range(-5, 5);
        }
        Invoke("ChangeZ", 1);
    }    
    void Update()
    {
        if (Game.Instance.state != Game.states.GOAL)
        {
            _z = 0;
            if (ball.character != null &&  Mathf.Abs(transform.position.x - ball.transform.position.x) < 2)
                actions.Idle();
            else if (transform.position.x < ball.transform.position.x)
                MoveTo(1, _z);
            else if (transform.position.x > ball.transform.position.x)
                MoveTo(-1, _z);
        }
    }
}
