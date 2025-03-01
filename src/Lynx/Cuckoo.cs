namespace Lynx;

public static class Cuckoo
{
    public static ulong Hash1(ulong key) => ((key >> 32) & 0x1fff);

    public static ulong Hash2(ulong key) => ((key >> 48) & 0x1fff);
}
