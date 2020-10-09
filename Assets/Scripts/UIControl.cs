using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    private const string TITLE = "Techne Alpha";

    public GameObject[] buttons;
    public Text selLabel;
    public Text titleLabel;

    private KeyCode[] hotbarKeys;

    private int selIndex;

    public void Start()
    {
        titleLabel.text = TITLE;

        foreach (CompType comp in CompType.components)
        {
            Texture2D texture = Resources.Load<Texture2D>("Textures/Component/" + comp.GetName());
            Texture2D texture_on = Resources.Load<Texture2D>("Textures/Component/" + comp.GetName() + "_ON");

            comp.SetTexture(texture, false);
            comp.SetTexture(texture_on, true);
            comp.LoadSprites();
        }

        hotbarKeys = new KeyCode[] {
            KeyCode.BackQuote,
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
            KeyCode.Alpha0,
            KeyCode.Minus,
            KeyCode.Equals
        };

        Select(0);
    }

    public void Update()
    {
        int index = -1;
        GameObject selected = EventSystem.current.currentSelectedGameObject;

        for (int i = 0; i < buttons.Length; i++)
            if (buttons[i] == selected)
                index = i;

        if (index != selIndex)
            Select(index);

        if (Input.anyKeyDown)
            for (int i = 0; i < hotbarKeys.Length; i++)
            {
                if (Input.GetKeyDown(hotbarKeys[i]))
                {
                    Select(i);
                    break;
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    if (index != -1 && index > 0)
                        Select(index - 1);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    if (index != -1 && index < buttons.Length - 1)
                        Select(index + 1);
                }
            }
    }

    private void Select(int index)
    {
        if (index < 0 || index >= buttons.Length)
            return;

        buttons[selIndex].GetComponent<Image>().sprite = CompType.GetCompTypeByName(buttons[selIndex].name.Split('_')[1]).GetSprite(false);

        EventSystem.current.SetSelectedGameObject(buttons[index]);
        CompType comp = CompType.GetCompTypeByName(buttons[index].name.Split('_')[1]);
        if (comp != null)
        {
            CircuitComponent.selType = comp;
            selLabel.text = EventSystem.current.currentSelectedGameObject.name.Split('_')[1];
        }
        buttons[index].GetComponent<Image>().sprite = comp.GetSprite(true);
        selIndex = index;
    }
}