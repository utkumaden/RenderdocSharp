
using System;

namespace RenderdocSharp
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class RenderdocApiVersionAttribute : Attribute
    {
        public Version MinVersion { get; }

        public RenderdocApiVersionAttribute(int major, int minor, int patch = 0)
        {
            MinVersion = new Version(major, minor, patch);
        }
    }
}