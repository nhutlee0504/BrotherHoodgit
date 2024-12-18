using FirebaseAdmin;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Microsoft.VisualBasic;
using SanGiaoDich_BrotherHood.Shared.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;
using static SanGiaoDich_BrotherHood.Client.Pages.Bill;
using static System.Net.WebRequestMethods;

namespace SanGiaoDich_BrotherHood.Client.Pages
{
	public partial class TTNguoiDung
	{
		private string currentUserName;
		public bool isLoading;
		private bool isCurrentUser => string.Equals(currentUserName, username, StringComparison.OrdinalIgnoreCase);
		[Parameter] public string username { get; set; }
		private Account userAccount;
		private InfoAccountDto infoAccountDto;
		private string errorMessage;
		private IEnumerable<AddressDetail> userAddress;
		private AddressDetail firstAddress;
		private IEnumerable<Bill> userBill;
		private int countBill;
		private IEnumerable<Product> userProducts;
		private IEnumerable<Product> allProduct;
		private string productErrorMessage;
		private Dictionary<int, string> categoryNames = new Dictionary<int, string>();
		private Dictionary<int, string> productImages = new Dictionary<int, string>();
		private string sellerName;
		private string image;
		private int idBillDetail;
		private string productname;


		private int currentPage = 1;
		private int itemsPerPage = 4;
		private int totalPosts = 0;
		private List<Product> products = new List<Product>();
		private List<Product> aProduct = new List<Product>();
		private Dictionary<string, string> fieldErrors = new Dictionary<string, string>();
		private string code;
		private string reciveCode;
		private string errorCode;
		private bool loading = false;
		private string successEmail;

		private class AccountInfoDto
		{
			public string UserName { get; set; }
			public string FullName { get; set; }
			public string PhoneNumber { get; set; }
			public string Gender { get; set; }
			public DateTime? Birthday { get; set; }
			public string ImageAccount { get; set; }
			public string Dob { get; set; }
		}

		public class IProd
		{
			public int idImage { get; set; }
			public int idProduct { get; set; }
			public string url { get; set; }
		}

		public class ReviewInfo
		{
			public int IdBillDetail { get; set; }
			public string SellerName { get; set; }
			public string Image { get; set; }
			public string ProductName { get; set; }
			public DateTime ExpiryTime { get; set; }
		}

		private IBrowserFile selectedFile;
		protected override async Task OnInitializedAsync()
		{
			await LoadUserData();
			await LoadProducts();
			await LoadAllProduct();
			await LoadCategoryNames(userProducts);
			await LoadFavoriteAccounts();
			totalPosts = products.Count;
			await LoadProductImages();
			UpdatePageProducts();
			await LoadBill();
			await LoadUserDataAsync();
			CountCompletedBills();

			try
			{
				var token = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "token");
				if (string.IsNullOrEmpty(token))
				{
					currentUserName = string.Empty;
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
					currentUserName = string.Empty;
				}
				else
				{
					currentUserName = string.Empty;
				}
			}
			catch (HttpRequestException)
			{

				currentUserName = string.Empty;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Đã xảy ra lỗi: {ex.Message}");
				currentUserName = string.Empty;
			}

		}
		private async Task LoadUserData()
		{
			isLoading = true;
			try
			{

				userAccount = await HttpClient.GetFromJsonAsync<Account>($"api/user/GetAccountInfoByName/{username}");
				userAddress = await HttpClient.GetFromJsonAsync<IEnumerable<AddressDetail>>($"api/AddressDetail/{username}");
				firstAddress = userAddress?.FirstOrDefault();
				userBill = await HttpClient.GetFromJsonAsync<IEnumerable<Bill>>($"api/bill/GetBillsByUserName/{username}");
				countBill = userBill.Count();
				infoAccountDto = new InfoAccountDto
				{
					FullName = userAccount.FullName,
					Email = userAccount.Email,
					PhoneNumber = userAccount.PhoneNumber,
					Gender = userAccount.Gender,
					Birthday = userAccount.Birthday,
					Introduce = userAccount.Introduce,
					Dob = userAccount.Dob,
				};
				isLoading = false;

			}
			catch (Exception ex)
			{
				errorMessage = "Không thể lấy thông tin tài khoản: " + ex.Message;
			}
		}

		private async Task SwitchTabToPosts()
		{
			activeTab = "posts";
		}

