<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="HeadofDepartment.aspx.cs" EnableViewState="true" Inherits="ExpenseTracking.HeadofDepartment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <script>
        $(function () {
            $("#tabs").tabs();
            $("#datepicker").datepicker({
                onSelect: function (selected, event) {
                    updateHiddenField(selected);
                }
            });
        });

        function updateHiddenField(value)
        {
            $('#<%=HiddenDateField.ClientID %>').val(value);
        }
    </script>
    <style>
        .Top_Padding {
            padding-top: 20px;        
        }
        .floatLeft {
            display:inline;
            float:left;
        }
        .floatRight {
            float:right;
        }
        ul {
            padding-top:30px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="tabs" style="overflow:auto;">
       <div>
           <div class="floatLeft" style="padding-right:600px;">
               <asp:LinkButton ID="BackButton" runat="server" Font-Underline="true" Text="< Back to Login" OnClick="BackButton_Click" />
           </div>
           <div class="floatLeft">
               <asp:LinkButton ID="ReloadButton" runat="server" Font-Underline="true" Text="Reload" OnClick="ReloadButton_Click" />
           </div>
       </div>
       <ul>
           <li><a href="#tabs-1">Add an Expense</a></li>
           <li><a href="#tabs-2">Approve Expenses</a></li>
           <li><a href="#tabs-3">Add Employee</a></li>
           <li><a href="#tabs-4">Groups</a></li>
       </ul>
       <div id="tabs-1">
           <div style="padding:10px;">
               <div>
                   <p><b>Please enter following details:</b></p>
               </div>
               <div class="Top_Padding">
                   <div class="floatLeft" style="padding-right:10px;width:90px;">
                       <asp:Label ID="DescriptionLabel" runat="server" Text="Description:" />
                   </div>
                   <div class="floatLeft">
                       <asp:TextBox ID="DescriptionTextBox" runat="server" MaxLength="50" />
                   </div>
               </div>
               <br />
               <div class="Top_Padding">
                   <div class="floatLeft" style="padding-right:10px; width:90px;">
                       <asp:Label ID="DateLabel" runat="server" Text="Date:" />
                   </div>
                   <div class="floatLeft">
                       <input type="text" id="datepicker" />
                       <asp:HiddenField ID="HiddenDateField" runat="server" />
                   </div>
               </div>
               <br />
               <div class="Top_Padding">
                   <div class="floatLeft" style="padding-right:10px; width:90px;">
                       <asp:Label ID="AmountLabel" runat="server" Text="Amount:" />
                   </div>
                   <div class="floatLeft">
                       <asp:TextBox ID="AmountTextBox" runat="server" TextMode="Number" />
                   </div>
               </div>
               <br />
               <div class="Top_Padding">
                   <div class="floatleft">
                       <asp:Button ID="AddExpenseButton" runat="server" Text="Submit Expense" OnClick="AddExpenseButton_Click" />
                   </div>
               </div>
           </div>
       </div>
       <div id="tabs-2">
          <div style="padding:10px;">
              <div>
                   <p><b>Employees List</b></p>
               </div>
              <div>
                   <div class="floatLeft" style="padding-bottom:20px;">
                      <asp:Table ID="EmployessExpenseTable" runat="server" BorderStyle="Solid" GridLines="Both" BorderWidth="1">
                          <asp:TableHeaderRow>
                              <asp:TableHeaderCell Width="20px">ID</asp:TableHeaderCell>
                              <asp:TableHeaderCell Width="100px">Name</asp:TableHeaderCell>
                              <asp:TableHeaderCell Width="80px">Date</asp:TableHeaderCell>
                              <asp:TableHeaderCell Width="80px">Amount</asp:TableHeaderCell>
                              <asp:TableHeaderCell>Status</asp:TableHeaderCell>
                              <asp:TableHeaderCell Width="100px">Employee ID</asp:TableHeaderCell>
                          </asp:TableHeaderRow>
                      </asp:Table>
                   </div>
               </div>
          </div>
       </div>
       <div id="tabs-3">
           <div style="padding:10px;">
               <div>
                   <p><b>Please enter following details:</b></p>
               </div>
               <div class="Top_Padding">
                   <div class="floatLeft" style="padding-right:10px;width:130px;">
                       <asp:Label ID="NameLabel" runat="server" Text="Name:" />
                   </div>
                   <div class="floatLeft">
                       <asp:TextBox ID="NameTextBox" runat="server" MaxLength="50" />
                   </div>
               </div>
               <br />
               <div class="Top_Padding">
                   <div class="floatLeft" style="padding-right:10px; width:130px;">
                       <asp:Label ID="NumberLabel" runat="server" Text="Phone Number:" />
                   </div>
                   <div class="floatLeft">
                       <asp:TextBox ID="NumberTextBox" runat="server" TextMode="Number" />
                   </div>
               </div>
               <br />
               <div class="Top_Padding">
                   <div class="floatLeft" style="padding-right:10px; width:130px;">
                       <asp:Label ID="EmailLabel" runat="server" Text="Email:" />
                   </div>
                   <div class="floatLeft">
                       <asp:TextBox ID="EmailTextBox" runat="server" TextMode="Email" />
                   </div>
               </div>
               <br />
               <div class="Top_Padding">
                   <div class="floatLeft" style="padding-right:10px; width:130px;">
                       <asp:Label ID="AssignGroupLabel" runat="server" Text="Assign Group:" />
                   </div>
                   <div class="floatLeft">
                      <asp:DropDownList ID="GroupDDL" runat="server" Width="200px">
                      </asp:DropDownList>
                   </div>
               </div>
               <br />
               <div class="Top_Padding">
                   <div class="floatleft">
                       <asp:Button ID="AddEmployeeButton" runat="server" Text="Add an Employee" OnClick="AddEmployeeButton_Click" />
                   </div>
               </div>
           </div>
       </div>
       <div id="tabs-4">
            <div style="padding:10px;">
               <div>
                   <p><b>Please enter following details:</b></p>
               </div>
               <div class="Top_Padding">
                   <div class="floatLeft" style="padding-right:10px;width:130px;">
                       <asp:Label ID="GroupNameLabel" runat="server" Text="Group Name:" />
                   </div>
                   <div class="floatLeft">
                       <asp:TextBox ID="GroupNameTextBox" runat="server" MaxLength="50" />
                   </div>
               </div>
               <br />
               <div class="Top_Padding">
                   <div class="floatLeft" style="padding-right:10px; width:130px;">
                       <asp:Label ID="GroupManagerLabel" runat="server" Text="Group Manager:" />
                   </div>
                   <div class="floatLeft">
                      <asp:DropDownList ID="EmployeeListDDL" runat="server" Width="200px">
                      </asp:DropDownList>
                   </div>
               </div>
               <br />
               <div class="Top_Padding">
                   <div class="floatleft">
                       <asp:Button ID="AddGroupButton" runat="server" Text="Add Group" OnClick="AddGroupButton_Click" />
                   </div>
               </div>
               <br />
               <div class="Top_Padding">
                    <div>
                        <p><b>Group List:</b></p>
                    </div>
                   <div class="floatLeft" style="padding-bottom:20px;">
                      <asp:Table ID="GroupsTable" runat="server" BorderStyle="Solid" GridLines="Both" BorderWidth="1">
                          <asp:TableHeaderRow>
                              <asp:TableHeaderCell Width="20px">ID</asp:TableHeaderCell>
                              <asp:TableHeaderCell Width="100px">Name</asp:TableHeaderCell>
                              <asp:TableHeaderCell Width="140px">Group Manager</asp:TableHeaderCell>
                          </asp:TableHeaderRow>
                      </asp:Table>
                   </div>
                </div>
           </div>
        </div>
   </div>
</asp:Content>
