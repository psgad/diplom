using Newtonsoft.Json;
using System.IO;

namespace Diplom.Models
{
    internal class serializer
    {
        public static void serialize<T>(T obj, string path) => File.WriteAllText(path, JsonConvert.SerializeObject(obj));

        public static T deserialize<T>(string path)
        {
            if (!File.Exists(path))
                File.WriteAllText(path, "[]");
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
    }
}
