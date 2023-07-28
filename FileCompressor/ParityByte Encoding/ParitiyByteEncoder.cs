﻿//-----------------------------------------------------------------------
// <copyright file="ParitiyByteEncoder.cs" company="FHWN">
//     Copyright (c) Monkey with a Typewriter GMBH. All rights reserved.
// </copyright>
// <author>Jens Hanssen</author>
// <summary>
// This class is used to add, validate and extract parity bytes from a byte array.
// </summary>
//-----------------------------------------------------------------------
namespace FileCompressor
{
    using System;

    /// <summary>
    /// This class is used to add, validate and extract parity bytes from a byte array.
    /// </summary>
    public class ParitiyByteEncoder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParitiyByteEncoder"/> class.
        /// </summary>
        public ParitiyByteEncoder()
        {
        }

        public byte[] AddParityBytesToByteArray(byte[] byteArray)
        {
            if (byteArray == null)
            {
                throw new ArgumentException($"{nameof(byteArray)} must be a valid byte array, not null!");
            }

            if (byteArray.Length > int.MaxValue / 2)
            {
                throw new ArgumentOutOfRangeException($"Method can only generate Byte arrays for arrays smaller than {int.MaxValue / 2}");
            }

            byte[] returnByteArray = new byte[byteArray.Length * 2];

            for (int i = 0; i < byteArray.Length; i++)
            {
                // original byte in every odd spot
                returnByteArray[i * 2] = byteArray[i];

                // parity bit in all even places
                returnByteArray[(i * 2) + 1] = (byte)(255 - byteArray[i]);
            }

            return returnByteArray;
        }

        public bool CheckByteArrayForParityBytes(byte[] byteArrayWithParityBytes)
        {
            // If the array is not dividable by two it cant be checked for parity bytes
            if (byteArrayWithParityBytes.Length % 2 != 0)
            {
                return false;
            }

            for (int i = 0; i < byteArrayWithParityBytes.Length; i += 2)
            {
                byte originalByte = byteArrayWithParityBytes[i];
                byte parityByte = byteArrayWithParityBytes[i + 1];

                if (originalByte + parityByte != 255)
                {
                    return false;
                }
            }

            // all byte pairs seem to be correct
            return true;
        }

        public byte[] RemoveParityBytesFromArray(byte[] byteArray)
        {
            // byte array length must be even, else there is a problem
            if (byteArray.Length % 2 != 0)
            {
                throw new ArgumentException($"Invalid byte array. Expected even length for byte array {nameof(byteArray)}");
            }

            int newSize = byteArray.Length / 2;
            byte[] resultArray = new byte[newSize];

            for (int i = 0, j = 0; i < byteArray.Length; i += 2, j++)
            {
                resultArray[j] = byteArray[i];
            }

            return resultArray;
        }
    }
}