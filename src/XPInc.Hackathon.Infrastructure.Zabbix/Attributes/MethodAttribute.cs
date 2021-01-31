using System;
using XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Enums;

namespace XPInc.Hackathon.XPInc.Hackathon.Infrastructure.Zabbix.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    internal class MethodAttribute : Attribute
    {
        public TypesEnum MethodType { get; set; }
        public MethodAttribute(TypesEnum methodtype)
        {
            MethodType = methodtype;
        }

    }
}
