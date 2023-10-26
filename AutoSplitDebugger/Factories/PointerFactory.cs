using System;
using System.Collections.Generic;
using AutoSplitDebugger.Config;
using AutoSplitDebugger.Interfaces;
using AutoSplitDebugger.ViewModels;

namespace AutoSplitDebugger.Factories
{
    public static class PointerFactory
    {
        public static IList<IPointerViewModel> CreatePointerViewModels(Memory memory, AutoSplitConfig config)
        {
            var vms = new List<IPointerViewModel>();

            foreach (var pointer in config.Pointers)
            {
                var type = pointer.Type;
                IPointerViewModel pointerVm = type switch
                {
                    PointerDataType._sbyte    => PointerViewModel<sbyte>.Create(memory, pointer),
                    PointerDataType._byte     => PointerViewModel<byte>.Create(memory, pointer),
                    PointerDataType._short    => PointerViewModel<short>.Create(memory, pointer),
                    PointerDataType._ushort   => PointerViewModel<ushort>.Create(memory, pointer),
                    PointerDataType._int      => PointerViewModel<int>.Create(memory, pointer),
                    PointerDataType._uint     => PointerViewModel<uint>.Create(memory, pointer),
                    PointerDataType._long     => PointerViewModel<long>.Create(memory, pointer),
                    PointerDataType._ulong    => PointerViewModel<ulong>.Create(memory, pointer),
                    PointerDataType._float    => PointerViewModel<float>.Create(memory, pointer),
                    PointerDataType._double   => PointerViewModel<double>.Create(memory, pointer),
                    PointerDataType._decimal  => PointerViewModel<decimal>.Create(memory, pointer),
                    _                         => throw new ArgumentException($"Unknown type '{type}'.")
                };

                vms.Add(pointerVm);
            }

            return vms;
        }

    }
}
