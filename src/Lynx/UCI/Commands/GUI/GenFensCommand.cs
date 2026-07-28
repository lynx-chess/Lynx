namespace Lynx.UCI.Commands.GUI;

public record class GenFensCommand
{
    public const string NoBook = "None";

    public uint Count { get; init; }

    public int Seed { get; init; }

    public string Book { get; init; } = string.Empty;

    public string Extra { get; init; } = string.Empty;

    public GenFensCommand(string rawcommand)
    {
        var items = rawcommand.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (items.Length >= 2 && uint.TryParse(items[1], out var count))
        {
            Count = count;
        }

        if (items.Length >= 4 && int.TryParse(items[3], out var seed))
        {
            Seed = seed;
        }

        if (items.Length >= 6)
        {
            Book = items[5];
        }

        if (items.Length >= 7)
        {
            Extra = string.Join(' ', items[6..]);
        }
    }
}
