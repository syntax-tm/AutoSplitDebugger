using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoSplitDebugger.Interfaces;

public interface IDisplayObject
{
    DisplayType DisplayType { get; }
}
