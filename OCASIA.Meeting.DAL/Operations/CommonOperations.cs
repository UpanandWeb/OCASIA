using OCASIA.Meeting.DAL.ApplicationModels;
using System;
using System.Web;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Entity;
using System.Collections;
using System.Web.Mvc;

namespace OCASIA.Meeting.DAL.Operations
{

    public class CommonOperations : Exception
    {
        public OCASIAMeetingContext meetingContext;
        public OCASIAMeetingUOW dbContext;
        public static string FilePath;
        public static string UI_DateFormt;

        public List<CollectionDetails> GetCountryList()
        {
            List<CollectionDetails> lstCountries = new List<CollectionDetails>();
            using (meetingContext = new OCASIAMeetingContext())
            {
                lstCountries = meetingContext.Countries.Where(el => el.IsActive).Select(el => new CollectionDetails() { ID = el.CountryID, Name = el.CountryName }).ToList();
            }
            return lstCountries;
        }

        public List<CollectionDetails> GetTitleList()
        {
            List<CollectionDetails> lstTitles = new List<CollectionDetails>();
            using (meetingContext = new OCASIAMeetingContext())
            {
                lstTitles = meetingContext.Titles.Where(el => el.IsActive).Select(el => new CollectionDetails() { ID = el.TitleID, Name = el.TitleName }).ToList();
            }
            return lstTitles;
        }

        public MeetingDetail GetMeetingDetail(int meetingID, string email = null)
        {
            MeetingDetail model = new MeetingDetail();
            if (meetingID == 0 && string.IsNullOrEmpty(email))
                return null;
            using (meetingContext = new OCASIAMeetingContext())
            {
                if (meetingID == 0)
                {
                    model = meetingContext.UserDetails.Where(el => el.IsActive && el.Email == email ).Select(el =>
                 new MeetingDetail()
                 {
                     MeetingID = el.MeetingID.Value,
                     MeetingName = el.Meetings.MeetingName,
                     UserDetailsID = el.UserDetailID,
                     Tabs = el.Meetings.RegistrationTabs,
                     EmailAddress = el.Meetings.OfficalEmail,
                     PageBanner = el.Meetings.PageBannerPath,
                     FaqDescription = el.Meetings.FaqDescription,
                     FaqPath = el.Meetings.FaqPath,
                     UserName = el.GivenName,
                     StartDate = el.Meetings.EventStartDate,
                     ContactNumber = el.Meetings.PhoneNumber,
                     ReadOnly = el.IsSubmitted == null ? false : el.IsSubmitted.Value,
                     IsPersonalFilled = string.IsNullOrEmpty(el.PassportNumber) ? false : true

                 }
                ).FirstOrDefault();
                    if (model == null)
                        return null;
                    model.IsTravelFilled = meetingContext.TravelDetails.Where(el => el.IsActive && el.UserDetailID == model.UserDetailsID).FirstOrDefault() == null ? true : false;
                    model.IsGuestsFilled = meetingContext.UserDetails.Where(el => el.IsActive && el.GuestOf == model.UserDetailsID.ToString()).FirstOrDefault() == null ? true : false;

                }
                else
                    model = meetingContext.Meetings.Where(el => el.IsActive && el.AllowRegistration && el.IsActive && el.MeetingID == meetingID).Select(el => new MeetingDetail()
                    {
                        MeetingID = el.MeetingID,
                        MeetingName = el.MeetingName,
                        StartDate = el.EventStartDate,
                        Tabs = el.RegistrationTabs,
                        ContactNumber = el.PhoneNumber,
                        EmailAddress = el.OfficalEmail,
                        FaqDescription = el.FaqDescription,
                        FaqPath = el.FaqPath,
                        PageBanner = el.PageBannerPath

                    }).FirstOrDefault();
                if (model == null) return null;

#if DEBUG
                model.PageBanner = "../Content/banners.jpg";
                model.FaqPath = "../Content/banners.jpg";

#else
                model.PageBanner=model.PageBanner?.Replace("~", Helper.FilePath);
                model.FaqPath=model.FaqPath?.Replace("~", Helper.FilePath);

#endif

            }
            return model;
        }

