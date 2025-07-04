using System.Collections.Generic;

namespace Diplom.Models
{
    public class Software
    {
        public string title {  get; set; }
        public List<string> names { get; set; }
        public string executable { get; set; }
        
        public software_type software_type { get; set; }

        public Software(string title, List<string> names, string executable, software_type software_type)
        {
            this.title = title;
            this.names = names;
            this.executable = executable;
            this.software_type = software_type;
        }

        public static Dictionary<string, string> getSoftwareTypes() => new Dictionary<string, string>
        {
            {"Приложение", "APP" },
            {"Ссылка на сайт", "WEBSITE" },
            {"Папка", "FOLDER" },
            {"Самописный скрипт", "SCRIPT" },
            {"Сочетание клавиш", "HOTKEY" },
            {"Файл", "FILE" }
        };
    }
    public class software_type {
        public string name;

        public software_type(string name)
        {
            this.name = name;
        }
    }
}
