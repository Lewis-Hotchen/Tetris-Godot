using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Tetris;

public static class Constants
{

// Setup mappings for colours to shapes
       public static Dictionary<Shapes, Color> ShapesToColorMapping = new() {
            {Shapes.I, Colors.Aqua},
            {Shapes.O, Colors.Yellow},
            {Shapes.T, Colors.Purple},
            {Shapes.J, Colors.Blue},
            {Shapes.L, Colors.Orange},
            {Shapes.S, Colors.Green},
            {Shapes.Z, Colors.Red}
        };
}