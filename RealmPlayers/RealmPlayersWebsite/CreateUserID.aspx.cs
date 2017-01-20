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

            int maxCount = PageUtility.GetQueryInt(Request, "count", 0);

            if (userStr == null || passStr == null)
                Response.Redirect("Index.aspx");

            System.Text.StringBuilder infoHTMLStrBuilder = new System.Text.StringBuilder(10000);
            if (userStr == "Admin" && passStr == VF.HiddenStrings.CreateUserID_AdminPassword)
            {
                //Admin
                infoHTMLStrBuilder.Append("<h2>Logged in as Admin</h2>");
                m_ThisContributor = new ContributorDB.ContributorDBElement();
                m_ThisContributor.Key = "Admin";
                m_ThisContributor.UserID = VF.HiddenStrings.DilatazuUserID;
            }
            else if (passStr == VF.HiddenStrings.CreateUserID_UserIDCreaterPassword)
            {
                m_ThisContributor = ContributorDB.GetContributor(userStr);
                if (m_ThisContributor == null)
                    Response.Redirect("Index.aspx");

                //Admin Contributor
                infoHTMLStrBuilder.Append("<h2>Logged in as " + m_ThisContributor.Name + "</h2>");
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
                    int count = 0;
                    infoHTMLStrBuilder.Append("<h3>Created UserIDs by " + addedByGroup.Key + "</h3><table><tr>");
                    infoHTMLStrBuilder.Append("<th>UserID</th><th>ContributorID</th><th>IP</th><th>Key</th></tr>");
                    foreach (var addedBy in addedByGroup.Value)
                    {
                        infoHTMLStrBuilder.Append("<tr>");
                        infoHTMLStrBuilder.Append("<td>" + addedBy.UserID + "</td>");
                        infoHTMLStrBuilder.Append("<td>" + addedBy.ContributorID + "</td>");
                        infoHTMLStrBuilder.Append("<td>" + addedBy.IP + "</td>");
                        infoHTMLStrBuilder.Append("<td>" + addedBy.Key + "</td>");
                        infoHTMLStrBuilder.Append("</tr>");
                        if (maxCount > 0 && ++count >= maxCount)
                            break;
                    }
                    infoHTMLStrBuilder.Append("</table>");
                }
            }
            else
            {
                infoHTMLStrBuilder.Append("<h3>Created UserIDs</h3>");
                var addedBys = ContributorDB.GetMongoDB().MongoDBCollection.Find(Query.EQ("AddedBy", userStr));
                var addedBysSorted = addedBys.OrderByDescending(_Value => _Value.ContributorID);
                foreach(var addedBy in addedBysSorted)
                {
                    infoHTMLStrBuilder.Append(addedBy.UserID + "<br/>");
                    if (maxCount > 0 && --maxCount == 0)
                        break;
                }
            }
            m_InfoHTML = infoHTMLStrBuilder.ToString();
        }

        protected void btnCreateUserID_Click(object sender, EventArgs e)
        {
            string userID;
            if(ContributorUtility.GenerateUserID(txtCreateUserID.Text, out userID) == true)
            {
                if(ContributorDB.AddVIPContributor(userID, m_ThisContributor.UserID) == true)
                {
                    if(PageUtility.GetQueryString(Request, "redirect", "null") == "null")
                        Response.Redirect(Request.RawUrl);
                    else
                        txtStatus.Text = "Added: " + userID;
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