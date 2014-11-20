using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace VF_WoWLauncher
{
    class wyUpdateControl
    {
        public static bool CheckIsUpdateAvailable(out string _UpdateTextData)
        {
            _UpdateTextData = "";
            if (System.IO.File.Exists("wyUpdate.exe") == false)
                return false;
            AssertVersionWycFile();

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "wyUpdate.exe";
            startInfo.Arguments = "/quickcheck /justcheck /noerr /outputinfo";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            var wyUpdateProcess = Process.Start(startInfo);
            
            string data = "";
            while (wyUpdateProcess.HasExited == false)
            {
                Utility.SoftThreadSleep(100);
                data += wyUpdateProcess.StandardOutput.ReadToEnd();
            }
            
            data += wyUpdateProcess.StandardOutput.ReadToEnd();
            int exitCode = wyUpdateProcess.ExitCode;

            _UpdateTextData = data;
            
            if (exitCode == 2)
                return true;
                //Return value 0 == no update
                //Return value 1 == Error
                //Return value 2 == Update
            return false;
        }
        public static void UpdateNow()
        {
            if (System.IO.File.Exists(StaticValues.LauncherToolsDirectory + "RunwyUpdateAndLauncher.bat") == false
                || System.IO.File.ReadAllText(StaticValues.LauncherToolsDirectory + "RunwyUpdateAndLauncher.bat") != StaticValues.RunwyUpdateAndLauncherFileData)
            {
                Utility.AssertDirectory(StaticValues.LauncherToolsDirectory);
                System.IO.File.WriteAllText(StaticValues.LauncherToolsDirectory + "RunwyUpdateAndLauncher.bat", StaticValues.RunwyUpdateAndLauncherFileData);
            }

            //startInfo.FileName = Settings.WowDirectory + "WoW.exe";
            //startInfo.WorkingDirectory = Settings.WowDirectory;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = StaticValues.LauncherToolsDirectory + "RunwyUpdateAndLauncher.bat";
            //startInfo.FileName = Settings.WowDirectory + "22VF_RealmPlayersUploader 1.5\\RunWoWAndUploaderNoCMDWindow.vbs";
            //startInfo.WorkingDirectory = Settings.WowDirectory + "22VF_RealmPlayersUploader 1.5\\";
            startInfo.Arguments = "\"" + StaticValues.LauncherWorkDirectory + "\"";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = false;
            startInfo.CreateNoWindow = true;

            //ProcessStartInfo startInfo = new ProcessStartInfo();
            //RunwyUpdateAndLauncher
            //startInfo.FileName = "wyUpdate.exe";
            //startInfo.Arguments = "/skipinfo";
            var wyUpdateProcess = Process.Start(startInfo);
            System.Windows.Forms.Application.Exit();
        }
        public static void AssertVersionWycFile()
        {
            if (Settings.Instance.UpdateToBeta == true && System.IO.File.Exists("betaclient.wyc") == false)
            {
                Settings.Instance.UpdateToBeta = false;
                CreateVersionWycFile();
            }
            else if (Settings.Instance.UpdateToBeta == false && System.IO.File.Exists("betaclient.wyc") == true)
            {
                Settings.Instance.UpdateToBeta = true;
                CreateVersionWycFile();
            }

            if (System.IO.File.Exists("client.wyc") == false
            || IsVersionWycFileValid() == false)
            {
                CreateVersionWycFile();
            }
        }
        public static bool IsVersionWycFileValid()
        {
            using (var zipFile = new ICSharpCode.SharpZipLib.Zip.ZipFile("client.wyc"))
            {
                var zipEntry = zipFile.GetEntry("iuclient.iuc");
                if (zipEntry == null)
                    return false;
                var zipInputStream = zipFile.GetInputStream(zipEntry);
                int firstByte = zipInputStream.ReadByte();//Måste läsa första byte innan Length på streamen stämmer. Konstigt
                byte[] iucClientFile = new byte[zipInputStream.Length];
                iucClientFile[0] = (byte)firstByte;
                zipInputStream.Read(iucClientFile, 1, iucClientFile.Length - 1);
                var fr = new wyUpdate.FileReader(new System.IO.MemoryStream(iucClientFile));
                if (fr.IsHeaderValid("IUCDFV2") == false)
                    return false;

                byte bType = (byte)fr.ReadByte();
                while (!fr.ReachedEndByte(bType, 0xFF))
                {
                    switch (bType)
                    {
                        case 0x03:
                            return (fr.ReadDeprecatedString() == StaticValues.LauncherVersion);
                        default:
                            fr.SkipField(bType);
                            break;

                    }
                    bType = (byte)fr.ReadByte();
                }
                return false;
            }
        }
        public static void CreateVersionWycFile()
        {
            {
                var fileStream = System.IO.File.Create("client.wyc");
                var zipOutputStream = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(fileStream);
                {
                    var iucMemoryStream = new System.IO.MemoryStream();
                    {
                        //var fileStream = System.IO.File.Create("iuclient.iuc");
                        var fw = new wyUpdate.FileWriter(iucMemoryStream);
                        fw.WriteHeader("IUCDFV2");
                        fw.WriteDeprecatedString(0x01, "RealmPlayers.com");//Company Name
                        fw.WriteDeprecatedString( 0x02, "VF_WoWLauncher");//Product Name
                        fw.WriteDeprecatedString(0x03, StaticValues.LauncherVersion); //Installed Version
                        //fw.WriteDeprecatedString(0x03, "0.9"); //Installed Version
                        fw.WriteString(0x0A, "TestAnything"); //GUID of the product
                        if (Settings.Instance.UpdateToBeta == true)
                            fw.WriteDeprecatedString(0x04, "ftp://WowLauncherUpdater@realmplayers.com:5511/Updates/VF_WowLauncher/BetaUpdate.wys"); //Server File Site(s)
                        else
                            fw.WriteDeprecatedString(0x04, "ftp://WowLauncherUpdater@realmplayers.com:5511/Updates/VF_WowLauncher/Update.wys"); //Server File Site(s)
                        fw.WriteDeprecatedString(0x09, "ftp://WowLauncherUpdater@realmplayers.com:5511/Updates/wyUpdate/Update.wys"); //wyUpdate Server Site(s) (can add any number of theese)
                        fw.WriteDeprecatedString(0x11, "Left"); //Header Image Alignment Either "Left", "Right", "Fill"
                        fw.WriteInt(0x12, 4); //Header text indent
                        fw.WriteDeprecatedString(0x13, "Black"); //Header text color (Black, White, Red etc etc etc)
                        fw.WriteDeprecatedString(0x14, "HeaderImage.png"); //Header filename
                        fw.WriteDeprecatedString(0x15, "LeftImage.png"); //Side image filename
                        fw.WriteDeprecatedString(0x18, "en-US"); //Language Culture (e.g. en-US or fr-FR)
                        fw.WriteBool(0x17, false); //Hide header divider? (default = false)
                        fw.WriteBool(0x19, false); //Close wyUpdate on successful update
                        fw.WriteString(0x1A, "VF_WoWLauncher Updater"); //Custom wyUpdate title bar
                        //WriteFiles.WriteString(fileStream, 0x1B, "DilaPublicSignKey"); //Public sign key -- DENNA MÅSTE VARA KORREKT ANNARS FAILAR SHA1!!!!!
                        iucMemoryStream.WriteByte(0xFF);
                        //fileStream.Close();
                    }
                    var byteArray = iucMemoryStream.ToArray();
                    var newEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry("iuclient.iuc");
                    newEntry.Size = byteArray.Length;
                    zipOutputStream.PutNextEntry(newEntry);
                    zipOutputStream.Write(byteArray, 0, byteArray.Length);
                    zipOutputStream.CloseEntry();
                }
                {
                    var newEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry("HeaderImage.png");
                    newEntry.Size = Properties.Resources.HeaderImagepng.Length;
                    zipOutputStream.PutNextEntry(newEntry);
                    zipOutputStream.Write(Properties.Resources.HeaderImagepng, 0, Properties.Resources.HeaderImagepng.Length);
                    zipOutputStream.CloseEntry();
                }
                {
                    var newEntry = new ICSharpCode.SharpZipLib.Zip.ZipEntry("LeftImage.png");
                    newEntry.Size = Properties.Resources.LeftImagepng.Length;
                    zipOutputStream.PutNextEntry(newEntry);
                    zipOutputStream.Write(Properties.Resources.LeftImagepng, 0, Properties.Resources.LeftImagepng.Length);
                    zipOutputStream.CloseEntry();
                }
                zipOutputStream.Close();
                fileStream.Close();
                //var newZipFile = ICSharpCode.SharpZipLib.Zip.ZipFile.Create("client.wyc");
                //newZipFile.BeginUpdate();
                //newZipFile.Add("iuclient.iuc");
                //newZipFile.Add(new ICSharpCode.SharpZipLib.Zip.ZipEntry(
                //newZipFile.Add(Properties.Resources.HeaderImagepng, "HeaderImage.png");
                //newZipFile.Add("LeftImage.png");
                //newZipFile.CommitUpdate();
                //newZipFile.Close();
            }
        }
    }
}
