using Microsoft.Extensions.Configuration;
using Trepub.Common.Facade;
using Trepub.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Trepub.IFS.FileSystem
{
    public class DirectoryStorage : IDirectoryStorage
    {
        protected IConfiguration configuration;

        public DirectoryStorage()
        {
            this.configuration = FacadeProvider.IfsFacade.GetConfigurationRoot(); 
        }

        public Task<bool?> Delete(int key)
        {
            string fullPath = this.GetFullPath(key);
            bool existed = System.IO.File.Exists(fullPath);
            if (existed)
                File.Delete(this.GetFullPath(key));
            return Task.FromResult((bool?)existed);
        }

        public Task<byte[]> Get(int key)
        {
            string fullPath = this.GetFullPath(key);
            try
            {
                return Task.FromResult(System.IO.File.ReadAllBytes(fullPath));
            }
            catch (FileNotFoundException e)
            {
                throw new KeyNotFoundException("File not found", innerException: e);
            }
        }

        public Task Put(int key, byte[] value)
        {
            string fullPath = this.GetFullPath(key);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            File.WriteAllBytes(fullPath, value);
            return Task.CompletedTask;
        }

        public Task<(bool, byte[])> TryGet(int key)
        {
            string fullPath = this.GetFullPath(key);
            if (!File.Exists(fullPath))
                return Task.FromResult((false, default(byte[])));
            try
            {
                return Task.FromResult((true, File.ReadAllBytes(fullPath)));
            }
            catch (FileNotFoundException)
            {
                return Task.FromResult((false, default(byte[])));
            }
        }

        public string GetFullPath(int key)
        {
            var pathWithFirstPart = 
                Path.Combine(configuration.GetSection("LocalSettings")["DirectoryStoragePath"], PathFirstPart(key));
            var pathWithSecondPart = Path.Combine(pathWithFirstPart, PathSecondPart(key));
            var pathWithThirdPart = Path.Combine(pathWithSecondPart, key.ToString());
            var fileFullPath = Path.Combine(pathWithSecondPart, key.ToString());
            return fileFullPath;
        }

        private Func<int, string> PathFirstPart = key => ((key / 1000 + 1) * 1000 - 1).ToString();
        private Func<int, string> PathSecondPart = key => (((key%1000)/100 + 1) * 100 - 1).ToString();
        
    }
}
