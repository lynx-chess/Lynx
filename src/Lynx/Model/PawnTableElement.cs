﻿namespace Lynx.Model;

#pragma warning disable CA1051 // Do not declare visible instance fields

public struct PawnTableElement
{
    public ulong Key;

    public int PackedScore;

    public void Update(ulong key, int packedScore)
    {
        Key = key;
        PackedScore = packedScore;
    }
}

#pragma warning restore CA1051 // Do not declare visible instance fields
