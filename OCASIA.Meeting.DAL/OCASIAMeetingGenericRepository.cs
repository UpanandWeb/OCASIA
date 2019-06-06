using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;

namespace OCASIA.Meeting.DAL
{
    public class OlympicsAsiaGenericRepository<T> : IOlympicsAsiaGenericRepository<T> where T : class
    {
        #region Global Var
        internal OCASIAMeetingContext context;
        internal DbSet<T> dbset;
        #endregion

        #region Constructors
        public OlympicsAsiaGenericRepository(OCASIAMeetingContext context)
        {
            this.context = context;
            this.dbset = context.Set<T>();
            this.context.Configuration.LazyLoadingEnabled = false;
        }
        #endregion        

        #region Sync
        #region Inser entry
        public int Add(T t)
        {
            context.Set<T>().Add(t);
            context.Set<T>().Distinct();

            try
            {
                return context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        var cls = validationErrors.Entry.Entity.GetType().FullName;
                        var prop = validationError.PropertyName;
                        var errmsg = validationError.ErrorMessage;
                        Trace.TraceInformation("Class: {0}, Property: {1}, Error: {2}",
                            validationErrors.Entry.Entity.GetType().FullName,
                            validationError.PropertyName,
                            validationError.ErrorMessage);                       
                    }
                }
                throw;
            }

            //return null;
        }
        #endregion

        #region Delete entry
        public int Delete(T t)
        {
            context.Entry(t).State = EntityState.Deleted;
            return context.SaveChanges();
        }
        #endregion

        #region Get All entry
        public IQueryable<T> GetAll()
        {
            return dbset;
        }
        #endregion

        #region Updat entry
        public int Update(T t)
        {
            int id = 0;
            try
            {
                context.Entry(t).State = EntityState.Modified;
                id = context.SaveChanges();
                return id;
            }
            catch
            {
                return id;
            }
        }
        #endregion

        #region Count entry
        public int Count()
        {
            return context.Set<T>().Count();
        }
        #endregion

        #region Get FirstOrDefault entry
        public T GetFirstOrDefault()
        {
            return context.Set<T>().FirstOrDefault();
        }
        public T FirstOrDefault(System.Linq.Expressions.Expression<Func<T, bool>> match = null)
        {
            var res = context.Set<T>().Where(match);
            return res != null ? res.FirstOrDefault() : null;//wher(predicate)?code speed optimization!
        }
        #endregion

        #region Find All entry
        public IQueryable<T> GetAllReffByID(System.Linq.Expressions.Expression<Func<T, bool>> match = null)
        {
            return dbset.Where(match);
        }
        #endregion

        #endregion
    }
}
