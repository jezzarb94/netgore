﻿/* Copyright (c) 2010 Michael Lidgren

Permission is hereby granted, free of charge, to any person obtaining a copy of this software
and associated documentation files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom
the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
USE OR OTHER DEALINGS IN THE SOFTWARE.

*/
using System;
using System.Diagnostics;
using System.Linq;

namespace Lidgren.Network
{
    /// <summary>
    /// Outgoing message used to send data to remote peer(s)
    /// </summary>
    [DebuggerDisplay("LengthBits={LengthBits}")]
    public sealed partial class NetOutgoingMessage
    {
        internal int m_fragmentChunkByteSize; // size, in bytes, of every chunk but the last one
        internal int m_fragmentChunkNumber; // which number chunk this is, starting with 0
        internal int m_fragmentGroup; // which group of fragments ths belongs to
        internal int m_fragmentGroupTotalBits; // total number of bits in this group
        internal bool m_isSent;
        internal NetMessageType m_messageType;
        internal int m_recyclingCount;

        internal NetOutgoingMessage()
        {
        }

        internal int Encode(byte[] intoBuffer, int ptr, int sequenceNumber)
        {
            //  8 bits - NetMessageType
            //  1 bit  - Fragment?
            // 15 bits - Sequence number
            // 16 bits - Payload length in bits

            intoBuffer[ptr++] = (byte)m_messageType;

            var low = (byte)((sequenceNumber << 1) | (m_fragmentGroup == 0 ? 0 : 1));
            intoBuffer[ptr++] = low;
            intoBuffer[ptr++] = (byte)(sequenceNumber >> 7);

            if (m_fragmentGroup == 0)
            {
                intoBuffer[ptr++] = (byte)m_bitLength;
                intoBuffer[ptr++] = (byte)(m_bitLength >> 8);

                var byteLen = NetUtility.BytesToHoldBits(m_bitLength);
                if (byteLen > 0)
                {
                    Buffer.BlockCopy(m_data, 0, intoBuffer, ptr, byteLen);
                    ptr += byteLen;
                }
            }
            else
            {
                var wasPtr = ptr;
                intoBuffer[ptr++] = (byte)m_bitLength;
                intoBuffer[ptr++] = (byte)(m_bitLength >> 8);

                //
                // write fragmentation header
                //
                ptr = NetFragmentationHelper.WriteHeader(intoBuffer, ptr, m_fragmentGroup, m_fragmentGroupTotalBits,
                    m_fragmentChunkByteSize, m_fragmentChunkNumber);
                var hdrLen = ptr - wasPtr - 2;

                // update length
                var realBitLength = m_bitLength + (hdrLen * 8);
                intoBuffer[wasPtr] = (byte)realBitLength;
                intoBuffer[wasPtr + 1] = (byte)(realBitLength >> 8);

                var byteLen = NetUtility.BytesToHoldBits(m_bitLength);
                if (byteLen > 0)
                {
                    Buffer.BlockCopy(m_data, (m_fragmentChunkNumber * m_fragmentChunkByteSize), intoBuffer, ptr, byteLen);
                    ptr += byteLen;
                }
            }

            NetException.Assert(ptr > 0);
            return ptr;
        }

        /// <summary>
        /// Encrypt this message using the XTEA algorithm; no more writing can be done before sending it
        /// </summary>
        public void Encrypt(NetXtea tea)
        {
            // need blocks of 8 bytes
            WritePadBits();
            var blocksNeeded = (m_bitLength + 63) / 64;
            var missingBits = (blocksNeeded * 64) - m_bitLength;
            var missingBytes = NetUtility.BytesToHoldBits(missingBits);
            for (var i = 0; i < missingBytes; i++)
            {
                Write((byte)0);
            }

            var result = new byte[m_data.Length];
            for (var i = 0; i < blocksNeeded; i++)
            {
                tea.EncryptBlock(m_data, (i * 8), result, (i * 8));
            }
            m_data = result;
        }

        internal int GetEncodedSize()
        {
            var retval = 5; // regular headers
            if (m_fragmentGroup != 0)
            {
                retval += NetFragmentationHelper.GetFragmentationHeaderSize(m_fragmentGroup, m_fragmentGroupTotalBits / 8,
                    m_fragmentChunkByteSize, m_fragmentChunkNumber);
            }
            retval += LengthBytes;

            return retval;
        }

        internal void Reset()
        {
            m_messageType = NetMessageType.LibraryError;
            m_bitLength = 0;
            m_isSent = false;
            m_recyclingCount = 0;
            m_fragmentGroup = 0;
        }

        /// <summary>
        /// Returns a string that represents this object
        /// </summary>
        public override string ToString()
        {
            return "[NetOutgoingMessage " + m_messageType + " " + LengthBytes + " bytes]";
        }
    }
}