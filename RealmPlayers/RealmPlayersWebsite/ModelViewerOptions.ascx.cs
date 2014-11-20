using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RealmPlayersServer
{
    public partial class ModelViewerOptions : System.Web.UI.UserControl
    {
        private bool m_View3DModel = false;
        private bool m_HideHead = false;
        public bool View3DModel
        {
            get
            {
                return m_View3DModel;
            }
        }
        public bool HideHead
        {
            get
            {
                return m_HideHead;
            }
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                string view3DModelStr = PageUtility.GetQueryString(Request, "modelviewer", "unknown").ToLower();
                string hideHeadStr = "false";
                if (view3DModelStr == "unknown")
                {
                    HttpCookie modelViewerControlCookie = Request.Cookies["ModelViewerControl"];
                    if (modelViewerControlCookie != null)
                    {
                        view3DModelStr = modelViewerControlCookie["View3DModel"];
                        hideHeadStr = modelViewerControlCookie["HideHead"];
                    }
                    else
                    {
                        if (Request.UserAgent.Contains("Chrome") == true)
                            view3DModelStr = "true";
                        else
                            view3DModelStr = "false";
                    }
                }
                m_View3DModel = ((view3DModelStr == "true") ? true : false);
                m_HideHead = ((hideHeadStr == "true") ? true : false);
                ModelViewerChb.Checked = m_View3DModel;
                HideHeadChb.Checked = m_HideHead;
                if (m_View3DModel == false)
                    HideHeadChb.Enabled = false;
                else
                    HideHeadChb.Enabled = true;
            }
            Page.PreLoad += Page_PreLoad;
        }
        protected void Page_PreLoad(object sender, EventArgs e)
        {
            if (IsPostBack == true)
            {
                m_View3DModel = ModelViewerChb.Checked;
                m_HideHead = HideHeadChb.Checked;
                if (m_View3DModel == false)
                    HideHeadChb.Enabled = false;
                else
                    HideHeadChb.Enabled = true;
                string view3DModelStr = (m_View3DModel ? "true" : "false");
                string hideHeadStr = (m_HideHead ? "true" : "false");
                
                string cookieViewModelValue = "";
                string cookieHideHeadValue = "";
                HttpCookie modelViewerControlCookie = Request.Cookies["ModelViewerControl"];
                if (modelViewerControlCookie != null)
                {
                    cookieViewModelValue = modelViewerControlCookie["View3DModel"];
                    cookieHideHeadValue = modelViewerControlCookie["HideHead"];
                }

                if (cookieViewModelValue != view3DModelStr || cookieHideHeadValue != hideHeadStr)
                {
                    HttpCookie cookie = new HttpCookie("ModelViewerControl");
                    cookie.Expires = DateTime.Now.AddDays(30);
                    cookie["View3DModel"] = view3DModelStr;
                    cookie["HideHead"] = hideHeadStr;
                    Response.Cookies.Add(cookie);
                    if (cookieViewModelValue != view3DModelStr)
                    {
                        string queryModelViewerStr = PageUtility.GetQueryString(Request, "modelviewer", "unknown");
                        if (queryModelViewerStr.ToLower() != view3DModelStr)
                        {
                            var newQuery = Request.Url.Query.Replace("modelviewer=" + queryModelViewerStr, "modelviewer=" + view3DModelStr);
                            //Error pga denna 
                            //Response.Redirect(Request.Url.AbsolutePath + newQuery);
                        }
                    }
                }
            }
        }
    }
}