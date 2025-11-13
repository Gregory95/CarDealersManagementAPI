using CarsDealersManagement.Domain.Entities;

namespace CarsDealersManagement.Domain.Repositories
{
    public interface IContactPersonsRepository : IBaseRepository<ContactPerson>
    {
        IQueryable<ContactPerson> GetAll();
    }
}