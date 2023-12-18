using Godot;
namespace Tetris;

public partial class MainGameWindow : Node2D
{
    private Timer fallTimer;
    private Grid Grid;
    private float cellSize;
    private bool isDebug = false;
    private Vector2 mouseHover = new(-1, -1);
    private Node2D panel;
    private Vector2 clickedPos;

    public override void _Ready()
    {
        var window = GetWindow();
        window.InitialPosition = Window.WindowInitialPosition.Absolute;

        SetupFallTimer();
        SetupGrid();
        panel = GetNode<Node2D>("UI");

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
        ProcessRotate();
        ProcessShiftRight();
        ProcessShiftLeft();
        ProcessPushDown();
        ProcessHardDrop();
        ProcessDebug();
    }

    private void ProcessDebug()
    {
        if (Input.IsActionJustPressed("debug"))
        {
            isDebug = !isDebug;
        }

        if (Input.IsActionJustPressed("pause"))
        {
            fallTimer.Paused = !fallTimer.Paused;
        }
    }

    public override void _Input(InputEvent @event)
    {
        // Mouse in viewport coordinates.
        if (@event is InputEventMouseButton eventMouseButton)
        {
            clickedPos = eventMouseButton.Position.Snapped(new Vector2(32, 32));
        }
    }

    private void ProcessHardDrop()
    {
        if (Input.IsActionJustPressed("ui_accept"))
        {
            fallTimer.Stop();
            Grid.ForceBlockDown();
            GenShape();
        }
    }


    private void ProcessPushDown()
    {
        if (Input.IsActionPressed("ui_down"))
        {
            fallTimer.WaitTime = 0.1f;
        }
        else
        {
            fallTimer.WaitTime = 0.5f;
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
            fallTimer.WaitTime = 0.02;
        }
    }

    private void GenShape()
    {
        Grid.AddShape();
        fallTimer.Start();
    }

    public override void _Draw()
    {
        if (isDebug)
        {
            GetNode<Label>("TimerLabel").Visible = true;
            GetNode<Label>("Label").Visible = true;

            //print columns
            for (int col = (int)Grid.GridStartX; col < Grid.GridEndX; col += (int)cellSize)
            {
                DrawDashedLine(new Vector2(col, 0), new Vector2(col, GetViewportRect().Size.Y), new Color(1, 1, 1, 0.5f), 1);
            }

            //print rows
            for (int row = (int)cellSize; row < GetViewportRect().Size.Y; row += (int)cellSize)
            {
                DrawDashedLine(new Vector2(Grid.GridStartX, row), new Vector2(Grid.GridEndX, row), new Color(1, 1, 1, 0.5f), 1);
            }


            GetNode<Label>("TimerLabel").Text = fallTimer.TimeLeft.ToString("#.##");
            GetNode<Label>("Label").Text = $"Pos: {clickedPos}";
        }
        else
        {
            if (GetNode<Label>("TimerLabel").Visible)
            {
                GetNode<Label>("TimerLabel").Visible = !GetNode<Label>("TimerLabel").Visible;
            }

            if (GetNode<Label>("Label").Visible)
            {
                GetNode<Label>("Label").Visible = !GetNode<Label>("Label").Visible;
            }
        }

        base._Draw();
    }
}