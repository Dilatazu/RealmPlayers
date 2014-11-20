using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Collections.Concurrent;

namespace VF_RaidDamageWebsite
{
    public class StaticValues : RealmPlayersServer.StaticValues
    {
        public static Dictionary<string, string> _RaidInstanceImages = new Dictionary<string, string>
        {
            {"Onyxia's Lair", "assets/img/raid/raid-onyxia.png"},
            {"Blackwing Lair", "assets/img/raid/raid-blackwinglair.png"}, 
            {"Molten Core", "assets/img/raid/raid-moltencore.png"}, 
            {"Zul'Gurub", "assets/img/raid/raid-zulgurub.png"}, 
            {"Ruins of Ahn'Qiraj", "assets/img/raid/raid-aqruins.png"}, 
            {"Naxxramas", "assets/img/raid/raid-naxxramas.png"}, 
            {"Ahn'Qiraj Temple", "assets/img/raid/raid-aqtemple.png"}
        };

        public static ConcurrentDictionary<string, int> g_MeasuredStrings = new ConcurrentDictionary<string, int>();
        public static int MeasureStringLength(string _String, int _FontSize = 12)
        {
            if (g_MeasuredStrings.ContainsKey(_String) == true)
            {
                return (g_MeasuredStrings[_String] * _FontSize) / 12;
            }
            int width = -1;

            using (var graphics = Graphics.FromImage(new Bitmap(1, 1)))
            {
                //Calculate width & height required to fit the text into the image
                //based on the font selected

                width = (int)graphics.MeasureString(_String.ToString(), new System.Drawing.Font("Arial", 12, GraphicsUnit.Point)).Width;
            }
            g_MeasuredStrings.AddOrUpdate(_String, width, (string _Key, int _OldValue) => {return width; });
            return width;
        }
    }
}