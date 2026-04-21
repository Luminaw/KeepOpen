using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace KeepOpen
{
    public class Config
    {
        public int IterationTimeSeconds { get; set; } = 5;
        public List<AppConfig> Programs { get; set; } = new();

        public static Config Load()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfiguration configuration = builder.Build();
            var config = new Config();
            configuration.Bind(config);
            return config;
        }
    }

    public class AppConfig
    {
        public string? Path { get; set; }
        public string? ProcessName { get; set; }
        public string? Arguments { get; set; }
    }
}
