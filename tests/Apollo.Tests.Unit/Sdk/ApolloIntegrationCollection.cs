// Copyright (c) Lykke Corp.
// See the LICENSE file in the project root for more information.

namespace Apollo.Tests.Unit.Sdk
{
    using Ironclad.Tests.Sdk;
    using Xunit;
    
    [CollectionDefinition(nameof(ApolloIntegrationCollection))]
    public class ApolloIntegrationCollection : 
        ICollectionFixture<ApolloIntegrationFixture>,
        ICollectionFixture<AuthenticationFixture>
    {
    }
}