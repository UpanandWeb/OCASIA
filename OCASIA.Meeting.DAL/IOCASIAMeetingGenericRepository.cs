using System;
using System.Linq;
using System.Linq.Expressions;

namespace OCASIA.Meeting.DAL
{
    public interface IOlympicsAsiaGenericRepository<T> where T : class
    {
        int Add(T t);
        int Delete(T t);
        IQueryable<T> GetAll();
        int Update(T t);
        int Count();
        T FirstOrDefault(Expression<Func<T, bool>> match);
        T GetFirstOrDefault();
        IQueryable<T> GetAllReffByID(Expression<Func<T, bool>> match);

    }
}
