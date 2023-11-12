using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
        protected readonly ILog log = LogManager.GetLogger(nameof(PointerViewModel<T>));

        private bool _isInit;
        private readonly Memory _memory;
        // ReSharper disable once NotAccessedField.Local
        private readonly PointerConfig _config;

        protected Timer _changedTimer;

        public string Name { get; set; }
        public int[] PointerPath { get; set; }
        public string Format { get; set; }
        public virtual bool HasValueSource { get; protected set;  }
        public virtual IValueSource ValueSource { get; set; }

        public bool IsValid => Address != null;
        public virtual nint? Address { get; set; }
        public virtual bool IsChanged { get; set; }
        public virtual bool IsWarn { get; set; }

        public virtual object DisplayValue { get; set; }
        public virtual string ValueText { get; set; }
        public virtual T? Value { get; set; }

        protected PointerViewModel(Memory memory)
        {
            _memory = memory;
        }
        
        protected PointerViewModel(Memory memory, PointerConfig config) : this(memory)
        {
            _config = config;

            Name = config.Name;
            Format = config.Format;

            if (config.IsTime)
            {
                var uot = config.UnitOfTime ?? throw new ArgumentNullException(nameof(config.UnitOfTime));

                ValueSource = new TimerValueSource<float>(uot);
            }
            else
            {
                ValueSource = config.ValueSourceConfig?.ToValueSource<T>();
            }
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
            return new PointerSnapshot<T>(Value, DisplayValue.ToString());
        }

        public void Refresh()
        {
            Address ??= _memory.ResolvePath(PointerPath);

            // read the value from memory
            var value = _memory.ReadMemory<T>(Address.Value);

            Value = value;
        }

        public bool Init()
        {
            if (_isInit) return true;

            var offsets = _config.Offsets.FirstOrDefault(o => o.Version.Equals(_memory.ModuleVersionInfo));

            if (offsets == null) return false;

            // the offsets for the current game version
            PointerPath = offsets.Offsets;

            // resolve the pointer using the version-specific offsets
            Address = _memory.ResolvePath(PointerPath);

            return true;
        }

        public void Clear()
        {
            Address = null;
            PointerPath = null;

            _isInit = false;
        }

        [UsedImplicitly]
        protected void OnValueSourceChanged()
        {
            HasValueSource = ValueSource != null;
        }

        [UsedImplicitly]
        protected async void OnValueChanged()
        {
            IsChanged = true;

            var refreshTask = Task.Run(RefreshDisplay);
            var resetChangeTask = Task.Run(ResetChangeTimer);

            await Task.WhenAll(refreshTask, resetChangeTask).ConfigureAwait(false);
        }

        private void RefreshDisplay()
        {
            if (!Value.HasValue)
            {
                // TODO: this might need to change, consider adding an optional null display value
                DisplayValue = string.Empty;
                ValueText = string.Empty;
            }
            else
            {
                var sourceText = ValueSource?.GetDisplay(Value);

                DisplayValue = sourceText;
                ValueText = string.Format(Format, Value);
            }
        }

        private void ResetChangeTimer()
        {
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
            // clear IsChanged so the UI can display the default state
            IsChanged = false;

            // unhook the Elapsed event and manually dispose of the timer we used
            _changedTimer.Elapsed -= ChangedTimerOnElapsed;
            _changedTimer.Dispose();
            _changedTimer = null;
        }
    }
}
