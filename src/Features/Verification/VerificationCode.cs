// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Features.Verification
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography;

    public class VerificationCode : IEquatable<VerificationCode>
    {
        private const int FixedLength = 4;

        private readonly string value;

        public VerificationCode(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Length != FixedLength || !value.All(char.IsDigit))
            {
                throw new ArgumentException($"The value of a verification code must consists of {FixedLength} digits.", nameof(value));
            }

            this.value = value;
        }

        public static VerificationCode Generate()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var buffer = new byte[4];

                rng.GetBytes(buffer);
                var seed = BitConverter.ToInt32(buffer, 0);

                var value = new Random(seed)
                    .Next(0, Convert.ToInt32(Math.Pow(10, FixedLength)))
                    .ToString(CultureInfo.InvariantCulture);

                return new VerificationCode(value.PadLeft(FixedLength, '0'));
            }
        }

        public static bool IsWellformed(string value)
        {
            return !string.IsNullOrEmpty(value) && value.Length == FixedLength && value.All(char.IsDigit);
        }

        public bool Equals(VerificationCode other) => other != null && other.value == this.value;

        public override bool Equals(object obj) => obj is VerificationCode other && this.Equals(other);

        public override int GetHashCode() => this.value.GetHashCode();

        public override string ToString() => this.value;
    }
}
