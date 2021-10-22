using System;

namespace AutomationControls.Attributes
{

    /// <summary>
    /// First argument is a user control to bind data to
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class DataProfileAttribute : Attribute
    {

        // This is a positional argument
        public DataProfileAttribute(Type ClassName)
        {
            _ClassName = ClassName;
        }

        private Type _ClassName;
        public Type ClassName
        {
            get { return _ClassName; }
        }


    }


}


[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
public class DataProfileMemberAttribute : Attribute
{

    // This is a positional argument
    public DataProfileMemberAttribute(Type ClassName)
    {
        _ClassName = ClassName;
    }

    private Type _ClassName;
    public Type ClassName
    {
        get { return _ClassName; }
    }


}