﻿namespace Lynx.Model;

public struct PlyStackEntry
{
    public int StaticEval { get; set; }

    public Move Move { get; set; }

    public PlyStackEntry()
    {
        StaticEval = int.MaxValue;
    }
}
