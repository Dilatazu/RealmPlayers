using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DatabaseCppPortApplication
{
    class CPP
    {
        [DllImport("DatabaseCppPort.dll")]
        public static extern void Begin_Serialize([MarshalAs(UnmanagedType.LPStr)]string _Filename);
        [DllImport("DatabaseCppPort.dll")]
        public static extern void Serialize_Int32([MarshalAs(UnmanagedType.I4)]int _Value);
        [DllImport("DatabaseCppPort.dll")]
        public static extern void Serialize_Float([MarshalAs(UnmanagedType.R4)]float _Value);
        [DllImport("DatabaseCppPort.dll")]
        public static extern void Serialize_UInt64([MarshalAs(UnmanagedType.U8)]UInt64 _Value);
        [DllImport("DatabaseCppPort.dll")]
        public static extern void Serialize_String([MarshalAs(UnmanagedType.LPStr)]string _Value);
        [DllImport("DatabaseCppPort.dll")]
        public static extern void End_Serialize();

        [DllImport("DatabaseCppPort.dll")]
        public static extern void Validate_ItemSummaryDatabase([MarshalAs(UnmanagedType.LPStr)]string _Filename);

        [DllImport("DatabaseCppPort.dll")]
        public static extern void Validate_RealmDatabase([MarshalAs(UnmanagedType.LPStr)]string _Filename);

    }
    partial class SerializeDB
    {
        public static void Cpp_ItemSummaryDatabase(string _Filename)
        {
            Console.WriteLine("Started Loading ItemSummaryDatabase");
            var itemSummaryDB = VF_RPDatabase.ItemSummaryDatabase.LoadSummaryDatabase(".");
            Console.WriteLine("Done with Loading ItemSummaryDatabase");

            Console.WriteLine("Started Serializing");
            CPP.Begin_Serialize(_Filename);

            CPP.Serialize_Int32(itemSummaryDB.m_Items.Count);
            foreach (var item in itemSummaryDB.m_Items)
            {
                CPP.Serialize_UInt64(item.Key);

                /*ItemSummary::Serialize*/
                CPP.Serialize_Int32(item.Value.m_ItemID);
                CPP.Serialize_Int32(item.Value.m_SuffixID);
                CPP.Serialize_Int32(item.Value.m_ItemOwners.Count);
                foreach (var itemOwner in item.Value.m_ItemOwners)
                {
                    CPP.Serialize_UInt64(itemOwner.Item1);
                    CPP.Serialize_UInt64((UInt64)itemOwner.Item2.ToBinary());
                }
                /*ItemSummary::Serialize*/
            }
            CPP.Serialize_Int32(itemSummaryDB.m_PlayerIDs.Count);
            foreach (var item in itemSummaryDB.m_PlayerIDs)
            {
                CPP.Serialize_String(item.Key);
                CPP.Serialize_UInt64(item.Value);
            }

            CPP.Serialize_UInt64(itemSummaryDB.m_EntityCounter_Emerald_Dream);
            CPP.Serialize_UInt64(itemSummaryDB.m_EntityCounter_Warsong);
            CPP.Serialize_UInt64(itemSummaryDB.m_EntityCounter_Al_Akir);
            CPP.Serialize_UInt64(itemSummaryDB.m_EntityCounter_Valkyrie);
            CPP.Serialize_UInt64(itemSummaryDB.m_EntityCounter_VanillaGaming);
            CPP.Serialize_UInt64(itemSummaryDB.m_EntityCounter_Rebirth);
            CPP.Serialize_UInt64(itemSummaryDB.m_EntityCounter_Archangel);

            Console.WriteLine("Done with Serializing");
            CPP.End_Serialize();

            Console.WriteLine("Started Validating ItemSummaryDatabase");
            CPP.Validate_ItemSummaryDatabase(_Filename);
            Console.WriteLine("Done with Validating ItemSummaryDatabase");
        }

        //public static void Cpp_PlayerSummaryDatabase(string _Filename)
        //{
        //    Console.WriteLine("Started Loading PlayerSummaryDatabase");
        //    var playerSummaryDB = VF_RPDatabase.PlayerSummaryDatabase.LoadSummaryDatabase(".");
        //    Console.WriteLine("Done with Loading PlayerSummaryDatabase");

        //    Console.WriteLine("Started Serializing");
        //    CPP.Begin_Serialize(_Filename);



        //    Console.WriteLine("Done with Serializing");
        //    CPP.End_Serialize();

        //    Console.WriteLine("Started Validating PlayerSummaryDatabase");
        //    CPP.Validate_PlayerSummaryDatabase(_Filename);
        //    Console.WriteLine("Done with Validating PlayerSummaryDatabase");
        //}
    }
}
