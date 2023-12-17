using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
namespace Tetris;

public partial class Grid : Node2D
{
    public TShape CurrentShape { get; set; }
    public List<GridSquare> GridSquares { get; }
    public int CellSize { get; } = 32;
    public Vector2 Dimensions { get; } = new(10, 25);
    public Vector2 GridSize { get; set; }
    public float GridXOffset { get; set; }
    public float GridStartX { get; set; }
    public float GridEndX { get; set; }
    public float GridStartY { get; set; }
    public float GridEndY { get; set; }

    public Grid()
    {
        GridSize = new Vector2(Dimensions.X * CellSize, Dimensions.Y * CellSize);
        GridSquares = new();

        Name = "Grid";
    }

    public override void _Ready()
    {
        //Set the offset for when the grid's x begins. This gives us a margin on either side.
        GridXOffset = GetViewportRect().Size.X - GridSize.X;

        //Set the grids position
        Position = new Vector2(Position.X + GridXOffset / 2, Position.Y);

        //Some helper variables for the grids bounds.
        GridStartX = GlobalPosition.X;
        GridEndX = GlobalPosition.X + GridSize.X;
        GridStartY = 0;
        GridEndY = GridSize.Y;

        base._Ready();
    }

    public override void _Process(double delta)
    {
        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawLine(new Vector2(0, 0), new Vector2(0, GridSize.Y), Colors.White, 1);
        DrawLine(new Vector2(GridSize.X, 0), new Vector2(GridSize.X, GridSize.Y), Colors.White, 1);
        base._Draw();
    }

    private List<List<GridSquare>> GetColumns()
    {
        var columns = new List<List<GridSquare>>();
        for (int cellCol = CellSize; cellCol <= GridSize.Y; cellCol += CellSize)
        {
            columns.Add(GridSquares.Where(x => x.GridPosition.Y == cellCol).ToList());
        }

        return columns;
    }

    private List<List<GridSquare>> GetRows()
    {
        var rows = new List<List<GridSquare>>();
        for (int cellRow = CellSize; cellRow < GridSize.Y; cellRow += CellSize)
        {
            rows.Add(GridSquares.Where(x => x.GridPosition.Y == cellRow).ToList());
        }

        return rows;
    }

    public void AddShapeToGrid()
    {
        foreach (var square in CurrentShape.Squares)
        {
            var squareToAdd = (GridSquare)square.Duplicate();
            squareToAdd.GlobalPosition = square.GlobalPosition;

            AddSquare(squareToAdd);
            squareToAdd.GlobalPosition = ToLocalGridCoords(squareToAdd.GlobalPosition);
            if (!TrySetSpriteOnCoords(squareToAdd))
            {
                RemoveChild(squareToAdd);
            }

            Console.WriteLine($"Square added at: {squareToAdd.GridPosition}");
        }

        CurrentShape.QueueFree();

        var rowsRemoved = CheckCompleteRows();
        ShiftRowsDown(rowsRemoved);
    }

    private void AddSquare(GridSquare square)
    {
        GridSquares.Add(square);
        AddChild(square);
    }

    private bool TrySetSpriteOnCoords(GridSquare square)
    {
        try
        {
            var local = ToLocalGridCoords(square.GlobalPosition);

            if (IsPositionOccupied(local))
            {
                Console.WriteLine($"Could not set sprite: {local}, it was occupied");
            }

            square.GridPosition = local;
            //square.Position = square.GridPosition;
        }
        catch (Exception e)
        {
            GD.Print("Sprite was not locked in", e);
            return false;
        }

        return IsPositionOccupied(square.GridPosition);
    }

    private Vector2 ToLocalGridCoords(Vector2 coords)
    {
        return coords + new Vector2(-GridXOffset / 2, 0);
    }

    private int[] CheckCompleteRows()
    {
        List<int> rowsRemovedCount = new();

        var completeRows = GetCompleteRows();
        foreach (var row in completeRows)
        {
            int removedRow = RemoveRow(row);
            rowsRemovedCount.Add(removedRow);
        }

        return rowsRemovedCount.ToArray();
    }



    private List<List<GridSquare>> GetCompleteRows()
    {
        return GetRows().Where(x => IsRowFull(x)).ToList();
    }

    private int RemoveRow(IEnumerable<GridSquare> allLockedInBlocksInRow)
    {
        List<GridSquare> queueFreeObjs = new();
        foreach (var block in allLockedInBlocksInRow)
        {
            GridSquares.Remove(block);
            queueFreeObjs.Add(block);
        }

        queueFreeObjs.ToList().ForEach(x => CallDeferred(nameof(FreeSquare), x));
        return (int)allLockedInBlocksInRow.DistinctBy(x => x.GridPosition.Y).Select(x => x.GridPosition.Y).FirstOrDefault();
    }

    private void ShiftRowsDown(int[] rowsRemoved)
    {
        foreach (var row in rowsRemoved)
        {
            for (int aboveRowComplete = row - 32; aboveRowComplete >= 0; aboveRowComplete -= CellSize)
            {
                var aboveRow = GetRow(aboveRowComplete);
                MoveRowDown(aboveRow);
            }
        }
    }

    private static void MoveRowDown(IEnumerable<GridSquare> aboveRow)
    {
        foreach (var square in aboveRow)
        {
            square.GridPosition += Vector2.Down * 32;
            square.Position += Vector2.Down * 32;
        }
    }

