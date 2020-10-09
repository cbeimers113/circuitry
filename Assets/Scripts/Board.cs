using System;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public const int DEF_WIDTH = 20;
    public const int DEF_HEIGHT = 10;

    private const float SLOT_SIZE = 0.95f;

    public GameObject wireUI;
    public GameObject overwriteUI;
    public GameObject debugUI;
    public GameObject debugConnTo;

    private GameObject[,] components;
    private GameObject wireStartObj;
    private GameObject wireEndObj;
    private GameObject wire;
    private Dropdown debugConns;
    private CircuitComponent debugComp;

    private int width;
    private int height;

    private bool uiOpen;

    void Start()
    {
        width = DEF_WIDTH;
        height = DEF_HEIGHT;

        Material compMaterial = Resources.Load("Materials/compSlot", typeof(Material)) as Material;
        CircuitComponent.slotMaterial = Resources.Load("Materials/compSlot", typeof(Material)) as Material;
        CircuitComponent.compMaterial = Resources.Load("Materials/component", typeof(Material)) as Material;
        CircuitComponent.slotColor = new Color(1f, 1f, 1f, 0.5f);
        CircuitComponent.slotHoverColor = new Color(1f, 1f, 1f, 1f);

        transform.localScale = new Vector3(GetWidth(), GetHeight(), 1);
        transform.position = new Vector3(GetWidth() / 2, GetHeight() / 2, 0);
        components = new GameObject[GetWidth(), GetHeight()];

        for (int y = 0; y < GetHeight(); y++)
            for (int x = 0; x < GetWidth(); x++)
            {
                GameObject component = GameObject.CreatePrimitive(PrimitiveType.Cube);
                component.name = "c(" + x + ", " + y + ")";
                component.transform.localScale = new Vector3(SLOT_SIZE, SLOT_SIZE, 1);
                component.transform.position = new Vector3(x + 0.5f, y + 0.5f, -0.026f);
                component.transform.eulerAngles = new Vector3(0, 0, 180);
                component.transform.parent = transform;
                component.GetComponent<MeshRenderer>().material = compMaterial;
                component.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, 0.5f));

                CircuitComponent comp = component.AddComponent<CircuitComponent>();
                comp.SetX(x);
                comp.SetY(y);

                components[x, y] = component;
            }

        gameObject.tag = "EditBoard";
        wireUI.SetActive(false);
        overwriteUI.SetActive(false);
        debugUI.SetActive(false);
    }

    public CircuitComponent GetCompAt(int x, int y)
    {
        if (x < 0 || x >= GetWidth() || y < 0 || y >= GetHeight())
            return null;

        return components[x, y].GetComponent<CircuitComponent>();
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
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

    public void OpenDebugUI(CircuitComponent comp)
    {
        debugComp = comp;
        uiOpen = true;
        debugUI.SetActive(true);
        debugConns = GameObject.Find("DebugConns").GetComponent<Dropdown>();
        GameObject.Find("DebugComp").GetComponent<Image>().sprite = debugComp.GetCompType().GetSprite(false);
        GameObject.Find("DebugName").GetComponent<Text>().text = debugComp.GetCompType().GetName();
        GameObject.Find("DebugCompState").GetComponent<Text>().text = debugComp.GetState() ? "HI" : "LO";
        GameObject.Find("DebugCompState").GetComponent<Text>().color = debugComp.GetState() ? Color.green : Color.red;

        debugConns.options.Clear();
        string alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        for (int i = 0; i < debugComp.GetInputs().Length; i++)
            debugConns.options.Add(new Dropdown.OptionData() { text = "" + alpha.ToCharArray()[i] });

        for (int i = 1; i > -1; i--)
            debugConns.value = i;

        OnDebugConnsDropdownChanged();
    }

    public void OnDebugConnsDropdownChanged()
    {
        CircuitComponent input = debugComp.GetInputs()[debugConns.value];

        if (input == null)
        {
            debugConnTo.SetActive(false);
        }
        else
        {
            CompType selComp = input.GetCompType();
            debugConnTo.SetActive(true);
            debugConnTo.GetComponent<Image>().sprite = selComp.GetSprite(debugComp.GetState());
        }
    }

    public void CloseDebugUI()
    {
        uiOpen = false;
        debugUI.SetActive(false);
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

    public void OpenBoardSettingsUI()
    {

    }

    public void Save()
    {
        string save_dir = Application.persistentDataPath;
        System.IO.Directory.CreateDirectory(save_dir);
        string save_path = "";

        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@save_path))
        {
            file.WriteLine("w." + GetWidth());
            file.WriteLine("h." + GetHeight());

            for (int y = 0; y < GetHeight(); y++)
                for (int x = 0; x < GetWidth(); x++)
                {
                    CircuitComponent comp = components[x, y].GetComponent<CircuitComponent>();
                    if (comp.GetCompType() != null)
                    {
                        string input_string = "";
                        CircuitComponent[] inputs = comp.GetInputs();

                        for (int i = 0; i < inputs.Length; i++)
                        {
                            string inp_type = inputs[i] == null ? "_" : inputs[i].GetX() + "-" + inputs[i].GetY();
                            input_string += i + ":" + inp_type + (i == inputs.Length - 1 ? "" : ",");
                        }

                        file.WriteLine(comp.GetCompType().GetName() + "." + x + "." + y + "." + input_string);
                    }
                }
        }
    }

    public void Load()
    {
        string load_dir = Application.persistentDataPath;
        System.IO.Directory.CreateDirectory(load_dir);
        string load_path = "";

        using (System.IO.StreamReader file = new System.IO.StreamReader(@load_path))
        {
            string line;
            while ((line = file.ReadLine()) != null)
            {
                string[] data = line.Split('.');
                if (data[0] == "w")
                    width = Int32.Parse(data[1]);
                else if (data[0] == "h")
                    height = Int32.Parse(data[1]);
                else
                {
                    CompType placeType = CompType.GetCompTypeByName(data[0]);
                    int x = Int32.Parse(data[1]);
                    int y = Int32.Parse(data[2]);
                    components[x, y].GetComponent<CircuitComponent>().SetType(placeType);

                    if (data.Length == 4 && data[3] != "")
                    {
                        string[] connData = data[3].Split(',');
                        
                        foreach (string datum in connData)
                        {
                            int index = Int32.Parse(datum.Split(':')[0]);

                            if (datum.Split(':')[1] != "_")
                            {
                                string[] pos = datum.Split(':')[1].Split('-');
                                CircuitComponent.wire = Instantiate(Resources.Load<GameObject>("Prefabs/Wire"));
                                CircuitComponent.wireStartObj = components[Int32.Parse(pos[0]), Int32.Parse(pos[1])];
                                CircuitComponent.wireEndObj = components[x, y];
                                components[x, y].GetComponent<CircuitComponent>().DragWire();
                                
                                CircuitComponent input = CircuitComponent.wireStartObj.GetComponent<CircuitComponent>();
                                CircuitComponent.wireEndObj.GetComponent<CircuitComponent>().SetInput(index, input);
                                CircuitComponent.wire.transform.parent = CircuitComponent.wireEndObj.transform;
                            }
                        }
                    }
                }
            }
        }
    }
}
