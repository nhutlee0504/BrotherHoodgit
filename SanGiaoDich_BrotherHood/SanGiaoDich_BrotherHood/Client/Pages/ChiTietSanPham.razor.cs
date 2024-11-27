using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Client.Pages
{
    public partial class ChiTietSanPham
    {
        [Parameter] public int ProductId { get; set; }
        private Product product;
        private string categoryName;
        private Account user;
        private List<ImageProduct> images;
        private string errorMessage;
        private bool isFavorite = false;  // Trạng thái yêu thích của sản phẩm

        protected override async Task OnInitializedAsync()
        {
            try
            {
                product = await GetProductById(ProductId);
                if (product != null)
                {
                    categoryName = await GetCategoryNameById(product.IDCategory);
                    user = await GetSellerByUsername(product.UserName);
                    images = await GetImagesByProductId(product.IDProduct);
                }

                // Kiểm tra token và lấy danh sách yêu thích nếu có
                var token = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "token");
                if (!string.IsNullOrEmpty(token))
                {
                    await CheckFavoriteStatus();
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message; // Ghi lại thông điệp lỗi
            }
        }

        private async Task<Product> GetProductById(int id)
        {
            var response = await httpclient.GetAsync($"api/product/GetProductById/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Product>();
            }
            throw new Exception("Cannot fetch product data");
        }

        public async Task<string> GetCategoryNameById(int idCategory)
        {
            var response = await httpclient.GetAsync($"api/category/GetCategoryByID/{idCategory}");
            if (response.IsSuccessStatusCode)
            {
                var category = await response.Content.ReadFromJsonAsync<Category>();
                return category?.NameCate;
            }
            return null;
        }

        private async Task<Account> GetSellerByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }
            return await httpclient.GetFromJsonAsync<Account>($"api/user/GetAccountInfoByName/{username}");
        }

        private async Task<List<ImageProduct>> GetImagesByProductId(int id)
        {
            var response = await httpclient.GetAsync($"api/ImageProduct/GetImageProduct/{id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<ImageProduct>>() ?? new List<ImageProduct>();
            }
            else
            {
                errorMessage = "Không thể tải ảnh từ API: " + response.ReasonPhrase;
                return new List<ImageProduct>(); // Trả về danh sách rỗng nếu có lỗi
            }
        }

        private async Task CheckFavoriteStatus()
        {
            // Gọi API để lấy danh sách sản phẩm yêu thích của người dùng
            var response = await httpclient.GetAsync("api/favorite/GetFavoriteAccount");
            if (response.IsSuccessStatusCode)
            {
                var favoriteList = await response.Content.ReadFromJsonAsync<List<Product>>();
                // Kiểm tra nếu sản phẩm hiện tại có trong danh sách yêu thích
                isFavorite = favoriteList.Any(fav => fav.IDProduct == ProductId);
            }
            else
            {
                // Nếu có lỗi khi lấy danh sách yêu thích
                errorMessage = "Không thể lấy danh sách yêu thích.";
            }
        }

        private async Task ToggleFavorite()
        {
            isFavorite = !isFavorite; // Chuyển trạng thái yêu thích (bỏ hoặc thêm)

            var token = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "token");
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    if (isFavorite)
                    {
                        var response = await httpclient.GetAsync($"api/favorite/AddFavorite/{ProductId}");

                        if (!response.IsSuccessStatusCode)
                        {
                            errorMessage = "Không thể thêm sản phẩm vào danh sách yêu thích.";
                        }
                    }
                    else
                    {
                        var response = await httpclient.DeleteAsync($"api/favorite/DeleteFavorite/{ProductId}");

                        if (!response.IsSuccessStatusCode)
                        {
                            errorMessage = "Không thể xóa sản phẩm khỏi danh sách yêu thích.";
                        }
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = $"Đã xảy ra lỗi: {ex.Message}";
                }
            }
            else
            {
                errorMessage = "Bạn cần đăng nhập để thực hiện hành động này.";
            }
        }



    }
}
