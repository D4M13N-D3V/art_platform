
using Microsoft.Extensions.Configuration;

namespace comissions.app.database;

public class ApplicationDatabaseConfigurationModel
{
    private readonly IConfiguration _configuration;
    
    public ApplicationDatabaseConfigurationModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public string Host => _configuration?.GetValue<string>("Database:Host") ?? "localhost";
    public int Port => _configuration?.GetValue<int>("Database:Port") ?? 5432;
    public string Database => _configuration?.GetValue<string>("Database:Database") ?? "artplatform";
    public string Username => _configuration?.GetValue<string>("Database:username") ?? "sa";
    public string Password => _configuration?.GetValue<string>("Database:password") ?? "P@ssw0rd";
}