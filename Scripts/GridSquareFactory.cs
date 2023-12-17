
using Godot;

namespace Tetris;

public class GridSquareFactory
{
    private readonly PackedScene gridSquare;

    public GridSquareFactory()
    {
       gridSquare = GD.Load<PackedScene>("res://Scenes/GridSquare.tscn"); 
    }

    public GridSquare CreateSquare(Color modulate) {
        var gs = (GridSquare) gridSquare.Instantiate();
        gs.GetNode<Sprite2D>("sprite").Modulate = modulate;
        return gs;
    }
}