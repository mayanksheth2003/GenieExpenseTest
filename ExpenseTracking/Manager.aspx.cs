using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI.WebControls;

namespace ExpenseTracking
{
    public partial class Manager : System.Web.UI.Page
    {
        private static string ConnectionString = ConfigurationManager.ConnectionStrings["AzureDatabase"].ConnectionString;
        //private static string ConnectionString = "Data Source=(localdb)\\ProjectsV12;Initial Catalog=TrackExpense;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";
        private int LoggedInUserId;
        protected void Page_Load(object sender, EventArgs e)
        {
            LoggedInUserId = string.IsNullOrEmpty(Session["LoggedInUserId"].ToString()) ? -1 : int.Parse(Session["LoggedInUserId"].ToString());
            PopulateMyEmployeesExpenseList();
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

        protected void BackButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Login.aspx", false);
        }

        protected void ReloadButton_Click(object sender, EventArgs e)
        {
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
                            case 1:
                            case 3:
                                approvalButton.Text = "Approve";
                                tableCell.BackColor = Color.LightYellow;
                                approvalButton.CommandArgument = row["Id"].ToString();
                                break;
                            case 2:
                                approvalButton.Text = "Pending for HOD";
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
                SqlCommand command = new SqlCommand("GetAllMyEmployeesExpense", connection);
                command.CommandType = CommandType.StoredProcedure;
                SqlParameter managerId = new SqlParameter("managerId", Session["LoggedInUserId"].ToString());
                command.Parameters.Add(managerId);
                SqlDataAdapter sda = new SqlDataAdapter(command);  
                DataTable dt = new DataTable();
                sda.Fill(dt);
                return dt;   
            }
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
    }
}