﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace BootstrapTable.Support
{
    internal static class Extensions
    {
        public static string ToStringLower(this object value)
        {
            return value != null ? value.ToString().ToLower() : null;
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var t in enumerable)
            {
                action(t);
            }
        }

        public static string SplitCamelCase(this string str)
        {
            return Regex.Replace(Regex.Replace(str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2"), @"(\p{Ll})(\P{Ll})", "$1 $2");
        }

        public static IDictionary<string, object> HtmlAttributesToDictionary(this object htmlAttributes)
        {
            return htmlAttributes as IDictionary<string, object> ?? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
        }

        public static Boolean IsAnonymousType(this Type type)
        {
            bool hasCompilerGeneratedAttribute = type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Count() > 0;
            bool nameContainsAnonymousType = type.FullName.Contains("AnonymousType");
            return hasCompilerGeneratedAttribute && nameContainsAnonymousType;
        }

        public static IHtmlString ToSerializedString(this object data)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return new HtmlString((serializer.Serialize(data)));
        }

        public static string GetName<TModel, TProperty>(this Expression<Func<TModel, TProperty>> field)
        {
            var member = field.Body as MemberExpression;
            return member.Member.Name;
        }

        public static string GetDisplayName<TModel, TProperty>(this Expression<Func<TModel, TProperty>> field)
        {
            var member = field.Body as MemberExpression;
            DisplayAttribute display = member.Member.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
            if (display != null && !string.IsNullOrEmpty(display.GetName()))
            {
                return display.GetName();
            }
            else
            {
                return member.Member.Name.SplitCamelCase();
            }
        }

        public static IEnumerable<PropertyInfo> GetSortedProperties(this Type type)
        {
            int order = int.MaxValue - type.GetProperties().Count();
            return type.GetProperties()
                .Select(p => new
                {
                    property = p,
                    order = p.GetCustomAttributes(typeof(DisplayAttribute), true).FirstOrDefault() != null ?
                        (((DisplayAttribute)p.GetCustomAttributes(typeof(DisplayAttribute), true).First()).GetOrder() ?? ++order) : ++order
                })
                .OrderBy(o => o.order)
                .Select(o => o.property);
        }
    }
}
