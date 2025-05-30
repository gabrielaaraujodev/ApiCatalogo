﻿using System.Linq.Expressions;

namespace ApiCatalogo.Repositories
{
    // Esta interface deve ser genérica para todas as entidades.
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        /*Expression<Func<T, bool>> predicate signfica que o método pode receber
        uma função lambda como parâmetro.*/
        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
        T Create(T entity);
        T Update(T entity);
        T Delete(T entity);
    }
}
