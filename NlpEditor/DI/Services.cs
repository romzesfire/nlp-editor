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
        public static AppConfiguration Configuration { get; set; }
        public static void Set()
        {
            Configuration = JsonConvert.DeserializeObject<AppConfiguration>(File.ReadAllText("appsettings.json"));
            _services = new ServiceCollection()
                .AddSingleton<INlpFileLoader, NlpFileFromExcelLoader>()
                .AddSingleton<INlpFileLoader, NlpFromNlpsLoader>()
                .AddSingleton<INlpFileLoader, NlpFromJsonLoader>()
                .AddSingleton<INetworkProvider, NetworksProvider>()
                .AddSingleton<IDuplicateChecker, DuplicateChecker>()
                .AddSingleton<INlpSaver, NlpToExcelSaver>()
                .AddSingleton<INlpSaver, NlpToJsonSaver>()
                .AddSingleton<INlpSaver, NlpToNlpsSaver>()
                .AddSingleton<IDesignationsLoader, GoogleLoader>()
                .AddSingleton<IDesignationsProvider, DesignationsGoogleProvider>()
                .Configure<GenieConfiguration>(options =>
                {
                    options.LicenseFile = Configuration.Genie.LicenseFile;
                })
                .Configure<NlpConfiguration>(options =>
                {
                    options.CodeRowIndex = Configuration.Nlp.CodeRowIndex;
                    options.CodeSystemCodeIndex = Configuration.Nlp.CodeSystemCodeIndex;
                    options.GenderRowIndex = Configuration.Nlp.GenderRowIndex;
                    options.PriorNameRowIndex = Configuration.Nlp.PriorNameRowIndex;
                    options.StartIndexColumn = Configuration.Nlp.StartIndexColumn;
                    options.StatusRowIndex = Configuration.Nlp.StatusRowIndex;
                    options.ValueCodeRowIndex = Configuration.Nlp.ValueCodeRowIndex;
                    options.ValueCodeSystemRowIndex = Configuration.Nlp.ValueCodeSystemRowIndex;
                    options.SymptomsStartRowIndex = Configuration.Nlp.SymptomsStartRowIndex;
                })
                .Configure<GoogleConfiguration>(o =>
                {
                    o.CodeIndexConcepts = Configuration.Google.CodeIndexConcepts;
                    o.CodeIndexExtensions = Configuration.Google.CodeIndexExtensions;
                    o.ConceptDesignations = Configuration.Google.ConceptDesignations;
                    o.DesignationsIndexConcepts = Configuration.Google.DesignationsIndexConcepts;
                    o.DesignationsIndexExtensions = Configuration.Google.DesignationsIndexExtensions;
                    o.SnomedExtensions = Configuration.Google.SnomedExtensions;
                })
                .BuildServiceProvider();

        }

        public static T GetService<T>()
        {
            return _services.GetService<T>();
        }
        public static IEnumerable<T> GetServices<T>()
        {
            return _services.GetServices<T>();
        }
    }
}
