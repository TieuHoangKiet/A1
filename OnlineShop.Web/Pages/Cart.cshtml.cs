using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OnlineShop.Web.Models;
using OnlineShop.Web.Services;

namespace OnlineShop.Web.Pages
{
    public class CartModel : PageModel
    {
        private readonly CartService _cartService;
        private readonly ProductService _productService;

        public Cart Cart { get; set; } = new Cart();

        [BindProperty]
        public int[] ProductIds { get; set; } = Array.Empty<int>();
        [BindProperty]
        public int[] Quantities { get; set; } = Array.Empty<int>();

        public CartModel(CartService cartService, ProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        public void OnGet()
        {
            Cart = _cartService.GetCart();
        }

        // Thêm s?n ph?m vào gi? hàng: POST /Cart?handler=Add
        public async Task<IActionResult> OnPostAddAsync(int productId, int quantity = 1)
        {
            if (quantity <= 0) quantity = 1;

            var product = await _productService.GetProductAsync(productId);
            if (product == null)
            {
                TempData["CartMessage"] = "S?n ph?m không t?n t?i.";
                return RedirectToPage();
            }

            // Ki?m tra t?n kho n?u có
            if (product.Stock <= 0)
            {
                TempData["CartMessage"] = "S?n ph?m t?m h?t hàng.";
                return RedirectToPage();
            }

            var cart = _cartService.GetCart();
            var existing = cart.Items.FirstOrDefault(x => x.ProductId == product.Id);
            if (existing != null)
            {
                var desired = existing.Quantity + quantity;
                if (product.Stock > 0 && desired > product.Stock)
                    existing.Quantity = product.Stock; // gi?i h?n theo stock
                else
                    existing.Quantity = desired;
            }
            else
            {
                var item = new CartItem
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = Math.Min(quantity, Math.Max(1, product.Stock)),
                    ThumbnailUrl = product.ThumbnailUrl ?? product.ImageUrl
                };
                cart.Items.Add(item);
            }

            _cartService.SaveCart(cart);
            TempData["CartMessage"] = "?ã thêm vào gi? hàng.";
            return RedirectToPage();
        }

        // C?p nh?t nhi?u m?c: POST /Cart?handler=Update
        public IActionResult OnPostUpdate()
        {
            // ProductIds và Quantities bind t? form
            if (ProductIds.Length != Quantities.Length)
            {
                TempData["CartMessage"] = "D? li?u c?p nh?t không h?p l?.";
                return RedirectToPage();
            }

            var cart = _cartService.GetCart();
            for (int i = 0; i < ProductIds.Length; i++)
            {
                var id = ProductIds[i];
                var qty = Quantities[i];
                if (qty <= 0)
                {
                    cart.Items.RemoveAll(x => x.ProductId == id);
                    continue;
                }

                var item = cart.Items.FirstOrDefault(x => x.ProductId == id);
                if (item != null)
                    item.Quantity = qty;
            }

            _cartService.SaveCart(cart);
            TempData["CartMessage"] = "C?p nh?t gi? hàng thành công.";
            return RedirectToPage();
        }

        // Xóa 1 s?n ph?m: POST /Cart?handler=Remove
        public IActionResult OnPostRemove(int productId)
        {
            _cartService.RemoveFromCart(productId);
            TempData["CartMessage"] = "?ã xóa s?n ph?m kh?i gi? hàng.";
            return RedirectToPage();
        }

        // Xóa toàn b? gi? hàng: POST /Cart?handler=Clear
        public IActionResult OnPostClear()
        {
            _cartService.ClearCart();
            TempData["CartMessage"] = "?ã xóa toàn b? gi? hàng.";
            return RedirectToPage();
        }
    }
}
