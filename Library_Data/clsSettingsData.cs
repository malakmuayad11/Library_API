using Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;

namespace Library_Data
{
    public static class clsSettingsData
    {
        public static string ConnectionString { get; private set; }

        public static void Initialize(IConfiguration configuration)
        {
            ConnectionString = configuration["ConnectionString"];
        }
    }
}
