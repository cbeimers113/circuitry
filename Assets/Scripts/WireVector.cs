using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireVector
{

    private CircuitComponent source;
    private GameObject wire;
    private Vector2 sourcePos;

    public WireVector(CircuitComponent source, GameObject wire, Vector2 sourcePos)
    {
        this.source = source;
        this.wire = wire;
        this.sourcePos = sourcePos;
    }

    public bool GetOutput()
    {
        return source.GetOutput();
    }

    public bool CheckSource()
    {
        return GameObject.FindGameObjectWithTag("EditBoard").GetComponent<Board>().GetCompAt((int)sourcePos.x, (int)sourcePos.y) != null;
    }

    public GameObject GetWire()
    {
        return wire;
    }
}
