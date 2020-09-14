using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public class UIControl : MonoBehaviour
{
    public static bool BlockedByUI;
    public static int numComps;

    private EventTrigger eventTrigger;
    private Canvas display;

    private static ArrayList buttons;
    private static ArrayList idleSprites;
    private static ArrayList hoverSprites;
    private static ArrayList names;

    private void Start()
    {
        eventTrigger = GetComponent<EventTrigger>();
        if (eventTrigger != null)
        {
            EventTrigger.Entry enterUIEntry = new EventTrigger.Entry();
            enterUIEntry.eventID = EventTriggerType.PointerEnter;
            enterUIEntry.callback.AddListener((eventData) => { EnterUI(); });
            eventTrigger.triggers.Add(enterUIEntry);

            EventTrigger.Entry exitUIEntry = new EventTrigger.Entry();
            exitUIEntry.eventID = EventTriggerType.PointerExit;
            exitUIEntry.callback.AddListener((eventData) => { ExitUI(); });
            eventTrigger.triggers.Add(exitUIEntry);
        }

        display = GameObject.FindGameObjectWithTag("Display").GetComponent<Canvas>();
        float width = display.GetComponent<RectTransform>().rect.width;
        float height = display.GetComponent<RectTransform>().rect.height;
        float iconScale = 75f;

        Texture2D[] loadedTextures = Resources.LoadAll<Texture2D>("Textures/Component/");
        ArrayList compTextures = new ArrayList();
        buttons = new ArrayList();
        hoverSprites = new ArrayList();
        idleSprites = new ArrayList();
        names = new ArrayList();

        foreach (Texture2D texture in loadedTextures)
            if (!texture.name.EndsWith("_ON"))
                compTextures.Add(texture);

        for (int i = 0; i < compTextures.Count; i++)
        {
            Texture2D texture = compTextures[i] as Texture2D;
            GameObject imgObj = new GameObject(texture.name);

            RectTransform imgTransform = imgObj.AddComponent<RectTransform>();
            imgTransform.transform.SetParent(display.transform);
            imgTransform.localScale = Vector3.one;
            imgTransform.anchoredPosition = new Vector2(width / 2 - iconScale + iconScale / 2, height / 2 - i * iconScale - iconScale / 2);
            imgTransform.sizeDelta = new Vector2(iconScale, iconScale);
            buttons.Add(imgObj);

            Image img = imgObj.AddComponent<Image>();
            img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            idleSprites.Add(img.sprite);

            Texture2D hoverTexture = Resources.Load("Textures/Component/" + texture.name + "_ON", typeof(Texture2D)) as Texture2D;
            hoverSprites.Add(Sprite.Create(hoverTexture, new Rect(0, 0, hoverTexture.width, hoverTexture.height), new Vector2(0, 0)));

            names.Add(texture.name);
        }

        numComps = names.Count;
    }

    private void Update()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            GameObject button = buttons[i] as GameObject;
            RectTransform rectTrans = button.GetComponent<RectTransform>();
            Vector2 localMousePosition = rectTrans.InverseTransformPoint(Input.mousePosition);
            bool hover = rectTrans.rect.Contains(localMousePosition);
            bool click = Input.GetMouseButtonDown(0);

            if (click && hover)
            {
                CircuitComponent.selType = i;
            }

            button.GetComponent<Image>().sprite = ((hover || CircuitComponent.selType == i) ? hoverSprites[i] : idleSprites[i]) as Sprite;
        }
    }

    public static string GetCompName(int index)
    {
        return names[index] as string;
    }

    public void EnterUI()
    {
        BlockedByUI = true;
    }

    public void ExitUI()
    {
        BlockedByUI = false;
    }
}