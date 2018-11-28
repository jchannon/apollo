﻿// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Ironclad.Tests.Sdk
{
    using System.Net.Http;
    using Xunit;

    /// <summary>
    /// Represents the base class for an authentication test.
    /// </summary>
    [Collection("Ironclad")]
    public class AuthenticationTest
    {
        private readonly AuthenticationFixture fixture;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationTest"/> class.
        /// </summary>
        /// <param name="fixture">The authentication fixture.</param>
        public AuthenticationTest(AuthenticationFixture fixture)
        {
            this.fixture = fixture;
        }

        /// <summary>
        /// Gets the authority.
        /// </summary>
        /// <value>The authority.</value>
        protected string Authority => this.fixture.Authority;

        /// <summary>
        /// Gets the authorized HTTP message handler.
        /// </summary>
        /// <value>The handler.</value>
        protected HttpMessageHandler Handler => this.fixture.Handler;
    }
}
