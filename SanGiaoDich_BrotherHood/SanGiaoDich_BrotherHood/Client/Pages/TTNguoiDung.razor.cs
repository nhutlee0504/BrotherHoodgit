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
        private string currentUserName;
        private bool isCurrentUser => string.Equals(currentUserName, username, StringComparison.OrdinalIgnoreCase);

        private class AccountInfoDto
        {
            public string UserName { get; set; }
            public string FullName { get; set; }
            public string PhoneNumber { get; set; }
            public string Gender { get; set; }
            public DateTime? Birthday { get; set; }
            public string ImageAccount { get; set; }
        }
        private bool isLoading = true;


        // Đối tượng để lưu tệp
        private IBrowserFile selectedFile;
		protected override async Task OnInitializedAsync()
		{
			await LoadUserData();
			await LoadProducts();
			await LoadCategoryNames(userProducts);
			try
			{
				// Kiểm tra token trước khi gọi API
				var token = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "token");
				if (string.IsNullOrEmpty(token))
				{
					currentUserName = string.Empty; // Không đăng nhập
					return;
				}

				var response = await HttpClient.GetAsync("api/User/GetMyInfo");

				if (response.IsSuccessStatusCode)
				{
					var accountInfo = await response.Content.ReadFromJsonAsync<AccountInfoDto>();
					currentUserName = accountInfo?.UserName ?? string.Empty;
				}
				else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
				{
					currentUserName = string.Empty; // Xử lý trường hợp không được phép
				}
				else
				{
					currentUserName = string.Empty; // Xử lý các trường hợp lỗi khác
				}
			}
			catch (HttpRequestException)
			{
				// Lỗi liên quan đến kết nối, không log lên console
				currentUserName = string.Empty;
			}
			catch (Exception ex)
			{
				// Xử lý lỗi ngoại lệ khác mà không ghi log
				Console.WriteLine($"Đã xảy ra lỗi: {ex.Message}"); // Chỉ log nếu cần
				currentUserName = string.Empty;
			}
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

        private Dictionary<string, string> fieldErrors = new Dictionary<string, string>();
        private async Task UpdateAccountInfo()
        {
            // Reset lỗi
            fieldErrors.Clear();

            // Kiểm tra họ tên
            if (string.IsNullOrWhiteSpace(infoAccountDto.FullName))
            {
                fieldErrors["FullName"] = "Họ tên không được để trống.";
            }

            // Kiểm tra email
            if (string.IsNullOrWhiteSpace(infoAccountDto.Email) || !IsValidEmail(infoAccountDto.Email))
            {
                fieldErrors["Email"] = "Email không hợp lệ.";
            }

            // Kiểm tra số điện thoại
            if (string.IsNullOrWhiteSpace(infoAccountDto.Phone) || !IsValidPhoneNumber(infoAccountDto.Phone))
            {
                fieldErrors["Phone"] = "Số điện thoại không hợp lệ. Vui lòng nhập số di động Việt Nam (10 chữ số).";
            }

            // Kiểm tra ngày sinh
            if (!infoAccountDto.Birthday.HasValue || !IsAgeValid(infoAccountDto.Birthday.Value))
            {
                fieldErrors["Birthday"] = "Người dùng phải trên 18 tuổi.";
            }

            // Nếu có lỗi, dừng lại và không thực hiện API call
            if (fieldErrors.Count > 0)
            {
                return;
            }

            try
            {
                var response = await HttpClient.PutAsJsonAsync("api/user/UpdateAccountInfo", infoAccountDto);
                if (response.IsSuccessStatusCode)
                {
                    var updatedUser = await response.Content.ReadFromJsonAsync<Account>();
                    userAccount = updatedUser;
                    fieldErrors.Clear();
                    NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
                }
                else
                {
                    var errorDetails = await response.Content.ReadAsStringAsync();
                    fieldErrors["General"] = $"Cập nhật không thành công. Chi tiết: {errorDetails}";
                }
            }
            catch (Exception ex)
            {
                fieldErrors["General"] = $"Đã xảy ra lỗi trong quá trình cập nhật: {ex.Message}";
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
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^(0[3|5|7|8|9])+([0-9]{8})$");
        }
        private bool IsAgeValid(DateTime birthday)
        {
            var today = DateTime.Today;
            var age = today.Year - birthday.Year;
            if (birthday.Date > today.AddYears(-age)) age--;
            return age >= 18;
        }

    }
}
