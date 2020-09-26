using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public const int WIDTH = 20;
    public const int HEIGHT = 10;

    private const float SLOT_SIZE = 0.95f;

    public GameObject wireUI;
    public GameObject overwriteUI;
    private GameObject[,] components;
    private GameObject wireStartObj;
    private GameObject wireEndObj;
    private GameObject wire;

    private bool uiOpen;

    void Start()
    {
        CircuitComponent.slotMaterial = Resources.Load("Materials/compSlot", typeof(Material)) as Material;
        CircuitComponent.compMaterial = Resources.Load("Materials/component", typeof(Material)) as Material;
        CircuitComponent.slotColor = new Color(1f, 1f, 1f, 0.5f);
        CircuitComponent.slotHoverColor = new Color(1f, 1f, 1f, 1f);

        transform.localScale = new Vector3(WIDTH, HEIGHT, 1);
        transform.position = new Vector3(WIDTH / 2, HEIGHT / 2, 0);
        Material compMaterial = Resources.Load("Materials/compSlot", typeof(Material)) as Material;
        components = new GameObject[WIDTH, HEIGHT];

        for (int y = 0; y < HEIGHT; y++)
            for (int x = 0; x < WIDTH; x++)
            {
                GameObject component = GameObject.CreatePrimitive(PrimitiveType.Cube);
                component.name = "c(" + x + ", " + y + ")";
                component.transform.localScale = new Vector3(SLOT_SIZE, SLOT_SIZE, 1);
                component.transform.position = new Vector3(x + 0.5f, y + 0.5f, -0.026f);
                component.transform.eulerAngles = new Vector3(0, 0, 180);
                component.GetComponent<MeshRenderer>().material = compMaterial;
                component.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, 0.5f));
                component.AddComponent<CircuitComponent>();
                component.transform.parent = transform;
                components[x, y] = component;
            }

        gameObject.tag = "EditBoard";
        wireUI.SetActive(false);
        overwriteUI.SetActive(false);
    }

    public CircuitComponent GetCompAt(int x, int y)
    {
        if (x < 0 || x >= WIDTH || y < 0 || y >= HEIGHT)
            return null;

        return components[x, y].GetComponent<CircuitComponent>();
    }

    public int GetWidth()
    {
        return WIDTH;
    }

    public int GetHeight()
    {
        return HEIGHT;
    }

    public void OpenWireUI(GameObject wireStartObj, GameObject wireEndObj, GameObject wire)
    {
        uiOpen = true;
        wireUI.SetActive(true);
        CompType sourceType = wireStartObj.GetComponent<CircuitComponent>().GetCompType();
        CompType destType = wireEndObj.GetComponent<CircuitComponent>().GetCompType();
        GameObject.Find("SourceImage").GetComponent<Image>().sprite = sourceType.GetSprite(false);
        GameObject.Find("DestImage").GetComponent<Image>().sprite = destType.GetSprite(false);
        GameObject.Find("SourceLabel").GetComponent<Text>().text = sourceType.GetName();
        GameObject.Find("DestLabel").GetComponent<Text>().text = destType.GetName();
        GameObject.Find("WireToLabel").GetComponent<Text>().text = "Wire " + sourceType.GetName() + " to " + destType.GetName() + ":";

        Dropdown dropDown = GameObject.Find("DestInputs").GetComponent<Dropdown>();
        dropDown.options.Clear();
        string alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        int numIn = destType.GetNumInputs();
        numIn = numIn == -1 ? 26 : numIn;
        for (int i = 0; i < numIn; i++)
            dropDown.options.Add(new Dropdown.OptionData() { text = "" + alpha.ToCharArray()[i] });

        dropDown.value = numIn - 1;
        for (int i = 0; i < numIn; i++)
            if (wireEndObj.GetComponent<CircuitComponent>().GetInputs()[i] == null)
            {
                dropDown.value = i;
                break;
            }

        this.wireStartObj = wireStartObj;
        this.wireEndObj = wireEndObj;
        this.wire = wire;
    }

    private void CloseWireUI()
    {
        uiOpen = false;
        wireUI.SetActive(false);
    }


    public bool IsUIOpen()
    {
        return uiOpen;
    }

    public void OnButtonOkay()
    {
        Dropdown dropDown = GameObject.Find("DestInputs").GetComponent<Dropdown>();
        CircuitComponent input = wireEndObj.GetComponent<CircuitComponent>().GetInputs()[dropDown.value];
        if (input != null)
        {
            overwriteUI.SetActive(true);
            GameObject.Find("OverwriteLabel").GetComponent<Text>().text = "This will overwrite " + input.GetCompType().GetName()
                                                 + "'s input '" + dropDown.options[dropDown.value].text + "'\n\nAre you sure ?";
        }
        else
            CompleteWire();
    }

    public void OnButtonCancel()
    {
        Destroy(wire);
        CloseWireUI();
    }

    public void OnButtonOverwriteConfirm()
    {
        overwriteUI.SetActive(false);
        CompleteWire();
    }

    public void OnButtonOverwriteCancel()
    {
        overwriteUI.SetActive(false);
    }

    private void CompleteWire()
    {
        CircuitComponent input = wireStartObj.GetComponent<CircuitComponent>();
        wireEndObj.GetComponent<CircuitComponent>().SetInput(GameObject.Find("DestInputs").GetComponent<Dropdown>().value, input);
        wire.transform.parent = wireEndObj.transform;
        CloseWireUI();
    }
}
