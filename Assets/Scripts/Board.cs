using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private const int WIDTH = 10;
    private const int HEIGHT = 5;

    private const float SIZE = 0.95f;

    private GameObject[,] components;

    void Start()
    {
        Material compMaterial = Resources.Load("Materials/compSlot", typeof(Material)) as Material;
        components = new GameObject[WIDTH, HEIGHT];
        for (int y = 0; y < HEIGHT; y++)
            for (int x = 0; x < WIDTH; x++)
            {
                GameObject component = GameObject.CreatePrimitive(PrimitiveType.Cube);
                component.name = "c(" + x + ", " + y + ")";
                component.transform.localScale = new Vector3(SIZE, SIZE, 1);
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

    void Update()
    {

    }
}
