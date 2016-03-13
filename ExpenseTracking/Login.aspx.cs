using System;
using System.Configuration;
using System.Data.SqlClient;

namespace ExpenseTracking
{
    public partial class Login : System.Web.UI.Page
    {
        private static string ConnectionString = ConfigurationManager.ConnectionStrings["AzureDatabase"].ConnectionString;
        //private static string ConnectionString = "Data Source=(localdb)\\ProjectsV12;Initial Catalog=TrackExpense;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Login_Click(object sender, EventArgs e)
        {
            //string connectionString = "Data Source=(localdb)\\ProjectsV12;Initial Catalog=TrackExpense;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";
            SqlConnection connection = new SqlConnection(ConnectionString);
            SqlCommand command = new SqlCommand("CheckUser", connection);
            command.CommandType = System.Data.CommandType.StoredProcedure;
            SqlParameter userEmail = new SqlParameter("email", EmailTextBox.Text);
            SqlParameter password = new SqlParameter("password", PasswordTextBox.Text);
            command.Parameters.Add(userEmail);
            command.Parameters.Add(password);
            try
            {
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int employeeType = int.Parse(reader["EmployeeType"].ToString());
                        Session["LoggedInUserId"] = null;
                        Session["LoggedInUserId"] = int.Parse(reader["Id"].ToString());
                        RedirectUsers(employeeType);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageLabel.Text = "Oops! Something went wrong. Please contact Administrator.";
            }
        }

        private void RedirectUsers(int employeeType)
        {
            switch (employeeType)
            {
                case 1:
                    Response.Redirect("~/HeadofDepartment.aspx", false);
                    break;
                case 2:
                    Response.Redirect("~/Manager.aspx", false);
                    break;
                case 3:
                    Response.Redirect("~/Employee.aspx", false);
                    break;
                default:
                    Response.Redirect("~/Login.aspx", false);
                    break;
            }
        }
    }
}