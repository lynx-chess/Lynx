namespace Lynx.Model;

public struct CastlingData
{
    private CastlingRights _castlingRights;

    public readonly CastlingRights CastlingRights => _castlingRights;

    public CastlingData(CastlingRights castlingRights)
    {
        _castlingRights = castlingRights;
    }

    public void UpdateCastlingRights(int sourceSquare, int targetSquare)
    {
        _castlingRights &= Constants.CastlingRightsUpdateConstants[sourceSquare];
        _castlingRights &= Constants.CastlingRightsUpdateConstants[targetSquare];
    }
}
