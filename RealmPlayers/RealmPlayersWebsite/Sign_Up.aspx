<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="Sign_Up.aspx.cs" Inherits="RealmPlayersServer.Sign_Up" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <li><a href="index.html">Home</a> <span class="divider">/</span></li>
        <li class="active">Sing Up</li>
      </ul><!--/.breadcrumb -->

    <header class="page-header">  
        <div class="row">
          <div class="span12">
            <h1>Sing Up</h1>
            <%--<form class="well span8">
              <div class="row">
                <div class="span3">
                  <label>Username</label>
                  <input type="text" class="span3" placeholder="Your Name">
                  <label>Password</label>
                  <input type="password" class="span3" placeholder="password">
                  <label>Password Confirmation</label>
                  <input type="password" class="span3" placeholder="password">
                  <label>Email Address</label>
                  <div class="input-prepend">
                    <span class="add-on"><i class="icon-envelope"></i></span><input type="text" id="inputIcon" class="span2" style="width:233px" placeholder="Your email address">
                  </div>
                </div>
              </div>
              <button type="submit" class="btn btn-primary pull-right">Send</button>
            </form>--%>
          </div>
        </div>
    </header>
</asp:Content>
