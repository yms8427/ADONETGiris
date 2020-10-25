using BilgeAdam.AdoNet.FormApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace BilgeAdam.AdoNet.FormApp
{
    public partial class frmProducts : Form
    {
        public frmProducts()
        {
            InitializeComponent();
        }
        private readonly string connectionString = "Server=.;Database=Northwind;User Id=sa;Password=123";
        private List<ProductListDto> products = new List<ProductListDto>();
        public int SelectedCategoryId { get; set; }

        private void frmMain_Load(object sender, EventArgs e)
        {
            var list = new List<ComboBoxSourceDto>();
            var query = "SELECT CategoryId AS Id, CategoryName AS Name FROM Categories";
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                var command = new SqlCommand(query, connection);
                var reader = command.ExecuteReader();
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

            cmbCategories.DataSource = list;
            cmbCategories.DisplayMember = nameof(ComboBoxSourceDto.Text);
            cmbCategories.ValueMember = nameof(ComboBoxSourceDto.Value);
        }

        private void cmbCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            var category = cmbCategories.SelectedItem as ComboBoxSourceDto;
            if (SelectedCategoryId == category.Value)
            {
                return;
            }
            SelectedCategoryId = category.Value;
            var query = $"SELECT ProductID AS Id, ProductName AS Name, UnitPrice AS Price, UnitsInStock AS Stock FROM Products WHERE CategoryId = {SelectedCategoryId}";
            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                var command = new SqlCommand(query, connection);
                var reader = command.ExecuteReader();
                products.Clear();
                while (reader.Read())
                {
                    var p = new ProductListDto
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Price = Convert.ToDouble(reader["Price"]),
                        Stock = Convert.ToInt32(reader["Stock"])
                    };
                    products.Add(p);
                }
                dgvProducts.DataSource = null;
                dgvProducts.DataSource = products;
            }
        }
    }
}