        public MeetingViewModel GetMeetingDetailbyID(int meetingID)
        {
            MeetingViewModel model = new MeetingViewModel();
            using (dbContext = new OCASIAMeetingUOW())
            {
                var el = dbContext.Repository<OCASIA.Meeting.DAL.Meeting>().GetAll().Where(a => a.MeetingID == meetingID).FirstOrDefault();
                if (el != null)
                {
                    model.MeetingID = el.MeetingID;
                    model.MeetingName = el.MeetingName;
                    model.Abbreviation = el.Abbreviation;
                    model.EventStartDate = el.EventStartDate;
                    model.EventEndDate = el.EventEndDate.Value.Date;
                    model.ApplicationStartDate = el.ApplicationStartDate.Value.Date;
                    model.ApplicationEndDate = el.ApplicationEndDate.Value.Date;
                    model.RegistrationTabs = el.RegistrationTabs;
                    model.PhoneNumber = el.PhoneNumber;
                    model.FaxNumber = el.FaxNumber;
                    model.OfficalEmail = el.OfficalEmail;
                    model.Address = el.Address;
                    model.MeetingDescription = el.MeetingDescription;
                    model.FaqDescription = el.FaqDescription;
                    model.FaqPath = el.FaqPath != null ? el.FaqPath.Replace("~/UploadFiles/" + el.MeetingName + "/", "") : null; 
                    model.PageBannerPath = el.PageBannerPath;
                    model.SchedulePath = el.SchedulePath != null ? el.SchedulePath.Replace("~/UploadFiles/" + el.MeetingName + "/", "") : null;
                };

                var res = dbContext.Repository<OCASIA.Meeting.DAL.RegistrationTabDetail>().GetAll().Where(a => a.MeetingID == meetingID).FirstOrDefault();
                if (res != null)
                {
                    model.BasicDescription = res.BasicDescription;
                    model.Description1 = res.Description1;
                    model.Description1PicturePath = res.Description1PicturePath;
                    model.FileName1 = res.FileName1;
                    model.Description2 = res.Description2;
                    model.Description2PicturePath = res.Description2PicturePath;
                    model.FileName2 = res.FileName2;
                    model.Description3 = res.Description3;
                    model.Description3PicturePath = res.Description3PicturePath;
                    model.FileName3 = res.FileName3;
                    model.Description4 = res.Description4;
                    model.Description4PicturePath = res.Description4PicturePath;
                    model.FileName4 = res.FileName4;
                    model.RegistrationTabID = res.RegistrationTabID;
                }
                return model;
            }
        }

        public bool GetUserType(string email)
        {
            using (OCASIAMeetingUOW db = new OCASIAMeetingUOW())
            {
                return db.Repository<ApplicationUser>().GetAll().Where(el => el.Email == email && el.IsActive && el.RoleCustomID == 1).FirstOrDefault() == null ? false : true;
            }

        }

        public List<CollectionDetails> GetRegistrationTabList()
        {
            List<CollectionDetails> lstRegistrationTabs = new List<CollectionDetails>();
            using (meetingContext = new OCASIAMeetingContext())
            {
                lstRegistrationTabs = meetingContext.RegistrationTabs.Where(el => el.IsActive).Select(el => new CollectionDetails() { ID = el.RegistrationTabID, Name = el.DisplayText }).ToList();
            }
            return lstRegistrationTabs;
        }

        public bool AddMettingDeatils(MeetingViewModel model)
        {
            if (model != null)
            {
                using (meetingContext = new OCASIAMeetingContext())
                {
                    Meeting mdl = new Meeting();
                    mdl.Abbreviation = model.Abbreviation;
                    mdl.MeetingName = model.MeetingName;
                    mdl.MeetingDescription = model.MeetingDescription;
                    mdl.EventStartDate = model.EventStartDate;
                    mdl.EventEndDate = model.EventEndDate;
                    mdl.ApplicationStartDate = model.ApplicationStartDate;
                    mdl.ApplicationEndDate = model.ApplicationEndDate;
                    mdl.PageBannerPath = model.PageBannerPath;
                    mdl.FaqDescription = model.FaqDescription;
                    mdl.FaqPath = model.FaqPath;
                    mdl.SchedulePath = model.SchedulePath;
                    mdl.OfficalEmail = model.OfficalEmail;
                    mdl.Address = model.Address;
                    mdl.PhoneNumber = model.PhoneNumber;
                    mdl.FaxNumber = model.FaxNumber;
                    if (model.RegistrationTabs != null)
                    {
                        mdl.RegistrationTabs = model.RegistrationTabs;
                        mdl.AllowRegistration = false;
                    }
                    else
                    {
                        mdl.AllowRegistration = false;
                    }
                    mdl.CreatedBy = model.CreatedBy;
                    mdl.CreatedOn = DateTime.Now;
                    mdl.IsActive = true;
                    mdl.IsPublish = false;

                    meetingContext.Meetings.Add(mdl);
                    meetingContext.SaveChanges();

                    if (!string.IsNullOrEmpty(model.BasicDescription))
                    {
                        RegistrationTabDetail obj = new RegistrationTabDetail();
                        obj.MeetingID = mdl.MeetingID;
                        obj.BasicDescription = model.BasicDescription;
                        obj.Description1 = model.Description1;
                        obj.Description1PicturePath = model.Description1PicturePath;
                        obj.FileName1 = model.FileName1;
                        obj.Description1 = model.Description1;
                        obj.Description1PicturePath = model.Description1PicturePath;
                        obj.FileName1 = model.FileName1;
                        obj.Description2 = model.Description2;
                        obj.Description2PicturePath = model.Description2PicturePath;
                        obj.FileName2 = model.FileName2;
                        obj.Description3 = model.Description3;
                        obj.Description3PicturePath = model.Description3PicturePath;
                        obj.FileName3 = model.FileName3;
                        obj.Description4 = model.Description4;
                        obj.Description4PicturePath = model.Description4PicturePath;
                        obj.FileName4 = model.FileName4;
                        obj.RegistrationTabID = model.RegistrationTabID;
                        obj.CreatedBy = model.CreatedBy;
                        obj.CreatedOn = DateTime.Now;
                        obj.IsActive = true;
                        meetingContext.RegistrationTabDetails.Add(obj);
                        meetingContext.SaveChanges();
                    }
                    return true;
                }
            }
            return false;
        }

