﻿using System;
using System.Bits;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Platyform.Extensions;

namespace Platyform.Network
{
    public interface ISocketSender
    {
        /// <summary>
        /// Asynchronously sends data to the socket.
        /// </summary>
        /// <param name="sourceStream">BitStream containing the data to send.</param>
        void Send(BitStream sourceStream);
    }
}