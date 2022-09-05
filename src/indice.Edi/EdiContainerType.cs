namespace indice.Edi
{
    /// <summary>
    /// Enum that specifies a hierarchy. These are the types of containers that can hold values. 
    /// </summary>
    internal enum EdiContainerType
    {
        /// <summary>
        /// Unspecified container
        /// </summary>

        /* Unmerged change from project 'indice.Edi (netstandard1.3)'
        Before:
                None = 0,

                /// <summary>
        After:
                None = 0,

                /// <summary>
        */

        /* Unmerged change from project 'indice.Edi (netstandard1.0)'
        Before:
                None = 0,

                /// <summary>
        After:
                None = 0,

                /// <summary>
        */

        /* Unmerged change from project 'indice.Edi (netstandard2.0)'
        Before:
                None = 0,

                /// <summary>
        After:
                None = 0,

                /// <summary>
        */

        /* Unmerged change from project 'indice.Edi (net5.0)'
        Before:
                None = 0,

                /// <summary>
        After:
                None = 0,

                /// <summary>
        */
        None = 0,

        /// <summary>
        /// <see cref="Segment"/> container.
        /// </summary>
        Segment = 1,

        /// <summary>
        /// <see cref="Element"/> container.
        /// </summary>
        Element = 2,

        /// <summary>
        /// <see cref="Component"/> container.
        /// </summary>
        Component = 3
    }
}
