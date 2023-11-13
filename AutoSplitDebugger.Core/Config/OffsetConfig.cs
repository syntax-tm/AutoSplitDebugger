using System;

namespace AutoSplitDebugger.Core.Config
{
    public class OffsetConfig
    {
        private readonly string[] _rawOffsets;

        /// <summary>
        /// Version number the offsets apply to.
        /// </summary>
        public VersionConfig Version { get; set; }
        /// <summary>
        /// The offsets used to initialize the pointer.
        /// </summary>
        /// <remarks>Supports normal and multi-level pointers.</remarks>
        public int[] Offsets { get; set; }
        /// <summary>
        /// The pointer with this offset.
        /// </summary>
        public PointerConfig PointerConfig { get; set; }

        public OffsetConfig(string[] offsets)
        {
            _rawOffsets = offsets;
            
            LoadOffsets();
        }

        private void LoadOffsets()
        {
            try
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
            catch (Exception e)
            {
                var message = $"An error occurred loading the offsets for {nameof(PointerConfig)} '{PointerConfig?.Name}'. {e.Message}";

                throw new (message, e);
            }
        }
    }
}
