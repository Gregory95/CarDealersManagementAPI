using CarsDealersManagement.Domain.Entities;
using CarsDealersManagement.Domain.Repositories;
using CarsDealersManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using PagingPackage;


namespace CarsDealersManagement.Infrastructure.Repositories
{
    public class DealersRepository(AppDbContext _db) : IDealersRepository
    {
        public async Task AddAsync(Dealer message, CancellationToken ct)
        {
            await _db.Dealers.AddAsync(message, ct);
            await _db.SaveChangesLightAsync(ct);
        }

        public void Delete(int Id)
        {
            var dealer = _db.Dealers.Find(Id);
            if (dealer is null) throw new ApplicationException("Dealer not found.");
            _db.Dealers.Remove(dealer);
            _db.SaveChanges();
        }

        public async Task DeleteAsync(int Id, CancellationToken ct)
        {
            var dealer = await _db.Dealers.FindAsync(Id, ct);
            if (dealer is null) throw new ApplicationException("Dealer not found.");
            _db.Dealers.Remove(dealer);
            await _db.SaveChangesAsync(ct);
        }

        public IQueryable<Dealer> GetAll()
        {
            return _db.Dealers.AsQueryable();
        }

        public async Task<IReadOnlyList<Dealer>> GetAllAsync(CancellationToken ct)
        {
            return await _db.Dealers.AsNoTracking().ToListAsync(ct);
        }

        public async Task<Dealer?> GetByIdAsync(int Id, CancellationToken ct)
        {
            return await _db.Dealers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == Id, ct);
        }

        public async Task UpdateAsync(Dealer message, CancellationToken ct)
        {
            var dealer = await _db.Dealers.FindAsync(message.Id, ct);
            if (dealer is null) throw new ApplicationException("Dealer not found.");
            _db.Entry(dealer).CurrentValues.SetValues(message);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<PagingToolkit<Dealer>> GetPagedAsync(IQueryable<Dealer> query, int pageNumber, int pageSize, CancellationToken ct)
        {
            return await PagingToolkit<Dealer>.CreateAsync(query, pageNumber, pageSize, ct);
        }
    }
}