        public bool EditMettingDeatils(MeetingViewModel model)
        {
            if (model != null)
            {
                using (dbContext = new OCASIAMeetingUOW())
                {
                    var mdl = dbContext.Repository<OCASIA.Meeting.DAL.Meeting>().GetAll().Where(a => a.MeetingID == model.MeetingID).FirstOrDefault();
                    if (mdl != null)
                    {
                        mdl.Abbreviation = model.Abbreviation;
                        mdl.MeetingName = model.MeetingName;
                        mdl.MeetingDescription = model.MeetingDescription;
                        mdl.EventStartDate = model.EventStartDate;
                        mdl.EventEndDate = model.EventEndDate;
                        mdl.ApplicationStartDate = model.ApplicationStartDate;
                        mdl.ApplicationEndDate = model.ApplicationEndDate;
                        mdl.PageBannerPath = model.PageBannerPath != null ? model.PageBannerPath : mdl.PageBannerPath;
                        mdl.FaqDescription = model.FaqDescription;
                        mdl.FaqPath = model.FaqPath != null ? model.FaqPath : mdl.FaqPath;
                        mdl.SchedulePath = model.SchedulePath != null ? model.SchedulePath : mdl.SchedulePath;
                        mdl.OfficalEmail = model.OfficalEmail;
                        mdl.Address = model.Address;
                        mdl.PhoneNumber = model.PhoneNumber;
                        mdl.FaxNumber = model.FaxNumber;
                        mdl.RegistrationTabs = model.RegistrationTabs;
                        mdl.UpdatedBy = model.CreatedBy;
                        mdl.UpdateOn = DateTime.Now;
                        dbContext.Repository<OCASIA.Meeting.DAL.Meeting>().Update(mdl);
                        dbContext.SaveChanges();
                    }
                    var obj = dbContext.Repository<OCASIA.Meeting.DAL.RegistrationTabDetail>().GetAll().Where(a => a.MeetingID == model.MeetingID).FirstOrDefault();
                    if (obj != null)
                    {
                        obj.MeetingID = mdl.MeetingID;
                        obj.BasicDescription = model.BasicDescription;
                        obj.Description1 = model.Description1;
                        obj.Description1PicturePath = model.Description1PicturePath != null ? model.SchedulePath : obj.Description1PicturePath;
                        obj.FileName1 = model.FileName1 != null ? model.FileName1 : obj.FileName1;
                        obj.Description2 = model.Description2;
                        obj.Description2PicturePath = model.Description2PicturePath != null ? model.Description2PicturePath : obj.Description2PicturePath;
                        obj.FileName2 = model.FileName2 != null ? model.FileName2 : obj.FileName2;
                        obj.Description3 = model.Description3;
                        obj.Description3PicturePath = model.Description3PicturePath != null ? model.Description3PicturePath : obj.Description3PicturePath;
                        obj.FileName3 = model.FileName3 != null ? model.FileName3 : obj.FileName3;
                        obj.Description4 = model.Description4;
                        obj.Description4PicturePath = model.Description4PicturePath != null ? model.Description4PicturePath : obj.Description4PicturePath;
                        obj.FileName4 = model.FileName4 != null ? model.FileName4 : obj.FileName4;
                        obj.RegistrationTabID = model.RegistrationTabID;
                        obj.UpdatedBy = model.CreatedBy;
                        obj.UpdateOn = DateTime.Now;
                        dbContext.Repository<OCASIA.Meeting.DAL.RegistrationTabDetail>().Update(obj);
                        dbContext.SaveChanges();
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
