namespace Lynx;

static partial class EvaluationParams
{
    public static ReadOnlySpan<int> PawnPhalanxBonus =>
    [
        0,          // Pack(   0,    0)
        131073,     // Pack(   1,    2)
        655370,     // Pack(  10,   10)
        1572886,    // Pack(  22,   24)
        5046327,    // Pack(  55,   77)
        12255440,   // Pack( 208,  187)
        26410954,   // Pack( -54,  403)
    ];

    public static ReadOnlySpan<int> BadBishop_SameColorPawnsPenalty =>
    [
        10616971,   // Pack( 139,  162)
        10354825,   // Pack( 137,  158)
        9502853,    // Pack( 133,  145)
        8913023,    // Pack( 127,  136)
        8454265,    // Pack( 121,  129)
        7929973,    // Pack( 117,  121)
        7471218,    // Pack( 114,  114)
        7012461,    // Pack( 109,  107)
        6815852,    // Pack( 108,  104)
    ];

    public static ReadOnlySpan<int> BadBishop_BlockedCentralPawnsPenalty =>
    [
        10092677,   // Pack( 133,  154)
        9109633,    // Pack( 129,  139)
        7602301,    // Pack( 125,  116)
        6094973,    // Pack( 125,   93)
        5439612,    // Pack( 124,   83)
        6619257,    // Pack( 121,  101)
        0,          // Pack(   0,    0)
        0,          // Pack(   0,    0)
        0,          // Pack(   0,    0)
    ];

    public static ReadOnlySpan<int> CheckBonus =>
    [
        0,          // Pack(   0,    0)
        196624,     // Pack(  16,    3)
        983057,     // Pack(  17,   15)
        196638,     // Pack(  30,    3)
        1245196,    // Pack(  12,   19)
    ];

    public static ReadOnlySpan<int> FriendlyKingDistanceToPassedPawnBonus =>
    [
        0,          // Pack(   0,    0)
        2490395,    // Pack(  27,   38)
        2097161,    // Pack(   9,   32)
        1310722,    // Pack(   2,   20)
        917507,     // Pack(   3,   14)
        720908,     // Pack(  12,   11)
        720911,     // Pack(  15,   11)
        196631,     // Pack(  23,    3)
    ];

    public static ReadOnlySpan<int> EnemyKingDistanceToPassedPawnPenalty =>
    [
        0,          // Pack(   0,    0)
        -1376289,   // Pack( -33,  -21)
        393206,     // Pack( -10,    6)
        1179653,    // Pack(   5,   18)
        1769485,    // Pack(  13,   27)
        2359321,    // Pack(  25,   36)
        3080217,    // Pack(  25,   47)
        3342368,    // Pack(  32,   51)
    ];

    public static ReadOnlySpan<int> VirtualKingMobilityBonus =>
    [
        -1441792,   // Pack(   0,  -22)
        -458750,    // Pack(   2,   -7)
        -393216,    // Pack(   0,   -6)
        -655359,    // Pack(   1,  -10)
        -524287,    // Pack(   1,   -8)
        -458750,    // Pack(   2,   -7)
        -327678,    // Pack(   2,   -5)
        -393214,    // Pack(   2,   -6)
        -131066,    // Pack(   6,   -2)
        196610,     // Pack(   2,    3)
        393214,     // Pack(  -2,    6)
        524282,     // Pack(  -6,    8)
        655350,     // Pack( -10,   10)
        786417,     // Pack( -15,   12)
        851954,     // Pack( -14,   13)
        851951,     // Pack( -17,   13)
        720886,     // Pack( -10,   11)
        720887,     // Pack(  -9,   11)
        327691,     // Pack(  11,    5)
        131096,     // Pack(  24,    2)
        131090,     // Pack(  18,    2)
        -262106,    // Pack(  38,   -4)
        -327642,    // Pack(  38,   -5)
        -524244,    // Pack(  44,   -8)
        -851913,    // Pack(  55,  -13)
        -1310620,   // Pack( 100,  -20)
        -2228106,   // Pack( 118,  -34)
        -1703791,   // Pack( 145,  -26)
    ];

    public static ReadOnlySpan<int> KnightMobilityBonus =>
    [
        0,          // Pack(   0,    0)
        3866643,    // Pack(  19,   59)
        6029342,    // Pack(  30,   92)
        7143461,    // Pack(  37,  109)
        8060973,    // Pack(  45,  123)
        9044019,    // Pack(  51,  138)
        9306172,    // Pack(  60,  142)
        9371716,    // Pack(  68,  143)
        8781902,    // Pack(  78,  134)
    ];

    public static ReadOnlySpan<int> BishopMobilityBonus =>
    [
        0,          // Pack(   0,    0)
        2162698,    // Pack(  10,   33)
        3670037,    // Pack(  21,   56)
        4849692,    // Pack(  28,   74)
        5570596,    // Pack(  36,   85)
        6225961,    // Pack(  41,   95)
        6553646,    // Pack(  46,  100)
        6619186,    // Pack(  50,  101)
        6881332,    // Pack(  52,  105)
        6684732,    // Pack(  60,  102)
        6553665,    // Pack(  65,  100)
        6422602,    // Pack(  74,   98)
        6881359,    // Pack(  79,  105)
        5832775,    // Pack(  71,   89)
        0,          // Pack(   0,    0)
    ];

    public static ReadOnlySpan<int> RookMobilityBonus =>
    [
        0,          // Pack(   0,    0)
        1441804,    // Pack(  12,   22)
        2031631,    // Pack(  15,   31)
        2359316,    // Pack(  20,   36)
        2949140,    // Pack(  20,   45)
        3276826,    // Pack(  26,   50)
        3801117,    // Pack(  29,   58)
        3932192,    // Pack(  32,   60)
        4325411,    // Pack(  35,   66)
        4587557,    // Pack(  37,   70)
        4849703,    // Pack(  39,   74)
        5046312,    // Pack(  40,   77)
        5177391,    // Pack(  47,   79)
        4718647,    // Pack(  55,   72)
        4128843,    // Pack(  75,   63)
    ];

    public static ReadOnlySpan<int> QueenMobilityBonus =>
    [
        0,          // Pack(   0,    0)
        13107199,   // Pack(  -1,  200)
        20316155,   // Pack(  -5,  310)
        22609918,   // Pack(  -2,  345)
        23855104,   // Pack(   0,  364)
        24838148,   // Pack(   4,  379)
        26083334,   // Pack(   6,  398)
        26935303,   // Pack(   7,  411)
        27459595,   // Pack(  11,  419)
        27787277,   // Pack(  13,  424)
        28246031,   // Pack(  15,  431)
        28508178,   // Pack(  18,  435)
        28573717,   // Pack(  21,  436)
        28966934,   // Pack(  22,  442)
        29098007,   // Pack(  23,  444)
        29294617,   // Pack(  25,  447)
        29687834,   // Pack(  26,  453)
        29098018,   // Pack(  34,  444)
        28901417,   // Pack(  41,  441)
        27983930,   // Pack(  58,  427)
        27590718,   // Pack(  62,  421)
        25559151,   // Pack( 111,  390)
        25034871,   // Pack( 119,  382)
        23724166,   // Pack( 134,  362)
        21299418,   // Pack( 218,  325)
        19398886,   // Pack( 230,  296)
        25690179,   // Pack(  67,  392)
        27459541,   // Pack( -43,  419)
    ];

}
