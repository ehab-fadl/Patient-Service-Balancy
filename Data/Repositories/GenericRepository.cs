using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using PatientService.Data.Context;
using PatientService.Models.Common;
using PatientService.Models.Interfaces;

namespace PatientService.Data.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly PatientDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(PatientDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.AsNoTracking().ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AsNoTracking().Where(predicate).ToListAsync();
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        _context.Add(entity);

        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        // إذا كان الكيان غير متتبع (Detached)، نقوم بإرفاقه وتحديده كـ Modified
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Update(entity);
        }
        // إذا كان متتبعاً بالفعل، فلا داعي لعمل أي شيء، EF Core سيعرف التغييرات تلقائياً
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        entity.IsDeleted = true;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(e => e.Id == id);
    }

    public virtual async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }
}