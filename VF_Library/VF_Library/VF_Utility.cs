using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

#if VF_LibrarySlot1
namespace VF_Library1
#elif VF_LibrarySlot2
namespace VF_Library2
#else
namespace VF
#endif
{
    public class Utility
    {
        public static string ConvertToUniqueFilename(string _Filename, char _PadCharacter = '_')
        {
            string extension = _Filename.Substring(_Filename.LastIndexOf('.'));
            while (System.IO.File.Exists(_Filename))
                _Filename = _Filename.Replace(extension, "" + _PadCharacter + extension);

            return _Filename;
        }
        public static string UrlEncode(string _String)
        {
            return System.Uri.EscapeDataString(_String).Replace("%20", "+");
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
#if !VF_NoForms
        public static System.Windows.Forms.DialogResult MessageBoxShow(string _Text, string _Caption = "", System.Windows.Forms.MessageBoxButtons _Buttons = System.Windows.Forms.MessageBoxButtons.OK)
        {
            return System.Windows.Forms.MessageBox.Show(new System.Windows.Forms.Form { TopMost = true }, _Text, _Caption, _Buttons);
        }
        public static bool DeleteFile(string _Filename)
        {
            do
            {
                try
                {
                    System.IO.File.Delete(_Filename);
                    return true;
                }
                catch (Exception)
                { }
            }
            while (MessageBoxShow("Could not delete \""
                + _Filename + "\", please close the file and press retry"
                , "", System.Windows.Forms.MessageBoxButtons.RetryCancel) == System.Windows.Forms.DialogResult.Retry);
            return false;
        }
        public static bool DeleteDirectory(string _Directoryname)
        {
            do
            {
                try
                {
                    System.IO.Directory.Delete(_Directoryname, true);
                    return true;
                }
                catch (Exception)
                { }
            }
            while (MessageBoxShow("Could not delete \""
                + _Directoryname + "\", please close any open file inside the directory and press retry"
                , "", System.Windows.Forms.MessageBoxButtons.RetryCancel) == System.Windows.Forms.DialogResult.Retry);
            return false;
        }
#endif
        public static void BackupFile(string _Filename, string _BackupFilename)
        {
            if (System.IO.File.Exists(_Filename) == true)
            {
                AssertFilePath(_BackupFilename);
                System.IO.File.Copy(_Filename, _BackupFilename, true);
            }
        }
#if !VF_NoProtoBuf
        public class ProtobufSerializer
        {
            private static ProtoBuf.Meta.RuntimeTypeModel m_SerializerModel = null;
            public static ProtoBuf.Meta.RuntimeTypeModel Serializer
            {
                get
                {
                    if (m_SerializerModel == null)
                    {
                        m_SerializerModel = ProtoBuf.Meta.TypeModel.Create();
                        m_SerializerModel.UseImplicitZeroDefaults = false;
                    }
                    return m_SerializerModel;
                }
            }
        }
        public static void SaveSerialize<T_Data>(string _Filename, T_Data _Data, bool _BackupOldOne = true)
        {
            if (_BackupOldOne == true)
            {
                BackupFile(_Filename);
            }
            AssertFilePath(_Filename);
            using (var file = File.Create(_Filename))
            {
                ProtobufSerializer.Serializer.Serialize(file, _Data);
            }
        }
        public static void PBSerialize<T_Data>(T_Data _Data, Stream _OutputStream)
        {
            ProtobufSerializer.Serializer.Serialize(_OutputStream, _Data);
        }
        public static bool PBDeserialize<T_Data>(Stream _InputStream, out T_Data _LoadedData)
        {
            _LoadedData = ProtoBuf.Serializer.Deserialize<T_Data>(_InputStream);
            return true;
        }
        public static bool LoadSerialize<T_Data>(string _Filename, out T_Data _LoadedData, int _TimeoutMS = 10000)
        {
            if (System.IO.File.Exists(_Filename) == false)
            {
                _LoadedData = default(T_Data);
                return false;
            }
            var timer = System.Diagnostics.Stopwatch.StartNew();
            do
            {
                try
                {
                    using (var file = File.OpenRead(_Filename))
                    {
                        _LoadedData = ProtoBuf.Serializer.Deserialize<T_Data>(file);
                    }
                    return true;
                }
                catch (Exception)
                { }
            } while (_TimeoutMS > timer.ElapsedMilliseconds);
            _LoadedData = default(T_Data);
            return false;
        }
#endif
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
        public static bool IsValidFilename(string _Filename)
        {
            var invalidChars = System.IO.Path.GetInvalidFileNameChars();
            foreach (var invalidChar in invalidChars)
            {
                if (_Filename.Contains(invalidChar))
                    return false;
            }
            return true;
        }
        public static List<string> GetFilesInDirectory(string _Directory, string _SearchPattern = "")
        {
            List<string> retList = new List<string>();
            string[] files;
            if (_SearchPattern == "")
                files = System.IO.Directory.GetFiles(_Directory);
            else
                files = System.IO.Directory.GetFiles(_Directory, _SearchPattern);
            foreach (var file in files)
            {
                retList.Add(file.Split('\\', '/').Last());
            }
            return retList;
        }
        public static List<string> GetDirectoriesInDirectory(string _Directory, string _SearchPattern = "")
        {
            List<string> retList = new List<string>();
            string[] dirs;
            if (_SearchPattern == "")
                dirs = System.IO.Directory.GetDirectories(_Directory);
            else
                dirs = System.IO.Directory.GetDirectories(_Directory, _SearchPattern);
            foreach (var dir in dirs)
            {
                retList.Add(dir.Split('\\', '/').Last());
            }
            return retList;
        }
#if !VF_NoForms
        public static void SoftThreadSleep(int _SleepDurationMs)
        {
            if (_SleepDurationMs < 50)
            {
                System.Threading.Thread.Sleep(_SleepDurationMs);
                return;
            }
            DateTime stopUtcTime = DateTime.UtcNow.AddMilliseconds(_SleepDurationMs);
            while (DateTime.UtcNow < stopUtcTime)
            {
                System.Threading.Thread.Sleep(50);
                System.Windows.Forms.Application.DoEvents();
            }
        }
#endif

        public static bool ContainsInvalidFilenameChars(string _Filename)
        {
            foreach (var invalidChar in System.IO.Path.GetInvalidFileNameChars())
            {
                if (_Filename.Contains(invalidChar) == true)
                    return true;
            }
            return false;
        }

        public static bool ContainsFilesAndDirectories(string _Directory, string[] _FileDirNames)
        {
            foreach (var fileDir in _FileDirNames)
            {
                string currPath = _Directory + "\\" + fileDir;
                if (System.IO.File.Exists(currPath) == false)
                {
                    if (System.IO.Directory.Exists(currPath) == false)
                        return false;
                }
            }
            return true;
        }
    }
}
