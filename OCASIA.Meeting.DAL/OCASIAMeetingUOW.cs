using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace OCASIA.Meeting.DAL
{
    public class OCASIAMeetingUOW : IOCASIAMeetingUOW, IDisposable
    {
        private OCASIAMeetingContext context = null;
        private bool disposed;
        private readonly Dictionary<Type, object> repositories;

        public OCASIAMeetingUOW()
        {
            context = new OCASIAMeetingContext();
            repositories = new Dictionary<Type, object>();
            disposed = false;
            context.Configuration.ProxyCreationEnabled = false;
            context.Configuration.AutoDetectChangesEnabled = false;
        }

        public IOlympicsAsiaGenericRepository<T> Repository<T>() where T : class
        {
            IOlympicsAsiaGenericRepository<T> repo = new OlympicsAsiaGenericRepository<T>(context);
            return repo;
        }

        #region Disposable


        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        #region SaveChanges
        public void SaveChanges()
        {
            try
            {
                context.SaveChanges();
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
        }
        #endregion

    }
}
