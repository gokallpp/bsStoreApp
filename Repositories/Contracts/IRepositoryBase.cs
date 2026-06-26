using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IRepositoryBase<T> 
    {
        // CRUD = R // Read
        IQueryable<T> FindAll(bool trackChanges); // T türündeki tüm varlıklarını geri getirme yöntemi; değişiklikleri izleme seçeneği de mevcuttur.
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges); // Belirli bir koşula uyan varlıkları geri getirme yöntemi; değişiklikleri izleme seçeneği de mevcuttur.

        void cREATE(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
