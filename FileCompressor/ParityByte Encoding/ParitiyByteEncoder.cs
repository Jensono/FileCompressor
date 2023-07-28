//-----------------------------------------------------------------------
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
    /// This class is used to add, validate and extract parity bytes from a byte array. The current implementation adds a parity byte for every byte giving , effectifly doubling the byte arrays length.
    /// A parity byte and normal byte always come in pairs. The first byte is always the original byte, the second is the diffrence of 255 and the original byte.
    /// </summary>
    public class ParitiyByteEncoder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParitiyByteEncoder"/> class. The current implementation adds a parity byte for every byte giving , effectifly doubling the byte arrays length.
        /// A parity byte and normal byte always come in pairs. The first byte is always the original byte, the second is the diffrence of 255 and the original byte.
        /// </summary>
        public ParitiyByteEncoder()
        {
        }

        /// <summary>
        /// This method adds aprity bytes to an array of bytes, and therefor expands it. The current implementation adds a parity byte for every byte giving , effectifly doubling the byte arrays length.
        /// A parity byte and normal byte always come in pairs. The first byte is always the original byte, the second is the diffrence of 255 and the original byte.
        /// </summary>
        /// <param name="byteArray"> The byte array for which to add parity bytes.</param>
        /// <returns> A byte array which was equiped with parity bytes, doubling its size compared to the orignal length.</returns>
        /// <exception cref="ArgumentNullException"> Is triggerd when the parameter was null. </exception>
        /// <exception cref="ArgumentOutOfRangeException"> Is triggered when the array given contains more entries then half of 2^32 .</exception>
        public byte[] AddParityBytesToByteArray(byte[] byteArray)
        {
            if (byteArray == null)
            {
                throw new ArgumentNullException($"{nameof(byteArray)} must be a valid byte array, not null!");
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

        /// <summary>
        /// This method validates all given orignial byte - parity byte pairs inside a given byte array.
        /// </summary>
        /// <param name="byteArrayWithParityBytes"> The byte array which should be validated.</param>
        /// <returns> A boolean indicating whether or not the given byte arrays parity bytes were all correct.</returns>
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

        /// <summary>
        /// This method removes the parity bytes from a byte array, extracting the orignal bytes.
        /// </summary>
        /// <param name="byteArray"> The byte array from which to remove the parity bytes from.</param>
        /// <returns> The orignal byte array that does not contain any parity bytes.</returns>
        /// <exception cref="ArgumentException"> Is raised if the given byte array cant be divided by 2, meaning its not an array which was equiped with parity bytes by this algorithm or just some random array.
        /// Per defintion all arrays with this algorithm must contain an even amount of bytes to be valid.
        /// </exception>
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