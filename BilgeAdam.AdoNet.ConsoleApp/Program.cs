using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilgeAdam.AdoNet.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "Server=.;Database=Northwind;User Id=sa;Password=123";
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
                    var c = new Category
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString()
                    };
                    Console.WriteLine($"Id: {c.Id}, Name: {c.Name}");
                }
                reader.Close();

                Console.WriteLine("___________________________________________________________");
                var productQuery = "SELECT ProductID AS Id, ProductName AS Name, UnitPrice AS Price, UnitsInStock AS Stock FROM Products WHERE CategoryId = 1";
                command = new SqlCommand(productQuery, connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var p = new Product
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString(),
                        Price = Convert.ToDouble(reader["Price"]),
                        Stock = Convert.ToInt32(reader["Stock"])
                    };
                    Console.WriteLine($"Id: {p.Id}, Name: {p.Name}, Price: {p.Price}, Stock: {p.Stock}");
                }
            }


            Console.ReadKey();
        }
    }

    class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
    }
}
