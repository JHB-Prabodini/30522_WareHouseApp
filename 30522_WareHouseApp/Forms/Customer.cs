using _30522_WareHouseApp.Data;
using _30522_WareHouseApp.Forms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace _30522_WareHouseApp.Forms
{
    public partial class Customer : Form
    {
        private int selectedCustomerId = -1;
        private DataTable customerTable;

        public Customer()
        {
            InitializeComponent();
        }

        private void Customer_Load(object sender, EventArgs e)
        {
            ConfigureDataGridViewLayout();
            LoadCustomerData();
            Customer_Load_SidebarStyling();
        }

        private void ConfigureDataGridViewLayout()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.RowHeadersVisible = true;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.BorderStyle = BorderStyle.Fixed3D;

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;

            if (txtSearch != null)
            {
                txtSearch.TextChanged -= txtSearch_TextChanged;
                txtSearch.TextChanged += txtSearch_TextChanged;
                txtSearch.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            }
        }

        private void LoadCustomerData()
        {
            try
            {
                selectedCustomerId = -1;
                ClearInputFields();

                string query = "SELECT CustomerId, Name, Address, Phone, Email FROM Customers";
                customerTable = DbHelper.ExecuteQuery(query);

                dataGridView1.DataSource = customerTable;

                if (dataGridView1.Columns["CustomerId"] != null) dataGridView1.Columns["CustomerId"].HeaderText = "Customer ID";
                if (dataGridView1.Columns["Name"] != null) dataGridView1.Columns["Name"].HeaderText = "Full Name";
                if (dataGridView1.Columns["Address"] != null) dataGridView1.Columns["Address"].HeaderText = "Address";
                if (dataGridView1.Columns["Phone"] != null) dataGridView1.Columns["Phone"].HeaderText = "Phone";
                if (dataGridView1.Columns["Email"] != null) dataGridView1.Columns["Email"].HeaderText = "Email";

                UpdateDashboardMetrics(customerTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading customer data: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (customerTable == null || txtSearch == null) return;

            string keyword = txtSearch.Text.Trim().Replace("'", "''");

            if (string.IsNullOrEmpty(keyword))
            {
                customerTable.DefaultView.RowFilter = string.Empty;
            }
            else
            {
                customerTable.DefaultView.RowFilter = string.Format("Name LIKE '{0}%' OR Phone LIKE '{0}%'", keyword);
            }
        }

        private void UpdateDashboardMetrics(DataTable dt)
        {
            if (dt == null) return;
            int totalCustomers = dt.Rows.Count;

            if (lblTotalEmployeesVal != null) lblTotalEmployeesVal.Text = totalCustomers.ToString("N0");
            if (lblActiveStaffVal != null) lblActiveStaffVal.Text = totalCustomers.ToString("N0");
        }

        // --- Add Button ---
        private void btnAdd_Click(object sender, EventArgs e) => AddCustomerRecord();

        private void AddCustomerRecord()
        {
            string custName = txtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(custName))
            {
                MessageBox.Show("Please enter a customer name.", "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string addressVal = textBox1.Text.Trim();   // Address TextBox
                string phoneVal = txtContact.Text.Trim();   // Contact TextBox
                string emailVal = txtSalary.Text.Trim();    // Email TextBox (named txtSalary in designer)

                string query = "INSERT INTO Customers (Name, Address, Phone, Email) VALUES (@name, @address, @phone, @email)";

                SqlParameter[] parameters = {
                    new SqlParameter("@name", custName),
                    new SqlParameter("@address", addressVal),
                    new SqlParameter("@phone", phoneVal),
                    new SqlParameter("@email", emailVal)
                };

                int rowsAffected = DbHelper.ExecuteNonQuery(query, parameters);
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Customer added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomerData();
                    ClearInputFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding customer: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- DataGridView Cell Clicks ---
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) => PopulateSelectedRowData(e.RowIndex);
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e) => PopulateSelectedRowData(e.RowIndex);

        private void PopulateSelectedRowData(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[rowIndex];
                if (row.Cells["CustomerId"].Value != null && row.Cells["CustomerId"].Value != DBNull.Value)
                {
                    int.TryParse(row.Cells["CustomerId"].Value.ToString(), out selectedCustomerId);

                    txtName.Text = row.Cells["Name"].Value?.ToString() ?? "";
                    textBox1.Text = row.Cells["Address"].Value?.ToString() ?? "";
                    txtContact.Text = row.Cells["Phone"].Value?.ToString() ?? "";
                    txtSalary.Text = row.Cells["Email"].Value?.ToString() ?? "";
                }
            }
        }

        // --- Edit Button ---
        private void btnEdit_Click(object sender, EventArgs e)
        {
            int targetRowIndex = -1;
            if (dataGridView1.SelectedRows.Count > 0) targetRowIndex = dataGridView1.SelectedRows[0].Index;
            else if (dataGridView1.CurrentRow != null) targetRowIndex = dataGridView1.CurrentRow.Index;

            if (targetRowIndex >= 0)
            {
                PopulateSelectedRowData(targetRowIndex);
            }
            else
            {
                MessageBox.Show("Please select a row in the table first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // --- Update Button ---
        private void button2_Click(object sender, EventArgs e) => UpdateCustomerRecord();

        private void UpdateCustomerRecord()
        {
            if (selectedCustomerId == -1)
            {
                if (dataGridView1.SelectedRows.Count > 0) int.TryParse(dataGridView1.SelectedRows[0].Cells["CustomerId"].Value?.ToString(), out selectedCustomerId);
                else if (dataGridView1.CurrentRow != null) int.TryParse(dataGridView1.CurrentRow.Cells["CustomerId"].Value?.ToString(), out selectedCustomerId);
            }

            if (selectedCustomerId == -1)
            {
                MessageBox.Show("Please select a customer from the table to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string custName = txtName.Text.Trim();
                string addressVal = textBox1.Text.Trim();
                string phoneVal = txtContact.Text.Trim();
                string emailVal = txtSalary.Text.Trim();

                string query = "UPDATE Customers SET Name = @name, Address = @address, Phone = @phone, Email = @email WHERE CustomerId = @id";

                SqlParameter[] parameters = {
                    new SqlParameter("@name", custName),
                    new SqlParameter("@address", addressVal),
                    new SqlParameter("@phone", phoneVal),
                    new SqlParameter("@email", emailVal),
                    new SqlParameter("@id", selectedCustomerId)
                };

                int rowsAffected = DbHelper.ExecuteNonQuery(query, parameters);
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Customer record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadCustomerData();
                    ClearInputFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating customer: " + ex.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Delete Button ---
        private void btnDelete_Click(object sender, EventArgs e) => DeleteCustomerRecord();

        private void DeleteCustomerRecord()
        {
            if (selectedCustomerId == -1)
            {
                if (dataGridView1.SelectedRows.Count > 0) int.TryParse(dataGridView1.SelectedRows[0].Cells["CustomerId"].Value?.ToString(), out selectedCustomerId);
                else if (dataGridView1.CurrentRow != null) int.TryParse(dataGridView1.CurrentRow.Cells["CustomerId"].Value?.ToString(), out selectedCustomerId);
            }

            if (selectedCustomerId == -1)
            {
                MessageBox.Show("Please select a customer to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Are you sure you want to delete this customer record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM Customers WHERE CustomerId = @id";
                    SqlParameter[] parameters = { new SqlParameter("@id", selectedCustomerId) };

                    int rowsAffected = DbHelper.ExecuteNonQuery(query, parameters);
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Customer deleted successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadCustomerData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting customer: " + ex.Message, "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearInputFields()
        {
            txtName.Text = "";
            textBox1.Text = "";
            txtContact.Text = "";
            txtSalary.Text = "";
            selectedCustomerId = -1;
        }

        // --- Standardized Sidebar Navigation ---
        private void navInventory_Click(object sender, EventArgs e) { Inventory inv = new Inventory(); inv.Show(); this.Hide(); }
        private void navCustomer_Click(object sender, EventArgs e) { /* Already here */ }
        private void navEmployee_Click(object sender, EventArgs e) { Employee emp = new Employee(); emp.Show(); this.Hide(); }

        private void label2_Click(object sender, EventArgs e) { navInventory_Click(sender, e); }
        private void label3_Click(object sender, EventArgs e) { navEmployee_Click(sender, e); }

        private void Customer_Load_SidebarStyling()
        {
            Control lblCust = FindControlRecursive("lblNavCustomer");
            Control lblInv = FindControlRecursive("lblNavInventory");
            Control lblEmp = FindControlRecursive("lblNavEmployee");

            if (lblCust is Label l1) l1.Font = new Font("Verdana", 11.5F, FontStyle.Bold);
            if (lblInv is Label l2) l2.Font = new Font("Verdana", 11.5F, FontStyle.Regular);
            if (lblEmp is Label l3) l3.Font = new Font("Verdana", 11.5F, FontStyle.Regular);
        }

        private Control FindControlRecursive(string name)
        {
            Control[] ctrls = Controls.Find(name, true);
            if (ctrls.Length > 0) return ctrls[0];
            return null;
        }

        private void label5_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes) { LoginForm login = new LoginForm(); login.Show(); this.Close(); }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Exit Application", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes) { Application.Exit(); }
        }

        private void groupBox1_Enter(object sender, EventArgs e) { }
        private void label16_Click(object sender, EventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
    }
}