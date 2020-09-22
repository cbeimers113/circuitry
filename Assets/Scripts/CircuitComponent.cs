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

    private static InputControl.InputCode inputCode;
    private static InputControl.InputCode prevState;
    private static InputControl input;
    private static Texture2D wireStartTexture;
    private static Texture2D wireEndTexture;
    private static GameObject mainCam;
    public static CompType selType;
    private static GameObject wireStartObj;
    private static GameObject wireEndObj;
    private static GameObject wire;

    private static bool wiring;
    private static float wireWidth;

    private MeshRenderer meshRenderer;
    private ArrayList inputs;
    private CompType compType;

    private bool toggled;
    private bool hover;
    private bool state;

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
        inputs = new ArrayList();
    }

    public void Update()
    {
        if (compType == CompType.COMP_DRAIN)
            GetOutput();

        if (EventSystem.current.IsPointerOverGameObject())
            return;

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
                    int numIn = compType.GetNumInputs();
                    if (compType != null && compType.TakesInput() && (numIn == -1 || inputs.Count < numIn))
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
                    mainCam.GetComponent<CameraControl>().ZoomTo(x, y);
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

        int rIndex = -1;
        foreach (WireVector wv in inputs)
        {
            if (!wv.CheckSource())
            {
                Destroy(wv.GetWire());
                rIndex = inputs.IndexOf(wv);
            }
        }

        if (rIndex > -1)
            inputs.RemoveAt(rIndex);
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

    private void SetType(CompType compType)
    {
        this.compType = compType;

        if (compType == null)
        {
            foreach (WireVector wv in inputs)
                Destroy(wv.GetWire());

            inputs.Clear();
        }

        UpdateTexture();
    }

    public bool GetOutput()
    {
        if (compType == null || (compType != CompType.COMP_DRAIN && !compType.HasOutput()))
            return false;

        if (compType == CompType.COMP_BUTTON)
            return state;

        bool[] in_vals = new bool[inputs.Count];

        for (int i = 0; i < inputs.Count; i++)
        {
            in_vals[i] = (inputs[i] as WireVector).GetOutput();
        }

        bool output = false;

        if (compType == CompType.COMP_LED || compType == CompType.COMP_OR || compType == CompType.COMP_BUFFER ||
            compType == CompType.COMP_NOR || compType == CompType.COMP_NOT)
        {
            for (int i = 0; i < in_vals.Length; i++)
                output |= in_vals[i];

            if (compType == CompType.COMP_NOR || compType == CompType.COMP_NOT)
                output = !output;
        }
        else if (compType == CompType.COMP_AND || compType == CompType.COMP_NAND)
        {
            output = in_vals.Length > 0;
            for (int i = 0; i < in_vals.Length; i++)
                output &= in_vals[i];

            if (compType == CompType.COMP_NAND)
                output = !output;
        }
        else if (compType == CompType.COMP_XOR || compType == CompType.COMP_XNOR)
        {
            for (int i = 0; i < in_vals.Length; i++)
                if (i == 0)
                    output = in_vals[i];
                else if (i == 1)
                    output ^= in_vals[i];

            if (compType == CompType.COMP_XNOR)
                output = !output;
        }

        state = output;
        UpdateTexture();
        return state;
    }

    private void CreateWire()
    {
        DragWire();
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        
        CircuitComponent input = wireStartObj.GetComponent<CircuitComponent>();
        Vector2 sourcePos = new Vector2(wireStartObj.transform.position.x, wireStartObj.transform.position.y);
        wireEndObj.GetComponent<CircuitComponent>().AddInput(input, sourcePos);
        wireEndObj = null;
    }

    private void DragWire()
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
        float z = -0.55f;

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

    public void AddInput(CircuitComponent input, Vector2 sourcePos)
    {
        inputs.Add(new WireVector(input, wire, sourcePos));
    }

    public CompType GetCompType()
    {
        return compType;
    }
}
