using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ProtoBuf;

namespace VF_RealmPlayersDatabase
{
    public class Utility
    {
        public static string DefaultServerLocation = "R:\\";
        public static string DefaultDebugLocation = "D:\\";

        public static string ConvertToUniqueFilename(string _Filename, char _PadCharacter = '_')
        {
            string extension = _Filename.Substring(_Filename.LastIndexOf('.'));
            while (System.IO.File.Exists(_Filename))
                _Filename = _Filename.Replace(extension, "" + _PadCharacter + extension);

            return _Filename;
        }
        public static Single ParseSingle(string _String)
        {
            return Single.Parse(_String.Replace('.', ','));
        }
        public static double ParseDouble(string _String)
        {
            return double.Parse(_String.Replace('.', ','));
        }
        public static void BackupFile(string _Filename)
        {
            BackupFile(_Filename, BackupMode.Backup_Daily);
        }
        public enum BackupMode
        {
            Backup_Always_TimeInFilename,
            Backup_Daily,
        }
        public static void BackupFile(string _Filename, BackupMode _BackupMode)
        {
            if (System.IO.File.Exists(_Filename) == true)
            {
                string fileFolder = System.IO.Path.GetDirectoryName(_Filename);
                if (fileFolder != "")
                    fileFolder += "\\";
                string backupbackupID = DateTime.Now.ToString("yyyy_MM_dd");
                string backupFileName = "";
                if (_BackupMode == BackupMode.Backup_Always_TimeInFilename)
                {
                    var fileName = System.IO.Path.GetFileNameWithoutExtension(_Filename) + DateTime.Now.ToString(" HH_mm_ss_fff") + Path.GetExtension(_Filename);
                    backupFileName = fileFolder + "Backup\\" + backupbackupID + "\\" + fileName;
                    backupFileName = ConvertToUniqueFilename(backupFileName, '_');
                }
                else if (_BackupMode == BackupMode.Backup_Daily)
                {
                    backupFileName = fileFolder + "Backup\\" + backupbackupID + "\\" + System.IO.Path.GetFileNameWithoutExtension(_Filename) + Path.GetExtension(_Filename);
                }
                else
                    throw new Exception("Unexpected!");
                BackupFile(_Filename, backupFileName);
            }
        }
        public static void BackupFile(string _Filename, string _BackupFilename)
        {
            if (System.IO.File.Exists(_Filename) == true)
            {
                AssertFilePath(_BackupFilename);
                System.IO.File.Copy(_Filename, _BackupFilename, true);
            }
        }
        public static void SaveSerialize<T_Data>(string _Filename, T_Data _Data, bool _BackupOldOne = true)
        {
            //AssertFilePath(_Filename);
            //BinaryFormatter bFormatter = new BinaryFormatter();
            //bFormatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            //var fileStream = System.IO.File.Open(_Filename, FileMode.Create);
            //bFormatter.Serialize(fileStream, _Data);
            //fileStream.Close();

            if (_BackupOldOne == true)
            {
                BackupFile(_Filename);// + ".ProtoBuf"
            }
            AssertFilePath(_Filename);
            using (var file = File.Create(_Filename))// + ".ProtoBuf"
            {
                Serializer.Serialize(file, _Data);
            }
        }
        public static bool LoadSerialize<T_Data>(string _Filename, out T_Data _LoadedData)
        {
            using (var file = File.OpenRead(_Filename))// + ".ProtoBuf"
            {
                _LoadedData = Serializer.Deserialize<T_Data>(file);
            }
            //if (System.IO.File.Exists(_Filename + ".ProtoBuf") == false)
            //{
            //    BinaryFormatter bFormatter = new BinaryFormatter();
            //    bFormatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            //    var fileStream = System.IO.File.Open(_Filename, FileMode.Open, FileAccess.Read);
            //    _LoadedData = (T_Data)bFormatter.Deserialize(fileStream);
            //    fileStream.Close();

            //    try
            //    {
            //        using (var file = File.Create(_Filename + ".ProtoBuf"))
            //        {
            //            Serializer.Serialize(file, _LoadedData);
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        try
            //        {
            //            File.Delete(_Filename + ".ProtoBuf");
            //        }
            //        catch (Exception)
            //        {}
            //    }
            //}
            //else
            //{
            //    try
            //    {
            //        using (var file = File.OpenRead(_Filename + ".ProtoBuf"))
            //        {
            //            _LoadedData = Serializer.Deserialize<T_Data>(file);
            //        }
            //    }
            //    catch (Exception)
            //    {
            //        BinaryFormatter bFormatter = new BinaryFormatter();
            //        bFormatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            //        var fileStream = System.IO.File.Open(_Filename, FileMode.Open, FileAccess.Read);
            //        _LoadedData = (T_Data)bFormatter.Deserialize(fileStream);
            //        fileStream.Close();
            //    }
            //}

            return true;
        }
        public static void AssertDirectory(string _Folder)
        {
            if (System.IO.Directory.Exists(_Folder) == false && _Folder != "")
                System.IO.Directory.CreateDirectory(_Folder);
        }
        public static void AssertFilePath(string _Filename)
        {
            string folderName = System.IO.Path.GetDirectoryName(_Filename);
            AssertDirectory(folderName);
        }
    }
}
