<%@ Page Title="" Language="C#" MasterPageFile="~/MasterFrame.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="RealmPlayersServer.Contact" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="BodyContent" runat="server">
    <ul class="breadcrumb">
        <li><a href="index.html">Home</a> <span class="divider">/</span></li>
        <li class="active">Contact</li>
      </ul><!--/.breadcrumb -->

    <header class="page-header">  
        <div class="row">
          <div class="span12">
            <h1>Contact</h1>
      <%--<form class="well span8">
        <div class="row">
          <div class="span3">
            <label>Name</label>
            <input type="text" class="span3" placeholder="Your Name">
            <label>Character Name</label>
            <input type="text" class="span3" placeholder="Your Character Name">
            <label>Email Address</label>
            <div class="input-prepend">
              <span class="add-on"><i class="icon-envelope"></i></span><input type="text" id="inputIcon" class="span2" style="width:233px" placeholder="Your email address">
            </div>
            <label>Subject
            <select id="subject" name="subject" class="span3">
              <option value="na" selected="">Choose One:</option>
              <option value="service">General Customer Service</option>
              <option value="suggestions">Suggestions</option>
              <option value="product">Support</option>
            </select>
            </label>
          </div>
          <div class="span5">
            <label>Message</label>
            <textarea name="message" id="message" class="input-xlarge span5" rows="10"></textarea>
          </div>
        </div>
        <button type="submit" class="btn btn-primary pull-right">Send</button>
      </form>--%>
          </div>
        </div>
    </header>
</asp:Content>
