using System.Collections.Generic;

namespace OnlineShop.Web.Models
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }
        public List<ReviewViewModel> Reviews { get; set; }
    }

}
