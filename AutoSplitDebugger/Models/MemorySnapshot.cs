using System;
using System.Collections.Generic;
using AutoSplitDebugger.Interfaces;

namespace AutoSplitDebugger.Models;

public class MemorySnapshot
{
    public Dictionary<string, IPointerSnapshot> Pointers { get; set; } = new ();
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    
    public MemorySnapshot(IList<IPointerViewModel> pointers)
    {
        foreach (var pointer in pointers)
        {
            var key = pointer.Name;
            Pointers[key] = pointer.CreateSnapshot();
        }
    }
}
