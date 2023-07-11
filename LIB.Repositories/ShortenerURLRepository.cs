using DAL.DataAccess.Models;
using LIB.Infrastructure;

namespace LIB.Repositories
{

    public interface IShortenerURLRepository : IRepository<ShortenerURL>
    {
    }

    public class ShortenerURLRepository : Repository<ShortenerURL>, IShortenerURLRepository
    {
        public ShortenerURLRepository(IDbFactory factory) : base(factory)
        {
        }
    }
}