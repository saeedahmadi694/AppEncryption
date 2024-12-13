using Encryptor.AspNetCore.Config;
using Encryptor.AspNetCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace SampleApp2.Data.Interceptors;
public class DecryptMaterializationInterceptor : IMaterializationInterceptor
{
    private readonly IEFCoreSetting _coreSetting;

    public DecryptMaterializationInterceptor(IEFCoreSetting coreSetting)
    {
        _coreSetting = coreSetting;
    }

    public object InitializedInstance(
        MaterializationInterceptionData materializationData,
        object entity)
    {
        _coreSetting.DecryptSensitiveColumns(entity);
        return entity;
    }
}
