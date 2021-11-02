using System;

namespace RioValleyChili.Core.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public class IssueAttribute : Attribute
    {
        public string Description;
        public string Todo;
        public string[] References;
        public IssueFlags Flags;

        public IssueAttribute() { }

        public IssueAttribute(string description)
        {
            Description = description;
        }
    }

    public enum IssueFlags
    {
        TodoWhenAccessFreedom
    }
}