    private static void FreeSquare(GridSquare square)
    {
        square.Free();
    }

    private void PrintGrid()
    {
        foreach (var pair in GridSquares)
        {
            Console.WriteLine($"{pair.Name}");
        }
    }

    public IEnumerable<GridSquare> GetRow(int row)
    {
        return GridSquares.Where(x => x.GlobalPosition.Y == row);
    }

    public IEnumerable<GridSquare> GetColumn(int col)
    {
        return GridSquares.Where(x => x.GlobalPosition.X == col);
    }

    public bool IsRowFull(IEnumerable<GridSquare> row)
    {
        var isRowFull = row.Where(x => IsPositionOccupied(x.GridPosition));
        if (isRowFull.Count() * CellSize == GridSize.X)
        {
            ;
        }
        return isRowFull.Count() * CellSize == GridSize.X;
    }

    public bool IsColliding(GridSquare square, Vector2 moveVector)
    {
        var vec = ToLocalGridCoords(square.GlobalPosition + moveVector);
        return IsPositionOccupied(vec);
    }

    public void AddShape()
    {
        CurrentShape = GetNode<ShapeFactory>("../ShapeFactory").Generate();
        CurrentShape.Name = "currentshape";
        CurrentShape.Position = new Vector2(GridSize.X / 2 - CellSize, CellSize).Snapped(new Vector2(CellSize, CellSize));
        AddChild(CurrentShape);
    }

    public bool IsPositionOccupied(Vector2 position)
    {
        var occupied = GridSquares.Any(x => x.GridPosition == position);
        return occupied;
    }

    public bool IsGlobalPositionOccupied(Vector2 position)
    {
        var occupied = GridSquares.Any(x => x.GlobalPosition == position);
        return occupied;
    }

#nullable enable
    public GridSquare? FindByPosition(Vector2 position)
    {
        return GridSquares.FirstOrDefault(x => x.GridPosition == position);
    }

    internal bool ShiftCurrentDown()
    {
        if (CanMoveInYDirection())
        {
            var fallVec = new Vector2(0, CellSize);
            CurrentShape.Position += fallVec;
            return true;
        }

        return false;
    }

    public bool ShiftCurrentRight()
    {
        if (CanMoveInXDirection(Vector2.Right))
        {
            var righVec = new Vector2(CellSize, 0);
            CurrentShape.Position += righVec;
            return true;
        }

        return false;
    }

    public bool ShiftCurrentLeft()
    {
        if (CanMoveInXDirection(Vector2.Left))
        {
            var leftVec = new Vector2(-CellSize, 0);
            CurrentShape.Position += leftVec;
            return true;
        }

        return false;
    }

    public bool CanMoveInXDirection(Vector2 direction)
    {
        var canMove = new List<bool>();
        foreach (var square in CurrentShape.Squares)
        {
            var positionToCheck = square.GlobalPosition + direction * CellSize;

            if (direction == Vector2.Left)
            {
                canMove.Add(IsPositionInGridStartX(positionToCheck));
            }
            else
            {
                canMove.Add(IsPositionInGridEndX(positionToCheck));
            }
        }

        return canMove.All(x => x);
    }

    public bool CanMoveInYDirection()
    {
        var isOnFloor = IsShapeOnFloor();
        var isCollingWithAnother = IsShapeOnCollidingWithAnother(Vector2.Down);
        return !isOnFloor && !isCollingWithAnother;
    }

    private bool IsShapeOnCollidingWithAnother(Vector2 direction)
    {
        var didCollide = false;
        foreach (var square in CurrentShape.Squares)
        {
            if (IsColliding(square, direction * CellSize))
            {
                didCollide = true;
            }
        }

        return didCollide;
    }

    private bool IsShapeOnFloor()
    {
        var didCollide = new List<bool>();
        foreach (var square in CurrentShape.Squares)
        {
            didCollide.Add(square.GlobalPosition.Y + Vector2.Down.Y * CellSize >= GridSize.Y);
        }

        return didCollide.Any(x => x);
    }

    public Vector2 GetSquaresDirectionOutOfBoundsX(GridSquare square)
    {

        if (IsPositionInGridStartX(square.GlobalPosition) && !IsPositionInGridEndX(square.GlobalPosition))
        {
            return Vector2.Right;
        }
        else if (!IsPositionInGridStartX(square.GlobalPosition) && IsPositionInGridEndX(square.GlobalPosition))
        {
            return Vector2.Left;
        }
        else
        {
            return Vector2.Zero;
        }
    }

    public Vector2 GetSquaresDirectionOutOfBoundsY(GridSquare square)
    {
        if (IsPositionInGridStartY(square.GlobalPosition) && !IsPositionInGridEndY(square.GlobalPosition))
        {
            return Vector2.Down;
        }
        else if (!IsPositionInGridStartY(square.GlobalPosition) && IsPositionInGridEndY(square.GlobalPosition))
        {
            return Vector2.Up;
        }
        else
        {
            return Vector2.Zero;
        }
    }

    private bool IsPositionInGridStartX(Vector2 position)
    {
        return position.X >= GridStartX;
    }

    private bool IsPositionInGridEndX(Vector2 position)
    {
        return position.X < GridEndX;
    }

    private bool IsPositionInGridStartY(Vector2 position)
    {
        return position.Y > GridStartY;
    }

    private bool IsPositionInGridEndY(Vector2 position)
    {
        return position.Y < GridEndY;
    }
}