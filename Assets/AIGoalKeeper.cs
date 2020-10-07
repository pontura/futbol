using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGoalKeeper : AI
{
    public override void CharacterCatchBall(Character _character)
    {
        if (Game.Instance.state != Game.states.PLAYING) return;
        base.CharacterCatchBall(_character);
        if(_character == character)
            GetComponent<AiPositionGoalKeeper>().CharacterCatchBall();
    }
}
