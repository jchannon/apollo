// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

using Ironclad.Tests.Sdk;
using Xunit;

namespace Apollo.Tests.Unit.Sdk
{
    [CollectionDefinition(nameof(ApolloIntegrationCollection))]
    public class ApolloIntegrationCollection : 
        ICollectionFixture<ApolloIntegrationFixture>,
        ICollectionFixture<AuthenticationFixture>
    {}
}