namespace Lynx.Generator;

internal static class Utils
{
    /// <summary>
    /// https://minuskelvin.net/chesswiki/content/packed-eval.html
    /// </summary>
    public static int Pack(short mg, short eg)
    {
        return (eg << 16) + mg;
    }
}
