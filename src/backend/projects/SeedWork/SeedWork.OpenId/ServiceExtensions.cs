using System;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Validation.AspNetCore;

public static class ServiceExtensions
{
    public static OpenIddictBuilder RegisterDependentOpenIdService(this IServiceCollection services, AuthSettings authSettings)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (authSettings == null)
        {
            throw new ArgumentNullException(nameof(authSettings));
        }

        if (string.IsNullOrWhiteSpace(authSettings.ClientId))
        {
            throw new ApplicationException($"{nameof(AuthSettings.ClientId)} should be set");
        }

        if (string.IsNullOrWhiteSpace(authSettings.ClientSecret))
        {
            throw new ApplicationException($"{nameof(AuthSettings.ClientSecret)} should be set");
        }

        if (string.IsNullOrWhiteSpace(authSettings.Issuer))
        {
            throw new ApplicationException($"{nameof(AuthSettings.Issuer)} should be set");
        }

        services.AddAuthentication(options => options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

        return services.AddOpenIddict()
            .AddValidation(options =>
            {
                // Note: the validation handler uses OpenID Connect discovery
                // to retrieve the address of the introspection endpoint.
                options.SetIssuer(authSettings.Issuer);

                // NOTE: client id and audience match to use introspect endpoint
                options.AddAudiences(authSettings.ClientId);

                // Configure the validation handler to use introspection and register the client
                // credentials used when communicating with the remote introspection endpoint.
                options.UseIntrospection()
                       .SetClientId(authSettings.ClientId)
                       .SetClientSecret(authSettings.ClientSecret);

                // Register the System.Net.Http integration.
                options.UseSystemNetHttp();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();
            });
    }
}
