using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputOnPC : IInputInterface
{
    public InputDirection InputKey()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            return InputDirection.Right;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            return InputDirection.Left;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            return InputDirection.Up;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            return InputDirection.Down;
        }

        return InputDirection.None;
    }
}
