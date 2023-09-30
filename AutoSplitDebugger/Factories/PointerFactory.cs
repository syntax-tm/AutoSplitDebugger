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
                    "sbyte"   => PointerViewModel<sbyte>.Create(memory, pointer),
                    "byte"    => PointerViewModel<byte>.Create(memory, pointer),
                    "short"   => PointerViewModel<short>.Create(memory, pointer),
                    "ushort"  => PointerViewModel<ushort>.Create(memory, pointer),
                    "int"     => PointerViewModel<int>.Create(memory, pointer),
                    "uint"    => PointerViewModel<uint>.Create(memory, pointer),
                    "long"    => PointerViewModel<long>.Create(memory, pointer),
                    "ulong"   => PointerViewModel<ulong>.Create(memory, pointer),
                    "float"   => PointerViewModel<float>.Create(memory, pointer),
                    "double"  => PointerViewModel<double>.Create(memory, pointer),
                    "decimal" => PointerViewModel<decimal>.Create(memory, pointer),
                    _         => throw new ArgumentException($"Unknown type '{type}'.")
                };

                vms.Add(pointerVm);
            }

            return vms;
        }

    }
}
