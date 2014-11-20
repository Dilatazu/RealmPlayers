using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_WoWLauncher
{
    struct AddonStatus
    {
        public enum Enum
        {
            Default = 0,
            Enabled = 1,
            Disabled = 2,
        }
        public string Account;
        public string Realm;
        public string Character;
        public AddonStatus.Enum Enabled;

        public bool IsSameChar(AddonStatus _AddonStatus)
        {
            return Account == _AddonStatus.Account && Realm == _AddonStatus.Realm && Character == _AddonStatus.Character;
        }
    }

    class CharacterAddons
    {
        RealmAddons m_Realm = null;
        string m_Character = null;

        Dictionary<string, AddonStatus.Enum> m_EnabledAddons = new Dictionary<string, AddonStatus.Enum>();
        bool m_Changed = false;
        DateTime m_LoadDateTime;
        public CharacterAddons(RealmAddons _Realm, string _Character)
        {
            m_Realm = _Realm;
            m_Character = _Character;
            Reload();
        }
        public string Character
        {
            get { return m_Character; }
        }

        public void Reload()
        {
            m_EnabledAddons.Clear();
            if (System.IO.File.Exists(CharacterAddonFilePath) == true)
            {
                m_LoadDateTime = DateTime.Now;
                m_Changed = false;
                string[] configLines = System.IO.File.ReadAllLines(CharacterAddonFilePath);
                foreach (string configLine in configLines)
                {
                    string[] addonNameAndConfig = configLine.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                    if (addonNameAndConfig.Length == 2)
                    {
                        AddonStatus.Enum addonEnabled = AddonStatus.Enum.Default;
                        if (addonNameAndConfig[1] == "enabled")
                        {
                            addonEnabled = AddonStatus.Enum.Enabled;
                        }
                        else if (addonNameAndConfig[1] == "disabled")
                        {
                            addonEnabled = AddonStatus.Enum.Disabled;
                        }
                        else
                        {
                            Logger.ConsoleWriteLine("Unexpected Addon Config State: " + addonNameAndConfig[1], ConsoleColor.Red);
                            continue;
                        }
                        m_EnabledAddons.AddOrSet(addonNameAndConfig[0], addonEnabled);
                    }
                }
            }
        }
        public void Save(bool _ForceSave)
        {
            if (m_Changed == true || _ForceSave == true)
            {
                string data = "";
                foreach (var configValue in m_EnabledAddons)
                {
                    data += configValue.Key + ": " + (configValue.Value == AddonStatus.Enum.Enabled ? "enabled" : "disabled") + "\r\n";
                }
                System.IO.File.WriteAllText(CharacterAddonFilePath, data);
                m_Changed = false;
            }
        }
        public string CharacterAddonFilePath
        {
            get { return m_Realm.RealmFolder + "\\" + m_Character + "\\AddOns.txt"; }
        }

        public AddonStatus GetAddonStatus(string _AddonName)
        {
            AddonStatus.Enum addonEnabled = AddonStatus.Enum.Default;
            if (m_EnabledAddons.ContainsKey(_AddonName) == true)
                addonEnabled = m_EnabledAddons[_AddonName];

            return new AddonStatus{ Character = m_Character, Enabled = addonEnabled };
        }
        public void SetAddonStatus(string _AddonName, AddonStatus.Enum _AddonStatus)
        {
            m_EnabledAddons.AddOrSet(_AddonName, _AddonStatus);
            m_Changed = true;
        }
    }
    class RealmAddons
    {
        AccountAddons m_Account = null;
        string m_Realm = null;
        List<CharacterAddons> m_CharacterAddons = new List<CharacterAddons>();
        public RealmAddons(AccountAddons _Account, string _Realm)
        {
            m_Account = _Account;
            m_Realm = _Realm;

            var realmCharacters = Utility.GetDirectoriesInDirectory(RealmFolder);
            foreach (var realmCharacter in realmCharacters)
            {
                m_CharacterAddons.Add(new CharacterAddons(this, realmCharacter));
            }
        }

        public string Realm
        {
            get { return m_Realm; }
        }
        public string RealmFolder
        {
            get { return Settings.GetWowDirectory(m_Account.WowVersion) + "WTF\\Account\\" + m_Account.Account + "\\" + m_Realm; }
        }

        public List<AddonStatus> GetAddonStatus(string _AddonName)
        {
            List<AddonStatus> result = new List<AddonStatus>();
            foreach (var charAddon in m_CharacterAddons)
            {
                AddonStatus addonStatus = charAddon.GetAddonStatus(_AddonName);
                addonStatus.Realm = m_Realm;
                result.Add(addonStatus);
            }

            return result;
        }
        public void SetAddonStatus(string _AddonName, AddonStatus _AddonStatus)
        {
            foreach (var character in m_CharacterAddons)
            {
                if (character.Character == _AddonStatus.Character)
                {
                    character.SetAddonStatus(_AddonName, _AddonStatus.Enabled);
                    return;
                }
            }
        }
        public void Save(bool _ForceSave)
        {
            foreach (var character in m_CharacterAddons)
            {
                character.Save(_ForceSave);
            }
        }
    }
    class AccountAddons
    {
        string m_Account = null;
        WowVersion m_WowVersion;
        List<RealmAddons> m_RealmAddons = new List<RealmAddons>();
        public AccountAddons(string _Account, WowVersion _WowVersion)
        {
            m_Account = _Account;
            m_WowVersion = _WowVersion;
            var realms = Utility.GetDirectoriesInDirectory(AccountFolder);
            foreach (var realm in realms)
            {
                m_RealmAddons.Add(new RealmAddons(this, realm));
            }
        }

        public WowVersion WowVersion
        {
            get { return m_WowVersion; }
        }
        public string Account
        {
            get { return m_Account; }
        }
        public string AccountFolder
        {
            get { return Settings.GetWowDirectory(m_WowVersion) + "WTF\\Account\\" + m_Account; }
        }

        public List<AddonStatus> GetAddonStatus(string _AddonName)
        {
            List<AddonStatus> result = new List<AddonStatus>();
            foreach (var realmAddon in m_RealmAddons)
            {
                List<AddonStatus> addonStatuses = realmAddon.GetAddonStatus(_AddonName);
                foreach (var addonStatus in addonStatuses)
                {
                    var newStatus = addonStatus;
                    newStatus.Account = m_Account;
                    result.Add(newStatus);
                }
            }
            return result;
        }
        public void SetAddonStatus(string _AddonName, AddonStatus _AddonStatus)
        {
            foreach (var realm in m_RealmAddons)
            {
                if (realm.Realm == _AddonStatus.Realm)
                {
                    realm.SetAddonStatus(_AddonName, _AddonStatus);
                    return;
                }
            }
        }
        public void Save(bool _ForceSave)
        {
            foreach (var realm in m_RealmAddons)
            {
                realm.Save(_ForceSave);
            }
        }
    }

    class AddonsWTF
    {
        List<AccountAddons> m_AccountAddons = new List<AccountAddons>();
        private AddonsWTF()
        { }

        public static AddonsWTF LoadAllAccountAddons(WowVersion _WowVersion)
        {
            var addonsWTF = new AddonsWTF();
            var accounts = Utility.GetDirectoriesInDirectory(Settings.GetWowDirectory(_WowVersion) + "WTF\\Account");
            foreach (var account in accounts)
            {
                addonsWTF.m_AccountAddons.Add(new AccountAddons(account, _WowVersion));
            }
            return addonsWTF;
        }

        public List<AddonStatus> GetAddonStatus(string _AddonName)
        {
            List<AddonStatus> result = new List<AddonStatus>();
            foreach (var account in m_AccountAddons)
            {
                result.AddRange(account.GetAddonStatus(_AddonName));
            }
            return result;
        }
        public void SetAddonStatus(string _AddonName, AddonStatus _AddonStatus)
        {
            foreach (var account in m_AccountAddons)
            {
                if (account.Account == _AddonStatus.Account)
                {
                    account.SetAddonStatus(_AddonName, _AddonStatus);
                    return;
                }
            }
        }
        public void SaveAll(bool _ForceSave = false)
        {
            foreach (var account in m_AccountAddons)
            {
                account.Save(_ForceSave);
            }
        }
    }
}
