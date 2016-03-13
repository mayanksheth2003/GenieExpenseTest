<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" MasterPageFile="~/Site1.Master" Inherits="ExpenseTracking.Login" %>

<asp:Content ID="HeadContent" ContentPlaceHolderID="head" runat="server">
        <link rel="stylesheet" href="scripts/style.css" />
    </asp:Content>
    <asp:Content ID="MainContent" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="container">
        <div id="login-form">
            <h3>Login</h3>
                <fieldset>
                    <div id="loginForm" runat="server">
                        <asp:TextBox ID="EmailTextBox" runat="server" TextMode="Email" Text="Email"
                             onfocus="if(this.value=='Email')this.value=''" ToolTip="Email" />
                        <asp:TextBox ID="PasswordTextBox" runat="server" TextMode="Password" Text="Password"
                            onfocus="if(this.value=='Password')this.value=''" ToolTip="Password" />
                        <asp:Button ID="LoginButton" Text="Login" runat="server" OnClick="Login_Click" />
                        <asp:Label ID="MessageLabel" runat="server" />
                    </div>
                </fieldset>
        </div>
    </div>
</asp:Content>
