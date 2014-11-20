using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;

using ContributorUtility = VF_RealmPlayersDatabase.ContributorUtility;
using ContributorDB = VF_RealmPlayersDatabase.ContributorDB;

namespace RealmPlayersServer
{
    public partial class CreateUserID : System.Web.UI.Page
    {
        public string m_InfoHTML = "";
        private ContributorDB.ContributorDBElement m_ThisContributor;
        protected void Page_Load(object sender, EventArgs e)
        {
            string userStr = Request.QueryString.Get("user");
            string passStr = Request.QueryString.Get("pass");

            if (userStr == null || passStr == null)
                Response.Redirect("Index.aspx");

            if (userStr == "Admin" && passStr == "***REMOVED***")
            {
                //Admin
                m_InfoHTML = "<h2>Logged in as Admin</h2>";
                m_ThisContributor = new ContributorDB.ContributorDBElement();
                m_ThisContributor.Key = "Admin";
                m_ThisContributor.UserID = "***REMOVED***";
            }
            else if (passStr == "***REMOVED***")
            {
                m_ThisContributor = ContributorDB.GetContributor(userStr);
                if (m_ThisContributor == null)
                    Response.Redirect("Index.aspx");

                //Admin Contributor
                m_InfoHTML = "<h2>Logged in as " + m_ThisContributor.Name + "</h2>";
            }
            else
                Response.Redirect("Index.aspx");

            if (m_ThisContributor.Key == "Admin") //Admin
            {
                var addedBys = ContributorDB.GetMongoDB().MongoDBCollection.FindAll();
                var addedBysSorted = addedBys.OrderByDescending(_Value => _Value.ContributorID);
                Dictionary<string, List<ContributorDB.ContributorDBElement>> addedByGroups = new Dictionary<string, List<ContributorDB.ContributorDBElement>>();
                foreach (var addedBy in addedBysSorted)
                {
                    if (addedBy.ContributorID >= VF_RealmPlayersDatabase.Contributor.ContributorTrustworthyIDBound)
                        addedByGroups.AddToList("Temp", addedBy);
                    else
                        addedByGroups.AddToList(addedBy.AddedBy, addedBy);
                }
                var addedByGroupsSorted = addedByGroups.OrderBy(_Value => _Value.Value.Count);
                foreach (var addedByGroup in addedByGroupsSorted)
                {
                    m_InfoHTML += "<h3>Created UserIDs by " + addedByGroup.Key + "</h3><table><tr>";
                    m_InfoHTML += "<th>UserID</th><th>ContributorID</th><th>IP</th><th>Key</th></tr>";
                    foreach (var addedBy in addedByGroup.Value)
                    {
                        m_InfoHTML += "<tr>";
                        m_InfoHTML += "<td>" + addedBy.UserID + "</td>";
                        m_InfoHTML += "<td>" + addedBy.ContributorID + "</td>";
                        m_InfoHTML += "<td>" + addedBy.IP + "</td>";
                        m_InfoHTML += "<td>" + addedBy.Key + "</td>";
                        m_InfoHTML += "</tr>";
                    }
                    m_InfoHTML += "</table>";
                }
            }
            else
            {
                m_InfoHTML += "<h3>Created UserIDs</h3>";
                var addedBys = ContributorDB.GetMongoDB().MongoDBCollection.Find(Query.EQ("AddedBy", userStr));
                var addedBysSorted = addedBys.OrderByDescending(_Value => _Value.ContributorID);
                foreach(var addedBy in addedBysSorted)
                {
                    m_InfoHTML += addedBy.UserID + "<br/>";
                }
            }
        }

        protected void btnCreateUserID_Click(object sender, EventArgs e)
        {
            string userID;
            if(ContributorUtility.GenerateUserID(txtCreateUserID.Text, out userID) == true)
            {
                if(ContributorDB.AddVIPContributor(userID, m_ThisContributor.UserID) == true)
                {
                    Response.Redirect(Request.RawUrl);
                }
                else
                {
                    txtStatus.Text = "Error: Could not create UserID, internal server issue, try again later!";
                }
            }
            else
            {
                txtStatus.Text = "Error: Could not create UserID, invalid Username format!";
            }
        }
    }
}