using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public const int WIDTH = 20;
    public const int HEIGHT = 10;

    private const float SLOT_SIZE = 0.95f;

    private GameObject[,] components;

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
                component.transform.position = new Vector3(x + 0.5f, y + 0.5f, -0.125f);
                component.transform.eulerAngles = new Vector3(0, 0, 180);
                component.GetComponent<MeshRenderer>().material = compMaterial;
                component.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, 0.5f));
                component.AddComponent<CircuitComponent>();
                component.transform.parent = transform;
                components[x, y] = component;
            }

        gameObject.tag = "EditBoard";
    }

    public CompType GetCompAt(int x, int y)
    {
        if (x < 0 || x >= WIDTH || y < 0 || y >= HEIGHT)
            return null;

        return components[x, y].GetComponent<CircuitComponent>().GetCompType();
    }
}
