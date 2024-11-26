using API.Dto;
using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface ICartItem
    {
        Task<CartItem> AddCartItem(CartItemDto cartItem);
        public Task<IEnumerable<CartItem>> GetAllCartItems(int inCart);
        Task<bool> DeleteCartItem(int cartItemId);
    }
}
