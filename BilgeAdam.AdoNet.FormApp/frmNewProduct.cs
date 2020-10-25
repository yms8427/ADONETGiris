using BilgeAdam.AdoNet.FormApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BilgeAdam.AdoNet.FormApp
{
    public partial class frmNewProduct : Form
    {
        public frmNewProduct()
        {
            InitializeComponent();
        }

        private void frmNewProduct_Load(object sender, EventArgs e)
        {
            using (var connection = CreateConnection())
            {
                LoadCategories(connection);
                LoadSuppliers(connection);
            }
        }

        private void LoadSuppliers(SqlConnection connection)
        {
            var query = "SELECT SupplierId AS Id, CompanyName AS Name FROM Suppliers";
            LoadComboBox(connection, cmbSuppliers, query);
        }

        private void LoadCategories(SqlConnection connection)
        {
            var query = "SELECT CategoryId AS Id, CategoryName AS Name FROM Categories";
            LoadComboBox(connection, cmbCategories, query);
        }

        private SqlConnection CreateConnection()
        {
            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["NrthCnnStr"].ConnectionString);
            connection.Open();
            return connection;
        }

        private void LoadComboBox(SqlConnection connection, ComboBox comboBox, string query)
        {
            var list = new List<ComboBoxSourceDto>();
            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var c = new ComboBoxSourceDto
                        {
                            Value = Convert.ToInt32(reader["Id"]),
                            Text = reader["Name"].ToString()
                        };
                        list.Add(c);
                    }
                }
            }
            comboBox.DataSource = list;
            comboBox.DisplayMember = nameof(ComboBoxSourceDto.Text);
            comboBox.ValueMember = nameof(ComboBoxSourceDto.Value);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(!ValidateProduct())
            {
                return;
            }
            var categoryId = (cmbCategories.SelectedItem as ComboBoxSourceDto).Value;
            var supplierId = (cmbSuppliers.SelectedItem as ComboBoxSourceDto).Value;
            var query = $"INSERT INTO Products(ProductName, CategoryID, SupplierID, UnitPrice, UnitsInStock) VALUES(@name, @cid, @sid, @price, @stock)";
            using (var connection = CreateConnection())
            {
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", txtName.Text);
                command.Parameters.AddWithValue("@cid", categoryId);
                command.Parameters.AddWithValue("@sid", supplierId);
                command.Parameters.Add(new SqlParameter("@price", nudPrice.Value));
                command.Parameters.Add(new SqlParameter("@stock", nudStock.Value));
                var result = command.ExecuteNonQuery();
                if (result > 0)
                {
                    MessageBox.Show("Ürün Kaydı Tamamlandı", "Northwind", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Ürün Kaydı Tamamlanamadı", "Northwind", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private bool ValidateProduct()
        {
            erp.Clear();
            var result = true;
            if (string.IsNullOrEmpty(txtName.Text.Trim()))
            {
                erp.SetError(txtName, "Ürün adı boş bırakılamaz");
                result &= false;
            }
            if (txtName.Text.Length > 35)
            {
                erp.SetError(txtName, "Ürün adı çok uzun");
                result &= false;
            }
            if (nudPrice.Value == 0)
            {
                erp.SetError(nudPrice, "Ürün fiyatı geçersiz");
                result &= false;
            }
            return result;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtName.Clear();
            nudPrice.Value = nudPrice.Minimum;
            nudStock.Value = nudStock.Minimum;
            cmbCategories.SelectedIndex = 0;
            cmbSuppliers.SelectedIndex = 0;
        }
    }
}
