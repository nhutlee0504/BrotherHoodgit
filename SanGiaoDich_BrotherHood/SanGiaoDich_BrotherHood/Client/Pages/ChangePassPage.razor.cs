using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using SanGiaoDich_BrotherHood.Shared.Dto;

namespace SanGiaoDich_BrotherHood.Client.Pages
{
    public partial class ChangePassPage
    {
        private InfoAccountDto account;
        private string LoggedInUser;
        private string oldPass;
        private string newPass;
        private string newPass2;
        private string errorOldPass;
        private string errorNewPass;
        private string errorNewPass2;
        private string errorMessage;
        private string successMessage;

        protected override async Task OnInitializedAsync()
        {
            await CheckLogined();
        }

        private async Task CheckLogined()
        {
            var token = await js.InvokeAsync<string>("localStorage.getItem", "token");

            if (!string.IsNullOrEmpty(token))
            {
                http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                await LoadAccount();
            }
            else
            {
                errorMessage = "Vui lòng đăng nhập";
                StateHasChanged();
                await Task.Delay(2000);
                navigation.NavigateTo("/");
            }
        }

        private async Task LoadAccount()
        {
            try
            {
                var response = await http.GetAsync("api/User/GetMyInfo");

                if (response.IsSuccessStatusCode)
                {
                    account = await response.Content.ReadFromJsonAsync<InfoAccountDto>();
                }
                else
                {
                    errorMessage = await response.Content.ReadAsStringAsync();
                    await Task.Delay(2000);
                    navigation.NavigateTo("/");
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Có lỗi xảy ra: " + ex.Message;
            }
        }

        private async Task SubmitForm()
        {
            ValidateForm();

            if (!string.IsNullOrEmpty(errorOldPass) || !string.IsNullOrEmpty(errorNewPass) || !string.IsNullOrEmpty(errorNewPass2))
            {
                return;
            }

            if (account.Password != GetHash(oldPass))
            {
                errorOldPass = "Mật khẩu cũ không đúng";
            }
            else if (newPass != newPass2)
            {
                errorNewPass2 = "Mật khẩu xác nhận không khớp";
            }
            else
            {
                try
                {
                    var info = new InfoAccountDto()
                    {
                        Password = newPass2
                    };
                    var response = await http.PutAsJsonAsync($"api/User/ChangePassword/{account.UserName}", info);
                    if (response.IsSuccessStatusCode)
                    {
                        errorNewPass2 = null;
                        errorNewPass = null;
                        successMessage = "Đổi mật khẩu thành công";
                        await js.InvokeVoidAsync("localStorage.removeItem", "token");
                        StateHasChanged();
                        await Task.Delay(2000);
                        navigation.NavigateTo("/login", forceLoad: true);
                    }
                    else
                    {
                        errorMessage = await response.Content.ReadAsStringAsync();
                        await Task.Delay(5000);
                        errorMessage = null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    errorMessage = "Đã xảy ra lỗi khi đổi mật khẩu.";
                }
            }
        }

        private void ValidateForm()
        {
            errorOldPass = string.IsNullOrEmpty(oldPass) ? "Vui lòng nhập mật khẩu cũ" : null;
            errorNewPass = string.IsNullOrEmpty(newPass) ? "Vui lòng nhập mật khẩu mới" : null;
            errorNewPass2 = string.IsNullOrEmpty(newPass2) ? "Vui lòng nhập mật khẩu xác nhận" : null;
        }

        private string GetHash(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length && i < 16; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
