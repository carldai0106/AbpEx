﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Abp.Application.Services.Dto;

namespace Abp.Extensions
{
    public static class EnumExtensions
    {
        public static List<ComboboxItemDto> GetSelectList(this Type type)
        {
            var list = new List<ComboboxItemDto>();

            foreach (var item in Enum.GetValues(type))
            {
                list.Add(new ComboboxItemDto
                {
                    DisplayText = GetDescription(item),
                    Value = ((int)item).ToString()
                });
            }

            return list;
        }

        public static List<EnumDto> GetEnumList(this Type type)
        {
            var list = new List<EnumDto>();
            foreach (var item in Enum.GetValues(type))
            {
                list.Add(new EnumDto
                {
                    Value = (int)item,
                    Name = item.ToString(),
                    Description = GetDescription(item)
                });
            }
            return list;
        }

        public static string GetDescription(object e)
        {
            //获取枚举的Type类型对象
            var type = e.GetType();
            //获取枚举的所有字段
            var fields = type.GetFields();

            //遍历所有枚举的所有字段
            foreach (var field in fields)
            {
                if (field.Name != e.ToString())
                {
                    continue;
                }
                //第二个参数true表示查找EnumDisplayNameAttribute的继承链

                if (field.IsDefined(typeof(DescriptionAttribute), true))
                {
                    var attr = field.GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }

            //如果没有找到自定义属性，直接返回属性项的名称
            return e.ToString();
        }
    }
}
