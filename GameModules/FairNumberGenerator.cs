using System;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;

namespace DiceGame.GameModules
{
    /// <summary>
    /// Generates provably fair random numbers using HMAC-SHA3 and modular addition.
    /// Ensures fairness by combining server and client inputs with cryptographic security.
    /// </summary>
    public class FairNumberGenerator
    {
        private readonly int _range;

        /// <summary>
        /// Initializes a new instance of the FairNumberGenerator class with a specified range.
        /// </summary>
        /// <param name="range">The upper bound (exclusive) for random number generation.</param>
        public FairNumberGenerator(int range)
        {
            if (range <= 0)
                throw new ArgumentException("Range must be a positive integer.");

            _range = range;
        }

        /// <summary>
        /// Gets the range of the random number generator.
        /// </summary>
        public int Range => _range;

        /// <summary>
        /// Generates a cryptographically secure random server seed (256 bits).
        /// </summary>
        /// <returns>Server seed as a byte array.</returns>
        public byte[] CreateServerSeed()
        {
            var key = new byte[32]; // 256-bit
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(key);
            return key;
        }

        /// <summary>
        /// Generates a cryptographically secure random number within the specified range.
        /// </summary>
        /// <returns>A random number between 0 and Range-1.</returns>
        public int GenerateComputerNumber()
        {
            return RandomNumberGenerator.GetInt32(_range);
        }

        /// <summary>
        /// Computes the HMAC-SHA3-256 of the number using the server seed.
        /// </summary>
        /// <param name="key">Cryptographically secure server seed.</param>
        /// <param name="number">Number to compute HMAC for.</param>
        /// <returns>HMAC as a hexadecimal string.</returns>
        public string ComputeHmac(byte[] key, int number)
        {
            var mac = new HMac(new Sha3Digest(256));
            mac.Init(new KeyParameter(key));
            var message = BitConverter.GetBytes(number);
            mac.BlockUpdate(message, 0, message.Length);
            var result = new byte[mac.GetMacSize()];
            mac.DoFinal(result, 0);
            return BitConverter.ToString(result).Replace("-", "").ToUpper();
        }

        /// <summary>
        /// Combines computer and user inputs using modular arithmetic.
        /// </summary>
        /// <param name="computerNumber">Computer-generated number.</param>
        /// <param name="userNumber">User-selected number.</param>
        /// <returns>The fair result modulo the range.</returns>
        public int CombineWithUserInput(int computerNumber, int userNumber)
        {
            if (computerNumber < 0 || computerNumber >= _range)
                throw new ArgumentException($"Computer number must be between 0 and {_range - 1}.");
            if (userNumber < 0 || userNumber >= _range)
                throw new ArgumentException($"User number must be between 0 and {_range - 1}. Example: Enter a number between 0 and {_range - 1}.");

            return (computerNumber + userNumber) % _range;
        }
    }
}