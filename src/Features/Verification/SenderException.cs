// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Features.Verification
{
    using System;

    public class SenderException : Exception
    {
        public SenderException(string message)
            : base(message)
        {
        }

        public SenderException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
