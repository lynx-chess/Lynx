namespace Lynx.Model;

/// <summary>
///  bin   dec
/// 0001    1   White king can O-O
/// 0010    2   White king can O-O-O
/// 0100    4   Black king can O-O
/// 1000    8   Black king can O-O-O
/// Examples:
/// * 1111      Both sides can castle both directions
/// * 1001      Black king => only O-O-O; White king => only O-O
/// </summary>
[Flags]
#pragma warning disable S4022 // Enumerations should have "Int32" storage
public enum CastlingRights : byte
#pragma warning restore S4022 // Enumerations should have "Int32" storage
{
    None = 0,   // RCS1135
    WK = 1, WQ = 2, BK = 4, BQ = 8
}
