using Firebase.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SanGiaoDich_BrotherHood.Shared.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
		private FirebaseStorage _firebaseStorage;
		private IBrowserFile selectedFile;
		public string DataUrl { get; set; }

		protected override async Task OnInitializedAsync()
		{
			_firebaseStorage = new FirebaseStorage("dbbrotherhood-ac2f1.appspot.com");
			await LoadUserData();
			await LoadProducts();
			await LoadCategoryNames(userProducts);
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
				if (images != null && images.Count > 0)
				{
					var imageUrl = images.First().Image;
					productImages[id] = imageUrl;
				}
				else
				{
					productImages[id] = "/defaultImg.png";
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				productImages[id] = "/defaultImg.png";
			}
		}

		private async Task OnFileSelected(InputFileChangeEventArgs e)
		{
			selectedFile = e.File;
			using var stream = new MemoryStream();
			await selectedFile.OpenReadStream().CopyToAsync(stream);
			DataUrl = $"data:{selectedFile.ContentType};base64,{Convert.ToBase64String(stream.ToArray())}";
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
			fieldErrors.Clear();
			try
			{
				var response = await HttpClient.PutAsJsonAsync("api/user/UpdateAccountInfo", infoAccountDto);
				if (response.IsSuccessStatusCode)
				{
					var updatedUser = await response.Content.ReadFromJsonAsync<Account>();
					userAccount = updatedUser;
					errorMessage = null;
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
			if (selectedFile == null)
			{
				errorMessage = "Vui lòng chọn một tệp để tải lên.";
				return;
			}

			const long maxFileSize = 10 * 1024 * 1024;
			if (selectedFile.Size > maxFileSize)
			{
				errorMessage = "Tệp tải lên không được lớn hơn 10MB.";
				return;
			}

			try
			{
				using var stream = selectedFile.OpenReadStream();
				var content = new MultipartFormDataContent();
				var fileContent = new StreamContent(stream);
				fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(selectedFile.ContentType);
				content.Add(fileContent, "imageFile", selectedFile.Name);
				var response = await HttpClient.PutAsync("api/user/UpdateProfileImage", content);

				if (response.IsSuccessStatusCode)
				{
					var updatedUser = await response.Content.ReadFromJsonAsync<Account>();
					if (updatedUser != null)
					{
						userAccount = updatedUser;
						errorMessage = null;
						NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
					}
					else
					{
						errorMessage = "Không thể cập nhật thông tin tài khoản.";
					}
				}
				else
				{
					var errorDetails = await response.Content.ReadAsStringAsync();
					errorMessage = $"Tải ảnh lên không thành công. Chi tiết: {errorDetails}";
				}
			}
			catch (Exception ex)
			{
				errorMessage = $"Đã xảy ra lỗi: {ex.Message}";
			}

			if (string.IsNullOrWhiteSpace(infoAccountDto.FullName))
			{
				fieldErrors["FullName"] = "Họ tên không được để trống.";
			}

			if (string.IsNullOrWhiteSpace(infoAccountDto.Email) || !IsValidEmail(infoAccountDto.Email))
			{
				fieldErrors["Email"] = "Email không hợp lệ.";
			}
			if (string.IsNullOrWhiteSpace(infoAccountDto.Phone) || !IsValidPhoneNumber(infoAccountDto.Phone))
			{
				fieldErrors["Phone"] = "Số điện thoại không hợp lệ. Vui lòng nhập số di động Việt Nam (10 chữ số).";
			}
			if (!infoAccountDto.Birthday.HasValue || !IsAgeValid(infoAccountDto.Birthday.Value))
			{
				fieldErrors["Birthday"] = "Người dùng phải trên 18 tuổi.";
			}
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
