using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface ICart
    {
        public Task<Cart> GetCartsByUserName(string userName);
        public Task<Cart> AddCart(Cart cart);
        public Task<Cart> UpdateCart(int IDCart, Cart cart);
        public Task<Cart> DeleteCart(int IDCart);
    }
}
