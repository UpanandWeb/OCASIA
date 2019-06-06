using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCASIA.Meeting.DAL.ApplicationModels
{
    public class EnumsClass
    {
        public enum RoleEnum
        {
            Admin = 1,
            User = 2,
            Participant = 3           
        }

       
    }
    public enum MeetingPage
    {
        Active = 1,
        Experired,
        NotPermitted,
        DoesNotExists
    }

    public enum OpertionState
    {
        Created = 1,
        Updated,
        Deleted,
        Error

    }

    public enum RegistrationTab
    {
        regstration,
        userdetails,
        travel

    }
}
