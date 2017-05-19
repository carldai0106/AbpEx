using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Abp.EntityFramework
{
    public class DbConfiguration
    {
        public const string DbInitCfg = "DbInitCfg.json";

        public static string GetAppDataPath()
        {
            var domain = AppDomain.CurrentDomain;
            var dicrectory = domain.GetData("DataDirectory");
            return dicrectory?.ToString() ?? domain.BaseDirectory;
        }

        private static string GetFullPathForDbInitCfg()
        {
            var path = GetAppDataPath();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fullPath = path.EndsWith("\\") ? path + DbInitCfg : path + "\\" + DbInitCfg;

            return fullPath;
        }

        public static Dictionary<string, string> GetDbInitCfg()
        {
            var fullPath = GetFullPathForDbInitCfg();

            if (!File.Exists(fullPath))
                return null;

            var jsonText = File.ReadAllText(fullPath);

            return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);
        }

        public static void SaveDbInitCfg(Dictionary<string, string> cfg)
        {
            var fullPath = GetFullPathForDbInitCfg();
            var oldCfg = GetDbInitCfg();

            Dictionary<string, string> newCfg;
            if (oldCfg == null)
                newCfg = cfg;
            else
            {
                var list = cfg.Concat(oldCfg.Where(x => !cfg.Keys.Contains(x.Key)));
                newCfg = list.ToDictionary(item => item.Key, item => item.Value);
            }

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            using (var stream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write))
            using (var writer = new StreamWriter(stream))
            {
                var jsonText = JsonConvert.SerializeObject(newCfg, Formatting.Indented);
                writer.Write(jsonText);
            }
        }
    }
}
