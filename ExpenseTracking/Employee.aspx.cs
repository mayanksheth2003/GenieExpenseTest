using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace ExpenseTracking
{
    public partial class Employee : System.Web.UI.Page
    {
        private static string ConnectionString = ConfigurationManager.ConnectionStrings["AzureDatabase"].ConnectionString;
        //private static string ConnectionString = "Data Source=(localdb)\\ProjectsV12;Initial Catalog=TrackExpense;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";
        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateExpenseHistory();
        }

        protected void AddExpenseButton_Click(object sender, EventArgs e)
        {
            int status = DetermineStatusofExpense(AmountTextBox.Text);
            int employeeId = string.IsNullOrEmpty(Session["LoggedInUserId"].ToString()) ? -1 : int.Parse(Session["LoggedInUserId"].ToString());

            SqlConnection connection = new SqlConnection(ConnectionString);
            SqlCommand command = new SqlCommand("AddExpense", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;

            SqlParameter description = new SqlParameter("description", DescriptionTextBox.Text);
            SqlParameter date = new SqlParameter("date", Convert.ToDateTime(HiddenDateField.Value.ToString()));
            SqlParameter amount = new SqlParameter("amount", AmountTextBox.Text);
            SqlParameter statusParameter = new SqlParameter("status", status);
            SqlParameter employeeIdParameter = new SqlParameter("employeeId", employeeId);

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

        private DataTable GetExpenseData()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand command = new SqlCommand("GetExpenses", connection))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter(command))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
        }

        private void PopulateExpenseHistory()
        {
            DataTable dataTable = GetExpenseData();

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
                        switch (int.Parse(row[column.ColumnName].ToString()))
                        {
                            case 1:
                                tableCell.Text = "Pending for Manager's Approval";
                                break;
                            case 2:
                                tableCell.Text = "Pending for Head of Department's Approval";
                                break;
                            case 3:
                                tableCell.Text = "Pending for Manager's and Head of Department's Approval";
                                break;
                            case 4:
                                tableCell.Text = "Approved";
                                break;
                            case 0:
                                tableCell.Text = "Rejected";
                                break;
                            default:
                                tableCell.Text = "Pending";
                                break;
                        }
                    }
                    else
                    {
                        tableCell.Text = row[column.ColumnName].ToString();
                    }
                    
                    tableRow.Cells.Add(tableCell);
                }
                ExpenseTable.Rows.Add(tableRow);
            }
        }

        private int DetermineStatusofExpense(string amountText)
        {
            int amount = int.Parse(amountText);
            int status = 5;

            if (amount >= 5000)
            {
                status = 3;
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