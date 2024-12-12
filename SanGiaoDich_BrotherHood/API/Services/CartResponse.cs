using API.Data;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class CartResponse : ICart
    {
        private readonly ApplicationDbContext _context;
        public CartResponse(ApplicationDbContext context) => _context = context;
        public async Task<Cart> AddCart(Cart cart)
        {
            try
            {
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
                return cart;
            }
            catch (System.Exception)
            {

                return null;
            }
        }

        public async Task<Cart> DeleteCart(int IDCart)
        {
            try
            {
                var cart = await _context.Carts.FindAsync(IDCart);
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
                return cart;
            }
            catch (System.Exception)
            {

                return null;
            }
        }

        public async Task<Cart> GetCartsByUserName(string userName)
        {
            return await _context.Carts.FirstOrDefaultAsync(x => x.UserName == userName);
        }

        public async Task<Cart> UpdateCart(int IDCart, Cart cart)
        {
            try
            {
                var cartUpdate = await _context.Carts.FindAsync(IDCart);
                if (cartUpdate == null)
                    return null;
                cartUpdate.UserName = cart.UserName;

                _context.Carts.Update(cartUpdate);
                await _context.SaveChangesAsync();
                return cart;
            }
            catch (System.Exception)
            {

                return null;
            }
        }

    }
}
