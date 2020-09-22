using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class InputControl : MonoBehaviour
{
    public enum InputCode
    {
        NO_INPUT,
        LEFT_CLICK,
        RIGHT_CLICK,
        MIDDLE_CLICK,
        SHIFT,
        CONTROL,
        CONTROL_SHIFT,
        SHIFT_LEFT_CLICK,
        SHIFT_RIGHT_CLICK,
        SHIFT_MIDDLE_CLICK,
        CONTROL_LEFT_CLICK,
        CONTROL_RIGHT_CLICK,
        CONTROL_MIDDLE_CLICK,
        CONTROL_SHIFT_LEFT_CLICK,
        CONTROL_SHIFT_RIGHT_CLICK,
        CONTROL_SHIFT_MIDDLE_CLICK
    }

    private const int NO_INPT = 0b000000;
    private const int MOUSE_L = 0b000001;
    private const int MOUSE_R = 0b000010;
    private const int MOUSE_M = 0b000100;
    private const int K_SHIFT = 0b001000;
    private const int K_CNTRL = 0b010000;

    private int input_state;

    private bool left_click;
    private bool right_click;
    private bool middle_click;
    private bool shift;
    private bool ctrl;

    public void Start()
    {
        Reset(false);
    }

    private void Reset(bool mouse_only)
    {
        left_click = false;
        right_click = false;
        middle_click = false;

        if (!mouse_only)
        {
            shift = false;
            ctrl = false;
        }
    }

    public void Update()
    {
        input_state = NO_INPT;
        if (Input.GetMouseButtonDown(0)) left_click = true;
        if (Input.GetMouseButtonDown(1)) right_click = true;
        if (Input.GetMouseButtonDown(2)) middle_click = true;
        if (Input.GetKeyDown(KeyCode.LeftShift)) shift = true;
        if (Input.GetKeyDown(KeyCode.LeftControl)) ctrl = true;

        if (Input.GetMouseButtonUp(0)) left_click = false;
        if (Input.GetMouseButtonUp(1)) right_click = false;
        if (Input.GetMouseButtonUp(2)) middle_click = false;
        if (Input.GetKeyUp(KeyCode.LeftShift)) shift = false;
        if (Input.GetKeyUp(KeyCode.LeftControl)) ctrl = false;

        if (left_click) input_state |= MOUSE_L;
        if (right_click) input_state |= MOUSE_R;
        if (middle_click) input_state |= MOUSE_M;
        if (shift) input_state |= K_SHIFT;
        if (ctrl) input_state |= K_CNTRL;
    }

    public InputCode GetInputCode()
    {
        switch (input_state)
        {
            case NO_INPT:
                return InputCode.NO_INPUT;
            case MOUSE_L:
                return InputCode.LEFT_CLICK;
            case MOUSE_R:
                return InputCode.RIGHT_CLICK;
            case MOUSE_M:
                return InputCode.MIDDLE_CLICK;
            case K_SHIFT:
                return InputCode.SHIFT;
            case K_CNTRL:
                return InputCode.CONTROL;
            case K_CNTRL | K_SHIFT:
                return InputCode.CONTROL_SHIFT;
            case K_SHIFT | MOUSE_L:
                return InputCode.SHIFT_LEFT_CLICK;
            case K_SHIFT | MOUSE_R:
                return InputCode.SHIFT_RIGHT_CLICK;
            case K_SHIFT | MOUSE_M:
                return InputCode.SHIFT_MIDDLE_CLICK;
            case K_CNTRL | MOUSE_L:
                return InputCode.CONTROL_LEFT_CLICK;
            case K_CNTRL | MOUSE_R:
                return InputCode.CONTROL_RIGHT_CLICK;
            case K_CNTRL | MOUSE_M:
                return InputCode.CONTROL_MIDDLE_CLICK;
            case K_CNTRL | K_SHIFT | MOUSE_L:
                return InputCode.CONTROL_SHIFT_LEFT_CLICK;
            case K_CNTRL | K_SHIFT | MOUSE_R:
                return InputCode.CONTROL_SHIFT_RIGHT_CLICK;
            case K_CNTRL | K_SHIFT | MOUSE_M:
                return InputCode.CONTROL_SHIFT_MIDDLE_CLICK;
        }
        return InputCode.NO_INPUT;
    }
}
