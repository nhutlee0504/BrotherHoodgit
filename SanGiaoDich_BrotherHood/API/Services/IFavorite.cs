using API.Dto;
using API.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IFavorite
    {
        public Task<IEnumerable<Favorite>> GetFavoritesAccount();

        public Task<Favorite> AddFavorite(int idProd);

        public Task<Favorite> DeleteFavorite(int idprod);
    }
}
