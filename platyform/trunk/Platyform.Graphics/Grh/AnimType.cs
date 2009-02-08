﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Platyform.Extensions;

namespace Platyform.Graphics
{
    /// <summary>
    /// Defines how the grh animates
    /// </summary>
    public enum AnimType
    {
        /// <summary>
        /// Grh that will not animate
        /// </summary>
        None,
        /// <summary>
        /// Grh will loop once then change to None back on the first frame
        /// </summary>
        LoopOnce,
        /// <summary>
        /// Grh will loop forever
        /// </summary>
        Loop
    }
}