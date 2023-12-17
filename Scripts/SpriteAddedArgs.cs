using System;
using Godot;

public class SpriteAddedArgs : EventArgs
{
    public Sprite2D[] SpriteAdded { get; }

    public SpriteAddedArgs(Sprite2D[] spriteAdded)
    {
        SpriteAdded = spriteAdded;
    }
}