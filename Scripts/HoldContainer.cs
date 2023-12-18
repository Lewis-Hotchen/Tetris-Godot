using Godot;
using System;
using Tetris;

public partial class HoldContainer : Node2D
{
    public Shapes Shape { get; private set; }

    public void HoldShape(Shapes shape) {
        Shape = shape;
        var tShape = GetNode<ShapeFactory>("ShapeFactory").Generate(shape);
        tShape.Scale *= 0.75f;
        tShape.Name = "heldShape";
        GetNode<Panel>("CenterContainer/Panel").AddChild(tShape);
        tShape.Position = new Vector2(32, 32);
    }

    public Shapes SwapShape(Shapes shape) {
        var returnShape = GetNode<TShape>("CenterContainer/Panel/heldShape").Shape;
        RemoveChild(GetNode<TShape>("CenterContainer/Panel/heldShape"));
        GetNode<TShape>("CenterContainer/Panel/heldShape")?.Free();
        HoldShape(shape);
        return returnShape;
    }
}
