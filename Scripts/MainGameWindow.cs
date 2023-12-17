using System;
using Godot;
namespace Tetris;

public partial class MainGameWindow : Node2D
{
    private Timer fallTimer;
    private Grid Grid;
    private float cellSize;

    public override void _Ready()
    {
        SetupFallTimer();
        SetupGrid();

        var shapeFactory = new ShapeFactory(Grid.CellSize)
        {
            Name = "ShapeFactory"
        };

        AddChild(shapeFactory);
        GenShape();
        base._Ready();
    }

    private void SetupFallTimer()
    {
        fallTimer = GetNode<Timer>("Timings/FallTimer");
        fallTimer.Timeout += OnFallTimeout;
    }

    private void SetupGrid()
    {
        Grid = new();
        cellSize = Grid.CellSize;
        AddChild(Grid);
    }

    private void RedrawGrid()
    {
        // foreach(var sprite in Grid.GridSquares.Where(x => x != null)) {
        //     if(!sprite.Value.IsQueuedForDeletion()) {
        //         RemoveChild(GetNode<Sprite2D>(sprite.Value.Name.ToString()));
        //         sprite.Value.GlobalPosition = sprite.Key;
        //         sprite.Value.Name = $"block_{sprite.Key}";
        //     }
        // }
    }

    /// <summary>
    /// On the tick of the fall timer, try and shift a block down, if it can.
    /// </summary>
    private void OnFallTimeout()
    {
        if (!Grid.ShiftCurrentDown())
        {
            fallTimer.Stop();
            Grid.AddShapeToGrid();
            GenShape();
        }
    }

    public override void _Process(double delta)
    {
        QueueRedraw();
        ProcessSpawnBlockDEBUG();
        ProcessRotate();
        ProcessShiftRight();
        ProcessShiftLeft();
        ProcessPushDown();
    }

    private void ProcessPushDown()
    {
        if (Input.IsActionPressed("ui_down"))
        {
            fallTimer.WaitTime = 0.05;
        }
        else
        {
            fallTimer.WaitTime = 1;
        }
    }

    private void ProcessShiftLeft()
    {
        if (Input.IsActionJustPressed("ui_left"))
        {
            Grid.ShiftCurrentLeft();
        }
    }

    private void ProcessShiftRight()
    {
        if (Input.IsActionJustPressed("ui_right"))
        {
            Grid.ShiftCurrentRight();
        }
    }

    private void ProcessRotate()
    {
        if (Input.IsActionJustPressed("ui_up"))
        {
            Grid.CurrentShape.Rotate(false);
            Grid.CurrentShape.ShiftInWall(Grid);
        }
    }

    private void ProcessSpawnBlockDEBUG()
    {
        if (Input.IsActionJustPressed("ui_accept"))
        {
            fallTimer.Paused = !fallTimer.Paused;
        }
    }

    private void GenShape()
    {
        Grid.AddShape();
        fallTimer.Start();
    }
}