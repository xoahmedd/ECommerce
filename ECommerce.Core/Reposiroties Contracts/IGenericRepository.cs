using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities;
using ECommerce.Core.Specification;

namespace ECommerce.Core.Reposiroties_Contracts
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        public Task<T?> GetByIdAsync(int? id);
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T?> GetByIdWithSpecAsync(ISpecification<T> spec);
        public Task<IEnumerable<T>> GetAllWithSpecAsync(ISpecification<T> spec);
        public Task<int> GetCountAsync(ISpecification<T> spec);

        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}

