namespace VF_RaidDamageDatabase.Models
{
    public class PurgedPlayer
    {
        public string Name { get; private set; }
        public string Realm { get; private set; }

        public PurgedPlayer(string name, string realm)
        {
            this.Name = name;
            this.Realm = realm;
        }
    }
}
