using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI.WebControls;

namespace ExpenseTracking
{
    public partial class HeadofDepartment : System.Web.UI.Page
    {
        private static string ConnectionString = ConfigurationManager.ConnectionStrings["AzureDatabase"].ConnectionString;
        //private static string ConnectionString = "Data Source=(localdb)\\ProjectsV12;Initial Catalog=TrackExpense;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";
        private int LoggedInUserId;

        protected void Page_Load(object sender, EventArgs e)
        {
            LoggedInUserId = string.IsNullOrEmpty(Session["LoggedInUserId"].ToString()) ? -1 : int.Parse(Session["LoggedInUserId"].ToString());
            PopulateMyEmployeesExpenseList();

            if(!IsPostBack)
            {
                PopulateEmployeeDropDownList();
                PopulateGroupDropDownList();
            }

            PopulateGroupsList();
        }

        protected void AddExpenseButton_Click(object sender, EventArgs e)
        {
            int status = DetermineStatusofExpense(AmountTextBox.Text);

            SqlConnection connection = new SqlConnection(ConnectionString);
            SqlCommand command = new SqlCommand("AddExpense", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlParameter description = new SqlParameter("description", DescriptionTextBox.Text);
            SqlParameter date = new SqlParameter("date", Convert.ToDateTime(HiddenDateField.Value.ToString()));
            SqlParameter amount = new SqlParameter("amount", AmountTextBox.Text);
            SqlParameter statusParameter = new SqlParameter("status", status);
            SqlParameter employeeIdParameter = new SqlParameter("employeeId", LoggedInUserId);

            command.Parameters.Add(description);
            command.Parameters.Add(date);
            command.Parameters.Add(amount);
            command.Parameters.Add(statusParameter);
            command.Parameters.Add(employeeIdParameter);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }

            Response.Redirect(Request.RawUrl, false);
        }

        protected void AddGroupButton_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            SqlCommand command = new SqlCommand("AddGroup", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlParameter name = new SqlParameter("name", GroupNameTextBox.Text);
            SqlParameter managerId = new SqlParameter("managerId", int.Parse(EmployeeListDDL.SelectedValue));
            
            command.Parameters.Add(name);
            command.Parameters.Add(managerId);
            
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }

            Response.Redirect(Request.RawUrl, false);
        }

        protected void AddEmployeeButton_Click(object sender, EventArgs e)
        {
            string passwordText = NameTextBox.Text.Substring(0, 5) + (string.IsNullOrEmpty(NumberTextBox.Text) ? "00000" : NumberTextBox.Text.Substring(0, 5));

            SqlConnection connection = new SqlConnection(ConnectionString);
            SqlCommand command = new SqlCommand("AddEmployee", connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlParameter name = new SqlParameter("name", NameTextBox.Text);
            SqlParameter number = new SqlParameter("number", NumberTextBox.Text);
            SqlParameter email = new SqlParameter("email", EmailTextBox.Text);
            SqlParameter password = new SqlParameter("password", passwordText);
            int groupSelectedValue = GroupDDL.SelectedValue == "-1" ? 0 : int.Parse(GroupDDL.SelectedValue);
            SqlParameter grpId = new SqlParameter("groupId", groupSelectedValue);

            command.Parameters.Add(name);
            command.Parameters.Add(number);
            command.Parameters.Add(email);
            command.Parameters.Add(password);
            command.Parameters.Add(grpId);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }

            Response.Redirect(Request.RawUrl, false);
        }

        private void PopulateMyEmployeesExpenseList()
        {
            DataTable dataTable = GetMyEmployeesExpense();

            foreach (DataRow row in dataTable.Rows)
            {
                TableRow tableRow = new TableRow();
                foreach (DataColumn column in dataTable.Columns)
                {
                    TableCell tableCell = new TableCell();
                    if (column.ColumnName == "Date")
                    {
                        tableCell.Text = Convert.ToDateTime(row[column.ColumnName].ToString()).ToShortDateString();
                    }
                    else if (column.ColumnName == "Status")
                    {
                        Button approvalButton = new Button();
                        approvalButton.Click += approvalButton_Click;
                        switch (int.Parse(row[column.ColumnName].ToString()))
                        {
                            case 2:
                            case 3:
                                approvalButton.Text = "Approve";
                                tableCell.BackColor = Color.LightYellow;
                                approvalButton.CommandArgument = row["Id"].ToString();
                                break;
                            case 1:
                                approvalButton.Text = "Pending for Manager";
                                approvalButton.Enabled = false;
                                tableCell.BackColor = Color.LightYellow;
                                break;
                            case 4:
                                approvalButton.Text = "Approved";
                                tableCell.BackColor = Color.LightGreen;
                                approvalButton.Enabled = false;
                                break;
                            case 0:
                                approvalButton.Text = "Rejected";
                                approvalButton.Enabled = false;
                                break;
                        }
                        approvalButton.CommandArgument = row["Id"].ToString() + ";" + row["Status"].ToString();
                        tableCell.Controls.Add(approvalButton);
                    }
                    else
                    {
                        tableCell.Text = row[column.ColumnName].ToString();
                    }

                    tableRow.Cells.Add(tableCell);
                }
                EmployessExpenseTable.Rows.Add(tableRow);
            }
        }