		private async Task SwitchTabToFavorites()
		{
			activeTab = "favorites";
		}
		private int CalculateTotalPages(int totalPosts, int itemsPerPage)
		{
			// Đảm bảo không có lỗi chia cho 0
			if (itemsPerPage == 0)
			{
				return 0;
			}

			int totalPages = (int)Math.Ceiling((double)totalPosts / itemsPerPage);

			Console.WriteLine($"Tổng số trang: {totalPages}");

			return itemsPerPage > 0 ? (int)Math.Ceiling((double)totalPosts / itemsPerPage) : 0; ;
		}
		private void UpdatePageProducts()
		{

			userProducts = products
	 .Skip((currentPage - 1) * itemsPerPage)
	 .Take(itemsPerPage)
	 .ToList();

		}
		private void ChangePage(int page)
		{
			int totalPages = CalculateTotalPages(totalPosts, itemsPerPage);

			if (page < 1 || page > totalPages)
			{
				return;
			}

			currentPage = page;
			UpdatePageProducts();
			StateHasChanged();
			Console.WriteLine($"Chuyển sang trang {currentPage}");
		}

		private async Task DeleteProduct(int productId)
		{
			try
			{
				var response = await HttpClient.DeleteAsync($"api/product/DeleteProduct/{productId}");

				if (response.IsSuccessStatusCode)
				{
					var productToDelete = userProducts.FirstOrDefault(p => p.IDProduct == productId);
					if (productToDelete != null)
					{

						productToDelete.Status = "Đã xóa";
					}
					await JSRuntime.InvokeVoidAsync("alert", "Sản phẩm đã được xóa thành công!");
				}
				else
				{
					var errorResponse = await response.Content.ReadAsStringAsync();
					await JSRuntime.InvokeVoidAsync("alert", $"Lỗi: {errorResponse}");
				}
			}
			catch (Exception ex)
			{

				Console.WriteLine($"Lỗi xảy ra khi xóa sản phẩm: {ex.Message}");
				await JSRuntime.InvokeVoidAsync("alert", "Đã xảy ra lỗi khi xóa sản phẩm.");
			}
		}

