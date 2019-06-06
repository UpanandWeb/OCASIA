using OCASIA.Meeting.DAL.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace OCASIA.Meeting.DAL.Operations
{
    public class MeetingOperations
    {
        public OCASIAMeetingContext meetingContext;
        public List<MeetingViewModel> GetMeetingDetails()
        {
            using (meetingContext = new OCASIAMeetingContext())
            {
                var res = meetingContext.Meetings.Include(a => a.Users).Where(el => el.IsActive).Select(el => new MeetingViewModel() { MeetingID = el.MeetingID, MeetingName = el.MeetingName, Abbreviation = el.Abbreviation, EventStartDate = el.EventStartDate, EventEndDate = el.EventEndDate, ApplicationStartDate = el.ApplicationStartDate, ApplicationEndDate = el.ApplicationEndDate, CreatedOn = el.CreatedOn, CreatedBy = el.Users.UserName, IsPublish = el.IsPublish, AllowRegistration = el.AllowRegistration }).OrderByDescending(el => el.MeetingID).AsQueryable();
                return res.ToList();
            }
            return null;
        }

        public List<MeetingViewModel> GetMeetingDetailsbyID(int MeetingID)
        {
            using (meetingContext = new OCASIAMeetingContext())
            {
                var res = meetingContext.Meetings.Include(a => a.RegistrationTabs).Include(a => a.RegistrationTabDetails).Include(a => a.Users).Where(el => el.MeetingID == MeetingID).Select(el => new MeetingViewModel() { MeetingID = el.MeetingID, MeetingName = el.MeetingName, Abbreviation = el.Abbreviation, EventStartDate = el.EventStartDate, EventEndDate = el.EventEndDate, ApplicationStartDate = el.ApplicationStartDate, ApplicationEndDate = el.ApplicationEndDate, CreatedOn = el.CreatedOn, CreatedBy = el.Users.UserName, IsPublish = el.IsPublish }).ToList();
                return res;
            }
            return null;
        }
    }
}
