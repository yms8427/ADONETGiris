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
    public partial class frmSearch : Form
    {
        public frmSearch()
        {
            InitializeComponent();
        }
        private string baseQuery = @"SELECT TOP 20 p.ProductID, p.ProductName, p.UnitPrice, p.UnitsInStock, c.CategoryName, s.CompanyName 
                                     FROM Products AS p
                                     INNER JOIN Categories AS c ON p.CategoryID = c.CategoryID
                                     INNER JOIN Suppliers AS s ON s.SupplierID = p.SupplierID
                                     {0}
                                     ORDER BY p.ProductName";
        private void frmSearch_Load(object sender, EventArgs e)
        {
            using (var connection = CreateConnection())
            {
                LoadCategories(connection);
                LoadSuppliers(connection);
                var query = string.Format(baseQuery, string.Empty);
                LoadProducts(query, connection);
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

        private void LoadProducts(string query, SqlConnection connection, params SqlParameter[] parameters)
        {
            var list = new List<DetailedProductListDto>();

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddRange(parameters);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var p = new DetailedProductListDto
                        {
                            Id = Convert.ToInt32(reader["ProductID"]),
                            Name = reader["ProductName"].ToString(),
                            Category = reader["CategoryName"].ToString(),
                            Company = reader["CompanyName"].ToString(),
                            Price = Convert.ToDouble(reader["UnitPrice"]),
                            Stock = Convert.ToInt32(reader["UnitsInStock"])
                        };
                        list.Add(p); 
                    }
                }
            }
            dgvProducts.DataSource = null;
            dgvProducts.DataSource = list;
        }

        private void btnSearchDetailed_Click(object sender, EventArgs e)
        {
            var categoryId = (cmbCategories.SelectedItem as ComboBoxSourceDto).Value;
            var supplierId = (cmbSuppliers.SelectedItem as ComboBoxSourceDto).Value;
            using (var connection = CreateConnection())
            {
                var parameters = new List<SqlParameter>
                { 
                    new SqlParameter("@categoryId", categoryId),
                    new SqlParameter("@supplierId", supplierId)
                };
                var query = string.Format(baseQuery, $"WHERE c.CategoryID = @categoryId AND s.SupplierID = @supplierId");
                LoadProducts(query, connection, parameters.ToArray());
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            using (var connection = CreateConnection())
            {
                var query = string.Format(baseQuery, $"WHERE p.ProductName LIKE @name");
                LoadProducts(query, connection, new SqlParameter("@name", txtSearch.Text + "%"));
            }
        }

        private void btnConsumed_Click(object sender, EventArgs e)
        {
            using (var connection = CreateConnection())
            {
                var query = string.Format(baseQuery, $"WHERE p.UnitsInStock = @s");
                LoadProducts(query, connection, new SqlParameter("@s", SqlDbType.Int) { Value = 0 });
            }
        }

        private void btnDiscounted_Click(object sender, EventArgs e)
        {
            using (var connection = CreateConnection())
            {
                var query = string.Format(baseQuery, $"WHERE p.Discontinued = @dis");
                LoadProducts(query, connection, new SqlParameter("@dis", 1));
            }
        }
    }
}
