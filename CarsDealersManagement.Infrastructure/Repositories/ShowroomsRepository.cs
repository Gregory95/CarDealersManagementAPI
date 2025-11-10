using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Repositories;
using CarsDealersManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using PagingPackage;


namespace CarsDealersManagement.Infrastructure.Repositories
{
    public class ShowroomsRepository(AppDbContext _db) : IShowroomsRepository
    {
        public async Task AddAsync(Showroom message, CancellationToken ct)
        {
            await _db.Showrooms.AddAsync(message, ct);
            await _db.SaveChangesAsync(ct);
        }

        public void Delete(int Id)
        {
            var showroom = _db.Showrooms.Find(Id);
            if (showroom is null) throw new ApplicationException("Showroom not found.");
            _db.Showrooms.Remove(showroom);
            _db.SaveChanges();
        }

        public async Task DeleteAsync(int Id, CancellationToken ct)
        {
            var showroom = await _db.Showrooms.FindAsync(Id, ct);
            if (showroom is null) throw new ApplicationException("showroom not found.");
            _db.Showrooms.Remove(showroom);
            await _db.SaveChangesAsync(ct);
        }

        public IQueryable<Showroom> GetAll()
        {
            return _db.Showrooms.AsQueryable();
        }

        public async Task<IReadOnlyList<Showroom>> GetAllAsync(CancellationToken ct)
        {
            return await _db.Showrooms.AsNoTracking().ToListAsync(ct);
        }

        public async Task<Showroom?> GetByIdAsync(int Id, CancellationToken ct)
        {
            return await _db.Showrooms.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id, ct);
        }

        public async Task UpdateAsync(Showroom message, CancellationToken ct)
        {
            var showroom = await _db.Showrooms.FindAsync(message.Id, ct);
            if (showroom is null) throw new ApplicationException("Showroom not found.");
            _db.Entry(showroom).CurrentValues.SetValues(message);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<PagingToolkit<Showroom>> GetPagedAsync(IQueryable<Showroom> query, int pageNumber, int pageSize, CancellationToken ct)
        {
            return await PagingToolkit<Showroom>.CreateAsync(query, pageNumber, pageSize, ct);
        }
    }
}
