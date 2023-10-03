using System;
using System.Diagnostics;
using System.Timers;
using AutoSplitDebugger.Config;
using AutoSplitDebugger.Interfaces;
using AutoSplitDebugger.Models;
using DevExpress.Mvvm.POCO;
using JetBrains.Annotations;
using log4net;

namespace AutoSplitDebugger.ViewModels
{
    [DebuggerDisplay("{Name}: {ValueText}")]
    public class PointerViewModel<T> : IPointerViewModel where T : struct, IParsable<T>
    {
        private const string DEFAULT_FORMAT = @"{0:N0}";

        protected readonly ILog log = LogManager.GetLogger(nameof(PointerViewModel<T>));
        
        private readonly Memory _memory;
        private readonly PointerConfig _config;

        protected Timer _changedTimer;

        public string Name { get; set; }
        public int[] PointerPath { get; }
        public string Format { get; set; }
        public virtual bool HasValueSource { get; protected set;  }
        public virtual IValueSource ValueSource { get; set; }

        public bool IsValid => Address != null;
        public virtual nint? Address { get; set; }
        public virtual bool IsChanged { get; set; }
        public virtual bool IsWarn { get; set; }

        public virtual string DisplayText { get; set; }
        public virtual string ValueText { get; set; }
        public virtual T? Value { get; set; }

        protected PointerViewModel(Memory memory)
        {
            _memory = memory;
        }
        
        protected PointerViewModel(Memory memory, PointerConfig config)
        {
            _memory = memory;
            _config = config;

            Name = config.Name;
            PointerPath = config.Offsets;
            Format = config.Format;
            ValueSource = config.ValueSource?.ToValueSource<T>();
        }

        public static PointerViewModel<T> Create(Memory memory)
        {
            return ViewModelSource.Create(() => new PointerViewModel<T>(memory));
        }
        
        public static PointerViewModel<T> Create(Memory memory, PointerConfig config)
        {
            return ViewModelSource.Create(() => new PointerViewModel<T>(memory, config));
        }

        public IPointerSnapshot CreateSnapshot()
        {
            return new PointerSnapshot<T>(Value, DisplayText);
        }

        public void Refresh()
        {
            Address ??= _memory.ResolvePath(PointerPath);

            var value = _memory.ReadMemory<T>(Address.Value);

            Value = value;
        }

        [UsedImplicitly]
        protected void OnValueSourceChanged()
        {
            HasValueSource = ValueSource != null;
        }

        [UsedImplicitly]
        protected void OnValueChanged()
        {
            IsChanged = true;

            if (!Value.HasValue)
            {
                DisplayText = string.Empty;
                ValueText = string.Empty;
            }
            else
            {
                var sourceText = ValueSource?.GetDisplayText(Value);

                DisplayText = sourceText;
                ValueText = string.Format(Format, Value);
            }

            // if a timer was already running, reset it
            if (_changedTimer != null)
            {
                _changedTimer.Stop();
                _changedTimer.Start();
                return;
            }

            // no timer was running, so create a new one
            _changedTimer = new (TimeSpan.FromSeconds(1));
            _changedTimer.Elapsed += ChangedTimerOnElapsed;
            _changedTimer.Start();
        }
        
        private void ChangedTimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            IsChanged = false;

            _changedTimer.Elapsed -= ChangedTimerOnElapsed;
            _changedTimer.Dispose();
            _changedTimer = null;
        }
    }
}
