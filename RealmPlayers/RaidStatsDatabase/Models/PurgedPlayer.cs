using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace VF_RaidDamageDatabase.Models
{
    public class PurgedPlayer
    {
        public VF_RealmPlayersDatabase.WowRealm Realm { get; private set; }
        public string Name { get; private set; }
        public DateTime BeginDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public PurgedPlayer(string name, string realm, string beginDate = "", string endDate = "")
        {
            this.Name = name;
            this.Realm = VF_RealmPlayersDatabase.WowRealm.All;
            this.BeginDate = DateTime.MinValue;
            this.EndDate = DateTime.MaxValue;

            VF_RealmPlayersDatabase.WowRealm realmValue;
            if(Enum.TryParse(realm.Replace(' ', '_'), out realmValue) == true)
            {
                Realm = realmValue;
            }
            DateTime dateValue;
            if (beginDate != "" && DateTime.TryParse(beginDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal | System.Globalization.DateTimeStyles.AdjustToUniversal, out dateValue) == true)
            {
                BeginDate = dateValue;
            }
            if (endDate != "" && DateTime.TryParse(endDate, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal | System.Globalization.DateTimeStyles.AdjustToUniversal, out dateValue) == true)
            {
                EndDate = dateValue;
            }
        }
    }
}
