using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace Galahad.Client.Properties
{
    using Gala.Data.Configuration;

    internal sealed class ClientSettings
    {
        private ClientSettings()
        {
        }

        public static async Task<ClientSettings> Load(string path = @"Properties\Galahad.Client.Config.json")
        {
            Assembly configAsm = typeof(ClientSettings).GetTypeInfo().Assembly;
            StorageFile file = await configAsm.GetStorageFile(ApplicationData.Current.LocalFolder, path);

            string data = null;
            ClientSettings result = null;

            try
            {
                data = File.ReadAllText(file.Path);
                result = JsonConvert.DeserializeObject<ClientSettings>(data);
                if (result == null) throw new NullReferenceException();
            }
            catch
            {
                // Copy Local Settings
                string appPath = Path.Combine("Gala.Data.Config", new FileInfo(file.Path).Name);
                if (File.Exists(file.Path)) File.Delete(file.Path);
                File.Copy(appPath, file.Path);
                data = File.ReadAllText(file.Path);
                result = JsonConvert.DeserializeObject<ClientSettings>(data);
                if (result == null) throw;
            }

            return JsonConvert.DeserializeObject<ClientSettings>(data);
        }

        public void Save(string filename = "Galahad.Client.Config.json")
        {
            string data = JsonConvert.SerializeObject(this);
            System.IO.File.WriteAllText(filename, data);
        }


        public Galatea.Diagnostics.DebuggerLogLevel DebuggerLogLevel { get; set; }

        public string IpAddress { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }
    }
}
