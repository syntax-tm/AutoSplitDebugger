using System;

namespace AutoSplitDebugger.Config;

public class PointerConfig
{
    private readonly string[] _rawOffsets;

    public string Name { get; set; }
    public string Type { get; set; }
    public string Format { get; set; }
    public ValueSourceConfig ValueSource { get; set; }
    public int[] Offsets { get; set; }

    public PointerConfig(string[] offsets)
    {
        _rawOffsets = offsets;
            
        LoadOffsets();
    }

    private void LoadOffsets()
    {
        Offsets = new int[_rawOffsets.Length];
        for (var i = 0; i < _rawOffsets.Length; i++)
        {
            var rawOffset = _rawOffsets[i];
            var isHex = rawOffset.StartsWith("0x");
            if (isHex)
            {
                Offsets[i] = Convert.ToInt32(rawOffset, 16);
            }
            else
            {
                Offsets[i] = int.Parse(rawOffset);
            }
        }
    }
}