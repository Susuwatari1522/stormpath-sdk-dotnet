﻿using Stormpath.SDK.Api;

namespace Stormpath.SDK.Client
{
    public interface IClientBuilder
    {
        IClientBuilder SetApiKey(IApiKey apiKey);

        //IClientBuilder SetAuthenticationScheme(AuthenticationScheme authenticationScheme);
        // TODO

        IClientBuilder SetConnectionTimeout(int timeout);

        IClientBuilder SetBaseUrl(string baseUrl);

        IClient Build();
    }
}
