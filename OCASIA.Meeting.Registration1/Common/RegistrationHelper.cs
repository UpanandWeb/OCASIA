using OCASIA.Meeting.DAL.ApplicationModels;
using OCASIA.Meeting.DAL.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OCASIA.Meeting.Registration
{
    public class RegistrationHelper
    {
        //static List<CollectionDetails> _titleList;
        //static List<CollectionDetails> _countryList;
        //public static List<CollectionDetails> TitleList { get { if (_titleList == null) { _titleList = (new CommonOperations()).GetTitleList(); } return _titleList; } }
        //public static List<CollectionDetails> CountryList { get { if (_countryList == null) { _countryList = (new CommonOperations()).GetCountryList(); } return _countryList; } }
        //public int MeetingID { get; set; }
        //public string MeetingName { get; set; }
        //public int UserDetailsID = 0;
        //public string UserName { get; set; }
        //MeetingDetail _meetingDetails;
        //public MeetingDetail MeetingDetails { get { if (_meetingDetails == null) { _meetingDetails = (new CommonOperations()).GetMeetingDetail(MeetingID); } return _meetingDetails; } }
        //List<CollectionDetails> tabs;
        //public List<CollectionDetails> Tabs
        //{
        //    get
        //    {
        //        if (MeetingDetails != null && tabs == null)
        //        {
        //            tabs = new List<CollectionDetails>();
        //            foreach (var item in MeetingDetails?.Tabs.Split(',').ToList())
        //            {
        //                string name = string.Empty;
        //                CollectionDetails data = new CollectionDetails(); ;
        //                switch (item)
        //                {
        //                    case "1": data = new CollectionDetails() { ID = 1, Name = "Information", IsSelected = true }; break;
        //                    case "2": data = new CollectionDetails() { ID = 2, Name = "Personal Details", IsSelected = false }; break;
        //                    case "3": data = new CollectionDetails() { ID = 3, Name = "Travel", IsSelected = false }; break;
        //                    case "4": data = new CollectionDetails() { ID = 4, Name = "Event Location", IsSelected = false }; break;
        //                    case "5": data = new CollectionDetails() { ID = 5, Name = "Guests", IsSelected = false }; break;
        //                }
        //                tabs.Add(data);
        //            }
        //        }
        //        return tabs;


        //    }
        //}
        //public bool ShowLogOut { get; set; } = false;
    }
}