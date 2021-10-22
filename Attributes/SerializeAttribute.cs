using System;

namespace AutomationControls.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SerializeAttribute : Attribute
    {

        public SerializeAttribute()
        {

        }


    }
}
