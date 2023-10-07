using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace YooAsset.Editor
{
    public class BuildEditorOnlyContext : IContextObject
    {
        private Dictionary<string, MemoryStream> mStreamBuffer;

        public BuildEditorOnlyContext()
            => mStreamBuffer = new Dictionary<string, MemoryStream>();

        public void Load(string path)
        {
            FileStream fileStream = null;
            MemoryStream bufferStream = null;
            try
            {
                fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                bufferStream = new MemoryStream();
                fileStream.CopyTo(bufferStream);
                mStreamBuffer.Add(path, bufferStream);
            }
            catch
            {
                mStreamBuffer.Remove(path);
                bufferStream?.Close();
            }
            finally
            {
                fileStream?.Close();
            }
        }

        public void RecoveryRelease()
        {
            Parallel.ForEach(mStreamBuffer, kv => 
            {
                string path = kv.Key;
                MemoryStream stream = kv.Value;
                stream.Position = 0;

                FileStream fileStream = null;
                try
                {
                    fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
                    stream.WriteTo(fileStream);
                }
                catch
                {
                    Debug.LogError($"EditorOnly文件恢复失败:{path}");
                }
                finally
                {
                    stream.Close();
                    fileStream?.Close();
                }
            });
            mStreamBuffer.Clear();
        }
    }
}