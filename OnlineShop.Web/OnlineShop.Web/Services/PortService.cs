using OnlineShop.Web.Models;
using System.Linq;

namespace OnlineShop.Web.Services
{
    public class PortService
    {
        private readonly ApiService _api;

        public PortService(ApiService api)
        {
            _api = api;
        }

        public async Task<List<PortViewModel>?> GetAllAsync()
        {
            var list = await _api.GetAsync<List<OnlineShop.Api.Models.Port>>("ports");
            if (list == null) return new List<PortViewModel>();
            return list.Select(p => new PortViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                Summary = p.Summary,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                Author = p.Author,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            }).ToList();
        }

        public async Task<PortViewModel?> GetAsync(int id)
        {
            var p = await _api.GetAsync<OnlineShop.Api.Models.Port>($"ports/{id}");
            if (p == null) return null;
            return new PortViewModel
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                Summary = p.Summary,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                Author = p.Author,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            };
        }
    }
}
