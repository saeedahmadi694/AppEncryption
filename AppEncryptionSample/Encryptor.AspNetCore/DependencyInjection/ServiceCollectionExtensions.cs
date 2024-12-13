using Encryptor.AspNetCore.Config;
using Encryptor.AspNetCore.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Encryptor.AspNetCore.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBookHouseEncriptionService(this IServiceCollection services, Action<EncryptionSetting> configure)
    {
        var encryptionSetting = new EncryptionSetting();
        configure(encryptionSetting);

        services.AddSingleton(encryptionSetting);
        services.AddSingleton((Func<IServiceProvider, ITransitService>)(sp =>
        {
            return new TransitService(
                encryptionSetting.Vault.Transit.EncryptionKey,
                encryptionSetting.Vault.Token,
                encryptionSetting.Vault.BaseAddress,
                encryptionSetting.Vault.Transit.Path);
        }));
        services.AddSingleton<IEFCoreSetting, EFCoreSetting>();
        return services;
    }
}

