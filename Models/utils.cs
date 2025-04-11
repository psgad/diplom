using Diplom.Properties;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using File = System.IO.File;

namespace Diplom.Models
{
    internal class utils
    {
        private static Dictionary<string, bool> properties = new Dictionary<string, bool>()
        {
            {"AppInstall", Settings.Default.IS_APP_INSTALL },
            {"AppPathFolderExists", Directory.Exists(Settings.Default.APP_PATH_FOLDER) },
            {"AppPathExists", File.Exists(Settings.Default.APP_PATH)},
            {"AppsDictExists", File.Exists(Settings.Default.APPS_DICT_PATH) },
            {"LogFileExists", File.Exists(Settings.Default.LOG_FILE_BOT) },
            {"UnInstallApp", File.Exists(Settings.Default.UNINSTALL_APP_PATH) },
        };
        public static bool validateAppInstall(APP_PROPERTIES property)
        {
            if (properties["AppPathExists"])
            {
                if (!properties["LogFileExists"]) File.WriteAllText(Settings.Default.LOG_FILE_BOT, "");
                if (!properties["AppsDictExists"]) File.WriteAllText(Settings.Default.APPS_DICT_PATH, "[]");
                properties = new Dictionary<string, bool>()
                {
                    {"AppInstall", Settings.Default.IS_APP_INSTALL },
                    {"AppPathFolderExists", Directory.Exists(Settings.Default.APP_PATH_FOLDER) },
                    {"AppPathExists", File.Exists(Settings.Default.APP_PATH)},
                    {"AppsDictExists", File.Exists(Settings.Default.APPS_DICT_PATH) },
                    {"LogFileExists", File.Exists(Settings.Default.LOG_FILE_BOT) },
                    {"UnInstallApp", File.Exists(Settings.Default.UNINSTALL_APP_PATH) },
                };
            }
            if (property == APP_PROPERTIES.AllProperties)
            {
                foreach (var _property in properties.Values)
                    if (!_property)
                        return false;
                return true;
            }
            else return properties[property.ToString()];
        }
        public static void AddToStartup(bool create = true)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                if (create)
                    key.SetValue("VoiceAssistantBot", $"\"{Settings.Default.APP_PATH}\"");
                else
                    key.DeleteValue("VoiceAssistantBot", false);
            }
            catch { }
        }
        public static void CreateOrDeleteDesktopShortcut(bool create = true)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string shortcutPath = Path.Combine(desktopPath, $"VoiceAssistantBot.lnk");
            if (create)
            {
                if (File.Exists(shortcutPath))
                {
                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                    shortcut.TargetPath = Settings.Default.APP_PATH;
                    shortcut.Save();
                }
            }
            else
            {
                if (File.Exists(shortcutPath))
                    File.Delete(shortcutPath);
            }
        }
        public static bool IsProcessRunning(string exeFullPath)
        {
            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    if (string.Equals(process.MainModule.FileName, exeFullPath, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                catch
                {
                    continue;
                }
            }
            return false;
        }
    }

    enum APP_PROPERTIES
    {
        AppInstall = 0,
        AppPathFolderExists = 1,
        AppPathExists = 2,
        AppsDictExists = 3,
        LogFileExists = 4,
        UnInstallApp = 5,
        AllProperties = 10
    }
}
