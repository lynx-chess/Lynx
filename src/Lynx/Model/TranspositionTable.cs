using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Lynx.Model;

public enum ElementType
{
    Exact,
    Alpha,
    Beta
}
public struct TranspositionTableElement
{
    public long Key { get; set; }

    public ElementType Type { get; set; }

    public int Score { get; set; }

    public Move Move { get; set; }

    public int Depth { get; set; }
}

public class TranspositionTable : HashSet<TranspositionTableElement>
{
    public TranspositionTable(int capacity) : base(capacity)
    {
    }
}
