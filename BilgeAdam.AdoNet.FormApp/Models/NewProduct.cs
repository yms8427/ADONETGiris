namespace BilgeAdam.AdoNet.FormApp.Models
{
    internal class NewProduct
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Stock { get; set; }
        public int SupplierId { get; set; }
    }
}
