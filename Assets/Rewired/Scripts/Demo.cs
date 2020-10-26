using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour
{
    private IInputManager input;

    private void Start()
    {
        input = InputManager.instance;
    }
    void Update()
    {
        for (int a = 0; a < 4; a++)
        {
            float h = input.GetAxis(a, InputAction.horizontal);
            float v = input.GetAxis(a, InputAction.vertical);

            if (h > 0.5f || h < -0.5f)
                print("horizontal " + h);
            if (v > 0.5f || v < -0.5f)
                print("vertical " + v);

            if(input.GetButtonDown(a, InputAction.action1))
            {
                print("player " + a + " Jump");
            }
            if (input.GetButtonDown(a, InputAction.action2))
            {
                print("player " + a + " attack");
            }
            if (input.GetButtonDown(a, InputAction.action3))
            {
                print("player " + a + " subAttack");
            }
        }

        //for (int a = 0; a < 4; a++)
        //{
        //    if (ReInput.players.GetPlayer(a).GetButtonDown(0))
        //    {
        //        print("player " + a + " button " + 0);
        //    }
        //    if (ReInput.players.GetPlayer(a).GetButtonDown(1))
        //    {
        //        print("player " + a + " button " + 1);
        //    }
        //    if (ReInput.players.GetPlayer(a).GetButtonDown(2))
        //    {
        //        print("player " + a + " button " + 2);
        //    }
        //}
    }
}
