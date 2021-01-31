using System;
using XPInc.Hackathon.Infra.Zabbix.Enums;

namespace XPInc.Hackathon.Infra.Zabbix.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    internal class MethodAttribute : Attribute
    {
        public Types MethodType { get; set; }
        public MethodAttribute(Types methodtype)
        {
            MethodType = methodtype;
        }

    }
}