using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIForce : MonoBehaviour
{
    public Color[] colors;
    public Image bar;
    float value;
    float speed;
    int dir;
    bool isOn;

    private void Awake()
    {
        Events.OnGameStatusChanged += OnGameStatusChanged;
        Events.PlayerProgressBarSetState += PlayerProgressBarSetState;
        Events.CharacterCatchBall += CharacterCatchBall;

        PlayerProgressBarSetState(false);
    }
    private void OnDestroy()
    {
        Events.OnGameStatusChanged -= OnGameStatusChanged;
        Events.PlayerProgressBarSetState -= PlayerProgressBarSetState;
        Events.CharacterCatchBall -= CharacterCatchBall;
    }
    void CharacterCatchBall(Character ch)
    {
        Reset();
    }
    void PlayerProgressBarSetState(bool isOn)
    {
        Reset();
        gameObject.SetActive(isOn);
    }
    void OnGameStatusChanged(Game.states state)
    {
        if (state == Game.states.PLAYING)
            isOn = true;
        else
            isOn = false;
        Reset();
    }
    private void Reset()
    {        
        dir = 1;
        speed = Data.Instance.settings.gameplay.forceBarSpeed;
        CancelInvoke();
        bar.fillAmount = value;
    }
    void OnEnable()
    {
        value = Data.Instance.settings.gameplay.startingUIForceBar;
        isOn = true;
        Reset();
        Loop();
    }
    public float GetForce()
    {
        return value;
    }
    void Loop()
    {
        if (isOn)
        {
            if (value >= 1 && dir == 1)
                dir = -1;
            else if (value <= 0 && dir == -1)
                dir = 1;
            if (value < 0.2f)
                bar.color = colors[0];
            else if (value < 0.6f)
                bar.color = colors[1];
            else
                bar.color = colors[2];
            value += Time.deltaTime * speed * dir;
            bar.fillAmount = value;
        }
        Invoke("Loop", 0.05f);
    }
}
