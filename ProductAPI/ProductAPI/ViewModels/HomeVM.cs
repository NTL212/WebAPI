using ProductAPI.Models;

namespace ProductAPI.ViewModels
{
    public class HomeVM
    {
        public List<Category> categories = new List<Category>();
        public List<Product> products = new List<Product>();
    }
}
