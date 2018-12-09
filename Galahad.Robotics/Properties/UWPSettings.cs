using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Storage;

using Newtonsoft.Json;

namespace Galahad.Properties
{
    internal sealed class UWPSettings
    {
        private UWPSettings()
        {
        }

        public static async Task<UWPSettings> Load(string path = @"Gala.Data.Config\UWPConfig.json")
        {
            //https://docs.microsoft.com/en-us/windows/uwp/files/file-access-permissions

            Assembly configAsm = typeof(UWPSettings).GetTypeInfo().Assembly;
            StorageFile file = await configAsm.GetStorageFile(ApplicationData.Current.LocalFolder, path);

            string data = null;
            UWPSettings result = null;

            try
            {
                data = File.ReadAllText(file.Path);
                result = JsonConvert.DeserializeObject<UWPSettings>(data);
                if (result == null) throw new NullReferenceException();
            }
            catch
            {
                // Copy Local Settings
                string appPath = Path.Combine("Gala.Data.Config", new FileInfo(file.Path).Name);
                if (File.Exists(file.Path)) File.Delete(file.Path);
                File.Copy(appPath, file.Path);
                data = File.ReadAllText(file.Path);
                result = JsonConvert.DeserializeObject<UWPSettings>(data);
                if (result == null) throw;
            }

            // TODO:  Auto-Update Storage File if settings schema changes


            return result;
        }

        public void Save(string filename = "config.json")
        {
            string data = JsonConvert.SerializeObject(this);
            System.IO.File.WriteAllText(filename, data);
        }

        public string DataAccessManagerConnectionString { get; set; }
        public string ChatbotName { get; set; }
        public string ChatbotAliceConfigFolder { get; set; }
        public string ChatbotResourcesFolder { get; set; }
        public short ChatbotDisplayResponseWaitTime { get; set; }
        public string DefaultUserName { get; set; }
        public short ColorTemplateHybridResultThreshold { get; set; }
        public short ColorBrightnessThreshold { get; set; }

        public int GpioLeftForwardPin { get; set; }
        public int GpioLeftReversePin { get; set; }
        public int GpioRightForwardPin { get; set; }
        public int GpioRightReversePin { get; set; }

        public int HttpRequestTimeout { get; set; }
        public string HttpServiceHostIpAddress { get; set; }
        public int HttpServiceHostPort { get; set; }

        public short ShapeOblongRecognitionLevel { get; set; }
        public decimal ShapeOblongThreshold { get; set; }
        public bool ShapeOblongRecognitionNormalization { get; set; }
        public bool SpeechIsSilent { get; set; }
        public Galatea.AI.Imaging.ImagingSettings ImagingSettings { get; set; }
    }
}
