using API.Data;
using API.Dto;
using API.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class CartItemResponse : ICartItem
    {
        private readonly ApplicationDbContext _context;
        public CartItemResponse(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CartItem> AddCartItem(CartItemDto cartItem)
        {
            var cartFind = await _context.Carts.FindAsync(cartItem.CartID);
            var prodFind = await _context.Products.FindAsync(cartItem.ProductID);
            if (cartFind == null)
            {
                throw new NotImplementedException("Không tìm thấy giỏ hàng");
            }
            if(prodFind == null)
            {
                throw new NotImplementedException("Không tìm thấy sản phẩm");
            }
            var newCartItem = new CartItem
            {
                IDCart = cartItem.CartID,
                IDProduct = cartItem.ProductID,
            };
            await _context.CartItems.AddAsync(newCartItem);
            await _context.SaveChangesAsync();
            return newCartItem;
        }
        public async Task<IEnumerable<CartItem>> GetAllCartItems(int idCart) // Thêm phương thức này
        {
            var cartitemFind = await _context.CartItems.Where(x => x.IDCart == idCart).ToListAsync();
            if (cartitemFind == null) 
            {
                throw new NotImplementedException("Không có sản phẩm trong giỏ hàng");
            }
            return cartitemFind;
        }
        public async Task<bool> DeleteCartItem(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem == null)
            {
                // Không tìm thấy sản phẩm trong giỏ hàng
                return false;
            }

            _context.CartItems.Remove(cartItem);  // Xóa sản phẩm khỏi giỏ hàng
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
