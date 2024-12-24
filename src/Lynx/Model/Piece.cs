namespace Lynx.Model;

#pragma warning disable CA1708 // Identifiers should differ by more than case

public enum Piece
{
    Unknown = -1,
    P, N, B, R, Q, K,   // White
    p, n, b, r, q, k,   // Black
    None
}

#pragma warning restore CA1708 // Identifiers should differ by more than case
