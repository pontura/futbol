using UnityEngine;

public class AIState
{
    public float timer;
    [HideInInspector] public AI ai;
    public Color color;
    AIState state;

    public virtual void Init(AI ai)
    {
        this.ai = ai;
        state = this;
    }
    public virtual void GotoBall() { }
    public virtual void SetActive() { }
    public virtual void OnReset() { state = this; }
    public virtual AIState UpdatedByAI() { return null; }
    public virtual void OnCharacterCatchBall(Character character) { }
    public virtual void OnBallNearOnAir() { }
    public virtual void OnFollow(Transform target) { }

    public void ResetAll()
    {
        if (ai.character.type == Character.types.GOALKEEPER)
            SetState(ai.aiIdleGK);
        else
            SetState(ai.aiIdle);
    }
    void Reset() { state = this; OnReset(); }

    public void SetState(AIState _newState) {
        state = _newState;
    }
    public AIState State() {
        if (state == null)
            state = this;
        if (state != this)
        {
            AIState newState = state;
            newState.SetActive();
            state = this;
            ai.SetNewDebugColor(newState.color);
            Reset();
           // Debug.Log("CAMBIA de " + this + "   a   " + newState + " character id: " + ai.character.data.avatarName + " color: " + color);
            return newState;
        }
        return this;
    }
   

    
}
