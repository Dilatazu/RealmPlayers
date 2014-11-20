using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VF_RealmPlayersDatabase
{
    public struct RPPContribution
    {
        Contributor m_Contributor;
        string m_Filename;
        public Contributor GetContributor()
        {
            return m_Contributor;
        }
        public string GetFilename()
        {
            return m_Filename;
        }
        public RPPContribution(Contributor _Contributor, string _Filename)
        {
            m_Contributor = _Contributor;
            m_Filename = _Filename;
        }
    }
}
