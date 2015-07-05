using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

using Old_RaidCollection = VF_RaidDamageDatabase.RaidCollection;
using Old_RaidBossFight = VF_RaidDamageDatabase.RaidBossFight;

namespace VF_RDDatabase
{
    //[ProtoContract]
    //public class Dungeon
    //{
    //    [ProtoMember(1)]
    //    private int m_UniqueDungeonID = -1;
    //    [ProtoMember(2)]
    //    private string m_Dungeon = "Unknown";
    //    [ProtoMember(3)]
    //    private DateTime m_DungeonStartDate = DateTime.MaxValue;
    //    [ProtoMember(4)]
    //    private DateTime m_DungeonEndDate = DateTime.MinValue;
    //    [ProtoMember(5)]
    //    public List<BossFight> m_BossFights = new List<BossFight>();
    //    [ProtoMember(6)]
    //    public List<string> m_GroupMembers = new List<string>();
        
    //    public Dungeon() { }
    //    public Dungeon(Old_RaidCollection_Raid _Raid)
    //    {
    //        //m_UniqueRaidID = _Raid.UniqueRaidID;
    //        //m_RaidID = _Raid.RaidID;
    //        //m_RaidResetDateTime = _Raid.RaidResetDateTime;
    //        //m_RaidInstance = _Raid.RaidInstance;
    //        //m_RaidStartDate = _Raid.RaidStartDate;
    //        //m_RaidEndDate = _Raid.RaidEndDate;
    //    }
    //}
}
