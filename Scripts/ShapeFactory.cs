using Godot;
using System;
using System.Collections.Generic;
using Tetris;

public partial class ShapeFactory : Node2D
{
    private List<TShape> tShapes;
    private float mass = 0.0f;
    public int CellSize { get; }

    public ShapeFactory(int CellSize)
    {
        this.CellSize = CellSize;

    }
    public override void _Ready()
    {
        tShapes = new();
        GetShapes();
        base._Ready();
    }

   

    public TShape GetShape(Shapes s) {
        return tShapes.Find(x => x.Shape == s);
    }

    private void GetShapes()
    {
        foreach(var s in Enum.GetValues<Shapes>()) {
            var tileMap = GetTileMapForShape(s);
            var shape = new TShape(tileMap, s, Constants.ShapesToColorMapping[s], CellSize);
            mass += shape.Weight;
            shape.Visible = false;
            tShapes.Add(shape);
            AddChild(shape);
        }
    }

    private static bool[,] GetTileMapForShape(Shapes s)
    {
        switch(s) {
            case Shapes.I:
                var cellsI = new bool[4,4];
                cellsI[1,0] = true;
                cellsI[1,1] = true;
                cellsI[1,2] = true;
                cellsI[1,3] = true;
                return cellsI;
            case Shapes.O:
                var cellsO = new bool[2,2];
                cellsO[0,0] = true;
                cellsO[0,1] = true;
                cellsO[1,0] = true;
                cellsO[1,1] = true;
                return cellsO;
            case Shapes.L:
                var cellsL = new bool[3,3];
                cellsL[1,0] = true;
                cellsL[1,1] = true;
                cellsL[1,2] = true;
                cellsL[0,2] = true;
                return cellsL;
            case Shapes.T:
                var cellsT = new bool[3,3];
                cellsT[0,1] = true;
                cellsT[1,0] = true;
                cellsT[1,1] = true;
                cellsT[1,2] = true;
                return cellsT;
            case Shapes.J:
                var cellsJ = new bool[3,3];
                cellsJ[0,0] = true;
                cellsJ[1,0] = true;
                cellsJ[1,1] = true;
                cellsJ[1,2] = true;
                return cellsJ;
            case Shapes.S:
                var cellsS = new bool[3,3];
                cellsS[1,0] = true;
                cellsS[1,1] = true;
                cellsS[0,1] = true;
                cellsS[0,2] = true;
                return cellsS;
            case Shapes.Z:
                var cellsZ = new bool[3,3];
                cellsZ[0,0] = true;
                cellsZ[0,1] = true;
                cellsZ[1,1] = true;
                cellsZ[1,2] = true;
                return cellsZ;
            default:
                return null;
        }
    }

    public TShape Generate() {
        var pick = GD.Randf() * mass;
        foreach(var t in tShapes) {
            pick -= t.Weight;
            if(pick <= 0.0f) {
                return new TShape(
                    t.Cells,
                    t.Shape,
                    t.ShapeColor,
                    CellSize
                ) {
                    Visible = true
                };
            }
        }

        throw new System.Exception(); //Should never reach here
    }
}