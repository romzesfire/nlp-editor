using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NlpEditor.Configuration;
using NlpEditor.Utils;

namespace NlpEditor.DI
{
    public static class Services
    {
        private static IServiceProvider _services { get; set; }
        public static void Set()
        {
            var configuration = JsonConvert.DeserializeObject<AppConfiguration>(File.ReadAllText("appsettings.json"));
            _services = new ServiceCollection()
                .AddSingleton<INlpFileLoader, NlpFileFromFileLoader>()
                .Configure<NlpConfiguration>(options =>
                {
                    options.CodeRowIndex = configuration.Nlp.CodeRowIndex;
                    options.CodeSystemCodeIndex = configuration.Nlp.CodeSystemCodeIndex;
                    options.GenderRowIndex = configuration.Nlp.GenderRowIndex;
                    options.PriorNameRowIndex = configuration.Nlp.PriorNameRowIndex;
                    options.StartIndexColumn = configuration.Nlp.StartIndexColumn;
                    options.StatusRowIndex = configuration.Nlp.StatusRowIndex;
                    options.ValueCodeRowIndex = configuration.Nlp.ValueCodeRowIndex;
                    options.ValueCodeSystemRowIndex = configuration.Nlp.ValueCodeSystemRowIndex;
                    options.SymptomsStartRowIndex = configuration.Nlp.SymptomsStartRowIndex;
                })
                .BuildServiceProvider();

        }

        public static T GetService<T>()
        {
            return _services.GetService<T>();
        }
    }
}
