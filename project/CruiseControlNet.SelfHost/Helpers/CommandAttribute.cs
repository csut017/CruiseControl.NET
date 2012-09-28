namespace CruiseControlNet.SelfHost.Helpers
{
    using System;

    /// <summary>
    /// Marks a method as a command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute
        : Attribute
    {
    }
}