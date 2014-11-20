using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RealmPlayersDatabase
{
    public class ContributorUtility
    {
        public static bool GenerateUserID(string _Username, out string _ReturnUserID)
        {
            _ReturnUserID = _Username;
            if (_Username.Contains('.'))
            {
                _ReturnUserID = _Username;
                return false;
            }
            _Username = _Username.Replace(" ", "");//Ta väck mellanslag
            if (_Username.Length < 3)
                return false;
            if (_Username[0] >= 'A' && _Username[0] <= 'Z')
            {
                //Första bokstaven måste vara stor bokstav
            }
            else
            {
                return false;
            }
            for (int i = 1; i < _Username.Length; ++i)
            {
                if (_Username[i] >= 'a' && _Username[i] <= 'z')
                {
                    //Enda tillåtna bokstäverna är från a-z i små bokstäver
                }
                else
                {
                    return false;
                }
            }
            string userID = _Username + ".";

            Random rand = new Random();
            for (int i = 0; i < 6; ++i)
                userID += rand.Next(0, 10); //Lägg till 6 random siffror

            if (userID.Length != _Username.Length + 7)
                return false;

            _ReturnUserID = userID;
            return true;
        }
    }
}
