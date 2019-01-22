using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Trepub.Common.Interfaces
{
    public interface IDirectoryStorage
    {
        Task<bool?> Delete(int key);

        Task<byte[]> Get(int key);

        Task Put(int key, byte[] value);

        Task<(bool, byte[])> TryGet(int key);

        string GetFullPath(int key);
    }
}
