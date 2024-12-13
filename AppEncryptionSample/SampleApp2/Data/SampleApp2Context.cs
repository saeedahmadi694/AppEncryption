using Encryptor.AspNetCore.Services;
using Microsoft.EntityFrameworkCore;
using SampleApp2.Data.Interceptors;
using SampleApp2.Models;

namespace SampleApp2.Data;

public class SampleApp2Context : DbContext
{
    public readonly IEFCoreSetting _coreSetting;
    public SampleApp2Context(DbContextOptions<SampleApp2Context> options, IEFCoreSetting coreSetting) : base(options)
    {
        _coreSetting = coreSetting;
    }

    public DbSet<User> User { get; set; }



    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _coreSetting.EncryptSensitiveColumnsAsync(ChangeTracker);
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(new DecryptMaterializationInterceptor(_coreSetting));
    }
}
