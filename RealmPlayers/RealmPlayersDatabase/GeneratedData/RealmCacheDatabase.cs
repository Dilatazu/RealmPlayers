using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace VF_RealmPlayersDatabase.GeneratedData
{
    public class RealmCacheDatabase
    {
        RealmDatabase m_DatabasePointer;
        private Dictionary<string, Guild> m_Guilds = new Dictionary<string, Guild>();
        private volatile bool m_CurrentlyGeneratingGuilds = false;
        private Dictionary<int, List<Tuple<DateTime, string>>> m_ItemsUsed = new Dictionary<int, List<Tuple<DateTime, string>>>();
        private object m_LockObj = new object();

        
        public RealmCacheDatabase(RealmDatabase _DatabasePointer)
        {
            m_DatabasePointer = _DatabasePointer;
        }
    }
}
