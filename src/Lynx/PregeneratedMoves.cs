using Lynx.Model;

namespace Lynx;
public static class PregeneratedMoves
{
    public static readonly int WhiteShortCastle = MoveExtensions.EncodeShortCastle(Constants.WhiteKingSourceSquare, Constants.WhiteShortCastleKingSquare, (int)Piece.K);
    public static readonly int WhiteLongCastle = MoveExtensions.EncodeLongCastle(Constants.WhiteKingSourceSquare, Constants.WhiteLongCastleKingSquare, (int)Piece.K);
    public static readonly int BlackShortCastle = MoveExtensions.EncodeShortCastle(Constants.BlackKingSourceSquare, Constants.BlackShortCastleKingSquare, (int)Piece.k);
    public static readonly int BlackLongCastle = MoveExtensions.EncodeLongCastle(Constants.BlackKingSourceSquare, Constants.BlackLongCastleKingSquare, (int)Piece.k);

    /// <summary>
    /// 8x4, indexed by target square, aka 'singlePushSquare'
    /// </summary>
    public static readonly Move[][] WhitePromotions =
    [
        [
            MoveExtensions.EncodePromotion((int)BoardSquare.a7, (int)BoardSquare.a8, (int)Piece.P, promotedPiece: (int)Piece.Q),
            MoveExtensions.EncodePromotion((int)BoardSquare.a7, (int)BoardSquare.a8, (int)Piece.P, promotedPiece: (int)Piece.R),
            MoveExtensions.EncodePromotion((int)BoardSquare.a7, (int)BoardSquare.a8, (int)Piece.P, promotedPiece: (int)Piece.N),
            MoveExtensions.EncodePromotion((int)BoardSquare.a7, (int)BoardSquare.a8, (int)Piece.P, promotedPiece: (int)Piece.B),
        ],
        [
            MoveExtensions.EncodePromotion((int)BoardSquare.b7, (int)BoardSquare.b8, (int)Piece.P, promotedPiece: (int)Piece.Q),
            MoveExtensions.EncodePromotion((int)BoardSquare.b7, (int)BoardSquare.b8, (int)Piece.P, promotedPiece: (int)Piece.R),
            MoveExtensions.EncodePromotion((int)BoardSquare.b7, (int)BoardSquare.b8, (int)Piece.P, promotedPiece: (int)Piece.N),
            MoveExtensions.EncodePromotion((int)BoardSquare.b7, (int)BoardSquare.b8, (int)Piece.P, promotedPiece: (int)Piece.B),
        ],
        [
            MoveExtensions.EncodePromotion((int)BoardSquare.c7, (int)BoardSquare.c8, (int)Piece.P, promotedPiece: (int)Piece.Q),
            MoveExtensions.EncodePromotion((int)BoardSquare.c7, (int)BoardSquare.c8, (int)Piece.P, promotedPiece: (int)Piece.R),
            MoveExtensions.EncodePromotion((int)BoardSquare.c7, (int)BoardSquare.c8, (int)Piece.P, promotedPiece: (int)Piece.N),
            MoveExtensions.EncodePromotion((int)BoardSquare.c7, (int)BoardSquare.c8, (int)Piece.P, promotedPiece: (int)Piece.B),
        ],
        [
            MoveExtensions.EncodePromotion((int)BoardSquare.d7, (int)BoardSquare.d8, (int)Piece.P, promotedPiece: (int)Piece.Q),
            MoveExtensions.EncodePromotion((int)BoardSquare.d7, (int)BoardSquare.d8, (int)Piece.P, promotedPiece: (int)Piece.R),
            MoveExtensions.EncodePromotion((int)BoardSquare.d7, (int)BoardSquare.d8, (int)Piece.P, promotedPiece: (int)Piece.N),
            MoveExtensions.EncodePromotion((int)BoardSquare.d7, (int)BoardSquare.d8, (int)Piece.P, promotedPiece: (int)Piece.B),
        ],
        [
            MoveExtensions.EncodePromotion((int)BoardSquare.e7, (int)BoardSquare.e8, (int)Piece.P, promotedPiece: (int)Piece.Q),
            MoveExtensions.EncodePromotion((int)BoardSquare.e7, (int)BoardSquare.e8, (int)Piece.P, promotedPiece: (int)Piece.R),
            MoveExtensions.EncodePromotion((int)BoardSquare.e7, (int)BoardSquare.e8, (int)Piece.P, promotedPiece: (int)Piece.B),
            MoveExtensions.EncodePromotion((int)BoardSquare.e7, (int)BoardSquare.e8, (int)Piece.P, promotedPiece: (int)Piece.N),
        ],
        [
            MoveExtensions.EncodePromotion((int)BoardSquare.f7, (int)BoardSquare.f8, (int)Piece.P, promotedPiece: (int)Piece.Q),
            MoveExtensions.EncodePromotion((int)BoardSquare.f7, (int)BoardSquare.f8, (int)Piece.P, promotedPiece: (int)Piece.R),
            MoveExtensions.EncodePromotion((int)BoardSquare.f7, (int)BoardSquare.f8, (int)Piece.P, promotedPiece: (int)Piece.N),
            MoveExtensions.EncodePromotion((int)BoardSquare.f7, (int)BoardSquare.f8, (int)Piece.P, promotedPiece: (int)Piece.B),
        ],
        [
            MoveExtensions.EncodePromotion((int)BoardSquare.g7, (int)BoardSquare.g8, (int)Piece.P, promotedPiece: (int)Piece.Q),
            MoveExtensions.EncodePromotion((int)BoardSquare.g7, (int)BoardSquare.g8, (int)Piece.P, promotedPiece: (int)Piece.R),
            MoveExtensions.EncodePromotion((int)BoardSquare.g7, (int)BoardSquare.g8, (int)Piece.P, promotedPiece: (int)Piece.N),
            MoveExtensions.EncodePromotion((int)BoardSquare.g7, (int)BoardSquare.g8, (int)Piece.P, promotedPiece: (int)Piece.B),
        ],
        [
            MoveExtensions.EncodePromotion((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.P, promotedPiece: (int)Piece.Q),
            MoveExtensions.EncodePromotion((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.P, promotedPiece: (int)Piece.R),
            MoveExtensions.EncodePromotion((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.P, promotedPiece: (int)Piece.N),
            MoveExtensions.EncodePromotion((int)BoardSquare.h7, (int)BoardSquare.h8, (int)Piece.P, promotedPiece: (int)Piece.B),
        ]
    ];

    /// <summary>
    /// 8x4, indexed by target square (aka 'singlePushSquare') - 56
    /// </summary>
    public static readonly Move[][] BlackPromotions =
    [
        [
            MoveExtensions.EncodePromotion((int)BoardSquare.a2, (int)BoardSquare.a1, (int)Piece.p, promotedPiece: (int)Piece.q),
            MoveExtensions.EncodePromotion((int)BoardSquare.a2, (int)BoardSquare.a1, (int)Piece.p, promotedPiece: (int)Piece.r),
            MoveExtensions.EncodePromotion((int)BoardSquare.a2, (int)BoardSquare.a1, (int)Piece.p, promotedPiece: (int)Piece.n),
            MoveExtensions.EncodePromotion((int)BoardSquare.a2, (int)BoardSquare.a1, (int)Piece.p, promotedPiece: (int)Piece.b),
        ],
        [
            MoveExtensions.EncodePromotion((int)BoardSquare.b2, (int)BoardSquare.b1, (int)Piece.p, promotedPiece: (int)Piece.q),
            MoveExtensions.EncodePromotion((int)BoardSquare.b2, (int)BoardSquare.b1, (int)Piece.p, promotedPiece: (int)Piece.r),
            MoveExtensions.EncodePromotion((int)BoardSquare.b2, (int)BoardSquare.b1, (int)Piece.p, promotedPiece: (int)Piece.n),
            MoveExtensions.EncodePromotion((int)BoardSquare.b2, (int)BoardSquare.b1, (int)Piece.p, promotedPiece: (int)Piece.b),
        ],
        [
            MoveExtensions.EncodePromotion((int)BoardSquare.c2, (int)BoardSquare.c1, (int)Piece.p, promotedPiece: (int)Piece.q),
            MoveExtensions.EncodePromotion((int)BoardSquare.c2, (int)BoardSquare.c1, (int)Piece.p, promotedPiece: (int)Piece.r),
            MoveExtensions.EncodePromotion((int)BoardSquare.c2, (int)BoardSquare.c1, (int)Piece.p, promotedPiece: (int)Piece.n),
            MoveExtensions.EncodePromotion((int)BoardSquare.c2, (int)BoardSquare.c1, (int)Piece.p, promotedPiece: (int)Piece.b),
        ],
        [
            MoveExtensions.EncodePromotion((int)BoardSquare.d2, (int)BoardSquare.d1, (int)Piece.p, promotedPiece: (int)Piece.q),
            MoveExtensions.EncodePromotion((int)BoardSquare.d2, (int)BoardSquare.d1, (int)Piece.p, promotedPiece: (int)Piece.r),
            MoveExtensions.EncodePromotion((int)BoardSquare.d2, (int)BoardSquare.d1, (int)Piece.p, promotedPiece: (int)Piece.n),
            MoveExtensions.EncodePromotion((int)BoardSquare.d2, (int)BoardSquare.d1, (int)Piece.p, promotedPiece: (int)Piece.b),
        ],
        [
            MoveExtensions.EncodePromotion((int)BoardSquare.e2, (int)BoardSquare.e1, (int)Piece.p, promotedPiece: (int)Piece.q),
            MoveExtensions.EncodePromotion((int)BoardSquare.e2, (int)BoardSquare.e1, (int)Piece.p, promotedPiece: (int)Piece.r),
            MoveExtensions.EncodePromotion((int)BoardSquare.e2, (int)BoardSquare.e1, (int)Piece.p, promotedPiece: (int)Piece.b),
            MoveExtensions.EncodePromotion((int)BoardSquare.e2, (int)BoardSquare.e1, (int)Piece.p, promotedPiece: (int)Piece.n),
        ],
        [
            MoveExtensions.EncodePromotion((int)BoardSquare.f2, (int)BoardSquare.f1, (int)Piece.p, promotedPiece: (int)Piece.q),
            MoveExtensions.EncodePromotion((int)BoardSquare.f2, (int)BoardSquare.f1, (int)Piece.p, promotedPiece: (int)Piece.r),
            MoveExtensions.EncodePromotion((int)BoardSquare.f2, (int)BoardSquare.f1, (int)Piece.p, promotedPiece: (int)Piece.n),
            MoveExtensions.EncodePromotion((int)BoardSquare.f2, (int)BoardSquare.f1, (int)Piece.p, promotedPiece: (int)Piece.b),
        ],
        [
            MoveExtensions.EncodePromotion((int)BoardSquare.g2, (int)BoardSquare.g1, (int)Piece.p, promotedPiece: (int)Piece.q),
            MoveExtensions.EncodePromotion((int)BoardSquare.g2, (int)BoardSquare.g1, (int)Piece.p, promotedPiece: (int)Piece.r),
            MoveExtensions.EncodePromotion((int)BoardSquare.g2, (int)BoardSquare.g1, (int)Piece.p, promotedPiece: (int)Piece.n),
            MoveExtensions.EncodePromotion((int)BoardSquare.g2, (int)BoardSquare.g1, (int)Piece.p, promotedPiece: (int)Piece.b),
        ],
        [
            MoveExtensions.EncodePromotion((int)BoardSquare.h2, (int)BoardSquare.h1, (int)Piece.p, promotedPiece: (int)Piece.q),
            MoveExtensions.EncodePromotion((int)BoardSquare.h2, (int)BoardSquare.h1, (int)Piece.p, promotedPiece: (int)Piece.r),
            MoveExtensions.EncodePromotion((int)BoardSquare.h2, (int)BoardSquare.h1, (int)Piece.p, promotedPiece: (int)Piece.n),
            MoveExtensions.EncodePromotion((int)BoardSquare.h2, (int)BoardSquare.h1, (int)Piece.p, promotedPiece: (int)Piece.b),
        ]
    ];
}
