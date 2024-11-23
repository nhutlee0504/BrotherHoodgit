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

namespace SanGiaoDich_BrotherHood.Client.Pages
{
    public partial class Post
    {
        private string ProductName { get; set; }
        private decimal ProductPrice { get; set; }
        private int ProductQuantity { get; set; }
        private int ProductCategoryId { get; set; }
        private string Description { get; set; }
        private string ProrityLevel { get; set; }
        private List<Category> categories = new List<Category>();
        private List<ImageFile> selectedFiles = new List<ImageFile> { null, null, null };

        private string ProductNameError { get; set; }
        private string ProductCategoryError { get; set; }
        private string ProductPriceError { get; set; }
        private string ProductQuantityError { get; set; }
        private string ProrityLevelError { get; set; }
        private string ImageError { get; set; }


        private class ImageFile
        {
            public string DataUrl { get; set; }
        }
        private void CloseImageErrorModal()
        {
            ImageError = string.Empty; // Đóng modal bằng cách xóa lỗi
        }


        protected override async Task OnInitializedAsync()
        {
            try
            {
                await LoadUserData();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Có lỗi xảy ra khi tải dữ liệu: {ex.Message}");
            }
        }

        private async Task LoadUserData()
        {
            try
            {
                categories = await HttpClient.GetFromJsonAsync<List<Category>>("api/Category/GetCategories") ?? new List<Category>();
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Không thể tải danh sách loại sản phẩm: {ex.Message}");
            }
        }

        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            try
            {
                var files = e.GetMultipleFiles(3);

                // Kiểm tra số lượng ảnh chọn được
                if (files.Count < 1 || files.Count > 3)
                {
                    ImageError = "Bạn phải chọn ít nhất 1 ảnh và tối đa 3 ảnh.";
                    selectedFiles.Clear();
                    return; // Không tiếp tục xử lý nếu số lượng ảnh sai
                }

                // Đặt lại lỗi hình ảnh
                ImageError = string.Empty;

                foreach (var file in files)
                {
                    for (int i = 0; i < selectedFiles.Count; i++)
                    {
                        if (selectedFiles[i] == null)
                        {
                            selectedFiles[i] = new ImageFile
                            {
                                DataUrl = await GetImageDataUrl(file)
                            };
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ImageError = "Bạn phải chọn ít nhất 1 ảnh và tối đa 3 ảnh.";
                selectedFiles.Clear();
                return; // Không tiếp tục xử lý nếu số lượng ảnh sai
            }
        }


        private async Task<string> GetImageDataUrl(IBrowserFile file)
        {
            try
            {
                using var stream = file.OpenReadStream(maxAllowedSize: 1024 * 1024 * 3);
                var buffer = new byte[file.Size];
                await stream.ReadAsync(buffer);
                return $"data:{file.ContentType};base64,{Convert.ToBase64String(buffer)}";
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Không thể đọc dữ liệu hình ảnh: {ex.Message}");
                return string.Empty;
            }
        }

        private async Task SaveProduct()
        {
            // Reset lỗi
            ProductNameError = string.Empty;
            ProductCategoryError = string.Empty;
            ProductPriceError = string.Empty;
            ProductQuantityError = string.Empty;
            ProrityLevelError = string.Empty;

            // Kiểm tra dữ liệu
            if (string.IsNullOrWhiteSpace(ProductName))
            {
                ProductNameError = "Vui lòng nhập tên sản phẩm.";
            }

            if (ProductCategoryId == 0)
            {
                ProductCategoryError = "Vui lòng chọn loại sản phẩm.";
            }

            if (ProductPrice <= 0)
            {
                ProductPriceError = "Giá sản phẩm phải lớn hơn 0.";
            }

            if (ProductQuantity <= 0)
            {
                ProductQuantityError = "Số lượng sản phẩm phải lớn hơn 0.";
            }

            if (string.IsNullOrWhiteSpace(ProrityLevel))
            {
                ProrityLevelError = "Vui lòng chọn mức độ ưu tiên.";
            }
            var selectedImageCount = selectedFiles.Count(file => file != null);
            if (selectedImageCount < 1 || selectedImageCount > 3)
            {
                ImageError = "Bạn phải chọn ít nhất 1 ảnh và tối đa 3 ảnh.";
                selectedFiles.Clear();
                return; // Dừng lại và hiển thị modal
            }

            // Nếu có lỗi thì không tiếp tục
            if (!string.IsNullOrWhiteSpace(ProductNameError) ||
                !string.IsNullOrWhiteSpace(ProductCategoryError) ||
                !string.IsNullOrWhiteSpace(ProductPriceError) ||
                !string.IsNullOrWhiteSpace(ProductQuantityError) ||
                !string.IsNullOrWhiteSpace(ProrityLevelError))
            {
                return;
            }

            // Tiếp tục lưu sản phẩm
            try
            {
                var productDto = new ProductDto
                {
                    Name = ProductName,
                    Price = ProductPrice,
                    Quantity = ProductQuantity,
                    CategoryId = ProductCategoryId,
                    Description = Description,
                    ProrityLevel = ProrityLevel
                };

                var response = await HttpClient.PostAsJsonAsync("api/product/AddProduct", productDto);

                if (response.IsSuccessStatusCode)
                {
                    var createdProduct = await response.Content.ReadFromJsonAsync<Product>();
                    var productId = createdProduct?.IDProduct ?? 0;

                    if (productId > 0)
                    {
                        await UploadImages(productId);
                    }
                    else
                    {
                        await JSRuntime.InvokeVoidAsync("alert", "Không thể lấy thông tin sản phẩm mới tạo.");
                    }
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    await JSRuntime.InvokeVoidAsync("alert", $"Có lỗi xảy ra khi thêm sản phẩm: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Có lỗi xảy ra khi thêm sản phẩm: {ex.Message}");
            }
        }

        private async Task UploadImages(int productId)
        {
            try
            {
                var content = new MultipartFormDataContent();

                foreach (var file in selectedFiles)
                {
                    if (file != null)
                    {
                        var byteArray = Convert.FromBase64String(file.DataUrl.Split(",")[1]);
                        var fileContent = new ByteArrayContent(byteArray);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                        content.Add(fileContent, "files", $"image_{Guid.NewGuid()}.jpg");
                    }
                }

                var uploadResponse = await HttpClient.PostAsync($"api/ImageProduct/ImageProduct/{productId}/upload", content);

                if (uploadResponse.IsSuccessStatusCode)
                {
                    await JSRuntime.InvokeVoidAsync("alert", "Sản phẩm đã được thêm thành công!");
                    NavigationManager.NavigateTo("/", forceLoad: true);
                }
                else
                {
                    var errorMessage = await uploadResponse.Content.ReadAsStringAsync();
                    await JSRuntime.InvokeVoidAsync("alert", $"Có lỗi xảy ra khi tải ảnh: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Có lỗi xảy ra khi tải ảnh: {ex.Message}");
            }
        }
    }
}
