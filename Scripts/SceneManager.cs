using Godot;
namespace Tetris;

public partial class SceneManager : Node {

    [Export]
    public string PauseKey { get; set; }

    private Node currentScene;

    [Export]
    public string MainGameWindowPath { get; set; }

    [Export]
    public string MainMenuPath { get; set; }

    public override void _Ready() {
        AddScene<Node>(MainMenuPath);
        base._Ready();
    }

    public void AddScene<T>(string path) where T : class
    {
        var scene = LoadScene<T>(path);
        if (scene is Node n)
        {

            AddChild(n);
            currentScene = n;
        }
    }

    public void SwitchScene<T>(string path) where T : class
    {
        currentScene.QueueFree();
        var scene = LoadScene<T>(path);
        if (scene is Node n)
        {
            AddChild(n);
            currentScene = n;
        }
    }

    public override void _Process(double delta) {
        if (Input.IsActionPressed(PauseKey))
        {
            SwitchScene<Node>(MainGameWindowPath);
        }

        base._Process(delta);
    }

    private static T LoadScene<T>(string path) where T : class
    {
        var packedGame = GD.Load<PackedScene>(path);
        return packedGame.Instantiate<T>();
    }
}