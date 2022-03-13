using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.Options;
using NlpEditor.Configuration;

namespace NlpEditor.Utils
{
    internal class GoogleLoader : IDesignationsLoader
    {


        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        readonly static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        readonly static string ApplicationName = "Google Sheets API .NET Quickstart";

        //public string ConceptDesignations { get; set; }
        //public string SnomedExtentions { get; set; }
        //public int CodeIndexConcepts { get; set; }
        //public int CodeIndexExtensions { get; set; }
        //public int DesignationsIndexConcepts { get; set; }
        //public int DesignationsIndexExtensions { get; set; }
        private GoogleConfiguration _configuration { get; set; }
        public GoogleLoader(IOptions<GoogleConfiguration> config)
        {
            _configuration = config.Value;
        }
        public Dictionary<string, string> GetDesignations()
        {
            UserCredential credential = null;
            Dictionary<string, string> designations = new Dictionary<string, string>();
            Dictionary<string, string> temp = new Dictionary<string, string>();
            try
            {
                using (var stream =
                    new FileStream("credentials.json", FileMode.Open, FileAccess.ReadWrite))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    string credPath = "token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                    Console.WriteLine("Credential file saved to: " + credPath);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при подключении к Google. Попробуйте запустить программу от имени администратора");
                return null;
            }

            
            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            String range = "A2:D";
            String spreadsheetId = _configuration.ConceptDesignations;
            int correctionIndex = _configuration.DesignationsIndexConcepts - 1;
            // Define request parameters.
            //var load = new Loading();
            //load.Show();
            bool tryAgain = false;
            var milisec = 2000;
            var codeIndex = _configuration.CodeIndexConcepts - 1;

            try
            {
                for (int i = 0; i < 2; i++)
                {
                    var countTry = 0;
                    do
                    {
                        try
                        {
                            temp.Clear();
                            var request1 = service.Spreadsheets.Values.BatchGet(spreadsheetId);
                            SpreadsheetsResource.ValuesResource.GetRequest request =
                                service.Spreadsheets.Values.Get(spreadsheetId, range);
                            Task.Delay(milisec).Wait();
                            ValueRange response = request.Execute();
                            IList<IList<Object>> values = response.Values;

                            if (values != null && values.Count > 0)
                            {
                                for (int j = 0; j < values.Count; j++)
                                {
                                    if (values[j].Count > correctionIndex && values[j][codeIndex].ToString() != "")
                                    {
                                        try
                                        {
                                            temp.Add(values[j][codeIndex].ToString(), values[j][correctionIndex].ToString());
                                        }
                                        catch
                                        {
                                            continue;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("No data found.");
                            }
                            tryAgain = false;
                        }
                        catch (Exception e)
                        {
                            tryAgain = true;
                            milisec += 2000;
                            var ex = e;
                            countTry += 1;
                            if (countTry == 5)
                            {
                                MessageBox.Show("Ошибка при загрузке переводов. Попробуйте ещё раз");
                                //load.Close();
                                return null;
                            }
                        }

                    }
                    while (tryAgain);

                    spreadsheetId = _configuration.SnomedExtensions;
                    range = "A2:H";
                    codeIndex = _configuration.CodeIndexExtensions - 1;
                    correctionIndex = _configuration.DesignationsIndexExtensions - 1;
                    designations = designations.Concat(temp).ToDictionary(x => x.Key, y => y.Value);

                }
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                MessageBox.Show($"Ошибка при загрузке переводов. Попробуйте ещё раз. {e}");
                //load.Close();
                return null;
            }
            //load.Close();
            return designations;
            //    SpreadsheetsResource.ValuesResource.GetRequest requestExt =
            //            service.Spreadsheets.Values.Get(spreadsheetExtId, rangeExt);
            //    Prints the names and majors of students in a sample spreadsheet:
            //https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit

        }
    }

    public interface IDesignationsLoader
    {
        public Dictionary<string, string> GetDesignations();
    }

}
