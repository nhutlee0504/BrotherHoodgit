using SanGiaoDich_BrotherHood.Server.Dto;
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public interface ICartItem
    {
        Task<CartItem> AddCartItem(CartItemDto cartItem);
        public Task<IEnumerable<CartItem>> GetAllCartItems(int idCart);
        Task<bool> DeleteCartItem(int cartItemId);
        Task<bool> updateStatus(int idProduct);
    }
}
