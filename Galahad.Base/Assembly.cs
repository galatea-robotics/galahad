using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Storage;

namespace Galahad
{
    internal static class AssemblyExtension
    {
        internal static async Task<StorageFile> GetStorageFile(this Assembly asm, StorageFolder storageFolder, string filename)
        {
            StorageFile dataFile;
            FileInfo fi = new FileInfo(filename);
            string storageFilename = fi.Name;
            string path = Path.Combine(storageFolder.Path, storageFilename);
            if (!File.Exists(path))
            {
                // Save AppX file to Storage folder
                File.Copy(filename, path);
            }

            dataFile = await storageFolder.GetFileAsync(storageFilename);

            return dataFile;
        }
    }
}
