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
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Lidgren.Network
{
    /// <summary>
    /// Methods to encrypt and decrypt data using the XTEA algorith
    /// </summary>
    public sealed class NetXtea
    {
        const int m_blockSize = 8;
        const int m_delta = unchecked((int)0x9E3779B9);
        const int m_keySize = 16;

        readonly int m_numRounds;

        readonly uint[] m_sum0;
        readonly uint[] m_sum1;

        /// <summary>
        /// 16 byte key
        /// </summary>
        public NetXtea(byte[] key, int rounds)
        {
            if (key.Length < 16)
                throw new NetException("Key too short!");

            m_numRounds = rounds;
            m_sum0 = new uint[m_numRounds];
            m_sum1 = new uint[m_numRounds];
            var tmp = new uint[8];

            int num2;
            var index = num2 = 0;
            while (index < 4)
            {
                tmp[index] = BitConverter.ToUInt32(key, num2);
                index++;
                num2 += 4;
            }
            for (index = num2 = 0; index < 32; index++)
            {
                m_sum0[index] = ((uint)num2) + tmp[num2 & 3];
                num2 += -1640531527;
                m_sum1[index] = ((uint)num2) + tmp[(num2 >> 11) & 3];
            }
        }

        /// <summary>
        /// 16 byte key
        /// </summary>
        public NetXtea(byte[] key) : this(key, 32)
        {
        }

        /// <summary>
        /// String to hash for key
        /// </summary>
        public NetXtea(string key) : this(SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(key)), 32)
        {
        }

        static uint BytesToUInt(byte[] bytes, int offset)
        {
            var retval = (uint)(bytes[offset] << 24);
            retval |= (uint)(bytes[++offset] << 16);
            retval |= (uint)(bytes[++offset] << 8);
            return (retval | bytes[++offset]);
        }

        public void DecryptBlock(byte[] inBytes, int inOff, byte[] outBytes, int outOff)
        {
            // Pack bytes into integers
            var v0 = BytesToUInt(inBytes, inOff);
            var v1 = BytesToUInt(inBytes, inOff + 4);

            for (var i = m_numRounds - 1; i >= 0; i--)
            {
                v1 -= (((v0 << 4) ^ (v0 >> 5)) + v0) ^ m_sum1[i];
                v0 -= (((v1 << 4) ^ (v1 >> 5)) + v1) ^ m_sum0[i];
            }

            UIntToBytes(v0, outBytes, outOff);
            UIntToBytes(v1, outBytes, outOff + 4);

            return;
        }

        public void EncryptBlock(byte[] inBytes, int inOff, byte[] outBytes, int outOff)
        {
            var v0 = BytesToUInt(inBytes, inOff);
            var v1 = BytesToUInt(inBytes, inOff + 4);

            for (var i = 0; i != m_numRounds; i++)
            {
                v0 += (((v1 << 4) ^ (v1 >> 5)) + v1) ^ m_sum0[i];
                v1 += (((v0 << 4) ^ (v0 >> 5)) + v0) ^ m_sum1[i];
            }

            UIntToBytes(v0, outBytes, outOff);
            UIntToBytes(v1, outBytes, outOff + 4);

            return;
        }

        static void UIntToBytes(uint value, byte[] destination, int destinationOffset)
        {
            destination[destinationOffset++] = (byte)(value >> 24);
            destination[destinationOffset++] = (byte)(value >> 16);
            destination[destinationOffset++] = (byte)(value >> 8);
            destination[destinationOffset++] = (byte)value;
        }
    }
}