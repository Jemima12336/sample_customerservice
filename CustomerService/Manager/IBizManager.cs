using System.Collections.Generic;

namespace CustomerService.Standard
{
    public interface IBizManager<TEntity>
        where TEntity: class
    {
        IList<TEntity> GetAll();

        void Add(TEntity entity);

        TEntity GetByID(string id);

        void UpdateByID(string id, TEntity entity);

        bool DeleteByID(string id);
    }
}