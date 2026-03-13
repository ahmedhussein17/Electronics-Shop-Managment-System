using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ElectronicsShopGUI
{
    public partial class Form1 : Form
    {
        private readonly string connectionString = @"Data Source=LAPTOP-E6KB1DCC\SQLEXPRESS;Initial Catalog=ElectronicsShopDB;Integrated Security=True;";
        private const int DefaultEmployeeId = 1;

        private Panel sidebar;
        private Button btnDashboard, btnAddProduct, btnAddStock, btnMakeSale, btnSalesHistory, btnUpdatePrice;
        private Panel mainPanel;
        private Panel lblProductsCount, lblRevenueSum;
        private DataGridView dgvProducts;

        public Form1()
        {
            InitUI();
            LoadDashboard();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(1200, 700);
        }

        private void InitUI()
        {
            this.Text = "Electronics Shop";
            this.BackColor = Color.White;

            // Sidebar
            sidebar = new Panel()
            {
                Width = 150,
                Dock = DockStyle.Left,
                BackColor = Color.FromArgb(20, 90, 160)
            };

            btnDashboard = CreateSidebarButton("Dashboard");
            btnAddProduct = CreateSidebarButton("Add Product");
            btnAddStock = CreateSidebarButton("Add Stock");
            btnMakeSale = CreateSidebarButton("Make Sale");
            btnSalesHistory = CreateSidebarButton("Sales History");
            btnUpdatePrice = CreateSidebarButton("Update Price");

            int btnTop = 30;
            foreach (var btn in new Button[] { btnDashboard, btnAddProduct, btnAddStock, btnMakeSale, btnSalesHistory, btnUpdatePrice })
            {
                btn.Top = btnTop;
                btn.Left = 10;
                sidebar.Controls.Add(btn);
                btnTop += btn.Height + 10;
            }

            btnDashboard.Click += (s, e) => LoadDashboard();
            btnAddProduct.Click += (s, e) => ShowAddProduct();
            btnAddStock.Click += (s, e) => ShowAddStock();
            btnMakeSale.Click += (s, e) => ShowMakeSale();
            btnSalesHistory.Click += (s, e) => ShowSalesHistory();
            btnUpdatePrice.Click += (s, e) => ShowUpdatePrice();

            mainPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke
            };

            this.Controls.Add(mainPanel);
            this.Controls.Add(sidebar);
        }

        private Button CreateSidebarButton(string text)
        {
            var btn = new Button()
            {
                Text = text,
                Width = 130,
                Height = 35,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(20, 90, 160),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(10, 70, 130);
            btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(20, 90, 160);
            return btn;
        }

        private void CenterControlInMainPanel(Control ctrl)
        {
            ctrl.Left = (mainPanel.ClientSize.Width - ctrl.Width) / 2;
            ctrl.Top = (mainPanel.ClientSize.Height - ctrl.Height) / 2;
            ctrl.Anchor = AnchorStyles.None;
            mainPanel.Controls.Add(ctrl);
        }

        // ----------------- DASHBOARD -----------------
        private void LoadDashboard()
        {
            mainPanel.Controls.Clear();

            Panel contentPanel = new Panel() { Width = 700, Height = 500, BackColor = Color.Transparent };

            var lblTitle = new Label()
            {
                Text = "Dashboard",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 90, 160),
                AutoSize = true,
                Top = 0,
                Left = 0
            };
            contentPanel.Controls.Add(lblTitle);

            lblProductsCount = CreateSummaryLabel("Total Products", "0", 20, 50);
            lblRevenueSum = CreateSummaryLabel("Total Revenue (EGP)", "0", 220, 50);

            contentPanel.Controls.AddRange(new Control[] { lblProductsCount, lblRevenueSum });

            dgvProducts = new DataGridView()
            {
                Left = 20,
                Top = 120,
                Width = 650,
                Height = 350,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            contentPanel.Controls.Add(dgvProducts);

            CenterControlInMainPanel(contentPanel);

            RefreshDashboardData();
        }

        private Panel CreateSummaryLabel(string title, string value, int left, int top)
        {
            var panel = new Panel() { Left = left, Top = top, Width = 180, Height = 60, BackColor = Color.Transparent };

            var lblTitle = new Label()
            {
                Text = title,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.Gray,
                AutoSize = false,
                Width = 180,
                Height = 20,
                Top = 0
            };

            var lblValue = new Label()
            {
                Text = value,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 90, 160),
                AutoSize = false,
                Width = 180,
                Height = 35,
                Top = 22
            };

            panel.Controls.Add(lblTitle);
            panel.Controls.Add(lblValue);
            panel.Tag = lblValue;

            return panel;
        }

        private void RefreshDashboardData()
        {
            int productCount = Convert.ToInt32(ExecuteScalar("SELECT COUNT(*) FROM Product"));
            ((Label)lblProductsCount.Tag).Text = productCount.ToString();

            decimal totalRevenue = Convert.ToDecimal(ExecuteScalar("SELECT ISNULL(SUM(total_amount),0) FROM SalesOrder"));
            ((Label)lblRevenueSum.Tag).Text = totalRevenue.ToString("N2");

            DataTable dt = ExecuteDataTable("SELECT p.product_id AS [ID], p.product_name AS [Name], c.category_name AS [Category], p.brand AS [Brand], p.price AS [Price (EGP)], p.stock_quantity AS [Stock] FROM Product p LEFT JOIN Category c ON p.category_id=c.category_id ORDER BY p.product_name");
            dgvProducts.DataSource = dt;
        }

        // ----------------- ADD PRODUCT -----------------
        private void ShowAddProduct()
        {
            mainPanel.Controls.Clear();
            int spacing = 40;

            var lblName = new Label() { Text = "Product Name:", Width = 120 };
            var txtName = new TextBox() { Width = 250 };

            var lblCategory = new Label() { Text = "Category:", Width = 120 };
            var cmbCategory = new ComboBox() { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };

            var lblBrand = new Label() { Text = "Brand:", Width = 120 };
            var txtBrand = new TextBox() { Width = 250 };

            var lblPrice = new Label() { Text = "Price (EGP):", Width = 120 };
            var txtPrice = new TextBox() { Width = 250 };

            var lblQty = new Label() { Text = "Initial Stock Qty:", Width = 120 };
            var txtQty = new TextBox() { Width = 250, Text = "0" };

            var btnSave = new Button() { Text = "Save", Width = 100, BackColor = Color.FromArgb(20, 90, 160), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnCancel = new Button() { Text = "Cancel", Width = 100, BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            // Position controls
            lblName.Top = 0; txtName.Top = 0;
            lblCategory.Top = spacing * 1; cmbCategory.Top = spacing * 1;
            lblBrand.Top = spacing * 2; txtBrand.Top = spacing * 2;
            lblPrice.Top = spacing * 3; txtPrice.Top = spacing * 3;
            lblQty.Top = spacing * 4; txtQty.Top = spacing * 4;
            btnSave.Top = spacing * 5; btnCancel.Top = spacing * 5;

            lblName.Left = lblCategory.Left = lblBrand.Left = lblPrice.Left = lblQty.Left = 0;
            txtName.Left = cmbCategory.Left = txtBrand.Left = txtPrice.Left = txtQty.Left = 130;
            btnSave.Left = 130; btnCancel.Left = 250;

            Panel contentPanel = new Panel() { Width = 400, Height = spacing * 6 + 50, BackColor = Color.Transparent };
            contentPanel.Controls.AddRange(new Control[] { lblName, txtName, lblCategory, cmbCategory, lblBrand, txtBrand, lblPrice, txtPrice, lblQty, txtQty, btnSave, btnCancel });
            CenterControlInMainPanel(contentPanel);

            // Load categories
            DataTable dtCat = ExecuteDataTable("SELECT category_id, category_name FROM Category ORDER BY category_name");
            cmbCategory.DataSource = dtCat;
            cmbCategory.DisplayMember = "category_name";
            cmbCategory.ValueMember = "category_id";

            btnCancel.Click += (s, e) => LoadDashboard();

            btnSave.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtName.Text) || !decimal.TryParse(txtPrice.Text, out decimal price) || !int.TryParse(txtQty.Text, out int qty) || cmbCategory.SelectedValue == null)
                {
                    MessageBox.Show("Invalid input."); return;
                }

                string sql = "INSERT INTO Product (product_name, category_id, brand, price, stock_quantity) VALUES (@name,@cat,@brand,@price,@qty)";
                using (var cn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@cat", (int)cmbCategory.SelectedValue);
                    cmd.Parameters.AddWithValue("@brand", txtBrand.Text.Trim());
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@qty", qty);
                    cn.Open(); cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Product added.");
                LoadDashboard();
            };
        }

        // ----------------- UPDATE PRICE -----------------
        private void ShowUpdatePrice()
        {
            mainPanel.Controls.Clear();
            int spacing = 40;

            var lblProduct = new Label() { Text = "Select Product:", Width = 120 };
            var cmbProducts = new ComboBox() { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            LoadProducts(cmbProducts);

            var lblPrice = new Label() { Text = "New Price (EGP):", Width = 120 };
            var txtPrice = new TextBox() { Width = 250 };

            var btnUpdate = new Button() { Text = "Update Price", Width = 150, BackColor = Color.FromArgb(20, 90, 160), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnCancel = new Button() { Text = "Cancel", Width = 150, BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            lblProduct.Top = 0; cmbProducts.Top = 0;
            lblPrice.Top = spacing; txtPrice.Top = spacing;
            btnUpdate.Top = spacing * 2; btnCancel.Top = spacing * 2;
            lblProduct.Left = lblPrice.Left = 0; cmbProducts.Left = txtPrice.Left = 130;
            btnUpdate.Left = 130; btnCancel.Left = 300;

            Panel contentPanel = new Panel() { Width = 450, Height = spacing * 3 + 50, BackColor = Color.Transparent };
            contentPanel.Controls.AddRange(new Control[] { lblProduct, cmbProducts, lblPrice, txtPrice, btnUpdate, btnCancel });
            CenterControlInMainPanel(contentPanel);

            btnCancel.Click += (s, e) => LoadDashboard();

            btnUpdate.Click += (s, e) =>
            {
                if (cmbProducts.SelectedValue == null || !decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0) { MessageBox.Show("Invalid input"); return; }

                string sql = "UPDATE Product SET price=@price WHERE product_id=@pid";
                using (var cn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@pid", (int)cmbProducts.SelectedValue);
                    cn.Open(); cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Price updated.");
                LoadDashboard();
            };
        }

        // ----------------- ADD STOCK -----------------
        private void ShowAddStock()
        {
            mainPanel.Controls.Clear();

            var lblProduct = new Label() { Text = "Select Product:", Width = 120 };
            var cmbProducts = new ComboBox() { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            LoadProducts(cmbProducts);

            var lblQty = new Label() { Text = "Quantity to Add:", Width = 120 };
            var txtQty = new TextBox() { Width = 100, Text = "1" };

            var btnAdd = new Button() { Text = "Add Stock", Width = 100, BackColor = Color.FromArgb(20, 90, 160), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnCancel = new Button() { Text = "Cancel", Width = 100, BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            int spacing = 40;
            lblProduct.Top = 0; cmbProducts.Top = 0;
            lblQty.Top = spacing; txtQty.Top = spacing;
            btnAdd.Top = spacing * 2; btnCancel.Top = spacing * 2;

            lblProduct.Left = lblQty.Left = 0; cmbProducts.Left = 130; txtQty.Left = 130;
            btnAdd.Left = 130; btnCancel.Left = 250;

            Panel contentPanel = new Panel() { Width = 400, Height = spacing * 3 + 50, BackColor = Color.Transparent };
            contentPanel.Controls.AddRange(new Control[] { lblProduct, cmbProducts, lblQty, txtQty, btnAdd, btnCancel });
            CenterControlInMainPanel(contentPanel);

            btnCancel.Click += (s, e) => LoadDashboard();

            btnAdd.Click += (s, e) =>
            {
                if (cmbProducts.SelectedValue == null || !int.TryParse(txtQty.Text, out int qty) || qty <= 0) { MessageBox.Show("Invalid input."); return; }
                string sql = "UPDATE Product SET stock_quantity = stock_quantity + @qty WHERE product_id = @pid";
                using (var cn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.AddWithValue("@qty", qty);
                    cmd.Parameters.AddWithValue("@pid", (int)cmbProducts.SelectedValue);
                    cn.Open(); cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Stock updated.");
                LoadDashboard();
            };
        }

        private void LoadProducts(ComboBox cmb)
        {
            DataTable dt = ExecuteDataTable("SELECT product_id, product_name FROM Product ORDER BY product_name");
            cmb.DataSource = dt;
            cmb.DisplayMember = "product_name";
            cmb.ValueMember = "product_id";
        }

        // ----------------- MAKE SALE -----------------
        private void ShowMakeSale()
        {
            mainPanel.Controls.Clear();

            var lblCustomer = new Label() { Text = "Customer Name:", Width = 120 };
            var txtCustomer = new TextBox() { Width = 250 };

            var lblPhone = new Label() { Text = "Phone (optional):", Width = 120 };
            var txtPhone = new TextBox() { Width = 250 };

            var lblEmail = new Label() { Text = "Email (optional):", Width = 120 };
            var txtEmail = new TextBox() { Width = 250 };

            var lblProduct = new Label() { Text = "Select Product:", Width = 120 };
            var cmbProducts = new ComboBox() { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            LoadProducts(cmbProducts);

            var lblQty = new Label() { Text = "Quantity:", Width = 120 };
            var txtQty = new TextBox() { Width = 80, Text = "1" };

            var btnAddItem = new Button() { Text = "Add Item", Width = 100, BackColor = Color.FromArgb(20, 90, 160), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var dgvOrderItems = new DataGridView() { Width = 500, Height = 200, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            var btnSaveSale = new Button() { Text = "Complete Sale", Width = 150, BackColor = Color.FromArgb(20, 90, 160), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnCancel = new Button() { Text = "Cancel", Width = 150, BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

            // Position controls
            lblCustomer.Top = 0; txtCustomer.Top = 0;
            lblPhone.Top = 30; txtPhone.Top = 30;
            lblEmail.Top = 60; txtEmail.Top = 60;
            lblProduct.Top = 90; cmbProducts.Top = 90;
            lblQty.Top = 120; txtQty.Top = 120;
            btnAddItem.Top = 90; btnAddItem.Left = 400;
            dgvOrderItems.Top = 160; btnSaveSale.Top = 380; btnSaveSale.Left = 150; btnCancel.Top = 380; btnCancel.Left = 320;

            lblCustomer.Left = lblPhone.Left = lblEmail.Left = lblProduct.Left = lblQty.Left = 0;
            txtCustomer.Left = txtPhone.Left = txtEmail.Left = cmbProducts.Left = txtQty.Left = 130;

            Panel contentPanel = new Panel() { Width = 550, Height = 450, BackColor = Color.Transparent };
            contentPanel.Controls.AddRange(new Control[] { lblCustomer, txtCustomer, lblPhone, txtPhone, lblEmail, txtEmail, lblProduct, cmbProducts, lblQty, txtQty, btnAddItem, dgvOrderItems, btnSaveSale, btnCancel });
            CenterControlInMainPanel(contentPanel);

            var orderItems = new List<OrderItem>();
            void RefreshGrid()
            {
                var dt = new DataTable();
                dt.Columns.Add("Product");
                dt.Columns.Add("Qty", typeof(int));
                dt.Columns.Add("Unit Price", typeof(decimal));
                dt.Columns.Add("Total", typeof(decimal));
                foreach (var i in orderItems) dt.Rows.Add(i.ProductName, i.Quantity, i.UnitPrice, i.Quantity * i.UnitPrice);
                dgvOrderItems.DataSource = dt;
            }
            RefreshGrid();

            btnAddItem.Click += (s, e) =>
            {
                if (cmbProducts.SelectedValue == null || !int.TryParse(txtQty.Text, out int qty) || qty <= 0) { MessageBox.Show("Invalid"); return; }
                int pid = (int)cmbProducts.SelectedValue;
                decimal price = Convert.ToDecimal(ExecuteScalar($"SELECT price FROM Product WHERE product_id={pid}"));
                string name = cmbProducts.Text;
                int stock = Convert.ToInt32(ExecuteScalar($"SELECT stock_quantity FROM Product WHERE product_id={pid}"));
                if (qty > stock) { MessageBox.Show("Not enough stock"); return; }
                orderItems.Add(new OrderItem { ProductId = pid, ProductName = name, Quantity = qty, UnitPrice = price });
                RefreshGrid();
            };

            btnSaveSale.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtCustomer.Text) || orderItems.Count == 0) { MessageBox.Show("Invalid"); return; }
                int customerId = GetOrCreateCustomer(txtCustomer.Text.Trim(), txtPhone.Text.Trim(), txtEmail.Text.Trim());
                decimal total = 0; foreach (var i in orderItems) total += i.Quantity * i.UnitPrice;

                using (var cn = new SqlConnection(connectionString))
                {
                    cn.Open();
                    var tran = cn.BeginTransaction();
                    try
                    {
                        int orderId;
                        using (var cmd = new SqlCommand("INSERT INTO SalesOrder(customer_id,employee_id,total_amount) OUTPUT INSERTED.order_id VALUES(@cid,@eid,@total)", cn, tran))
                        {
                            cmd.Parameters.AddWithValue("@cid", customerId);
                            cmd.Parameters.AddWithValue("@eid", DefaultEmployeeId);
                            cmd.Parameters.AddWithValue("@total", total);
                            orderId = (int)cmd.ExecuteScalar();
                        }

                        foreach (var item in orderItems)
                        {
                            var cmdItem = new SqlCommand("INSERT INTO SalesOrderDetails(order_id,product_id,quantity,item_price) VALUES(@oid,@pid,@qty,@price)", cn, tran);
                            cmdItem.Parameters.AddWithValue("@oid", orderId);
                            cmdItem.Parameters.AddWithValue("@pid", item.ProductId);
                            cmdItem.Parameters.AddWithValue("@qty", item.Quantity);
                            cmdItem.Parameters.AddWithValue("@price", item.UnitPrice);
                            cmdItem.ExecuteNonQuery();

                            var cmdStock = new SqlCommand("UPDATE Product SET stock_quantity = stock_quantity - @qty WHERE product_id=@pid", cn, tran);
                            cmdStock.Parameters.AddWithValue("@qty", item.Quantity);
                            cmdStock.Parameters.AddWithValue("@pid", item.ProductId);
                            cmdStock.ExecuteNonQuery();
                        }
                        tran.Commit();
                        MessageBox.Show("Sale completed.");
                        LoadDashboard();
                    }
                    catch { tran.Rollback(); MessageBox.Show("Error"); }
                }
            };

            btnCancel.Click += (s, e) => LoadDashboard();
        }

        private int GetOrCreateCustomer(string name, string phone = null, string email = null)
        {
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand("SELECT customer_id FROM Customer WHERE customer_name=@name", cn))
            {
                cmd.Parameters.AddWithValue("@name", name);
                cn.Open();
                var result = cmd.ExecuteScalar();
                if (result != null)
                    return (int)result;

                cmd.CommandText = "INSERT INTO Customer(customer_name, phone, email) OUTPUT INSERTED.customer_id VALUES(@name,@phone,@email)";
                cmd.Parameters.AddWithValue("@phone", string.IsNullOrWhiteSpace(phone) ? DBNull.Value : (object)phone);
                cmd.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(email) ? DBNull.Value : (object)email);

                return (int)cmd.ExecuteScalar();
            }
        }

        // ----------------- SALES HISTORY -----------------
        private void ShowSalesHistory()
        {
            mainPanel.Controls.Clear();

            DataTable dt = ExecuteDataTable(@"
                SELECT so.order_id AS [Order ID], c.customer_name AS [Customer], so.order_date AS [Date],
                       p.product_name AS [Product], sod.quantity AS [Qty], sod.item_price AS [Price (EGP)],
                       (sod.quantity * sod.item_price) AS [Total]
                FROM SalesOrder so
                INNER JOIN Customer c ON so.customer_id = c.customer_id
                INNER JOIN SalesOrderDetails sod ON so.order_id = sod.order_id
                INNER JOIN Product p ON sod.product_id = p.product_id
                ORDER BY so.order_date DESC
            ");

            var dgv = new DataGridView()
            {
                DataSource = dt,
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            mainPanel.Controls.Add(dgv);
        }

        // ----------------- HELPER METHODS -----------------
        private object ExecuteScalar(string sql)
        {
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, cn))
            {
                cn.Open();
                return cmd.ExecuteScalar();
            }
        }

        private DataTable ExecuteDataTable(string sql)
        {
            using (var cn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(sql, cn))
            using (var da = new SqlDataAdapter(cmd))
            {
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private class OrderItem
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
        }
    }

}
