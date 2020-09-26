using System.Collections;
using UnityEngine;

public class CompType
{
    public static ArrayList components = new ArrayList();
    
    public static readonly CompType DRAIN = new CompType("DRAIN", false, true, false, -1);
    public static readonly CompType BUTTON = new CompType("BUTTON", true, false, true, 0);
    public static readonly CompType LED = new CompType("LED", true, true, false, 1);
    public static readonly CompType AND = new CompType("AND", true, true, false, 2);
    public static readonly CompType OR = new CompType("OR", true, true, false, 2);
    public static readonly CompType XOR = new CompType("XOR", true, true, false, 2);
    public static readonly CompType BUFFER = new CompType("BUFFER", true, true, false, -1);
    public static readonly CompType NAND = new CompType("NAND", true, true, false, 2);
    public static readonly CompType NOR = new CompType("NOR", true, true, false, 2);
    public static readonly CompType XNOR = new CompType("XNOR", true, true, false, 2);
    public static readonly CompType NOT = new CompType("NOT", true, true, false, 1);

    private string c_name;
    private bool has_output;
    private bool takes_input;
    private bool interactive;
    private int numInputs;

    private Texture2D texture;
    private Texture2D texture_pwr;
    private Sprite sprite;
    private Sprite sprite_pwr;

    public CompType(string name, bool has_output, bool takes_input, bool interactive, int numInputs)
    {
        this.c_name = name;
        this.has_output = has_output;
        this.takes_input = takes_input;
        this.interactive = interactive;
        this.numInputs = numInputs;
        components.Add(this);
    }

    public string GetName()
    {
        return c_name;
    }

    public bool HasOutput()
    {
        return has_output;
    }

    public bool TakesInput()
    {
        return takes_input;
    }

    public bool IsInteractive()
    {
        return interactive;
    }

    public int GetNumInputs()
    {
        return numInputs;
    }

    public Texture2D GetTexture(bool pwr)
    {
        if (pwr && texture_pwr != null) return texture_pwr;
        return texture;
    }

    public Sprite GetSprite(bool pwr)
    {
        if (pwr) return sprite_pwr;
        return sprite;
    }
    public void SetTexture(Texture2D texture, bool powered)
    {
        if (powered)
            texture_pwr = texture;
        else
            this.texture = texture;
    }

    public void LoadSprites()
    {
        sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0, 0));
        
        if(texture_pwr != null)
            sprite_pwr = Sprite.Create(texture_pwr, new Rect(0.0f, 0.0f, texture_pwr.width, texture_pwr.height), new Vector2(0, 0));
    }

    public static CompType GetCompTypeByName(string name)
    {
        foreach (CompType comp in components)
            if (comp.GetName() == name)
                return comp;
        return null;
    }
}
