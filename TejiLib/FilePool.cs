using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TejiLib {
    public class FilePool {

        public FilePool() {
            fileList = new Dictionary<string, FileReaderItem>();
        }

        Dictionary<string, FileReaderItem> fileList;

        object listOperation = new object();
        readonly int FILE_BLOCK_SIZE = 1024;

        string GetFileFullPath(string hash) {
            return Information.WorkPath.Enter("cache").Enter(hash).Path();
        }

        public (bool status, int blockCount, int blockSize, int lastBlockSize) AddReadFile(string hash) {
            lock (listOperation) {
                if (fileList.ContainsKey(hash)) {
                    if (fileList[hash].IsWritable) return (false, default(int), default(int), default(int));
                    fileList[hash].UsageCount += 1;
                    return (true, fileList[hash].BlockCount, fileList[hash].BlockSize, fileList[hash].LastBlockLength);
                } else {
                    if (!File.Exists(GetFileFullPath(hash))) return (false, default(int), default(int), default(int));
                    var cache = new FileStream(GetFileFullPath(hash), FileMode.Open, FileAccess.Read);
                    var length = (int)cache.Length;
                    fileList.Add(GetFileFullPath(hash), new FileReaderItem() { fs = cache, IsWritable = false, BlockSize = FILE_BLOCK_SIZE, BlockCount = (length % FILE_BLOCK_SIZE == 0 ? length / FILE_BLOCK_SIZE : (length / FILE_BLOCK_SIZE) + 1), LastBlockLength = length % FILE_BLOCK_SIZE, UsageCount = 1 });
                    return (true, fileList[hash].BlockCount, fileList[hash].BlockSize, fileList[hash].LastBlockLength);
                }
            }
        }

        public bool AddWriteFile(string hash, int blockCount, int blockSize, int lastBlockSize) {
            lock (listOperation) {
                if (fileList.ContainsKey(hash)) return false;
                if (File.Exists(GetFileFullPath(hash))) return false;

                var cache = new FileStream(GetFileFullPath(hash), FileMode.CreateNew, FileAccess.Write);
                fileList.Add(GetFileFullPath(hash), new FileReaderItem() { fs = cache, IsWritable = true, BlockSize = blockSize, BlockCount = blockCount, LastBlockLength = lastBlockSize, UsageCount = 1 });
                return true;
            }
        }


        public void RemoveReadFile(string hash) {
            lock (listOperation) {
                if (fileList.ContainsKey(hash)) {
                    fileList[hash].UsageCount -= 1;
                    if (fileList[hash].UsageCount == 0) {
                        fileList[hash].fs.Close();
                        fileList[hash].fs.Dispose();
                        fileList.Remove(hash);
                    }
                }
            }
        }

        public void RemoveWriteFile(string hash) {
            lock (listOperation) {
                if (fileList.ContainsKey(hash)) {
                    fileList[hash].fs.Close();
                    fileList[hash].fs.Dispose();
                    fileList.Remove(hash);
                }
            }
        }

        public byte[] Read(string hash, int index) {
            lock (listOperation) {
                if (!fileList.ContainsKey(hash)) return null;
            }
            lock (fileList[hash].lockFS) {
                fileList[hash].fs.Seek(fileList[hash].BlockSize * (index - 1), SeekOrigin.Begin);
                byte[] data;
                if (index == fileList[hash].BlockCount) {
                    data = new byte[fileList[hash].LastBlockLength];
                    fileList[hash].fs.Read(data, 0, fileList[hash].LastBlockLength);
                } else {
                    data = new byte[fileList[hash].BlockSize];
                    fileList[hash].fs.Read(data, 0, FILE_BLOCK_SIZE);
                }
                return data;
            }
        }

        public void Write(string hash, int index, byte[] data) {
            lock (listOperation) {
                if (!fileList.ContainsKey(hash)) return;
            }
            lock (fileList[hash].lockFS) {
                fileList[hash].fs.Seek(fileList[hash].BlockSize * (index - 1), SeekOrigin.Begin);
                fileList[hash].fs.Write(data, 0, data.Length);
            }
        }

    }

    class FileReaderItem {
        public FileStream fs { get; set; }
        public bool IsWritable { get; set; }
        public int BlockSize { get; set; }
        public int BlockCount { get; set; }
        public int LastBlockLength { get; set; }
        public int UsageCount { get; set; }
        public object lockFS = new object();
    }
}
