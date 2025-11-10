using CarsDealersManagement.Domain.Entities;
using PagingPackage;

namespace CarsDealersManagement.Domain.Repositories
{
    public interface IContactPersonsRepository : IBaseRepository<ContactPerson>
    {
        IQueryable<ContactPerson> GetAll();
    }
}