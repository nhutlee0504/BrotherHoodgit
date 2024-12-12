
using SanGiaoDich_BrotherHood.Shared.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SanGiaoDich_BrotherHood.Server.Dto;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public interface IFavorite
    {
        public Task<IEnumerable<Favorite>> GetFavoritesAccount();

        public Task<Favorite> AddFavorite(int idProd);

        public Task<Favorite> DeleteFavorite(int idprod);
    }
}