        protected void approvalButton_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (!string.IsNullOrEmpty(button.CommandArgument))
            {
                string[] ids = button.CommandArgument.Split(';');
                int expenseId = int.Parse(ids[0]);
                int statusCode = int.Parse(ids[1]);
                ApproveExpense(expenseId, statusCode);
            }
        }

        private void ApproveExpense(int expenseId, int statusCode)
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            SqlCommand command = new SqlCommand("ApproveExpense", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            SqlParameter expenseIdParameter = new SqlParameter("expenseId", expenseId);
            SqlParameter statusCodeParameter = new SqlParameter("statusCode", statusCode);
            SqlParameter loggedInUserIdParameter = new SqlParameter("loggedInUserId", LoggedInUserId);
            command.Parameters.Add(expenseIdParameter);
            command.Parameters.Add(statusCodeParameter);
            command.Parameters.Add(loggedInUserIdParameter);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }

            Response.Redirect(Request.RawUrl, false);
        }

        private DataTable GetMyEmployeesExpense()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM [dbo].[Expense] WHERE Status IN (0, 1, 2, 3, 4)", connection);
                //command.CommandType = CommandType.StoredProcedure;
               // SqlParameter managerId = new SqlParameter("managerId", Session["LoggedInUserId"].ToString());
                //command.Parameters.Add(managerId);
                SqlDataAdapter sda = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;
            }
        }

        private void PopulateEmployeeDropDownList()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            SqlCommand command = new SqlCommand("GetEmployees", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            DataSet EmployeedataSet = new DataSet();
            dataAdapter.Fill(EmployeedataSet);

            EmployeeListDDL.DataTextField = EmployeedataSet.Tables[0].Columns["Name"].ToString();
            EmployeeListDDL.DataValueField = EmployeedataSet.Tables[0].Columns["Id"].ToString();
            EmployeeListDDL.DataSource = EmployeedataSet.Tables[0];
            EmployeeListDDL.DataBind();

            ListItem defaultListItem = new ListItem("-Select one-", "0");
            EmployeeListDDL.Items.Insert(0, defaultListItem);

            connection.Close();
        }

        private void PopulateGroupsList()
        {
            DataTable dataTable = GetAllGroups();

            foreach (DataRow row in dataTable.Rows)
            {
                TableRow tableRow = new TableRow();
                foreach (DataColumn column in dataTable.Columns)
                {
                    TableCell tableCell = new TableCell();
                    tableCell.Text = row[column.ColumnName].ToString();
                    tableRow.Cells.Add(tableCell);
                }
                GroupsTable.Rows.Add(tableRow);
            }
        }

        private DataTable GetAllGroups()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("SELECT Groups.Id, Groups.Name, Employee.Name FROM [dbo].[GROUPS] INNER JOIN [dbo].[Employee] ON Groups.ManagerId = Employee.Id", connection);
                SqlDataAdapter sda = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;
            }
        }

        private void PopulateGroupDropDownList()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            SqlCommand command = new SqlCommand("GetGroups", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            DataSet groupDataSet = new DataSet();
            dataAdapter.Fill(groupDataSet);

            GroupDDL.DataTextField = groupDataSet.Tables[0].Columns["Name"].ToString();
            GroupDDL.DataValueField = groupDataSet.Tables[0].Columns["Id"].ToString();
            GroupDDL.DataSource = groupDataSet.Tables[0];
            GroupDDL.DataBind();

            ListItem defaultListItem = new ListItem("-Select one-", "0");
            GroupDDL.Items.Insert(0, defaultListItem);

            connection.Close();
        }

        private int DetermineStatusofExpense(string amountText)
        {
            int amount = int.Parse(amountText);
            int status = 5;

            if (amount >= 5000)
            {
                status = 2;
            }
            else if (amount >= 1000 && amount < 5000)
            {
                status = 1;
            }
            else if (amount < 1000)
            {
                status = 4;
            }

            return status;
        }

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Login.aspx", false);
        }

        protected void ReloadButton_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.RawUrl, false);
        }
    }
}