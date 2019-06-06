using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCASIA.Meeting.DAL
{
    public interface IOCASIAMeetingUOW
    {
        void SaveChanges();
        IOlympicsAsiaGenericRepository<T> Repository<T>() where T : class;
    }
}
