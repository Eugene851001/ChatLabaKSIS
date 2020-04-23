using SerializeHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ServerHttp
{

    public struct FileInfo
    {
        public int FileID;
        public string UserFileName;
        public string StoredFileName;
    }

    class FilesHandler
    {
        public const int emptyID = -1;

        ISerialize serializer;
        List<FileInfo> filesInfo;
        string savePath;
        int currentFileID;

        public FilesHandler(ISerialize serializer, string savePath)
        {
            currentFileID = 0;
            this.savePath = savePath;
            filesInfo = new List<FileInfo>();
            this.serializer = serializer;
        }

        #region FileTable
        public bool IsExistsFile(int fileID)
        {
            foreach (var fileInfo in filesInfo)
            {
                if (fileInfo.FileID == fileID)
                    return true;
            }
            return false;
        }

        public bool IsExistsFile(string userFileName)
        {
            foreach(var fileInfo in filesInfo)
            {
                if (fileInfo.UserFileName.Equals(userFileName))
                    return true;
            }
            return false;
        }

        public string GetUserFileName(int fileID)
        {
            string userFileName = null;
            if(IsExistsFile(fileID))
            {
                foreach(var fileInfo in filesInfo)
                {
                    if (fileID == fileInfo.FileID)
                        userFileName = fileInfo.UserFileName;
                }
            }
            else
            {
                throw new FileNotFoundException();
            }

            return userFileName;
        }

        public string GetStorageFileName(int fileID)
        {
            string storedFileName = null;
            if (IsExistsFile(fileID))
            {
                foreach (var fileInfo in filesInfo)
                {
                    if (fileID == fileInfo.FileID)
                        storedFileName = fileInfo.StoredFileName;
                }
            }
            else
            {
                throw new FileNotFoundException();
            }

            return storedFileName;
        }

        public int GetFileID(string userFileName)
        {
            int id = 0;
            if (!IsExistsFile(userFileName))
                throw new FileNotFoundException();
            foreach(var fileInfo in filesInfo)
            {
                if (fileInfo.UserFileName.Equals(userFileName))
                    id = fileInfo.FileID;
            }
            return id;
        }

        private void deleteFileFromList(int fileID)
        {
            if (!IsExistsFile(fileID))
                throw new FileNotFoundException();
            FileInfo requiredFileInfo = new FileInfo();
            foreach(var fileInfo in filesInfo)
            {
                if (fileInfo.FileID == fileID)
                    requiredFileInfo = fileInfo;
            }
            filesInfo.Remove(requiredFileInfo);
        }

        #endregion

        void createFile(string fileName, byte[] content)
        {
            FileStream fout = new FileStream(savePath + "/" + fileName, FileMode.Create);
            try
            {
                fout.Write(content, 0, content.Length);
            }
            catch
            {

            }
            finally
            {
                fout.Close();
            }
        }

        public int AddFile(string name, byte[] content)
        {
            int id = emptyID;
            if(!IsExistsFile(name))
            {
                MD5 md5hash = MD5.Create();
                byte[] bufferID  = md5hash.ComputeHash(content);
                string fileHash = "";

                for (int i = 0; i < bufferID.Length; i++)
                {
                    fileHash += bufferID[i].ToString("x2");
                }
                createFile(fileHash, content);
                filesInfo.Add(new FileInfo() { UserFileName = name, FileID = currentFileID++,  
                    StoredFileName = fileHash });
                id = GetFileID(name);

            }
            return id;
        }

        public void DeleteFile(int fileID)
        {
            if(IsExistsFile(fileID))
            {
                File.Delete(savePath + "/" + GetStorageFileName(fileID));
                deleteFileFromList(fileID);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        public long GetFileSize(int fileID)
        {
            if (!IsExistsFile(fileID))
                throw new FileNotFoundException();
            string storedFileName = GetStorageFileName(fileID);
            long result;
            FileStream fin;
            try
            {
                fin = new FileStream(savePath + "/" + storedFileName, FileMode.Open);
                result = fin.Length;
                fin.Close();
            }
            catch
            {
                result = -1;
            }
            return result;
        }

        public byte[] GetFileContent(int fileID)
        {
            byte[] content = null;
            if (!IsExistsFile(fileID))
                throw new FileNotFoundException();
            string storedFileName = GetStorageFileName(fileID);
            Console.WriteLine("Stored file name: " + storedFileName);
            FileStream fin= null;
            try
            {
                Console.WriteLine(savePath + "/" + storedFileName);
                fin = new FileStream(savePath + "/" + storedFileName, FileMode.Open);
                Console.WriteLine("Opened");
            }
            catch 
            {
                Console.WriteLine("Error while openinng");
                return null;
            }   
            try 
            {
                content = new byte[fin.Length];
                fin.Read(content, 0, (int)fin.Length);
                Console.WriteLine("Succesfully reading");
            }
            catch
            {
                Console.WriteLine("Error while reading");
                content = null;
            }
            finally
            {
                fin.Close();
            }
            return content;
        }

        #region SavingAndLoadingFilesInfo
        public bool LoadInfo(string fileName)
        {
            bool result = true;
            FileStream fin;
            try
            {
                fin = new FileStream(fileName, FileMode.Open);
            }
            catch
            {
                return false;
            }
            try
            {
                filesInfo = (List<FileInfo>)serializer.Deserialize(fin, typeof(List<FileInfo>));
                currentFileID = filesInfo[filesInfo.Count - 1].FileID;
                currentFileID++;
            }
            catch
            {
                result = false;
            }
            finally
            {
                fin.Close();
            }
            return result;
        }

        public bool SaveInfo(string fileName)
        {
            bool result = true;
            FileStream fout;
            try
            {
                fout = new FileStream(fileName, FileMode.Create);
            }
            catch
            {
                return false;
            }
            try
            {
                byte[] buffer = serializer.Serialize(filesInfo, fileName, typeof(List<FileInfo>));
                fout.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                result = false;
            }
            finally
            {
                fout.Close();
            }
            return result;
        }
        #endregion

    }
}
