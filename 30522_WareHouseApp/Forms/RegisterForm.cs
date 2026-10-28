using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using _30522_WareHouseApp.Data;

namespace _30522_WareHouseApp.Forms
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void RegisterForm_Load(object sender, EventArgs e)
        {
            if (cmbRole.Items.Count == 0)
            {
                cmbRole.Items.Add("Admin");
                cmbRole.Items.Add("Employee");
                cmbRole.Items.Add("ShippingOperator");
                cmbRole.SelectedIndex = 1;
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter both Username and Password.", "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string query = "INSERT INTO Users (Username, Password, Role) VALUES (@user, @pass, @role)";
                SqlParameter[] parameters = {
                    new SqlParameter("@user", txtUsername.Text.Trim()),
                    new SqlParameter("@pass", txtPassword.Text.Trim()),
                    new SqlParameter("@role", cmbRole.SelectedItem != null ? cmbRole.SelectedItem.ToString() : "Employee")
                };

                int rowsAffected = DbHelper.ExecuteNonQuery(query, parameters);
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Account created successfully! You can now log in.", "Registration Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoginForm login = new LoginForm();
                    login.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Registration Error: " + ex.Message, "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblLoginLink_Click(object sender, EventArgs e)
        {
            LoginForm login = new LoginForm();
            login.Show();
            this.Close();
        }
    }
}