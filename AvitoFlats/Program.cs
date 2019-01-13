using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace AvitoFlats
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            
            var service = new AvitoFlatService(configuration.Get<AvitoConfigs>());
            service.Start();

            Console.WriteLine("In progress...");
            Console.ReadKey();
        }
    } 
}