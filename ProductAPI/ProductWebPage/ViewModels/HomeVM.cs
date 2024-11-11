using ProductDataAccess.Models;

namespace ProductWebPage.ViewModels
{
    public class HomeVM
    {
        public List<Category> categories = new List<Category>();
        public List<Product> products = new List<Product>();
    }
}
