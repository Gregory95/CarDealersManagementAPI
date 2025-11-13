using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Repositories;
using CarsDealersManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Pagination.Query.EntityFramework;


namespace CarsDealersManagement.Infrastructure.Repositories
{
    public class ContactPersonsRepository(AppDbContext _db) : IContactPersonsRepository
    {
        public async Task AddAsync(ContactPerson message, CancellationToken ct)
        {
            await _db.ContactPersons.AddAsync(message, ct);
            await _db.SaveChangesAsync(ct);
        }

        public void Delete(int Id)
        {
            var contactPerson = _db.ContactPersons.Find(Id);
            if (contactPerson is null) throw new ApplicationException("Contact person not found.");
            _db.ContactPersons.Remove(contactPerson);
            _db.SaveChanges();
        }

        public async Task DeleteAsync(int Id, CancellationToken ct)
        {
            var contactPerson = await _db.ContactPersons.FindAsync(Id, ct);
            if (contactPerson is null) throw new ApplicationException("Contact person not found.");
            _db.ContactPersons.Remove(contactPerson);
            await _db.SaveChangesAsync(ct);
        }

        public IQueryable<ContactPerson> GetAll()
        {
            return _db.ContactPersons.AsQueryable();
        }

        public async Task<IReadOnlyList<ContactPerson>> GetAllAsync(CancellationToken ct)
        {
            return await _db.ContactPersons.AsNoTracking().ToListAsync(ct);
        }

        public async Task<ContactPerson?> GetByIdAsync(int Id, CancellationToken ct)
        {
            return await _db.ContactPersons.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id, ct);
        }

        public async Task<PagingWrap<ContactPerson>> GetPagedAsync(IQueryable<ContactPerson> query, int pageNumber, int pageSize, CancellationToken ct)
        {
            return await PagingWrap<ContactPerson>.CreateAsync(query, pageNumber, pageSize, ct);
        }

        public async Task UpdateAsync(ContactPerson message, CancellationToken ct)
        {
            var contactPerson = await _db.ContactPersons.FindAsync(message.Id, ct);
            if (contactPerson is null) throw new ApplicationException("Contact person not found.");
            _db.Entry(contactPerson).CurrentValues.SetValues(message);
            await _db.SaveChangesAsync(ct);
        }
    }
}
