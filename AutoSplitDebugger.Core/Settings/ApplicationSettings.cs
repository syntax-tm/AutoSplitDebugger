namespace AutoSplitDebugger.Core.Settings;

public class ApplicationSettings
{
    private const decimal DEFAULT_CHANGE_DURATION = 1;
    private const string DEFAULT_SETTINGS_FILE_NAME = @"appSettings.json";

    private static readonly object syncLock = new ();
    private static ApplicationSettings _instance;

    public bool ShowChangeHighlight { get; set; } = true;
    public decimal ChangeHighlightDuration { get; set; } = DEFAULT_CHANGE_DURATION;

    public static ApplicationSettings Current
    {
        get
        {
            if (_instance != null) return _instance;

            lock (syncLock)
            {
                _instance = new ();


            }

            return _instance;
        }
    }

}
