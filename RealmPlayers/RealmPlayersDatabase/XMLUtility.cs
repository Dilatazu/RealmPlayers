using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RealmPlayersDatabase
{
    class XMLUtility
    {
        public static System.Xml.XmlNode GetChild(System.Xml.XmlNode _Node, string _Name)
        {
            foreach (System.Xml.XmlNode node in _Node.ChildNodes)
            {
                if ((node.Attributes[0].Name == "name" || node.Attributes[0].Name == "index")
                    && node.Attributes[0].Value == _Name)
                    return node;
            }
            return null;
        }
        public static string GetChildValue(System.Xml.XmlNode _Node, string _Name, string _DefaultReturn = null)
        {
            var node = GetChild(_Node, _Name);
            if (node != null)
                return node.Attributes["value"].Value;
            if (_DefaultReturn == null)
                throw new Exception("Node was null");
            return _DefaultReturn;
        }
    }
}
