using CarsDealersManagement.Domain.Entities;

namespace CarsDealersManagement.Domain.Repositories
{
    public interface IShowroomsRepository : IBaseRepository<Showroom>
    {
        IQueryable<Showroom> GetAll();
    }
}