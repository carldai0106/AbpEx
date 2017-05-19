using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Abp.Application.Services.Dto;

namespace Abp.Utils
{
    public class Utilities
    {
        public static string LocalizedEnumItem<T>(object itemOfEnum) where T : struct
        {
            if (itemOfEnum == null)
                return string.Empty;

            var name = itemOfEnum.ToString();
            return LocalizedEnum(typeof(T), name);
        }
      
        public static string LocalizedEnumItem<T>(int? value) where T : struct
        {
            if (!value.HasValue)
                return string.Empty;

            var @enum = (T)Enum.ToObject(typeof(T), value);
            var name = @enum.ToString();
            return LocalizedEnum(typeof(T), name);
        }

        private static string LocalizedEnum(Type source, string name)
        {
            var fields = source.GetFields(BindingFlags.Public | BindingFlags.Static);
            var field = fields.FirstOrDefault(x => x.Name == name);
            if (field == null)
                return name;

            if (field.IsDefined(typeof(DescriptionAttribute), true))
            {
                var description = field.GetCustomAttribute<DescriptionAttribute>(false);
                return description.Description;
            }

            return name;
        }

        public static IList<CountryDto> GetCountries()
        {
            var list = new List<CountryDto>();

            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (var region in cultures.Select(culture => 
                new RegionInfo(culture.LCID)).Where(
                region => list.Count(x => x.EnglishName == region.EnglishName) == 0))
            {
                list.Add(new CountryDto
                {
                    Name = region.Name,
                    EnglishName = region.EnglishName,
                    DisplayName = region.DisplayName
                });
            }

            return list;
        }

        public static string GetCfgByKey(string key)
        {
            return ConfigurationManager.AppSettings[key] ?? "";
        }

        public static string GetMigrationScriptsPath()
        {
            return AppDomain.CurrentDomain.GetData("DataDirectory") + GetCfgByKey("MigrationScripts");
        }

        public static void SaveBase64StringToImage(string base64String, string fullPath)
        {
            var base64Data = Regex.Match(base64String, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            using (var fs = new FileStream(fullPath, FileMode.Create))
            {
                using (var bw = new BinaryWriter(fs))
                {
                    var data = Convert.FromBase64String(base64Data);
                    bw.Write(data);
                    bw.Close();
                }
            }
        }
    }
}
