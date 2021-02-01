using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Reflection;
using Newtonsoft.Json;
using RestSharp;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Messages;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Messages.Base;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Request;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Response;

namespace XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Extensions
{
    public static class Extensions
    {
        /// <summary>
        ///     A generic extension method that aids in reflecting 
        ///     and retrieving any attribute that is applied to an `Enum`.
        /// </summary>
        public static TAttribute GetAttributeEnum<TAttribute>(this Enum enumValue)
                where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }

        /// <summary>
        ///     A generic extension method that aids in reflecting 
        ///     and retrieving any attribute that is applied to an `Class`.
        /// </summary>
        public static TValue GetAttributeValue<TAttribute, TValue>(
        this Type type,
        Func<TAttribute, TValue> valueSelector)
        where TAttribute : Attribute
        {
            var att = type.GetCustomAttributes(
                typeof(TAttribute), true
            ).FirstOrDefault() as TAttribute;
            if (att != null)
            {
                return valueSelector(att);
            }
            return default(TValue);
        }


        public static string GetDisplayName(this Enum enu)
        {
            var attr = GetDisplayAttribute(enu);
            return attr != null ? attr.Name : enu.ToString();
        }
        private static DisplayAttribute GetDisplayAttribute(object value)
        {
            Type type = value.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException(string.Format("Type {0} is not an enum", type));
            }

            // Get the enum field.
            var field = type.GetField(value.ToString());
            return field == null ? null : field.GetCustomAttribute<DisplayAttribute>();
        }



    }

    public static class JsonMixins
    {
        public static string AsJson(this object This)
        {
            return JsonConvert.SerializeObject(This);
        }

        public static T JsonAs<T>(this string This)
        {
            return JsonConvert.DeserializeObject<T>(This);
        }

        public static object JsonAs(this string This, Type type)
        {
            return JsonConvert.DeserializeObject(This, type);
        }

        public static bool TryParseJson<T>(this string This, out T value)
        {
            try
            {
                value = This.JsonAs<T>();
                return true;
            }
            catch (Exception)
            {
                value = default;
            }

            return false;
        }
    }

    public static class AuthenticationServiceExtension
    {
        public static void Authentication<T>(this IHttpMessage<T> request) where T : class
        {
            var client = new RestClient();
            var response = client.Execute(new LoginZabbixRequest(new LoginMessage("Admin", "zabbix")), Method.POST);
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                request.Auth = response.Content.JsonAs<LoginResponse>().Auth.ToString().Replace("-", "");
            }
        }
    }

}
