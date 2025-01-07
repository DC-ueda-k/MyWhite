using System;
using System.Configuration;

namespace MyWhite
{
    public class SettingsManager
    {
        public string ScreenshotSavePath { get; set; }

        public SettingsManager()
        {
            ScreenshotSavePath = string.Empty;
        }

        public void LoadSettings()
        {
            ScreenshotSavePath = Properties.Settings.Default.ScreenshotSavePath;
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.ScreenshotSavePath = ScreenshotSavePath;
            Properties.Settings.Default.Save();
        }
    }
}
