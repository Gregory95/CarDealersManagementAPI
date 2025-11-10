using CarsDealersManagement.Domain.Entities;

namespace CarsDealersManagement.Domain.Repositories
{
    public interface IDealersRepository : IBaseRepository<Dealer>
    {
        IQueryable<Dealer> GetAll();
    }
}