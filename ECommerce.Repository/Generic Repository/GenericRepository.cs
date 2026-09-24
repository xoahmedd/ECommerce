using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;
using ECommerce.Core.Reposiroties_Contracts;
using ECommerce.Core.Specification;
using ECommerce.Repository.Data;

namespace ECommerce.Repository.Generic_Repository
{
    public class GenericRepository<T> :IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _dbContext;

        public GenericRepository(StoreContext dbContext) 
        {
            _dbContext = dbContext;
        }
        public async Task<T?> GetByIdAsync(int? id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySepcification(spec).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecification<T> spec)
        {
            return await ApplySepcification(spec).ToListAsync();
        }

        public async Task<int> GetCountAsync(ISpecification<T> spec)
        {
            return await ApplySepcification(spec).CountAsync();
        }

        private IQueryable<T> ApplySepcification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), spec);
        }

        public void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }
    }
}

