﻿using System;

namespace indice.Edi.Serialization
{
    /// <summary>
    /// <see cref="EdiMessageAttribute"/> marks a propery/class to be deserialized for any message found.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class EdiMessageAttribute : EdiStructureAttribute
    {


    }
}
