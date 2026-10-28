using _30522_WareHouseApp.Data;
using _30522_WareHouseApp.Forms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace _30522_WareHouseApp.Forms
{
    public partial class Employee : Form
    {
        private int selectedEmployeeId = -1;
        private DataTable employeeTable;

        public Employee()
        {
            InitializeComponent();
        }

        private void Employee_Load(object sender, EventArgs e)
        {
            ConfigureDataGridViewLayout();
            PopulateAllComboBoxes();
            LoadEmployeeData();
        }

        private void PopulateAllComboBoxes()
        {
            Control cbRole = FindControlRecursive("txtPosition", "txtRole", "comboBox1");
            if (cbRole is ComboBox cbR)
            {
                cbR.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                if (cbR.Items.Count == 0)
                {
                    cbR.Items.AddRange(new object[] { "Manager", "Supervisor", "Warehouse Staff", "Inventory Specialist", "Forklift Operator", "Dispatcher", "Driver", "Clerk" });
                }
            }

            Control cbDept = FindControlRecursive("txtDepartment", "comboBox2");
            if (cbDept is ComboBox cbD)
            {
                cbD.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                if (cbD.Items.Count == 0)
                {
                    cbD.Items.AddRange(new object[] { "Inventory", "Logistics", "Warehouse Operations", "Shipping & Receiving", "Sales", "Administration", "Quality Control" });
                }
            }

            Control cbStatus = FindControlRecursive("txtStatus", "comboBox3");
            if (cbStatus is ComboBox cbS)
            {
                cbS.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                if (cbS.Items.Count == 0)
                {
                    cbS.Items.AddRange(new object[] { "Active", "Inactive", "On Leave", "Probation" });
                }
            }
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

            Control searchBox = FindControlRecursive("txtSearch", "searchBox");
            if (searchBox is System.Windows.Forms.TextBox tb)
            {
                tb.TextChanged -= txtSearch_TextChanged;
                tb.TextChanged += txtSearch_TextChanged;
                tb.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            }
        }

        private void LoadEmployeeData()
        {
            try
            {
                selectedEmployeeId = -1;
                ClearInputFields();

                string query = "SELECT EmployeeId, Name, Role, Department, Status, Salary, Contact FROM Employees ORDER BY EmployeeId ASC";
                employeeTable = DbHelper.ExecuteQuery(query);

                if (!employeeTable.Columns.Contains("Department")) employeeTable.Columns.Add("Department", typeof(string));
                if (!employeeTable.Columns.Contains("Status")) employeeTable.Columns.Add("Status", typeof(string));

                dataGridView1.DataSource = employeeTable;

                if (dataGridView1.Columns["EmployeeId"] != null)
                {
                    dataGridView1.Columns["EmployeeId"].HeaderText = "Employee ID";
                    dataGridView1.Columns["EmployeeId"].FillWeight = 60;
                }
                if (dataGridView1.Columns["Name"] != null)
                {
                    dataGridView1.Columns["Name"].HeaderText = "Full Name";
                    dataGridView1.Columns["Name"].FillWeight = 110;
                }
                if (dataGridView1.Columns["Role"] != null)
                {
                    dataGridView1.Columns["Role"].HeaderText = "Position / Role";
                    dataGridView1.Columns["Role"].FillWeight = 90;
                }
                if (dataGridView1.Columns["Department"] != null)
                {
                    dataGridView1.Columns["Department"].HeaderText = "Department";
                    dataGridView1.Columns["Department"].FillWeight = 90;
                }
                if (dataGridView1.Columns["Status"] != null)
                {
                    dataGridView1.Columns["Status"].HeaderText = "Status";
                    dataGridView1.Columns["Status"].FillWeight = 75;
                }
                if (dataGridView1.Columns["Salary"] != null)
                {
                    dataGridView1.Columns["Salary"].HeaderText = "Salary";
                    dataGridView1.Columns["Salary"].DefaultCellStyle.Format = "Rs #,##0.00";
                    dataGridView1.Columns["Salary"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dataGridView1.Columns["Salary"].FillWeight = 95;
                }
                if (dataGridView1.Columns["Contact"] != null)
                {
                    dataGridView1.Columns["Contact"].HeaderText = "Contact No";
                    dataGridView1.Columns["Contact"].FillWeight = 100;
                }

                UpdateDashboardMetrics(employeeTable);

                Control searchBox = FindControlRecursive("txtSearch", "searchBox");
                if (searchBox is System.Windows.Forms.TextBox tb)
                {
                    txtSearch_TextChanged(tb, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading employee data: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Control FindControlRecursive(params string[] names)
        {
            foreach (string name in names)
            {
                Control[] ctrls = Controls.Find(name, true);
                if (ctrls.Length > 0) return ctrls[0];
            }
            return null;
        }

        private string GetFieldValue(params string[] names)
        {
            Control c = FindControlRecursive(names);
            if (c is ComboBox cb) return cb.Text.Trim();
            if (c is System.Windows.Forms.TextBox tb) return tb.Text.Trim();
            return "";
        }

        private void SetFieldValue(string val, params string[] names)
        {
            Control c = FindControlRecursive(names);
            if (c is ComboBox cb) cb.Text = val ?? "";
            else if (c is System.Windows.Forms.TextBox tb) tb.Text = val ?? "";
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (employeeTable == null) return;

            string keyword = "";
            if (sender is System.Windows.Forms.TextBox tb)
            {
                keyword = tb.Text.Trim().Replace("'", "''");
            }

            if (string.IsNullOrEmpty(keyword))
            {
                employeeTable.DefaultView.RowFilter = string.Empty;
            }
            else
            {
                employeeTable.DefaultView.RowFilter = string.Format("Name LIKE '{0}%' OR Role LIKE '{0}%'", keyword);
            }
        }

        private void UpdateDashboardMetrics(DataTable dt)
        {
            if (dt == null) return;

            int totalEmployees = dt.Rows.Count;
            int activeStaff = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"] != DBNull.Value && row["Status"].ToString().Equals("Active", StringComparison.OrdinalIgnoreCase))
                {
                    activeStaff++;
                }
            }

            Control totalLbl = FindControlRecursive("lblTotalEmployeesVal", "label2");
            if (totalLbl != null) totalLbl.Text = totalEmployees.ToString("N0");

            Control activeLbl = FindControlRecursive("lblActiveStaffVal", "label4");
            if (activeLbl != null) activeLbl.Text = activeStaff.ToString("N0");
        }

        // --- Add Button ---
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string empName = GetFieldValue("txtName", "textBox1");
            if (string.IsNullOrWhiteSpace(empName))
            {
                MessageBox.Show("Please enter an employee name.", "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string roleVal = GetFieldValue("txtPosition", "txtRole", "comboBox1");
                string deptVal = GetFieldValue("txtDepartment", "comboBox2");
                string statusVal = GetFieldValue("txtStatus", "comboBox3");
                decimal salaryVal = decimal.TryParse(GetFieldValue("txtSalary", "textBox5"), out decimal sal) ? sal : 0.00m;
                string contactVal = GetFieldValue("txtContact", "textBox3");

                string query = "INSERT INTO Employees (Name, Role, Department, Status, Salary, Contact) " +
                               "VALUES (@name, @role, @dept, @status, @salary, @contact)";

                SqlParameter[] parameters = {
                    new SqlParameter("@name", empName),
                    new SqlParameter("@role", roleVal),
                    new SqlParameter("@dept", deptVal),
                    new SqlParameter("@status", statusVal),
                    new SqlParameter("@salary", salaryVal),
                    new SqlParameter("@contact", contactVal)
                };

                int rowsAffected = DbHelper.ExecuteNonQuery(query, parameters);
                if (rowsAffected > 0)
                {
                    MessageBox.Show("New employee added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadEmployeeData();
                    ClearInputFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding employee: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- DataGridView Row Click (Captures ID silently without filling text boxes) ---
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) => CaptureSelectedRowId(e.RowIndex);
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e) => CaptureSelectedRowId(e.RowIndex);

        private void CaptureSelectedRowId(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < dataGridView1.Rows.Count && dataGridView1.Rows[rowIndex].Cells[0].Value != null)
            {
                DataGridViewRow row = dataGridView1.Rows[rowIndex];
                if (row.Cells["EmployeeId"].Value != DBNull.Value)
                {
                    selectedEmployeeId = Convert.ToInt32(row.Cells["EmployeeId"].Value);
                }
            }
        }

        // --- Edit Button (ONLY loads data into text boxes) ---
        private void btnEdit_Click(object sender, EventArgs e) => LoadSelectedRowForEdit();
        private void button4_Click(object sender, EventArgs e) => LoadSelectedRowForEdit();

        private void LoadSelectedRowForEdit()
        {
            int targetRowIndex = -1;
            if (dataGridView1.SelectedRows.Count > 0) targetRowIndex = dataGridView1.SelectedRows[0].Index;
            else if (dataGridView1.CurrentRow != null) targetRowIndex = dataGridView1.CurrentRow.Index;

            if (targetRowIndex >= 0 && targetRowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow row = dataGridView1.Rows[targetRowIndex];
                if (row.Cells["EmployeeId"].Value != DBNull.Value)
                {
                    selectedEmployeeId = Convert.ToInt32(row.Cells["EmployeeId"].Value);
                }

                SetFieldValue(row.Cells["Name"].Value?.ToString(), "textBox1", "txtName", "txtFullName");
                SetFieldValue(row.Cells["Role"].Value?.ToString(), "txtPosition", "txtRole", "comboBox1");
                SetFieldValue(row.Cells["Department"].Value?.ToString(), "txtDepartment", "comboBox2");
                SetFieldValue(row.Cells["Contact"].Value?.ToString(), "txtContact", "textBox3");
                SetFieldValue(row.Cells["Salary"].Value?.ToString(), "txtSalary", "textBox5");
                SetFieldValue(row.Cells["Status"].Value?.ToString(), "txtStatus", "comboBox3");


            }
            else
            {
                MessageBox.Show("Please select a row in the table first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // --- Update Button (ONLY saves changes to the database) ---
        private void btnUpdate_Click(object sender, EventArgs e) => UpdateEmployeeRecord();
        private void button2_Click(object sender, EventArgs e) => UpdateEmployeeRecord();

        private void UpdateEmployeeRecord()
        {
            if (selectedEmployeeId == -1 && dataGridView1.SelectedRows.Count > 0)
            {
                CaptureSelectedRowId(dataGridView1.SelectedRows[0].Index);
            }

            if (selectedEmployeeId == -1)
            {
                MessageBox.Show("Please select an employee from the table to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string empName = GetFieldValue("textBox1", "txtName", "txtFullName");
                string roleVal = GetFieldValue("txtPosition", "txtRole", "comboBox1");
                string deptVal = GetFieldValue("txtDepartment", "comboBox2");
                string contactVal = GetFieldValue("txtContact", "textBox3");
                decimal salaryVal = decimal.TryParse(GetFieldValue("txtSalary", "textBox5"), out decimal sal) ? sal : 0.00m;
                string statusVal = GetFieldValue("txtStatus", "comboBox3");


                string query = "UPDATE Employees SET Name = @name, Role = @role, Department = @dept, Status = @status, Salary = @salary, Contact = @contact WHERE EmployeeId = @id";

                SqlParameter[] parameters = {
                    new SqlParameter("@name", empName),
                    new SqlParameter("@role", roleVal),
                    new SqlParameter("@dept", deptVal),
                    new SqlParameter("@contact", contactVal),
                    new SqlParameter("@salary", salaryVal),
                    new SqlParameter("@status", statusVal),
                    new SqlParameter("@id", selectedEmployeeId)
                };

                int rowsAffected = DbHelper.ExecuteNonQuery(query, parameters);
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Employee record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadEmployeeData();
                    ClearInputFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating employee: " + ex.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- Delete Button ---
        private void btnDelete_Click(object sender, EventArgs e) => DeleteEmployeeRecord();
        private void button3_Click(object sender, EventArgs e) => DeleteEmployeeRecord();

        private void DeleteEmployeeRecord()
        {
            if (selectedEmployeeId == -1 && dataGridView1.SelectedRows.Count > 0)
            {
                CaptureSelectedRowId(dataGridView1.SelectedRows[0].Index);
            }

            if (selectedEmployeeId == -1)
            {
                MessageBox.Show("Please click on a row in the table first to select an employee to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show("Are you sure you want to delete this employee record?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM Employees WHERE EmployeeId = @id";
                    SqlParameter[] parameters = { new SqlParameter("@id", selectedEmployeeId) };

                    int rowsAffected = DbHelper.ExecuteNonQuery(query, parameters);
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Employee deleted successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadEmployeeData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting employee: " + ex.Message, "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearInputFields()
        {
            SetFieldValue("", "textBox1", "txtName", "txtFullName");
            SetFieldValue("", "txtPosition", "txtRole", "comboBox1");
            SetFieldValue("", "txtDepartment", "comboBox2");
            SetFieldValue("", "txtStatus", "comboBox3");
            SetFieldValue("", "txtSalary", "textBox5");
            SetFieldValue("", "txtContact", "textBox3");
            selectedEmployeeId = -1;
        }

        private void btnClear_Click(object sender, EventArgs e) => ClearInputFields();
        private void btnLoad_Click(object sender, EventArgs e) => LoadEmployeeData();


        // --- Sidebar Navigation ---
        // --- Standardized Sidebar Navigation ---
        private void navInventory_Click(object sender, EventArgs e)
        {
            Inventory inv = new Inventory();
            inv.Show();
        }

        private void navCustomer_Click(object sender, EventArgs e)
        {
            Customer cust = new Customer();
            cust.Show();
        }

        private void navEmployee_Click(object sender, EventArgs e) { /* Already here */ }

        // Label fallback hooks & Active Bold Styling
        private void label2_Click(object sender, EventArgs e) { navInventory_Click(sender, e); } // Inventory label
        private void label4_Click(object sender, EventArgs e) { navCustomer_Click(sender, e); } // Customer label
        private void label3_Click(object sender, EventArgs e) { } // Employee label (Self)

        private void Employee_Load_SidebarStyling()
        {
            // Bold Employee, regular for others (adjust label control names if needed)
            Control lblEmp = FindControlRecursive("lblNavEmployee", "label3");
            Control lblInv = FindControlRecursive("lblNavInventory", "label2");
            Control lblCust = FindControlRecursive("lblNavCustomer", "label4");

            if (lblEmp is Label l1) l1.Font = new Font("Verdana", 11.5F, FontStyle.Bold);
            if (lblInv is Label l2) l2.Font = new Font("Verdana", 11.5F, FontStyle.Regular);
            if (lblCust is Label l3) l3.Font = new Font("Verdana", 11.5F, FontStyle.Regular);
        }

        private void label5_Click(object sender, EventArgs e) // Logout
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
           
        }

 
    }
}