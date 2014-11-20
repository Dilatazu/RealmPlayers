using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_WoWLauncher
{
    class ApplicationUpdater
    {
        public static void CreateUpdateFile()
        {
            /*
            {
                var fileStream = System.IO.File.Create("iuclient.iuc");
                var fileWriter = new wyUpdate.FileWriter(fileStream);
                fileWriter.WriteHeader("IUCDFV2");
                fileWriter.WriteDeprecatedString(0x01, "Dilatazu(CompanyName)");//Company Name
                fileWriter.WriteDeprecatedString( 0x02, "RealmPlayers(Product)");//Product Name
                fileWriter.WriteDeprecatedString( 0x03, "133"); //Installed Version
                fileWriter.WriteString( 0x0A, "TestAnything"); //GUID of the product
                fileWriter.WriteDeprecatedString( 0x04, "file:D:\\QinarwTFS\\CppProjects\\VF_WoWLauncher\\VF_WoWLauncher\\bin\\Debug\\iupdate.wys"); //Server File Site(s)
                fileWriter.WriteDeprecatedString( 0x09, "file:D:\\QinarwTFS\\CppProjects\\VF_WoWLauncher\\VF_WoWLauncher\\bin\\Debug\\iupdateWY.wys"); //wyUpdate Server Site(s) (can add any number of theese)
                fileWriter.WriteDeprecatedString( 0x11, "Left"); //Header Image Alignment Either "Left", "Right", "Fill"
                fileWriter.WriteInt( 0x12, 4); //Header text indent
                fileWriter.WriteDeprecatedString( 0x13, "Red"); //Header text color (Black, White, Red etc etc etc)
                fileWriter.WriteDeprecatedString( 0x14, "dilatazusignature400x100.jpg"); //Header filename
                fileWriter.WriteDeprecatedString( 0x15, "PaypalPay3.png"); //Side image filename
                fileWriter.WriteDeprecatedString( 0x18, "en-US"); //Language Culture (e.g. en-US or fr-FR)
                fileWriter.WriteBool( 0x17, false); //Hide header divider? (default = false)
                fileWriter.WriteBool( 0x19, false); //Close wyUpdate on successful update
                fileWriter.WriteString( 0x1A, "DilaCustomTitleBar"); //Custom wyUpdate title bar
                //WriteFiles.WriteString(fileStream, 0x1B, "DilaPublicSignKey"); //Public sign key -- DENNA MÅSTE VARA KORREKT ANNARS FAILAR SHA1!!!!!
                fileStream.Close();
            }
            //{
            //    var fileStream = System.IO.File.Create("iuclient2.iuc");
            //    WriteFiles.WriteHeader(fileStream, "IUCDFV2");
            //    WriteFiles.WriteDeprecatedString(fileStream, 0x01, "Dilatazu(CompanyName)");//Company Name
            //    WriteFiles.WriteDeprecatedString(fileStream, 0x02, "RealmPlayers(Product)");//Product Name
            //    WriteFiles.WriteDeprecatedString(fileStream, 0x03, "134"); //Installed Version
            //    WriteFiles.WriteString(fileStream, 0x0A, "TestAnything"); //GUID of the product
            //    WriteFiles.WriteDeprecatedString(fileStream, 0x04, "file:D:\\QinarwTFS\\CppProjects\\VF_WoWLauncher\\VF_WoWLauncher\\bin\\Debug\\iupdate.wys"); //Server File Site(s)
            //    WriteFiles.WriteDeprecatedString(fileStream, 0x09, "file:D:\\QinarwTFS\\CppProjects\\VF_WoWLauncher\\VF_WoWLauncher\\bin\\Debug\\iupdateWY.wys"); //wyUpdate Server Site(s) (can add any number of theese)
            //    WriteFiles.WriteDeprecatedString(fileStream, 0x11, "Left"); //Header Image Alignment Either "Left", "Right", "Fill"
            //    WriteFiles.WriteInt(fileStream, 0x12, 4); //Header text indent
            //    WriteFiles.WriteDeprecatedString(fileStream, 0x13, "Red"); //Header text color (Black, White, Red etc etc etc)
            //    WriteFiles.WriteDeprecatedString(fileStream, 0x14, "PaypalPay3.png"); //Header filename
            //    WriteFiles.WriteDeprecatedString(fileStream, 0x15, "PaypalPay3.png"); //Side image filename
            //    WriteFiles.WriteDeprecatedString(fileStream, 0x18, "en-US"); //Language Culture (e.g. en-US or fr-FR)
            //    WriteFiles.WriteBool(fileStream, 0x17, false); //Hide header divider? (default = false)
            //    WriteFiles.WriteBool(fileStream, 0x19, false); //Close wyUpdate on successful update
            //    WriteFiles.WriteString(fileStream, 0x1A, "DilaCustomTitleBar"); //Custom wyUpdate title bar
            //    WriteFiles.WriteString(fileStream, 0x1B, "DilaPublicSignKey"); //Public sign key
            //    fileStream.Close();
            //}
            {
                var fileStream = System.IO.File.Create("updtdetails.udt");
                var fileWriter = new wyUpdate.FileWriter(fileStream);
                fileWriter.WriteHeader( "IUUDFV2");
                fileWriter.WriteInt( 0x20, 0);//Number of registry changes (precedes RegChange list)
                //List of RegChanges, (See RegChange)
                //WriteFiles.WriteDeprecatedString(fileStream, 0x30, "");//Relative path to Desktop shortcuts that must exist to install new Desktop shortcuts
                //WriteFiles.WriteDeprecatedString(fileStream, 0x31, "");//Relative path to Start Menu shortcuts that must exist to install new Start Menu shortcuts
                //WriteFiles.WriteString(fileStream, 0x32, "");//Service to stop before update
                //WriteFiles.WriteString(fileStream, 0x33, "");//Service to start after update
                //WriteFiles.WriteInt(fileStream, 0x34, 0);//Number of arguments to use with the last “start” service
                //WriteFiles.WriteString(fileStream, 0x35, "");//Argument to use with the last “start” service
                // 0x8D //Start of a Shortcut Info
                //List of ShortcutInfos, (See ShortcutInfo)
                fileWriter.WriteInt( 0x21, 0);//Number of file infos (precedes file info list)
                //fileStream.WriteByte(0x8B);//Beginning of File information
                //WriteFiles.WriteDeprecatedString(fileStream, 0x40, "PaypalPay_NotClicked_Smaller.png");//Relative file path
                //WriteFiles.WriteBool(fileStream, 0x41, false);//Execute file?
                //WriteFiles.WriteBool(fileStream, 0x42, false);//Execute before updating
                //WriteFiles.WriteBool(fileStream, 0x45, false);//Wait for execution to finish before continuing,
                ////fileStream.WriteByte(0x8F);//Rollback on failure
                //WriteFiles.WriteInt(fileStream, 0x4D, 0);//Return code exception (i.e. if this code is returned, don’t rollback)
                //WriteFiles.WriteDeprecatedString(fileStream, 0x43, "");//Command line arguments
                //WriteFiles.WriteBool(fileStream, 0x44, false);//Is the file a .NET assembly?
                //WriteFiles.WriteBool(fileStream, 0x46, false);//Delete the file?
                //WriteFiles.WriteDeprecatedString(fileStream, 0x47, "");//Delta patch relative path
                //WriteFiles.WriteLong(fileStream, 0x48, 0);//New file's Adler32 checksum (only used when delta patching files)
                //WriteFiles.WriteInt(fileStream, 0x49, 0);//CPUVersion { 0 = AnyCPU, 1 = x86, 2 = x64 } (for NGENing & GAC installing assemblies)
                //WriteFiles.WriteInt(fileStream, 0x4A, 0);//ProcessWindowStyle { 0 = Normal, 1 = Hidden, 2 = Min, 3 = Max }
                //WriteFiles.WriteInt(fileStream, 0x4E, 0);//ElevationType { SameAswyUpdate = 0, Elevated = 1, NotElevated = 2 }
                //WriteFiles.WriteInt(fileStream, 0x4B, 0);//Framework version { -1 = Unknown, 0 = .NET 2.0, 1 = .NET 4.0 }
                //WriteFiles.WriteInt(fileStream, 0x4C, 0);//Register COM dll {0 = None, 1= .NET Assembly, 2=Register, 4=Unregister}
                //fileStream.WriteByte(0x9B);//End of File information
                //fileStream.WriteByte(0x8B);//Beginning of File information
                //WriteFiles.WriteDeprecatedString(fileStream, 0x40, "iuclient.iuc");//Relative file path
                //WriteFiles.WriteBool(fileStream, 0x41, false);//Execute file?
                //WriteFiles.WriteBool(fileStream, 0x42, false);//Execute before updating
                //WriteFiles.WriteBool(fileStream, 0x45, false);//Wait for execution to finish before continuing,
                ////fileStream.WriteByte(0x8F);//Rollback on failure
                //WriteFiles.WriteInt(fileStream, 0x4D, 0);//Return code exception (i.e. if this code is returned, don’t rollback)
                //WriteFiles.WriteDeprecatedString(fileStream, 0x43, "");//Command line arguments
                //WriteFiles.WriteBool(fileStream, 0x44, false);//Is the file a .NET assembly?
                //WriteFiles.WriteBool(fileStream, 0x46, false);//Delete the file?
                //WriteFiles.WriteDeprecatedString(fileStream, 0x47, "");//Delta patch relative path
                //WriteFiles.WriteLong(fileStream, 0x48, 0);//New file's Adler32 checksum (only used when delta patching files)
                //WriteFiles.WriteInt(fileStream, 0x49, 0);//CPUVersion { 0 = AnyCPU, 1 = x86, 2 = x64 } (for NGENing & GAC installing assemblies)
                //WriteFiles.WriteInt(fileStream, 0x4A, 0);//ProcessWindowStyle { 0 = Normal, 1 = Hidden, 2 = Min, 3 = Max }
                //WriteFiles.WriteInt(fileStream, 0x4E, 0);//ElevationType { SameAswyUpdate = 0, Elevated = 1, NotElevated = 2 }
                //WriteFiles.WriteInt(fileStream, 0x4B, 0);//Framework version { -1 = Unknown, 0 = .NET 2.0, 1 = .NET 4.0 }
                //WriteFiles.WriteInt(fileStream, 0x4C, 0);//Register COM dll {0 = None, 1= .NET Assembly, 2=Register, 4=Unregister}
                //fileStream.WriteByte(0x9B);//End of File information
                //fileStream.WriteByte(0x8B);//Beginning of File information
                //WriteFiles.WriteDeprecatedString(fileStream, 0x40, "wyUpdate.exe");//Relative file path
                //WriteFiles.WriteBool(fileStream, 0x41, false);//Execute file?
                //WriteFiles.WriteBool(fileStream, 0x42, false);//Execute before updating
                //WriteFiles.WriteBool(fileStream, 0x45, false);//Wait for execution to finish before continuing,
                ////fileStream.WriteByte(0x8F);//Rollback on failure
                //WriteFiles.WriteInt(fileStream, 0x4D, 0);//Return code exception (i.e. if this code is returned, don’t rollback)
                //WriteFiles.WriteDeprecatedString(fileStream, 0x43, "");//Command line arguments
                //WriteFiles.WriteBool(fileStream, 0x44, true);//Is the file a .NET assembly?
                //WriteFiles.WriteBool(fileStream, 0x46, false);//Delete the file?
                //WriteFiles.WriteDeprecatedString(fileStream, 0x47, "");//Delta patch relative path
                //WriteFiles.WriteLong(fileStream, 0x48, 0);//New file's Adler32 checksum (only used when delta patching files)
                //WriteFiles.WriteInt(fileStream, 0x49, 0);//CPUVersion { 0 = AnyCPU, 1 = x86, 2 = x64 } (for NGENing & GAC installing assemblies)
                //WriteFiles.WriteInt(fileStream, 0x4A, 0);//ProcessWindowStyle { 0 = Normal, 1 = Hidden, 2 = Min, 3 = Max }
                //WriteFiles.WriteInt(fileStream, 0x4E, 0);//ElevationType { SameAswyUpdate = 0, Elevated = 1, NotElevated = 2 }
                //WriteFiles.WriteInt(fileStream, 0x4B, 0);//Framework version { -1 = Unknown, 0 = .NET 2.0, 1 = .NET 4.0 }
                //WriteFiles.WriteInt(fileStream, 0x4C, 0);//Register COM dll {0 = None, 1= .NET Assembly, 2=Register, 4=Unregister}
                //fileStream.WriteByte(0x9B);//End of File information
                //WriteFiles.WriteDeprecatedString(fileStream, 0x60, "");//Folder to delete (providing it's empty on the user's machine)
                fileWriter.WriteByte(0xFF);//EOF byte
                fileStream.Close();
            }
            {
                var newZipFile = ICSharpCode.SharpZipLib.Zip.ZipFile.Create("PaypalPay_NotClicked_Smaller.wyu");
                newZipFile.BeginUpdate();
                newZipFile.Add("updtdetails.udt");
                newZipFile.AddDirectory("base");
                newZipFile.Add("PaypalPay_NotClicked_Smaller.png", "base/PaypalPay_NotClicked_Smaller.png");
                //newZipFile.Add("iuclient2.iuc", "base/iuclient.iuc");
                newZipFile.CommitUpdate();
                newZipFile.Close();
            }
            {
                var newZipFile = ICSharpCode.SharpZipLib.Zip.ZipFile.Create("client.wyc");
                newZipFile.BeginUpdate();
                newZipFile.Add("iuclient.iuc");
                newZipFile.Add("PaypalPay3.png");
                newZipFile.Add("dilatazusignature400x100.jpg");
                newZipFile.CommitUpdate();
                newZipFile.Close();
            }
            long fileSize = 0;
            long fileAdlerSum = 0;
            byte[] fileSHA1 = new byte[] { };
            {
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes("PaypalPay_NotClicked_Smaller.wyu");
                    fileSize = fileBytes.Length;

                    Adler32 ad = new Adler32();
                    ad.Reset();
                    ad.Update(fileBytes);
                    fileAdlerSum = ad.Value;
                }
                {
                    var fileStream = System.IO.File.OpenRead("PaypalPay_NotClicked_Smaller.wyu");
                    using (System.Security.Cryptography.SHA1CryptoServiceProvider sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider())
                    {
                        fileSHA1 = sha1.ComputeHash(fileStream);
                    }
                    fileStream.Close();
                }
            }
            {
                var fileStream = System.IO.File.Create("iupdate.wys");
                var fileWriter = new wyUpdate.FileWriter(fileStream);
                fileWriter.WriteHeader( "IUSDFV2");
                fileWriter.WriteDeprecatedString( 0x01, "136");//Current Latest Version
                fileWriter.WriteDeprecatedString( 0x02, "file:D:\\QinarwTFS\\CppProjects\\VF_WoWLauncher\\VF_WoWLauncher\\bin\\Debug\\PaypalPay_NotClicked_Smaller.wyu");//Server file http/ftp site (for updating mirrors locally)
                fileWriter.WriteDeprecatedString( 0x07, "2.6.18.0");//Minimum client version needed to install the update
                //WriteFiles.WriteInt(fileStream, 0x0F, 0x00);//Dummy variable with a length int how many bytes that should be jumped before skip all version is reached. ----- written to skip all versions except the catch-all version
                //CAN BE REPEATED//CAN BE REPEATED//CAN BE REPEATED//CAN BE REPEATED
                fileWriter.WriteDeprecatedString( 0x0B, "133");//Version to update
                fileWriter.WriteDeprecatedString( 0x03, "file:D:\\QinarwTFS\\CppProjects\\VF_WoWLauncher\\VF_WoWLauncher\\bin\\Debug\\PaypalPay_NotClicked_Smaller.wyu"); //Update file http/ftp site
                //0x80 Changes are in RTF format if this byte is present
                fileWriter.WriteDeprecatedString( 0x04, "This is the latest changes yo for program"); //Latest changes
                fileWriter.WriteLong( 0x09, fileSize); //Update's filesize
                fileWriter.WriteLong( 0x08, fileAdlerSum); //Update file's Adler32 checksum
                fileWriter.WriteByteArray( 0x14, fileSHA1); //Signed SHA1 hash of the file
                fileWriter.WriteInt( 0x0A, 1); //Installing to folders (flags: 1 = base dir, 2 = system 32 dir (x86), 4 = comm desktop, 8 = com star menu, 16 = com app data, 32 = system 32 dir (x64), 64 = Windows root drive, CommonFilesx86 = 128, CommonFilesx64 = 256, 512 = ServiceOrCOMRegistration, 1024 = NonCurrentUserReg )
                //CAN BE REPEATED//CAN BE REPEATED//CAN BE REPEATED//CAN BE REPEATED
                fileWriter.WriteDeprecatedString( 0x20, "program can not be updated yo");//Link text to show when the installed version of your software can't be updated
                fileWriter.WriteDeprecatedString( 0x21, "http://realmplayers.com/");//Link URL to show when the installed version of your software can't be updated
                fileWriter.WriteByte(0xFF);//EOF byte
                fileStream.Close();
            }
            {
                var fileStream = System.IO.File.Create("iupdateWY.wys");
                var fileWriter = new wyUpdate.FileWriter(fileStream);
                fileWriter.WriteHeader( "IUSDFV2");
                fileWriter.WriteDeprecatedString( 0x01, "2.6.18.0");//Current Latest Version
                fileWriter.WriteDeprecatedString( 0x02, "file:D:\\QinarwTFS\\CppProjects\\VF_WoWLauncher\\VF_WoWLauncher\\bin\\Debug\\PaypalPay_NotClicked_Smaller.wyu");//Server file http/ftp site (for updating mirrors locally)
                fileWriter.WriteDeprecatedString( 0x07, "2.6.18.0");//Minimum client version needed to install the update
                //WriteFiles.WriteInt(fileStream, 0x0F, 123);//Dummy variable with a length int written to skip all versions except the catch-all version
                fileWriter.WriteDeprecatedString( 0x0B, "2.6.18.0");//Version to update
                fileWriter.WriteDeprecatedString( 0x03, "file:D:\\QinarwTFS\\CppProjects\\VF_WoWLauncher\\VF_WoWLauncher\\bin\\Debug\\PaypalPay_NotClicked_Smaller.wyu"); //Update file http/ftp site
                //0x80 Changes are in RTF format if this byte is present
                fileWriter.WriteDeprecatedString( 0x04, "This is the latest changes yo for wyUpdate"); //Latest changes
                fileWriter.WriteLong( 0x09, fileSize); //Update's filesize
                fileWriter.WriteLong( 0x08, fileAdlerSum); //Update file's Adler32 checksum
                fileWriter.WriteByteArray( 0x14, fileSHA1); //Signed SHA1 hash of the file
                fileWriter.WriteInt( 0x0A, 1); //Installing to folders (flags: 1 = base dir, 2 = system 32 dir (x86), 4 = comm desktop, 8 = com star menu, 16 = com app data, 32 = system 32 dir (x64), 64 = Windows root drive, CommonFilesx86 = 128, CommonFilesx64 = 256, 512 = ServiceOrCOMRegistration, 1024 = NonCurrentUserReg )
                fileWriter.WriteDeprecatedString( 0x20, "wyUpdate can not be updated yo");//Link text to show when the installed version of your software can't be updated
                fileWriter.WriteDeprecatedString( 0x21, "http://realmplayers.com/");//Link URL to show when the installed version of your software can't be updated
                fileStream.WriteByte(0xFF);//EOF byte
                fileStream.Close();
            }

            //Utility.MessageBoxShow(System.BitConverter.ToString(fileSHA1));
            */
        }
    }
}
