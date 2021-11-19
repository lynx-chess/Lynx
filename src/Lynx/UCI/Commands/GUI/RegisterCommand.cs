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
public sealed class RegisterCommand : GUIBaseCommand
{
    public const string Id = "register";

    public bool Later { get; }

    public string Name { get; } = string.Empty;

    public string Code { get; } = string.Empty;

    public RegisterCommand(string command)
    {
        var items = command.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);

        if (string.Equals("later", items[1], System.StringComparison.OrdinalIgnoreCase))
        {
            Later = true;
            return;
        }

        var sb = new StringBuilder();

        foreach (var item in items[1..])
        {
            if (string.Equals("name", item, System.StringComparison.OrdinalIgnoreCase))
            {
                Code = sb.ToString().TrimEnd();
                sb.Clear();
            }
            else if (string.Equals("code", item, System.StringComparison.OrdinalIgnoreCase))
            {
                Name = sb.ToString().TrimEnd();
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
            Name = sb.ToString().TrimEnd();
        }
        else
        {
            Code = sb.ToString().TrimEnd();
        }
    }
}
