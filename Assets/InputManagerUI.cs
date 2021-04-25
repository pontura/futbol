using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerUI : MonoBehaviour
{
    public float[] horizontalAxis;
    public float[] verticalAxis;

    float[] lastXDirection;
    float[] lastYDirection;

    public enum buttonTypes
    {
        BUTTON_1,
        BUTTON_2,
        BUTTON_3
    }
    private void Start()
    {
        horizontalAxis = new float[4];
        verticalAxis = new float[4];

        lastXDirection = new float[4];
        lastYDirection = new float[4];
    }
    void Update()
    {
        if (!Data.Instance.settings.loaded)
            return;
        if (Data.Instance.isMobile || Data.Instance.settings.mainSettings.isArcade)
        {
            if (Data.Instance.isMobile)
            {
                if (Input.GetMouseButtonDown(0))
                    Events.OnButtonDown(1, 1);
                else if (Input.GetMouseButtonUp(0))                    
                    Events.OnButtonClick(1, 1);
            }
            else {
                for (int id = 0; id < 4; id++)
                {

                    horizontalAxis[id] = Input.GetAxis("Horizontal" + (id + 1));
                    verticalAxis[id] = Input.GetAxis("Vertical" + (id + 1));

                    if (Input.GetButtonDown("Button" + (id + 1) + "_1"))
                        Events.OnButtonDown((id + 1), 1);

                    else if (Input.GetButtonDown("Button" + (id + 1) + "_2"))
                        Events.OnButtonDown((id + 1), 2);

                    else if (Input.GetButtonDown("Button" + (id + 1) + "_3"))
                        Events.OnButtonDown((id + 1), 3);

                    if (Input.GetButtonUp("Button" + (id + 1) + "_1"))
                        Events.OnButtonClick((id + 1), 1);

                    else if (Input.GetButtonUp("Button" + (id + 1) + "_2"))
                        Events.OnButtonClick((id + 1), 2);

                    else if (Input.GetButtonUp("Button" + (id + 1) + "_3"))
                        Events.OnButtonClick((id + 1), 3);

                    if (lastXDirection[id] != horizontalAxis[id])
                    {
                        if (horizontalAxis[id] > 0.25f)
                            Events.OnRight((id + 1), true);
                        else if (horizontalAxis[id] < -0.25f)
                            Events.OnRight((id + 1), false);
                        lastXDirection[id] = horizontalAxis[id];
                    }
                }

            }
        }
        else
        {
            int id = 0;
            for (id = 0; id < 4; id++)
            {
                if (InputManager.instance.GetButtonDown(id, InputAction.action1))
                    Events.OnButtonDown(id + 1,1);
                if (InputManager.instance.GetButtonDown(id, InputAction.action2))
                    Events.OnButtonDown(id + 1, 2);
                if (InputManager.instance.GetButtonDown(id, InputAction.action3))
                    Events.OnButtonDown(id + 1, 3);

                if (InputManager.instance.GetButtonUp(id, InputAction.action1))
                    Events.OnButtonClick(id + 1, 1);
                if (InputManager.instance.GetButtonUp(id, InputAction.action2))
                    Events.OnButtonClick(id + 1, 2);
                if (InputManager.instance.GetButtonUp(id, InputAction.action3))
                    Events.OnButtonClick(id + 1, 3);

                horizontalAxis[id] = (int)Mathf.Round(InputManager.instance.GetAxis(id, InputAction.horizontal));
                verticalAxis[id] = (int)Mathf.Round(InputManager.instance.GetAxis(id, InputAction.vertical));

                if (lastXDirection[id] != horizontalAxis[id])
                {
                    if (horizontalAxis[id] > 0.25f)
                        Events.OnRight(id+1, true);
                    else if (horizontalAxis[id] < -0.25f)
                        Events.OnRight(id+1, false);
                    lastXDirection[id] = horizontalAxis[id];
                }
            }

        }


    }
}