		private async Task UpgradePriorityLevel(int productId)
		{
			try
			{
				var response = await HttpClient.PutAsync($"api/product/UpgradeProrityLevel/{productId}", null);
				if (response.IsSuccessStatusCode)
				{
					var productToUpdate = userProducts.FirstOrDefault(p => p.IDProduct == productId);
					if (productToUpdate != null)
					{
						productToUpdate.ProrityLevel = "Ưu tiên";
					}
					await JSRuntime.InvokeVoidAsync("alert", "Sản phẩm đã được nâng cấp thành công!");
				}
				else
				{
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
		private string activeTab = "posts";

		private List<Favorite> favorites = new List<Favorite>();
		private Dictionary<int, Product> favoriteProducts = new Dictionary<int, Product>();
		private string favoriteErrorMessage;
		private bool isLoadingFavorites = false;

		private async Task LoadFavoriteAccounts()
		{
			try
			{
				isLoadingFavorites = true;
				var response = await HttpClient.GetFromJsonAsync<List<Favorite>>("api/Favorite/GetFavoriteAccount");

				if (response != null)
				{
					favorites = response;
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
			catch (Exception ex)
			{
				favoriteErrorMessage = "Đã xảy ra lỗi khi tải danh sách yêu thích: " + ex.Message;
				Console.WriteLine(ex);
			}
			finally
			{
				isLoadingFavorites = false;
			}
		}

		public class FavoriteAccountDto
		{
			public int IDFavorite { get; set; }
			public string AccountName { get; set; }
			public int IDProduct { get; set; }
		}

		private async Task LoadProducts()
		{
			try
			{
				var response = await HttpClient.GetFromJsonAsync<List<Product>>($"api/product/GetProductByNameAccount/{username}");

				if (response == null || response.Count == 0)
				{
					Console.WriteLine("API không trả về sản phẩm.");
					productErrorMessage = "Chưa có bài đăng nào được tạo";
					userProducts = new List<Product>();
					return;
				}

				products = response;
				totalPosts = products.Count;
				productErrorMessage = null;

				Console.WriteLine($"Số lượng sản phẩm: {totalPosts}");
				UpdatePageProducts();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi tải sản phẩm: {ex.Message}");
				productErrorMessage = "Đã xảy ra lỗi khi tải sản phẩm. Vui lòng thử lại sau.";
			}
		}

		private async Task LoadAllProduct()
		{
			try
			{
				var response = await HttpClient.GetFromJsonAsync<List<Product>>($"api/product/GetAllProduct");

				if (response == null || response.Count == 0)
				{
					productErrorMessage = "Chưa có bài đăng nào được tạo";
					allProduct = new List<Product>();
					return;
				}

				aProduct = response;
				totalPosts = aProduct.Count;
				productErrorMessage = null;

				Console.WriteLine($"Số lượng sản phẩm: {totalPosts}");
				UpdatePageProducts();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi tải sản phẩm: {ex.Message}");
				productErrorMessage = "Đã xảy ra lỗi khi tải sản phẩm. Vui lòng thử lại sau.";
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
						var images = await HttpClient.GetFromJsonAsync<List<ImageProduct>>($"api/ImageProduct/GetImageProduct/{product.IDProduct}");
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
			foreach (var product in aProduct)
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

		private async Task CheckMail()
		{
			// Reset errors before validating
			fieldErrors.Clear();

			bool hasError = false;

			// Validate Email
			if (string.IsNullOrWhiteSpace(infoAccountDto.Email) || !IsValidEmail(infoAccountDto.Email))
			{
				fieldErrors["Email"] = "Email không hợp lệ";
				hasError = true;
			}

			// If there are errors, do not submit
			if (hasError)
			{
				return;
			}
			try
			{
				loading = true;
				var sendCode = RandomCode();
				var emailMessage = $"Mã xác nhận: {sendCode}";
				await SendEmailAsync(infoAccountDto.Email, "[BroderHood] Mã xác nhận cập nhật Email", emailMessage);
				successEmail = "Mã xác nhận đã được gửi đến Email của bạn";
				loading = false;
				code = sendCode;
			}
			catch (Exception ex)
			{

				Console.WriteLine("Lỗi gửi Email: ", ex);
			}
		}

		private async Task UpdateAccountInfo()
		{
			if (reciveCode == null)
			{
				errorCode = "Mã xác nhận rỗng";
				return;
			}
			else if (reciveCode != code)
			{
				errorCode = "Mã xác nhận không chính xác";
				return;
			}
			try
			{
				var response = await HttpClient.PutAsJsonAsync($"api/user/UpdateAccountInfo?email={infoAccountDto.Email}", new { });

				if (response.IsSuccessStatusCode)
				{
					// Handle success
					NavigationManager.NavigateTo($"/ThongTinNguoiDung/{username}", forceLoad: true);
				}
				else
				{
					errorMessage = "Có lỗi xảy ra khi cập nhật Email";
				}
			}
			catch (Exception ex)
			{
				errorMessage = $"Lỗi: {ex.Message}";
			}
		}

		private async Task UpdateAccountInfo2()
		{
			try
			{
				if (string.IsNullOrEmpty(infoAccountDto.PhoneNumber) && string.IsNullOrEmpty(infoAccountDto.Introduce))
				{
					errorMessage = "Số điện thoại hoặc mô tả phải được nhập.";
					return;
				}

				var response = await HttpClient.PutAsJsonAsync($"api/user/UpdateAccountInfo2?phone={infoAccountDto.PhoneNumber}&description={infoAccountDto.Introduce}", new { });

				if (response.IsSuccessStatusCode)
				{
					// Cập nhật thành công
					NavigationManager.NavigateTo($"/ThongTinNguoiDung/{username}", forceLoad: true);
				}
				else
				{
					var errorResponse = await response.Content.ReadAsStringAsync();
					errorMessage = $"Cập nhật thất bại: {errorResponse}";
				}
			}
			catch (Exception ex)
			{
				errorMessage = $"Có lỗi xảy ra: {ex.Message}";
			}
		}
		private bool IsValidEmail(string email)
		{
			return email.Contains("@") && email.Contains(".");
		}

		private bool IsValidPhoneNumber(string phone)
		{
			return phone.Length == 10 && phone.All(char.IsDigit);
		}

		private bool IsAgeValid(DateTime? birthday)
		{
			if (!birthday.HasValue)
			{
				return false; // Nếu ngày sinh null, trả về false
			}

			DateTime birthDate = birthday.Value;
			int age = DateTime.Now.Year - birthDate.Year;
			if (DateTime.Now < birthDate.AddYears(age)) age--;  // Điều chỉnh nếu chưa qua sinh nhật năm nay
			return age >= 18;
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
					selectedForDeletion.Remove(productId);
				}
				else
				{
					selectedForDeletion.Add(productId);
				}

				if (selectedForDeletion.Count > 0)
				{
					var response = await HttpClient.DeleteAsync($"api/favorite/DeleteFavorite/{productId}");
					if (response.IsSuccessStatusCode)
					{
						favorites.RemoveAll(f => selectedForDeletion.Contains(f.IDProduct));
						foreach (var productIdToRemove in selectedForDeletion)
						{
							favoriteProducts.Remove(productIdToRemove);
						}
						selectedForDeletion.Clear();
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
		private async Task SendEmailAsync(string email, string subject, string message)
		{
			var emailRequest = new EmailRequest
			{
				To = email,
				Subject = subject,
				Body = message
			};

			var response = await HttpClient.PostAsJsonAsync("api/Email/SendEmail", emailRequest);
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Lỗi gửi Email");
			}
		}

		private class EmailRequest
		{
			public string To { get; set; }
			public string Subject { get; set; }
			public string Body { get; set; }
		}

		private string RandomCode()
		{
			const string chars = "0123456789";
			var random = new Random();
			return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
		}

		private string ToTitleCase(string input)
		{
			if (string.IsNullOrEmpty(input))
				return input;

			var words = input.Split(' ');
			for (int i = 0; i < words.Length; i++)
			{
				if (words[i].Length > 0)
				{
					words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
				}
			}
			return string.Join(" ", words);
		}
		private async Task RemoveFavorite(int productId)
		{
			try
			{
				var response = await HttpClient.DeleteAsync($"api/favorite/DeleteFavorite/{productId}");
				if (response.IsSuccessStatusCode)
				{
					favorites.RemoveAll(f => f.IDProduct == productId);
					favoriteProducts.Remove(productId);
				}
				else
				{
					var errorMessage = await response.Content.ReadAsStringAsync();
					Console.WriteLine($"Lỗi khi xóa sản phẩm yêu thích: {errorMessage}");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi xảy ra khi xóa sản phẩm yêu thích: {ex.Message}");
			}
		}
		private List<BillModel> bills;
		private List<BillModel> allBills;
		private List<BillDetailModel> allBillDetails = new List<BillDetailModel>();
		private List<BillModel> saleBills = new List<BillModel>();

		private int totalBuyCompletedBills = 0;
		private int totalSellCompletedBills = 0;
		private async Task LoadBill()
		{
			try
			{

				allBills = await HttpClient.GetFromJsonAsync<List<BillModel>>("api/Bill/GetBill");
				bills = new List<BillModel>(allBills);

				foreach (var bill in allBills)
				{
					var billDetails = await HttpClient.GetFromJsonAsync<List<BillDetailModel>>($"api/BillDetail/GetBillDetailsByIDBill/{bill.IDBill}");
					bill.BillDetails = billDetails ?? new List<BillDetailModel>();

					foreach (var detail in bill.BillDetails)
					{
						var product = await HttpClient.GetFromJsonAsync<ProductModel>($"api/Product/GetProductById/{detail.IDProduct}");

						if (product != null)
						{
							if (product.UserName == username)
							{
								saleBills.Add(bill); // Đây là đơn hàng bán của người dùng
								break;
							}
						}
					}

					allBillDetails.AddRange(billDetails);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Lỗi khi tải dữ liệu: " + ex.Message);
				bills = new List<BillModel>();
				saleBills = new List<BillModel>();
				allBillDetails = new List<BillDetailModel>();
			}
		}

		// Đếm số lượng đơn hàng hoàn thành cho mua và bán
		private void CountCompletedBills()
		{
			totalBuyCompletedBills = 0;
			totalSellCompletedBills = 0;

			foreach (var bill in bills)
			{
				if (bill.Status == "Hoàn thành")
				{
					// Kiểm tra đơn hàng mua
					if (bill.UserName == username)
					{
						totalBuyCompletedBills++;
					}

					// Kiểm tra đơn hàng bán
					if (bill.SellerUserName == username)
					{
						totalSellCompletedBills++;
					}
				}
			}
		}
		private List<RatingDto> userRatings = new List<RatingDto>();

		private async Task LoadUserDataAsync()
		{
			try
			{
				try
				{
					var response = await HttpClient.GetFromJsonAsync<List<RatingDto>>($"api/Rating/GetRatingsUser{username}");
					userRatings = response ?? new List<RatingDto>();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Lỗi khi lấy đánh giá người dùng: {ex.Message}");
					userRatings = new List<RatingDto>(); // Đảm bảo hiển thị không có đánh giá nào khi có lỗi
				}

				CalculateAverageRating();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Có lỗi xảy ra: {ex.Message}");
			}
			finally
			{
				isLoading = false;
			}
		}

		private double averageRating = 0;
		private void CalculateAverageRating()
		{
			if (userRatings != null && userRatings.Any())
			{
				// Calculate the average of the ratings
				averageRating = userRatings.Average(r => r.Star);
			}
			else
			{
				averageRating = 0;
			}
		}

		private bool isModalVisible = false;

		private void NavigatePost()
		{
			if (userAccount.Email == null || userAccount.ID == null)
			{
				// Hiển thị modal nếu thông tin không đầy đủ
				isModalVisible = true;
			}
			else
			{
				// Điều hướng đến trang đăng bài nếu thông tin đầy đủ
				NavigationManager.NavigateTo("/post", forceLoad: true);
			}
		}

		private void CloseModal()
		{
			isModalVisible = false;
		}
	}
}
