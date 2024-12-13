using Encryptor.AspNetCore.Config;
using Encryptor.AspNetCore.DependencyInjection;

namespace SampleApp2.DependencyInjection;

public static class AddConfiguredInjection
{
    public static IServiceCollection AddConfiguredVandar(this IServiceCollection services, IConfiguration configuration)
    {

        var minioOptions = new EncryptionConfig();
        minioOptions = configuration.GetSection(EncryptionConfig.Key).Get<EncryptionConfig>();
        services.AddBookHouseEncriptionService(options =>
        {
            options.Vault = new VaultSetting
            {
                BaseAddress = minioOptions.Vault.BaseAddress,
                Token = minioOptions.Vault.Token,
                Transit = new TransitSetting
                {
                    EncryptionKey = minioOptions.Vault.Transit.EncryptionKey,
                    Path = minioOptions.Vault.Transit.Path
                }
            };
            options.SensitiveColumns = minioOptions.SensitiveColumns;
        });
        //services.AddScoped<IEFCoreSetting, EFCoreSetting>();
        return services;
    }
}

