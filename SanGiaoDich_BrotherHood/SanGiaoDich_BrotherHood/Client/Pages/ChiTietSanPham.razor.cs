using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

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
        private bool isFavorite = false;
        private bool isLoading = true;
        AccountInfoDto? accountInfo;
        private bool IsLoggedIn { get; set; } = false;
        private string name = string.Empty;
        private IEnumerable<Product> allProduct;
        private List<Product> aProduct = new List<Product>();
        private Dictionary<int, string> productImages = new Dictionary<int, string>();

        protected override async Task OnInitializedAsync()
        {

            await LoadProductDetails();
            await LoadAllProduct();
            await LoadProductImages();
            await CheckTokenAndLoadAccountInfo();
        }

        private async Task CheckTokenAndLoadAccountInfo()
        {
            var token = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "token");

            if (!string.IsNullOrEmpty(token))
            {
                httpclient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                await LoadAccountInfo();
            }
            else
            {
                accountInfo = null;
                isLoading = false;
                IsLoggedIn = false;
            }
        }

        private async Task LoadAccountInfo()
        {
            isLoading = true;
            errorMessage = null;

            try
            {
                var response = await httpclient.GetAsync("api/User/GetMyInfo");

                if (response.IsSuccessStatusCode)
                {
                    accountInfo = await response.Content.ReadFromJsonAsync<AccountInfoDto>();
                    IsLoggedIn = true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    errorMessage = $"Lỗi: {response.StatusCode} - {errorContent}";
                    IsLoggedIn = false;
                    //await Logout();
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Có lỗi xảy ra khi lấy thông tin tài khoản: " + ex.Message;
                IsLoggedIn = false;
            }
            finally
            {
                isLoading = false;
            }
        }

        private class AccountInfoDto
        {
            public string UserName { get; set; }
            public decimal PreSystem { get; set; }
            public string FullName { get; set; }
            public string PhoneNumber { get; set; }
            public string Gender { get; set; }
            public DateTime? Birthday { get; set; }
            public string ImageAccount { get; set; }
        }

        private async Task LoadAllProduct()
        {
            try
            {
                var response = await httpclient.GetFromJsonAsync<List<Product>>($"api/product/GetAllProduct");

                if (response == null || response.Count == 0)
                {
                    allProduct = new List<Product>();
                    return;
                }

                aProduct = response;
            }
            catch (Exception ex)
            {

            }
        }


        private async Task LoadProductImages()
        {
            try
            {
                foreach (var product in aProduct)
                {
                    try
                    {
                        var images = await httpclient.GetFromJsonAsync<List<ImageProduct>>($"api/ImageProduct/GetImageProduct/{product.IDProduct}");
                        if (images != null && images.Count > 0)
                        {
                            productImages[product.IDProduct] = images.First().Image;
                        }
                        else
                        {
                            // Không có ảnh => gán ảnh mặc định
                            productImages[product.IDProduct] = "/defaultImg.png";
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Lỗi khi tải ảnh cho sản phẩm ID: {product.IDProduct}, {ex.Message}");
                        // Lỗi trong quá trình gọi API => gán ảnh mặc định
                        productImages[product.IDProduct] = "/defaultImg.png";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tải ảnh sản phẩm: {ex.Message}");
            }
        }

        private string GetImage(int id)
        {
            return productImages.ContainsKey(id) ? productImages[id] : "/images/defaultImg.png";
        }

        private async Task Logout()
        {
            await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "token");
            IsLoggedIn = false;
            accountInfo = null;
            Navigation.NavigateTo("/", forceLoad: true);
        }

        private async Task LoadProductDetails()
        {
            try
            {
                product = await GetProductById(ProductId);
                if (product != null)
                {
                    categoryName = await GetCategoryNameById(product.IDCategory);
                    user = await GetSellerByUsername(product.UserName);
                    images = await GetImagesByProductId(product.IDProduct);

                    await LoadRelatedProducts();
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
            var account = await httpclient.GetFromJsonAsync<Account>($"api/user/GetAccountInfoByName/{username}");
            if (account != null)
            {
                name = account.UserName; // Gán tên người dùng vào biến name
            }
            return account;
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
                return new List<ImageProduct>();
            }
        }

        private async Task CheckFavoriteStatus()
        {
            var response = await httpclient.GetAsync("api/favorite/GetFavoriteAccount");
            if (response.IsSuccessStatusCode)
            {
                var favoriteList = await response.Content.ReadFromJsonAsync<List<Product>>();
                isFavorite = favoriteList.Any(fav => fav.IDProduct == ProductId);
            }
            else
            {
                errorMessage = "Không thể lấy danh sách yêu thích.";
            }
        }

        private async Task ToggleFavorite()
        {
            isFavorite = !isFavorite;

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


        private async Task GoToMessagingPage()
        {
            await CheckTokenAndLoadAccountInfo();
            if (IsLoggedIn)
            {
            }
            else
            {
                Navigation.NavigateTo("/login");
            }
            if (product != null && accountInfo != null)
            {
                try
                {
                    var conversation = await GetConversation(product.UserName);
                    if (conversation == null)
                    {
                        conversation = await CreateConversation(product.UserName);
                    }

                    // Tạo tin nhắn và gửi đi
                    var message = new Messages
                    {
                        UserSend = accountInfo.UserName,
                        Content = $"Chào bạn, tôi quan tâm đến sản phẩm {product.Name}. Mã sản phẩm là {product.IDProduct}",
                        TypeContent = "Text",
                        CreatedDate = DateTime.Now,
                        IsDeleted = false,
                        Status = "Sent",
                    };

                    // Gửi tin nhắn
                    var response = await httpclient.PostAsJsonAsync($"api/message/CreateMess?username={accountInfo.UserName}&userGive={product.UserName}", message);

                    if (response.IsSuccessStatusCode)
                    {
                        // Sau khi gửi tin nhắn thành công, điều hướng đến trang tin nhắn
                        Navigation.NavigateTo($"/nhantin?productName={Uri.EscapeDataString(product.Name)}");
                    }
                    else
                    {
                        errorMessage = "Không thể gửi tin nhắn.";
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = $"Lỗi khi gửi tin nhắn: {ex.Message}";
                }
            }
            else
            {
                errorMessage = "Không tìm thấy thông tin sản phẩm hoặc người dùng hiện tại.";
            }
        }

        private async Task<Conversation> GetConversation(string userGive)
        {
            try
            {
                var url = $"api/Conversation/GetConversations/{accountInfo.UserName}";
                var conversations = await httpclient.GetFromJsonAsync<List<Conversation>>(url);

                // Tìm cuộc hội thoại giữa người dùng hiện tại và người nhận
                return conversations?.FirstOrDefault(c =>
                    (c.Username == accountInfo.UserName && c.UserGive == userGive) ||
                    (c.Username == userGive && c.UserGive == accountInfo.UserName));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy hội thoại: {ex.Message}");
                return null;
            }
        }

        private async Task<Conversation> CreateConversation(string userGive)
        {
            try
            {
                var newConversation = new Conversation
                {
                    Username = accountInfo.UserName,
                    UserGive = userGive
                };

                var response = await httpclient.PostAsJsonAsync("api/Conversation/CreateConversation", newConversation);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Conversation>();
                }
                else
                {
                    Console.WriteLine("Lỗi khi tạo hội thoại.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tạo hội thoại: {ex.Message}");
                return null;
            }
        }
        private List<Product> relatedProducts;
        private async Task LoadRelatedProducts()
        {
            if (product != null)
            {
                try
                {
                    // Gọi API lấy toàn bộ sản phẩm
                    var response = await httpclient.GetAsync("api/product/GetAllProduct");
                    if (response.IsSuccessStatusCode)
                    {
                        var allProducts = await response.Content.ReadFromJsonAsync<List<Product>>();

                        // Lọc các sản phẩm cùng loại và loại bỏ sản phẩm hiện tại
                        relatedProducts = allProducts
                            .Where(p => p.IDCategory == product.IDCategory && p.IDProduct != product.IDProduct)
                            .Take(4) // Lấy tối đa 4 sản phẩm
                            .ToList();
                    }
                    else
                    {
                        errorMessage = "Không thể tải danh sách sản phẩm: " + response.ReasonPhrase;
                        relatedProducts = new List<Product>();
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = "Có lỗi xảy ra khi tải sản phẩm cùng loại: " + ex.Message;
                    relatedProducts = new List<Product>();
                }
            }
        }
    }
}
