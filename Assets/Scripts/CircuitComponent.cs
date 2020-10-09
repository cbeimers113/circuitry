using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class CircuitComponent : MonoBehaviour
{
    public static Material slotMaterial;
    public static Material compMaterial;
    public static Color slotColor;
    public static Color slotHoverColor;
    private static ArrayList tracked;

    private static InputControl.InputCode inputCode;
    private static InputControl.InputCode prevState;
    private static InputControl input;
    private static Texture2D wireStartTexture;
    private static Texture2D wireEndTexture;
    private static GameObject mainCam;
    public static CompType selType;
    public static GameObject wireStartObj;
    public static GameObject wireEndObj;
    public static GameObject wire;

    private static bool wiring;
    private static float wireWidth;

    private CircuitComponent[] inputs;
    private GameObject[] wires;
    private MeshRenderer meshRenderer;
    private CompType compType;
    private Board board;

    private bool toggled;
    private bool hover;
    private bool state;

    private int x;
    private int y;

    private int clock;
    private int clockTime;

    public void Start()
    {
        prevState = InputControl.InputCode.NO_INPUT;
        input = GameObject.FindGameObjectWithTag("Manager").GetComponent<InputControl>();
        wireStartTexture = Resources.Load<Texture2D>("Textures/WIRE_START");
        wireEndTexture = Resources.Load<Texture2D>("Textures/WIRE_END");
        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        selType = CompType.components[0] as CompType;
        meshRenderer = GetComponent<MeshRenderer>();
        compType = null;
        toggled = false;
        hover = false;
        state = false;
        inputs = new CircuitComponent[0];
        wires = new GameObject[0];
        board = transform.parent.gameObject.GetComponent<Board>();
        clockTime = 50;
    }

    public void Update()
    {
        if (MouseInputUIBlocker.BlockedByUI || board.IsUIOpen())
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            return;
        }

        if (compType == CompType.OUTPUT)
        {
            tracked = new ArrayList();
            state = GetOutput();
        }

        for (int i = 0; i < inputs.Length; i++)
            if (inputs[i] != null && inputs[i].compType == null)
            {
                inputs[i] = null;
                if (wires[i] != null)
                {
                    Destroy(wires[i]);
                    wires[i] = null;
                }
            }

        if (hover)
        {
            wireEndObj = this.gameObject;
            meshRenderer.material.SetColor("_Color", slotHoverColor);
            inputCode = input.GetInputCode();

            if (wiring)
            {
                DragWire();
                if (prevState != inputCode)
                {
                    wiring = false;
                    if (compType != null && wireStartObj != this && compType.TakesInput())
                    {
                        CreateWire();
                    }
                    else
                    {
                        Destroy(wire);
                    }
                }
            }

            if (toggled)
            {
                if (prevState != inputCode)
                {
                    toggled = false;
                }
            }

            switch (inputCode)
            {
                case InputControl.InputCode.NO_INPUT:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    break;
                case InputControl.InputCode.LEFT_CLICK:
                    SetType(selType);
                    break;
                case InputControl.InputCode.RIGHT_CLICK:
                    if (compType != null && compType.IsInteractive() && !toggled)
                    {
                        state = !state;
                        toggled = true;
                        UpdateTexture();
                    }
                    break;
                case InputControl.InputCode.SHIFT:
                    if (!wiring)
                        Cursor.SetCursor(wireStartTexture, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case InputControl.InputCode.SHIFT_LEFT_CLICK:
                    if (!wiring && compType != null && compType.HasOutput())
                    {
                        wiring = true;
                        wireStartObj = this.gameObject;
                        wire = Instantiate(Resources.Load<GameObject>("Prefabs/Wire"));
                        wireWidth = wire.transform.localScale.y;
                        Cursor.SetCursor(wireEndTexture, new Vector2(16, 16), CursorMode.Auto);
                    }
                    break;
                case InputControl.InputCode.SHIFT_RIGHT_CLICK:
                    SetType(null);
                    break;
                case InputControl.InputCode.SHIFT_MIDDLE_CLICK:
                    float x = transform.position.x;
                    float y = transform.position.y;
                    mainCam.GetComponent<CameraControl>().FocusOn(x, y);
                    break;
                case InputControl.InputCode.CONTROL_LEFT_CLICK:
                    if (compType != null && compType.TakesInput())
                        board.OpenDebugUI(this);
                    break;
                default:
                    break;
            }

            prevState = inputCode;
        }
        else
        {
            if (compType == null)
                meshRenderer.material.SetColor("_Color", slotColor);
        }
    }

    private void UpdateTexture()
    {
        if (compType != null)
        {
            Texture2D texture = compType.GetTexture(state);
            meshRenderer.material = compMaterial;
            meshRenderer.material.SetTexture("_MainTex", texture);
        }
        else
        {
            meshRenderer.material = slotMaterial;
            meshRenderer.material.SetColor("_Color", slotColor);
        }
        Resources.UnloadUnusedAssets();
    }

    public void SetType(CompType compType)
    {
        this.compType = compType;

        if (compType == null)
        {
            inputs = new CircuitComponent[0];
            wires = new GameObject[0];

            foreach (Transform child in transform)
                Destroy(child.gameObject);
        }
        else
        {
            int numIn = compType.GetNumInputs();
            numIn = numIn == -1 ? 26 : numIn;
            inputs = new CircuitComponent[numIn];
            wires = new GameObject[numIn];
            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = null;  // TODO: allow copying over wire connections?
                wires[i] = null;
            }
        }

        UpdateTexture();
    }

    public bool GetOutput()
    {
        if (compType == null || (compType != CompType.OUTPUT && !compType.HasOutput()))
            return false;

        if (compType == CompType.BUTTON)
            return state;

        if (tracked.Contains(this))
            return state;

        tracked.Add(this);

        bool[] in_vals = new bool[inputs.Length];

        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i] == null)
                in_vals[i] = false;
            else
                in_vals[i] = inputs[i].GetOutput();
        }

        bool output = false;

        if (compType == CompType.LED || compType == CompType.OR || compType == CompType.BUFFER ||
            compType == CompType.NOR || compType == CompType.NOT || compType == CompType.OUTPUT)
        {
            for (int i = 0; i < in_vals.Length; i++)
                output |= in_vals[i];

            if (compType == CompType.NOR || compType == CompType.NOT)
                output = !output;
        }
        else if (compType == CompType.AND || compType == CompType.NAND)
        {
            output = in_vals.Length > 0;
            for (int i = 0; i < in_vals.Length; i++)
                output &= in_vals[i];

            if (compType == CompType.NAND)
                output = !output;
        }
        else if (compType == CompType.XOR || compType == CompType.XNOR)
        {
            for (int i = 0; i < in_vals.Length; i++)
                if (i == 0)
                    output = in_vals[i];
                else if (i == 1)
                    output ^= in_vals[i];

            if (compType == CompType.XNOR)
                output = !output;
        }
        else if (compType == CompType.CLOCK)
        {
            clock++;
            output = state;

            if (clock == clockTime)
            {
                if (!in_vals[0])
                    output = !state;
                clock = 0;
            }
        }

        state = output;
        UpdateTexture();
        return state;
    }

    public void CreateWire()
    {
        DragWire();

        if (wireEndObj.GetComponent<CircuitComponent>().GetCompType() != CompType.OUTPUT)
            board.OpenWireUI(wireStartObj, wireEndObj, wire);
        else
        {
            CircuitComponent input = wireStartObj.GetComponent<CircuitComponent>();
            wireEndObj.GetComponent<CircuitComponent>().SetInput(-1, input);
            wire.transform.parent = wireEndObj.transform;
        }

        wireEndObj = null;
        wireStartObj = null;
    }

    public void DragWire()
    {
        if (wireEndObj == null || wire == null)
            return;

        float startX = wireStartObj.transform.position.x;
        float startY = wireStartObj.transform.position.y;
        float endX = wireEndObj.transform.position.x;
        float endY = wireEndObj.transform.position.y;
        float deltaX = startX - endX;
        float deltaY = startY - endY;
        float dist = Mathf.Sqrt(deltaX * deltaX + deltaY * deltaY);
        float theta = Mathf.Atan2(deltaY, deltaX);
        float rot = Mathf.Rad2Deg * theta + 270;
        float scaledWidth = wireWidth * dist;
        float r = -2f;
        float offsX = r * (float)Math.Cos(theta);
        float offsY = r * (float)Math.Sin(theta);
        float x = startX + 0.5f * scaledWidth * offsX;
        float y = startY + 0.5f * scaledWidth * offsY;
        float z = -0.475f;

        wire.transform.localScale = new Vector3(wire.transform.localScale.x, scaledWidth, wire.transform.localScale.z);
        wire.transform.eulerAngles = new Vector3(0, 0, rot);
        wire.transform.position = new Vector3(x, y, z);
    }

    private void OnMouseOver()
    {
        hover = true;
    }

    private void OnMouseExit()
    {
        hover = false;
    }

    public void SetInput(int index, CircuitComponent input)
    {
        if (index >= 0)
        {
            if (wires[index] != null)
                Destroy(wires[index]);

            inputs[index] = input;
            wires[index] = wire;
        }
        else
        {
            int add_index = 0;
            for (int i = 0; i < inputs.Length; i++)
                if (inputs[i] == null)
                {
                    add_index = i;
                    break;
                }

            if (wires[add_index] != null)
                Destroy(wires[add_index]);

            inputs[add_index] = input;
            wires[add_index] = wire;
        }
    }

    public void RemoveWire(int index)
    {
        Destroy(wires[index]);
        wires[index] = null;
    }

    public CircuitComponent[] GetInputs()
    {
        return inputs;
    }

    public CompType GetCompType()
    {
        return compType;
    }

    public bool GetState()
    {
        return state;
    }

    public void SetX(int x)
    {
        this.x = x;
    }

    public void SetY(int y)
    {
        this.y = y;
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }
}
