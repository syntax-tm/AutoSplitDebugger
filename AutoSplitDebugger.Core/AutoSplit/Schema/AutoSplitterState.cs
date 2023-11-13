using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSplitDebugger.Core.AutoSplit.Schema;

public class AutoSplitterState : AutoSplitterAction
{

    public List<AutoSplitterPointer> Pointers { get; set; }

}
