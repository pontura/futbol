using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIGoalKeeper : AI
{
    public override void CharacterCatchBall(Character _character)
    {
        base.CharacterCatchBall(_character);
        if(_character == character)
            GetComponent<AiPositionGoalKeeper>().CharacterCatchBall();
    }
}
