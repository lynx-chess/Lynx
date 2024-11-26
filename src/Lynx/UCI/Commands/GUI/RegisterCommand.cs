using System.Text;

namespace Lynx.UCI.Commands.GUI;

/// <summary>
/// register
///	this is the command to try to register an engine or to tell the engine that registration
///	will be done later. This command should always be sent if the engine has send "registration error"
///	at program startup.
///	The following tokens are allowed:
///	* later
///	   the user doesn't want to register the engine now
///	* name
///	   the engine should be registered with the name
///	* code
///	   the engine should be registered with the code
///	Example:
///	   "register later"
///	   "register name Stefan MK code 4359874324"
/// </summary>
public sealed class RegisterCommand : IGUIBaseCommand
{
    public const string Id = "register";

    public bool Later { get; }

    public string Name { get; } = string.Empty;

    public string Code { get; } = string.Empty;

    public RegisterCommand(ReadOnlySpan<char> command)
    {
        const string later = "later";
        const string name = "name";
        const string code = "code";

        Span<Range> items = stackalloc Range[6];
        var itemsLength = command.Split(items, ' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (command[items[1]].Equals(later, StringComparison.OrdinalIgnoreCase))
        {
            Later = true;
            return;
        }

        var sb = new StringBuilder();

        for (int i = 1; i < itemsLength; ++i)
        {
            var item = command[items[i]];
            if (item.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                Code = sb.ToString();
                sb.Clear();
            }
            else if (item.Equals(code, StringComparison.OrdinalIgnoreCase))
            {
                Name = sb.ToString();
                sb.Clear();
            }
            else
            {
                sb.Append(item);
                sb.Append(' ');
            }
        }

        if (string.IsNullOrEmpty(Name))
        {
            Name = sb.ToString();
        }
        else
        {
            Code = sb.ToString();
        }
    }
}
