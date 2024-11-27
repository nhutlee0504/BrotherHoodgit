using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Linq;
using static System.Net.WebRequestMethods;

namespace SanGiaoDich_BrotherHood.Client.Pages
{
    public partial class SanPham
    {
        private List<Product> products = new List<Product>();
        private List<ImageProduct> images = new List<ImageProduct>();
        private List<Category> categories = new List<Category>();
        private Dictionary<int, string> productImages = new Dictionary<int, string>();
        private List<int> selectedCategories = new List<int>();
        private int lowPrice;
        private int highPrice;
        private string message = null;
        private List<Product> pagedProducts;
        private int currentPage = 1;
        private int totalPages;
        private int pageSize = 4; // số lượng sp mỗi page

        protected override async Task OnInitializedAsync()
        {
            await LoadProducts();
            await LoadCategories();
            UpdatePageProducts();
        }

        private async Task LoadProducts()
        {
            try
            {
                products = await http.GetFromJsonAsync<List<Product>>("api/product/GetAllProduct");
                foreach (var product in products)
                {
                    productImages[product.IDProduct] = "/defaultImg.png";
                    _ = LoadImagesByIdProduct(product.IDProduct);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task LoadImagesByIdProduct(int id)
        {
            try
            {
                var images = await http.GetFromJsonAsync<List<ImageProduct>>($"api/imageproduct/GetImageProduct/{id}");

                // Lấy ảnh đầu tiên hoặc ảnh mặc định nếu không có
                if (images != null && images.Count > 0)
                {
                    var imageUrl = images.First().Image; // Lấy ảnh đầu tiên
                    productImages[id] = imageUrl;
                }
                else
                {
                    productImages[id] = "/defaultImg.png"; // Ảnh mặc định
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                productImages[id] = "/defaultImg.png"; // Ảnh mặc định nếu có lỗi
            }
        }


        private string GetImage(int id)
        {
            return productImages.ContainsKey(id) ? productImages[id] : "/images/defaultImg.png";
        }

        private async Task LoadCategories()
        {
            try
            {
                categories = await http.GetFromJsonAsync<List<Category>>("api/Category/GetCategories");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void UpdatePageProducts()
        {
            var prods = products.Where(x => x.Status.ToLower().Contains("đã duyệt")).ToList();
            totalPages = (int)Math.Ceiling((double)prods.Count / pageSize);
            pagedProducts = prods.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }

        private void ChangePage(int page)
        {
            currentPage = page;
            UpdatePageProducts();
        }

        private async Task FilterByCategories(int IDCategory, bool isChecked)
        {
            if (isChecked)
            {
                selectedCategories.Add(IDCategory);
            }
            else
            {
                selectedCategories.Remove(IDCategory);
            }
            if (selectedCategories.Count > 0)
                products = (await http.GetFromJsonAsync<List<Product>>("api/product/GetAllProduct"))
                            .Where(x => selectedCategories.Contains(x.IDCategory)).ToList();
            else
            {
                products = await http.GetFromJsonAsync<List<Product>>("api/product/GetAllProduct");
            }
            UpdatePageProducts();
        }

        private async Task FilterByPrice()
        {
            if (lowPrice < 0 || highPrice < 0)
            {
                message = "Giá không được nhỏ hơn 0";
                await Task.Delay(2000);
                message = null;
                products = await http.GetFromJsonAsync<List<Product>>("api/product/GetAllProduct");
            }

            var filteredProducts = products;
            filteredProducts = filteredProducts.Where(x => x.Price >= lowPrice && x.Price <= highPrice).ToList();

            if (filteredProducts.Count == 0)
            {
                message = "Không tìm thấy sản phẩm trong khoản giá";
                await Task.Delay(2000);
                message = null;
                //products = await http.GetFromJsonAsync<List<Product>>("api/product/GetAllProduct");
            }
            else
            {
                products = filteredProducts;
            }
            UpdatePageProducts();
        }

        private async Task SortOrder(string value)
        {
            try
            {
                var filteredProducts = products;

                if (value == "priceLowToHigh")
                {
                    filteredProducts = filteredProducts.OrderBy(p => p.Price).ToList();
                }
                else if (value == "priceHighToLow")
                {
                    filteredProducts = filteredProducts.OrderByDescending(p => p.Price).ToList();
                }
                products = filteredProducts;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            UpdatePageProducts();
        }
    }
}
