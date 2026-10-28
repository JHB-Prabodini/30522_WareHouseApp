using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using _30522_WareHouseApp.Data;

namespace _30522_WareHouseApp.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter both Username and Password.", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "SELECT UserId, Username, Role FROM Users WHERE Username = @user AND Password = @pass";
                SqlParameter[] parameters = {
                    new SqlParameter("@user", txtUsername.Text.Trim()),
                    new SqlParameter("@pass", txtPassword.Text.Trim())
                };

                DataTable dt = DbHelper.ExecuteQuery(query, parameters);

                if (dt.Rows.Count > 0)
                {
                    string username = dt.Rows[0]["Username"].ToString();
                    string role = dt.Rows[0]["Role"].ToString();

                    MessageBox.Show($"Welcome, {username}!", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Inventory inventoryDashboard = new Inventory();
                    inventoryDashboard.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid Username or Password.", "Authentication Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection error: " + ex.Message, "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblRegisterLink_Click(object sender, EventArgs e)
        {
            RegisterForm register = new RegisterForm();
            register.Show();
            this.Hide();
        }
    }
}