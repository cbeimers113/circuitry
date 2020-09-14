using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitComponent : MonoBehaviour
{
    public static int selType = 0;

    private GameObject mainCam;
    private MeshRenderer meshRenderer;

    private int compType;
    private bool state;

    void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        meshRenderer = GetComponent<MeshRenderer>();
        compType = -1;
        state = false;
    }

    void Update()
    {
        GetOutput();
    }

    private void UpdateTexture()
    {
        if (compType != -1)
        {
            string fName = UIControl.GetCompName(compType) + (state ? "_ON" : "");
            meshRenderer.material = Resources.Load("Materials/component", typeof(Material)) as Material;
            meshRenderer.material.SetTexture("_MainTex", Resources.Load("Textures/Component/" + fName, typeof(Texture)) as Texture);
        }
        else
        {
            meshRenderer.material = Resources.Load("Materials/compSlot", typeof(Material)) as Material;
            meshRenderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 0.5f));
        }
    }

    private void SetType(int compType)
    {
        this.compType = compType;
        UpdateTexture();
    }

    public bool GetOutput()
    {
        if (compType == -1)
            return false;

        bool output = true;
        switch (UIControl.GetCompName(compType))
        {
            case "BUTTON":
                output = state;
                break;
            case "LED":
                output = false;
                break;
        }
        UpdateTexture();

        state = output;
        return state;
    }

    private void OnMouseOver()
    {
        meshRenderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 0.5f));

        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);
        bool middleClick = Input.GetMouseButtonDown(2);
        bool shift = Input.GetKey(KeyCode.LeftShift);

        if (!UIControl.BlockedByUI)
        {
            if (leftClick)
            {
                if (shift)
                {

                }
                else
                {
                    SetType(selType);
                }
            }
            else if (rightClick)
            {
                if (shift)
                {
                    state = !state;
                }
                else
                {
                    SetType(-1);
                }
            }
            if (middleClick)
            {
                if (shift)
                {
                    float x = transform.position.x;
                    float y = transform.position.y;
                    mainCam.GetComponent<CameraControl>().ZoomTo(x, y);
                }
                else
                {

                }
            }
        }
    }

    private void OnMouseExit()
    {
        if (compType == -1)
            meshRenderer.material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, 0.5f));
    }
}
