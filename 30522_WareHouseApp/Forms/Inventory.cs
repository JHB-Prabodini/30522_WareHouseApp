using _30522_WareHouseApp.Data;
using _30522_WareHouseApp.Forms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace _30522_WareHouseApp
{
    public partial class Inventory : Form
    {
        private int selectedItemId = -1;
        private DataTable inventoryTable;

        public Inventory()
        {
            InitializeComponent();
        }

        private void Inventory_Load(object sender, EventArgs e)
        {
            ConfigureDataGridViewLayout();
            PopulateCategoryComboBox();
            LoadInventoryData();
            HighlightActiveSidebar();
        }

        private void HighlightActiveSidebar()
        {
            Control lblInv = FindControlRecursive("lblNavInventory", "label2");
            Control lblCust = FindControlRecursive("lblNavCustomer", "label4");
            Control lblEmp = FindControlRecursive("lblNavEmployee", "label3");

            if (lblInv is Label l1) l1.Font = new Font("Verdana", 11.5F, FontStyle.Bold);
            if (lblCust is Label l2) l2.Font = new Font("Verdana", 11.5F, FontStyle.Regular);
            if (lblEmp is Label l3) l3.Font = new Font("Verdana", 11.5F, FontStyle.Regular);
        }

        private void PopulateCategoryComboBox()
        {
            Control cbControl = FindControlRecursive("comboBox1", "cmbCategory", "textBox2");
            if (cbControl is ComboBox cb)
            {
                cb.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                if (cb.Items.Count == 0)
                {
                    cb.Items.AddRange(new object[] { "Bathware", "Electronics", "Hardware", "Perishables", "General" });
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

            dataGridView1.CellFormatting -= dataGridView1_CellFormatting;
            dataGridView1.CellFormatting += new DataGridViewCellFormattingEventHandler(dataGridView1_CellFormatting);

            Control searchBox = FindControlRecursive("txtSearch", "searchBox");
            if (searchBox is System.Windows.Forms.TextBox tb)
            {
                tb.TextChanged -= txtSearch_TextChanged;
                tb.TextChanged += txtSearch_TextChanged;
                tb.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            }

            foreach (string name in new[] { "textBox1", "txtName", "txtItemName", "textBox3", "txtQuantity", "txtQty", "textBox4", "txtUnitPrice", "txtPrice", "textBox5", "txtSupplier", "txtSupplierName" })
            {
                Control c = FindControlRecursive(name);
                if (c is System.Windows.Forms.TextBox tBox)
                {
                    tBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
                }
            }

            Control totalLbl = FindControlRecursive("lblTotalStockVal");
            if (totalLbl != null) totalLbl.Font = new Font("Segoe UI", 14F, FontStyle.Regular);

            Control lowLbl = FindControlRecursive("lblLowStockVal");
            if (lowLbl != null) lowLbl.Font = new Font("Segoe UI", 14F, FontStyle.Regular);
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Quantity" && e.Value != null && e.Value != DBNull.Value)
            {
                if (int.TryParse(e.Value.ToString(), out int qty))
                {
                    e.CellStyle.ForeColor = qty <= 10 ? Color.Red : Color.Black;
                }
            }
        }

        private void LoadInventoryData()
        {
            try
            {
                selectedItemId = -1;
                ClearInputFields();

                string query = "SELECT ItemId, ItemName, Category, Quantity, UnitPrice, SupplierName, LastUpdated FROM Inventory ORDER BY ItemId ASC";
                inventoryTable = DbHelper.ExecuteQuery(query);
                dataGridView1.DataSource = inventoryTable;

                if (dataGridView1.Columns["ItemId"] != null)
                {
                    dataGridView1.Columns["ItemId"].HeaderText = "Item ID";
                    dataGridView1.Columns["ItemId"].FillWeight = 60;
                }
                if (dataGridView1.Columns["ItemName"] != null)
                {
                    dataGridView1.Columns["ItemName"].HeaderText = "Item Name";
                    dataGridView1.Columns["ItemName"].FillWeight = 110;
                }
                if (dataGridView1.Columns["Category"] != null)
                {
                    dataGridView1.Columns["Category"].HeaderText = "Category";
                    dataGridView1.Columns["Category"].FillWeight = 90;
                }
                if (dataGridView1.Columns["Quantity"] != null)
                {
                    dataGridView1.Columns["Quantity"].HeaderText = "Quantity";
                    dataGridView1.Columns["Quantity"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.Columns["Quantity"].FillWeight = 75;
                }
                if (dataGridView1.Columns["UnitPrice"] != null)
                {
                    dataGridView1.Columns["UnitPrice"].HeaderText = "Unit Price";
                    dataGridView1.Columns["UnitPrice"].DefaultCellStyle.Format = "Rs #,##0.00";
                    dataGridView1.Columns["UnitPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dataGridView1.Columns["UnitPrice"].FillWeight = 95;
                }
                if (dataGridView1.Columns["SupplierName"] != null)
                {
                    dataGridView1.Columns["SupplierName"].HeaderText = "Supplier Name";
                    dataGridView1.Columns["SupplierName"].FillWeight = 100;
                }
                if (dataGridView1.Columns["LastUpdated"] != null)
                {
                    dataGridView1.Columns["LastUpdated"].HeaderText = "Last Updated";
                    dataGridView1.Columns["LastUpdated"].DefaultCellStyle.Format = "M/d/yyyy hh:mm tt";
                    dataGridView1.Columns["LastUpdated"].FillWeight = 105;
                }

                UpdateDashboardMetrics(inventoryTable);

                Control searchBox = FindControlRecursive("txtSearch", "searchBox");
                if (searchBox is System.Windows.Forms.TextBox tb)
                {
                    txtSearch_TextChanged(tb, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading inventory data: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            if (inventoryTable == null) return;

            string keyword = "";
            if (sender is System.Windows.Forms.TextBox tb)
            {
                keyword = tb.Text.Trim().Replace("'", "''");
            }

            if (string.IsNullOrEmpty(keyword))
            {
                inventoryTable.DefaultView.RowFilter = string.Empty;
            }
            else
            {
                inventoryTable.DefaultView.RowFilter = string.Format("ItemName LIKE '{0}%'", keyword);
            }
        }

        private void UpdateDashboardMetrics(DataTable dt)
        {
            if (dt == null) return;

            int totalStock = 0;
            int lowStockAlerts = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (row["Quantity"] != DBNull.Value)
                {
                    int qty = Convert.ToInt32(row["Quantity"]);
                    totalStock += qty;
                    if (qty <= 10) lowStockAlerts++;
                }
            }

            Control totalLbl = FindControlRecursive("lblTotalStockVal");
            if (totalLbl != null)
            {
                totalLbl.Text = totalStock.ToString("N0");
                totalLbl.Font = new Font("Segoe UI", 14F, FontStyle.Regular);
            }

            Control lowLbl = FindControlRecursive("lblLowStockVal");
            if (lowLbl != null)
            {
                lowLbl.Text = lowStockAlerts.ToString();
                lowLbl.ForeColor = lowStockAlerts > 0 ? Color.Red : Color.DarkGreen;
                lowLbl.Font = new Font("Segoe UI", 14F, FontStyle.Regular);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string itemName = GetFieldValue("textBox1", "txtName", "txtItemName");
            if (string.IsNullOrWhiteSpace(itemName))
            {
                MessageBox.Show("Please enter an Item Name.", "Validation Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string categoryText = GetFieldValue("comboBox1", "textBox2", "cmbCategory");
                int qtyVal = int.TryParse(GetFieldValue("textBox3", "txtQuantity", "txtQty"), out int q) ? q : 0;
                decimal priceVal = decimal.TryParse(GetFieldValue("textBox4", "txtUnitPrice", "txtPrice"), out decimal p) ? p : 0.00m;
                string supplierVal = GetFieldValue("textBox5", "txtSupplier", "txtSupplierName");

                string query = "INSERT INTO Inventory (ItemName, Category, Quantity, UnitPrice, SupplierName, LastUpdated) " +
                               "VALUES (@name, @cat, @qty, @price, @sup, GETDATE())";

                SqlParameter[] parameters = {
                    new SqlParameter("@name", itemName),
                    new SqlParameter("@cat", categoryText),
                    new SqlParameter("@qty", qtyVal),
                    new SqlParameter("@price", priceVal),
                    new SqlParameter("@sup", supplierVal)
                };

                int rowsAffected = DbHelper.ExecuteNonQuery(query, parameters);
                if (rowsAffected > 0)
                {
                    MessageBox.Show("New inventory item added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadInventoryData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) => PopulateSelectedRowData(e.RowIndex);
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e) => PopulateSelectedRowData(e.RowIndex);

        private void PopulateSelectedRowData(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < dataGridView1.Rows.Count && dataGridView1.Rows[rowIndex].Cells[0].Value != null)
            {
                DataGridViewRow row = dataGridView1.Rows[rowIndex];

                if (row.Cells["ItemId"].Value != DBNull.Value)
                {
                    selectedItemId = Convert.ToInt32(row.Cells["ItemId"].Value);
                }

                SetFieldValue(row.Cells["ItemName"].Value?.ToString(), "textBox1", "txtName", "txtItemName");
                SetFieldValue(row.Cells["Category"].Value?.ToString(), "comboBox1", "textBox2", "cmbCategory");
                SetFieldValue(row.Cells["Quantity"].Value?.ToString(), "textBox3", "txtQuantity", "txtQty");
                SetFieldValue(row.Cells["UnitPrice"].Value?.ToString(), "textBox4", "txtUnitPrice", "txtPrice");

                if (dataGridView1.Columns.Contains("SupplierName"))
                {
                    SetFieldValue(row.Cells["SupplierName"].Value?.ToString(), "textBox5", "txtSupplier", "txtSupplierName");
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                PopulateSelectedRowData(dataGridView1.SelectedRows[0].Index);
            }
            else
            {
                MessageBox.Show("Please click on a row in the table to select an item.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (selectedItemId == -1)
            {
                MessageBox.Show("Please select an item from the table to update.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string itemName = GetFieldValue("textBox1", "txtName", "txtItemName");
                string categoryText = GetFieldValue("comboBox1", "textBox2", "cmbCategory");
                int qtyVal = int.TryParse(GetFieldValue("textBox3", "txtQuantity", "txtQty"), out int q) ? q : 0;
                decimal priceVal = decimal.TryParse(GetFieldValue("textBox4", "txtUnitPrice", "txtPrice"), out decimal p) ? p : 0.00m;
                string supplierVal = GetFieldValue("textBox5", "txtSupplier", "txtSupplierName");

                string query = "UPDATE Inventory SET ItemName = @name, Category = @cat, Quantity = @qty, UnitPrice = @price, SupplierName = @sup, LastUpdated = GETDATE() WHERE ItemId = @id";

                SqlParameter[] parameters = {
                    new SqlParameter("@name", itemName),
                    new SqlParameter("@cat", categoryText),
                    new SqlParameter("@qty", qtyVal),
                    new SqlParameter("@price", priceVal),
                    new SqlParameter("@sup", supplierVal),
                    new SqlParameter("@id", selectedItemId)
                };

                int rowsAffected = DbHelper.ExecuteNonQuery(query, parameters);
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Inventory record updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadInventoryData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating item: " + ex.Message, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (selectedItemId == -1 && dataGridView1.SelectedRows.Count > 0)
            {
                PopulateSelectedRowData(dataGridView1.SelectedRows[0].Index);
            }

            if (selectedItemId == -1)
            {
                MessageBox.Show("Please click on a row in the table first to select an item to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show("Are you sure you want to delete this inventory item?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM Inventory WHERE ItemId = @id";
                    SqlParameter[] parameters = { new SqlParameter("@id", selectedItemId) };

                    int rowsAffected = DbHelper.ExecuteNonQuery(query, parameters);
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Item deleted successfully!", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadInventoryData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting item: " + ex.Message, "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearInputFields()
        {
            SetFieldValue("", "textBox1", "txtName", "txtItemName");
            Control cb = FindControlRecursive("comboBox1", "textBox2", "cmbCategory");
            if (cb is ComboBox cBox) { cBox.SelectedIndex = -1; cBox.Text = ""; }
            SetFieldValue("", "textBox3", "txtQuantity", "txtQty");
            SetFieldValue("", "textBox4", "txtUnitPrice", "txtPrice");
            SetFieldValue("", "textBox5", "txtSupplier", "txtSupplierName");
            selectedItemId = -1;
        }

        private void btnClear_Click(object sender, EventArgs e) => ClearInputFields();
        private void btnLoad_Click(object sender, EventArgs e) => LoadInventoryData();

        // --- Standardized Sidebar Navigation & Logout ---
        private void navCustomer_Click(object sender, EventArgs e) { Customer cust = new Customer(); cust.Show();  }
        private void navEmployee_Click(object sender, EventArgs e) { Forms.Employee emp = new Forms.Employee(); emp.Show();}

        private void label2_Click(object sender, EventArgs e) { /* Already Inventory */ }
        private void label2_Click_1(object sender, EventArgs e) { label2_Click(sender, e); }
        private void label3_Click(object sender, EventArgs e) { navEmployee_Click(sender, e); } // Employee label
        private void label4_Click(object sender, EventArgs e) { navCustomer_Click(sender, e); } // Customer label

        private void label5_Click(object sender, EventArgs e) // Logout
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
        }
        private void label5_Click_1(object sender, EventArgs e) { label5_Click(sender, e); }

        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void panel2_Paint_1(object sender, PaintEventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox1_TextChanged_1(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void textBox3_TextChanged(object sender, EventArgs e) { }
        private void textBox4_TextChanged(object sender, EventArgs e) { }
        private void textBox5_TextChanged(object sender, EventArgs e) { }
        private void txtName_TextChanged(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label7_Click(object sender, EventArgs e) { }
        private void label8_Click(object sender, EventArgs e) { }
        private void label9_Click(object sender, EventArgs e) { }
        private void label10_Click(object sender, EventArgs e) { }
        private void label13_Click(object sender, EventArgs e) { }
        private void label15_Click(object sender, EventArgs e) { }
        private void groupBox2_Enter(object sender, EventArgs e) { }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void lblTotalStockVal_Enter(object sender, EventArgs e) { }

        private void button3_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to exit?", "Exit Application", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }
    }
}