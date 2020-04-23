using System;
using System.Collections.Generic;
using System.IO;
using SerializeHandler;

namespace ServerHttp
{

    public struct UserInfo
    {
        public string Name;
        public string Password;
        public int ID;
    }
    class UsersInfoHandler
    {

        ISerialize serializer;
        List<UserInfo> usersInfo;
        int userCounter;

        public UsersInfoHandler(ISerialize serializer)
        {
            this.serializer = serializer;
            userCounter = 0;
            usersInfo = new List<UserInfo>();
        }

        public bool IsExistsUser(string name)
        {
            foreach(var userInfo in usersInfo)
            {
                if (userInfo.Name.Equals(name))
                    return true;
            }
            return false;
        }

        public bool AddUser(UserInfo userInfo)
        {
            if(!IsExistsUser(userInfo.Name))
            {
                userInfo.ID = userCounter++;
                usersInfo.Add(userInfo);
                return true;
            }
            return false;
        }

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
                usersInfo = (List<UserInfo>)serializer.Deserialize(fin, typeof(List<UserInfo>));
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
                byte[] buffer = serializer.Serialize(usersInfo, fileName, typeof(List<UserInfo>));
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
    }
}
