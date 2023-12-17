using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Tetris;

public partial class TShape : Node2D
{
    public TShape(bool[,] cells, Shapes shape, Color shapeColor, int stepSize)
    {
        Cells = cells;
        Shape = shape;
        ShapeColor = shapeColor;
        StepSize = stepSize;
        gridSquareFactory = new();
        Squares = new();
    }

    public bool[,] Cells { get; set; }

    public Shapes Shape { get; set; }

    [Export]
    public Color ShapeColor { get; set; }

    [Export]
    public float Weight { get; set; } = 1.0f;

    public int TSize { get; set; }
    public int StepSize { get; set; }
    public List<GridSquare> Squares {get;}

    private Vector2 edgePos = Vector2.Zero;
    private readonly int[][] coords = { new int[] { -1, 1 }, new int[] { -1, 0, 1 }, new int[] { -2, -1, 1, 2 } };
    private readonly GridSquareFactory gridSquareFactory;

    public override void _Ready()
    {
        TSize = Mathf.Max(Cells.GetLength(0), Cells.GetLength(1));
        SetTilePositions(true);
        base._Ready();
    }

    public void Rotate(bool left = true)
    {
        var rm = coords[TSize - 2];

        bool[,] rotatedTiles = Cells.Clone() as bool[,];

        if (left)
        {
            for (int y = 0; y < TSize; y++)
            {
                var xx = Array.IndexOf(rm, -rm[y]);
                for (int x = 0; x < TSize; x++)
                {
                    rotatedTiles[y, x] = Cells[x, xx];
                }
            }
        }
        else
        {
            for (int x = 0; x < TSize; x++)
            {
                var yy = Array.IndexOf(rm, -rm[x]);
                for (int y = 0; y < TSize; y++)
                {
                    rotatedTiles[y, x] = Cells[yy, y];
                }
            }
        }

        Cells = rotatedTiles;
        SetTilePositions(false);
    }

    private void SetTilePositions(bool addTiles = false)
    {
        var pos = Vector2.Zero;
        var idx = 0;
        var newTileCount = 1;
        for (int row = 0; row < Cells.GetLength(0); row++)
        {
            for (int col = 0; col < Cells.GetLength(1); col++)
            {
                if (Cells[row, col])
                {
                    if (addTiles)
                    {
                        var newTile = gridSquareFactory.CreateSquare(ShapeColor);
                        newTile.Position = pos;
                        newTile.Name = $"t{newTileCount}";
                        Squares.Add(newTile);
                        AddChild(newTile);
                        newTileCount++;
                    }
                    else
                    {
                        var square = (GridSquare)GetChild(idx);
                        square.Position = pos;
                        idx += 1;
                    }
                }

                pos.X += StepSize;
            }

            pos.X = 0;
            pos.Y += StepSize;
        }

    }

    public void ShiftInBlock(Grid grid)
    {
        var squaresInBlock = new Dictionary<Vector2, List<GridSquare>>(){
            {Vector2.Down, new List<GridSquare>()},
            {Vector2.Left, new List<GridSquare>()},
            {Vector2.Right, new List<GridSquare>()},
        };

        // float shiftValue = 32f;

        // foreach(var square in Squares) {
            
        // }
    }

    public void ShiftInWall(Grid grid)
    {
        var squaresInWall = new Dictionary<Vector2, List<GridSquare>>(){
            {Vector2.Down, new List<GridSquare>()},
            {Vector2.Left, new List<GridSquare>()},
            {Vector2.Right, new List<GridSquare>()},
            {Vector2.Zero, new List<GridSquare>()},
        };

        float shiftValue = 32f;

        foreach (var square in Squares)
        {
            squaresInWall[grid.GetSquaresDirectionOutOfBoundsX(square)].Add(square);
            squaresInWall[grid.GetSquaresDirectionOutOfBoundsY(square)].Add(square);
        }

        int numberOfCellsToShiftX = 0;
        int numberOfCellsToShiftY = 0;

        foreach (var pair in squaresInWall)
        {
            numberOfCellsToShiftX = pair.Value.Count;
            numberOfCellsToShiftY = pair.Value.Count;

            if (pair.Value.DistinctBy(y => y.Position.Y).Count() > 1)
            { //Confirming multiple squares have multiple y positions
                numberOfCellsToShiftX = 1;
            }

            if (pair.Value.DistinctBy(y => y.Position.X).Count() > 1)
            { //Confirming multiple squares have multiple x positions
                numberOfCellsToShiftY = 1;
            }

            if (pair.Key == Vector2.Down)
            {
                GlobalPosition = new Vector2(GlobalPosition.X, GlobalPosition.Y - shiftValue * numberOfCellsToShiftY);
            }

            if (pair.Key == Vector2.Right)
            {
                GlobalPosition = new Vector2(GlobalPosition.X - shiftValue * numberOfCellsToShiftX, GlobalPosition.Y);
            }

            if (pair.Key == Vector2.Left)
            {
                GlobalPosition = new Vector2(GlobalPosition.X + shiftValue * numberOfCellsToShiftX, GlobalPosition.Y);
            }
        }
    }

    private static bool IsInAnotherBlock(GridSquare square, Grid grid) { 
        return grid.IsGlobalPositionOccupied(square.GlobalPosition);
    }
}