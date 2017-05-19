using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Abp.Localization;

namespace Abp.Web.Mvc.Extensions
{
    public static class EnumExtensions
    {
        public static List<SelectListItem> GetSelectList(this Type type, bool createDefaultSelectedItem = false)
        {
            var list = new List<SelectListItem>();

            if (!createDefaultSelectedItem)
            {
                list.Add(new SelectListItem
                {
                    Text = LocalizationHelper.GetString(AbpExWebMvcConsts.LocalizationSourceName, "PleaseSelectOne"),
                    Value = ""
                });
            }

            foreach (var item in Enum.GetValues(type))
            {
                var description = Abp.Extensions.EnumExtensions.GetDescription(item);
                int? value = (int)item;
                if (createDefaultSelectedItem && description == "None" && value == 0)
                {
                    description = LocalizationHelper.GetString(AbpExWebMvcConsts.LocalizationSourceName, "PleaseSelectOne");
                    value = null;
                }

                list.Add(new SelectListItem
                {
                    Text = description,
                    Value = value?.ToString() ?? ""
                });
            }

            return list;
        }
    }
}
