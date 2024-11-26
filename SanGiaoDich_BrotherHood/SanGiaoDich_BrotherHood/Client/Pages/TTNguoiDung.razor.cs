﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SanGiaoDich_BrotherHood.Shared.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace SanGiaoDich_BrotherHood.Client.Pages
{
	public partial class TTNguoiDung
	{
		[Parameter] public string username { get; set; }
		private Account userAccount;
		private InfoAccountDto infoAccountDto; // New DTO for temporary storage
		private string errorMessage;
		private IEnumerable<AddressDetail> userAddress;
		private AddressDetail firstAddress;
		private IEnumerable<Bill> userBill;
		private int countBill;
		private IEnumerable<Product> userProducts;
		private string productErrorMessage;
		private Dictionary<int, string> categoryNames = new Dictionary<int, string>();
		private Dictionary<int, string> productImages = new Dictionary<int, string>();

		// Đối tượng để lưu tệp
		private IBrowserFile selectedFile;
		protected override async Task OnInitializedAsync()
		{
			await LoadUserData();
			await LoadProducts();
			await LoadCategoryNames(userProducts);
			await LoadFavoriteAccounts();
		}
		private async Task LoadUserData()
		{
			try
			{
				userAccount = await HttpClient.GetFromJsonAsync<Account>($"api/user/GetAccountInfoByName/{username}");
				userAddress = await HttpClient.GetFromJsonAsync<IEnumerable<AddressDetail>>($"api/addressdetail/GetAddressDetailsByUserName/{username}");
				firstAddress = userAddress?.FirstOrDefault();
				userBill = await HttpClient.GetFromJsonAsync<IEnumerable<Bill>>($"api/bill/GetBillsByUserName/{username}");
				countBill = userBill.Count();
				// Initialize the DTO with user account info
				infoAccountDto = new InfoAccountDto
				{
					FullName = userAccount.FullName,
					Email = userAccount.Email,
					Phone = userAccount.PhoneNumber,
					Gender = userAccount.Gender,
					Birthday = userAccount.Birthday,
					Introduce = userAccount.Introduce
				};
			}
			catch (Exception ex)
			{
				errorMessage = "Không thể lấy thông tin tài khoản: " + ex.Message;
			}
		}

        private async Task DeleteProduct(int productId)
        {
            try
            {
                // Make a DELETE request to the API to delete the product
                var response = await HttpClient.DeleteAsync($"api/product/DeleteProduct/{productId}");

                if (response.IsSuccessStatusCode)
                {
                    // If deletion is successful, find and update the product in the list
                    var productToDelete = userProducts.FirstOrDefault(p => p.IDProduct == productId);
                    if (productToDelete != null)
                    {
                        // Optionally, update the product status on the frontend
                        productToDelete.Status = "Đã xóa";
                    }
                    // Optionally, display a success message
                    await JSRuntime.InvokeVoidAsync("alert", "Sản phẩm đã được xóa thành công!");
                }
                else
                {
                    // If there is an error from the API, display the error message
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    await JSRuntime.InvokeVoidAsync("alert", $"Lỗi: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during the delete operation
                Console.WriteLine($"Lỗi xảy ra khi xóa sản phẩm: {ex.Message}");
                await JSRuntime.InvokeVoidAsync("alert", "Đã xảy ra lỗi khi xóa sản phẩm.");
            }
        }

        private async Task UpgradePriorityLevel(int productId)
        {
            try
            {
                // Gọi API nâng cấp mức độ sản phẩm
                var response = await HttpClient.PutAsync($"api/product/UpgradeProrityLevel/{productId}", null);
                if (response.IsSuccessStatusCode)
                {
                    // Nếu nâng cấp thành công, cập nhật trạng thái sản phẩm trong giao diện
                    var productToUpdate = userProducts.FirstOrDefault(p => p.IDProduct == productId);
                    if (productToUpdate != null)
                    {
                        productToUpdate.ProrityLevel = "Ưu tiên";
                    }
                    await JSRuntime.InvokeVoidAsync("alert", "Sản phẩm đã được nâng cấp thành công!");
                }
                else
                {
                    // Nếu API trả về lỗi
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Lỗi khi nâng cấp sản phẩm: {errorResponse}");
                    await JSRuntime.InvokeVoidAsync("alert", $"Lỗi: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi xảy ra khi nâng cấp sản phẩm: {ex.Message}");
                await JSRuntime.InvokeVoidAsync("alert", "Đã xảy ra lỗi khi nâng cấp sản phẩm.");
            }

        }
        private string activeTab = "posts"; // Tab mặc định là bài đăng

        private void SwitchTabToPosts()
        {
            activeTab = "posts";
        }

        private void SwitchTabToFavorites()
        {
            activeTab = "favorites";
        }
        private List<Favorite> favorites = new List<Favorite>();
        private Dictionary<int, Product> favoriteProducts = new Dictionary<int, Product>(); // Lưu thông tin sản phẩm yêu thích
        private string favoriteErrorMessage;
        private bool isLoadingFavorites = false;

        private async Task LoadFavoriteAccounts()
        {
            try
            {
                isLoadingFavorites = true; // Bắt đầu tải dữ liệu
                var response = await HttpClient.GetFromJsonAsync<List<Favorite>>("api/Favorite/GetFavoriteAccount");

                if (response != null)
                {
                    favorites = response;

                    // Lấy thông tin sản phẩm yêu thích cho từng sản phẩm
                    foreach (var favorite in favorites)
                    {
                        var product = await HttpClient.GetFromJsonAsync<Product>($"api/product/GetProductById/{favorite.IDProduct}");
                        if (product != null)
                        {
                            favoriteProducts[favorite.IDProduct] = product;
                        }
                    }
                }
                else
                {
                    favoriteErrorMessage = "Danh sách yêu thích trống.";
                }
            }
            catch (HttpRequestException ex)
            {
                favoriteErrorMessage = "Không thể kết nối đến API: " + ex.Message;
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                favoriteErrorMessage = "Đã xảy ra lỗi khi tải danh sách yêu thích: " + ex.Message;
                Console.WriteLine(ex);
            }
            finally
            {
                isLoadingFavorites = false; // Kết thúc quá trình tải
            }
        }



        public class FavoriteAccountDto
        {
            public int IDFavorite { get; set; }
            public string AccountName { get; set; }
        }

        private async Task LoadProducts()
		{
			try
			{
				userProducts = await HttpClient.GetFromJsonAsync<List<Product>>("api/product/GetAllProduct");
				foreach (var product in userProducts)
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
				var images = await HttpClient.GetFromJsonAsync<List<ImageProduct>>($"api/imageproduct/GetImageProduct/{id}");

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

		private async Task<Category> GetCategoryById(int id)
		{
			try
			{
				return await HttpClient.GetFromJsonAsync<Category>($"api/category/GetCategoryByID/{id}");
			}
			catch (Exception ex)
			{
				errorMessage = "Không thể lấy thông tin danh mục: " + ex.Message;
				return null;
			}
		}
		private async Task LoadCategoryNames(IEnumerable<Product> products)
		{
			foreach (var product in products)
			{
				if (!categoryNames.ContainsKey(product.IDCategory))
				{
					var category = await GetCategoryById(product.IDCategory);
					if (category != null)
					{
						categoryNames[product.IDCategory] = category.NameCate;
					}
				}
			}
		}


		private async Task UpdateAccountInfo()
		{
			try
			{
				var response = await HttpClient.PutAsJsonAsync("api/user/UpdateAccountInfo", infoAccountDto);
				if (response.IsSuccessStatusCode)
				{
					var updatedUser = await response.Content.ReadFromJsonAsync<Account>();
					userAccount = updatedUser; // Update local user account with the new information
					errorMessage = null; // Clear error message
					NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
				}
				else
				{
					errorMessage = "Cập nhật thông tin không thành công.";
				}
			}
			catch (Exception ex)
			{
				errorMessage = "Lỗi xảy ra: " + ex.Message;
			}
		}
		private async Task UploadFile()
		{
			if (selectedFile != null)
			{
				const long maxFileSize = 10 * 1024 * 1024; // 10MB
				if (selectedFile.Size > maxFileSize)
				{
					errorMessage = "Tệp tải lên không được lớn hơn 10MB.";
					return;
				}
				var content = new MultipartFormDataContent();
				var streamContent = new StreamContent(selectedFile.OpenReadStream(maxFileSize)); // Thay đổi ở đây
				streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(selectedFile.ContentType);
				content.Add(streamContent, "imageFile", selectedFile.Name);

				var response = await HttpClient.PutAsync("api/user/UpdateProfileImage", content);
				if (response.IsSuccessStatusCode)
				{
					var updatedUser = await response.Content.ReadFromJsonAsync<Account>();
					userAccount = updatedUser;
					errorMessage = null;
					NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
				}
				else
				{
					var errorDetails = await response.Content.ReadAsStringAsync();
					errorMessage = $"Tải ảnh lên không thành công. Chi tiết: {errorDetails}";
				}
			}
		}
        private HashSet<int> selectedForDeletion = new HashSet<int>();

        private async Task ToggleFavorite(int productId)
        {
            try
            {
                if (selectedForDeletion.Contains(productId))
                {
                    // Nếu sản phẩm đã được chọn để xóa, bỏ chọn và giữ lại sản phẩm trong danh sách yêu thích
                    selectedForDeletion.Remove(productId);
                }
                else
                {
                    // Nếu sản phẩm chưa được chọn để xóa, chúng ta sẽ thêm vào danh sách xóa
                    selectedForDeletion.Add(productId);
                }

                // Chỉ xóa khi có sản phẩm đã được chọn trong selectedForDeletion
                if (selectedForDeletion.Count > 0)
                {
                    // Gửi yêu cầu xóa sản phẩm sau khi người dùng xác nhận (nếu cần)
                    var response = await HttpClient.DeleteAsync($"api/favorite/DeleteFavorite/{productId}");
                    if (response.IsSuccessStatusCode)
                    {
                        // Cập nhật danh sách yêu thích sau khi xóa
                        favorites.RemoveAll(f => selectedForDeletion.Contains(f.IDProduct));
                        foreach (var productIdToRemove in selectedForDeletion)
                        {
                            favoriteProducts.Remove(productIdToRemove);
                        }
                        selectedForDeletion.Clear(); // Sau khi xóa, dọn dẹp trạng thái
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Lỗi khi xóa sản phẩm yêu thích: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi xảy ra khi thay đổi trạng thái yêu thích: {ex.Message}");
            }
        }



    }
}
