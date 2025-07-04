using Diplom.Properties;
using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows;
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
            {"BindingsPath", File.Exists(Settings.Default.BINDINGS_PATH) },
            {"AppDataFolderPath", File.Exists(Settings.Default.APPDATA_FOLDER_PATH) },
            {"ScenariosPath", File.Exists(Settings.Default.SCENARIOS_PATH) },
        };
        public static bool validateAppInstall(APP_PROPERTIES property)
        {
            properties = new Dictionary<string, bool>()
            {
                {"AppInstall", Settings.Default.IS_APP_INSTALL },
                {"AppPathFolderExists", Directory.Exists(Settings.Default.APP_PATH_FOLDER) },
                {"AppPathExists", File.Exists(Settings.Default.APP_PATH)},
                {"AppsDictExists", File.Exists(Settings.Default.APPS_DICT_PATH) },
                {"LogFileExists", File.Exists(Settings.Default.LOG_FILE_BOT) },
                {"UnInstallApp", File.Exists(Settings.Default.UNINSTALL_APP_PATH) },
                {"BindingsPath", File.Exists(Settings.Default.BINDINGS_PATH) },
                {"AppDataFolderPath", File.Exists(Settings.Default.APPDATA_FOLDER_PATH) },
                {"ScenariosPath", File.Exists(Settings.Default.SCENARIOS_PATH) },
            };
            if (!properties["AppInstall"])
                if (properties["AppPathExists"]) properties["AppInstall"] = true;
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
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string shortcutPath = Path.Combine(desktopPath, "VoiceAssistantBot.lnk");

                if (create)
                {
                    if (string.IsNullOrEmpty(Settings.Default.APP_PATH) || !File.Exists(Settings.Default.APP_PATH))
                    {
                        Console.WriteLine($"Ошибка: Путь к приложению ({Settings.Default.APP_PATH}) некорректен или файл не существует.");
                        return;
                    }

                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                    shortcut.TargetPath = Settings.Default.APP_PATH;
                    shortcut.WorkingDirectory = Path.GetDirectoryName(Settings.Default.APP_PATH);
                    shortcut.Description = "Voice Assistant Bot Shortcut";
                    shortcut.IconLocation = Settings.Default.APP_PATH + ", 0";
                    shortcut.Save();
                }
                else
                {
                    if (File.Exists(shortcutPath))
                    {
                        File.Delete(shortcutPath);
                        Console.WriteLine($"Ярлык удалён: {shortcutPath}");
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Ошибка доступа при создании/удалении ярлыка: {ex.Message}. Запустите приложение с правами администратора.");
            }
            catch (COMException ex)
            {
                Console.WriteLine($"Ошибка COM при работе с ярлыком: {ex.Message}. Проверьте, доступен ли Windows Script Host.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неизвестная ошибка при создании/удалении ярлыка: {ex.Message}");
            }
        }
        public static bool IsProcessRunning(string exeFileName)
        {
            string processName = Path.GetFileNameWithoutExtension(exeFileName);

            return Process.GetProcessesByName(processName).Any();
        }

        public static bool HasMultipleWords(string text) => text != null ? Regex.IsMatch(text.Trim(), @"\S+\s+\S+") : false;
        public static bool IsValidRussianText(string text) => text != null ? Regex.IsMatch(text, @"^[а-яА-ЯёЁ]+$") : false;
        public static bool ValidateTitle(string title, string subtitle)
        {

            if (HasMultipleWords(title))
            {
                MessageBox.Show($"{subtitle} должно состоять из 1 слова");
                return false;
            }
            else if (!IsValidRussianText(title))
            {
                MessageBox.Show($"{subtitle} должно содержать только русские буквы, без спец символов и цифр", $"Ошибка в поле {subtitle}");
                return false;
            }
            else if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show($"{subtitle} не может быть пустым.");
                return false;
            }
            return true;
        }
        public static List<Software> getAllApps() => serializer.deserialize<List<Software>>(Settings.Default.APPS_DICT_PATH) ?? new List<Software>();
        public static List<Scenarios_> getAllScenarios() => serializer.deserialize<List<Scenarios_>>(Settings.Default.SCENARIOS_PATH) ?? new List<Scenarios_>();
        public static Dictionary<string, string> getAllBindings() => serializer.deserialize<Dictionary<string, string>>(Settings.Default.BINDINGS_PATH) ?? new Dictionary<string, string>();

        public static void ValidateAppInstallAndMoveToPage(Action action)
        {
            if (validateAppInstall(APP_PROPERTIES.AppInstall))
                action();
            else MessageBoxCustom.Show("Для начала нужно установить приложение");
        }
        public static void ValidateAppInstallAndMoveToPage(Action action, bool rule, string error_message)
        {
            if (rule)
                action();
            else MessageBoxCustom.Show(error_message);
        }
        public static void ValidateAppInstallAndMoveToPage(Action action, List<bool> rules, List<string> error_messages)
        {
            for (int i = 0; i < rules.Count; i++)
                if (!rules[i])
                {
                    MessageBoxCustom.Show(error_messages[i]);
                    return;
                }
            action();
        }
        public static bool ValidateScenariosName(string name, bool update)
        {
            List<Scenarios_> scenarios = getAllScenarios();
            if (update)
                return scenarios.Where(scenaries => scenaries.name != name).Select(scenaries => scenaries.name).ToList().Contains(name);
            return scenarios.Select(scenaries => scenaries.name).ToList().Contains(name);
        }
    }

    enum APP_PROPERTIES
    {
        AppInstall,
        AppPathFolderExists,
        AppPathExists,
        AppsDictExists,
        LogFileExists,
        UnInstallApp,
        BindingsPath,
        AppDataFolderPath,
        ScenariosPath,
        AllProperties = 10
    }
}