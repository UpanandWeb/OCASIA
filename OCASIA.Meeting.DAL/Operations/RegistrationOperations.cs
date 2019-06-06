using System;
using System.Linq;
using Microsoft.AspNet.Identity;
using OCASIA.Meeting.DAL.ApplicationModels;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Collections.Generic;

namespace OCASIA.Meeting.DAL.Operations
{
    public class RegistrationOperations
    {
        public string MeetingName { get; set; }
        public string Tabs { get; set; }
        public int MeetingID { get; set; }
        public OCASIAMeetingContext meetingContext;

        /// <summary>
        /// Step 1 : user creation
        /// </summary>
        /// <param name="model">Registration details</param>
        /// <returns></returns>
        public int RegisterUser(ParticipantRegistration model)
        {
            string createdBy = string.Empty;
            int userDetailsID = 0;
            #region Create useer
            using (OCASIAMeetingUOW db = new OCASIAMeetingUOW())
            {
                var res = db.Repository<ApplicationUser>().GetAll().Where(f => f.Email == model.Email).FirstOrDefault();
                if (res == null)
                {
                    var user = new ApplicationUser()
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        RoleCustomID = 3,
                        IsActive = true,
                        IspasswordActive = true
                    };
                    var userManager = new Microsoft.AspNet.Identity.UserManager<ApplicationUser>(new Microsoft.AspNet.Identity.EntityFramework.UserStore<ApplicationUser>(new OCASIAMeetingContext()));
                    var ures = userManager.Create(user, model.Password);
                    if (ures.Succeeded)
                    {
                        // userManager.AddClaim(user.Id, new System.Security.Claims.Claim("FullName", model.Email));
                        Passwordhash mdl = new Passwordhash()
                        {
                            UserId = user.Id,
                            Password = model.Password
                        };
                        db.Repository<Passwordhash>().Add(mdl);
                        db.SaveChanges();
                        createdBy = mdl.UserId;
                    }
                }
                else
                    return -1;
            }
            #endregion
            if (string.IsNullOrEmpty(createdBy))
                return userDetailsID;
            userDetailsID = CreateUserDetails(model, createdBy);
            return userDetailsID;
        }

        private int CreateUserDetails(ParticipantRegistration model, string createdBy)
        {
            int userDetailsID = 0;
            using (meetingContext = new OCASIAMeetingContext())
            {
                UserDetail request = new UserDetail()
                {
                    FirstLastName = model.FirstLastName,
                    TitleID = model.TitleID,
                    FamilyName = model.FamilyName,
                    GivenName = model.GivenName,
                    Email = model.Email,
                    IsActive = true,
                    IsPublish = true,
                    CreatedBy = createdBy,
                    CreatedOn = DateTime.Now,
                    MeetingID = model.MeetingID
                };

                meetingContext.UserDetails.Add(request);
                meetingContext.SaveChanges();
                userDetailsID = request.UserDetailID;
                return userDetailsID;
            }

        }




        #region pesonal details



        /// <summary>
        /// Step 2: User details
        /// </summary>
        /// <param name="userDetail">Personal details</param>
        /// <returns></returns>

        public bool UpdatePersonalDetails(UserDetailsModel userDetail, bool Isguest)
        {
            bool success = false;
            using (OCASIAMeetingUOW db = new OCASIAMeetingUOW())
            {
                var response = db.Repository<UserDetail>().GetAll().Where(el => el.IsActive && el.UserDetailID == userDetail.UserDetailID).FirstOrDefault();
                if (response == null)
                    return success;
                if (Isguest == true)
                {
                    var res = db.Repository<UserDetail>().GetAll().Where(el => el.IsActive && el.IsGust == Isguest && el.UserDetailID == userDetail.UserDetailID).FirstOrDefault();
                    if (res == null)
                    {
                        UserDetail mdl = new UserDetail();

                        mdl.TitleID = userDetail.TitleID;
                        mdl.GivenName = userDetail.GivenName;
                        mdl.FamilyName = userDetail.FamilyName;
                        mdl.Email = userDetail.Email;
                        mdl.FirstLastName = userDetail.FirstLastName;
                        mdl.AssitantEmail = userDetail.AssitantEmail;
                        mdl.CountryOfResidanceID = userDetail.CountryOfResidanceID;
                        mdl.CountryID = userDetail.CountryID;
                        mdl.Organization = userDetail.Organization;
                        mdl.OrganizationDesignation = userDetail.OrganizationDesignation;
                        mdl.OrganizationFunction = userDetail.OrganizationFunction;
                        mdl.Company = userDetail.Company;
                        mdl.AddressLine1 = userDetail.AddressLine1;
                        mdl.AddressLine2 = userDetail.AddressLine2;
                        mdl.PostalCode = userDetail.PostalCode;
                        mdl.City = userDetail.City;
                        mdl.StateProvince = userDetail.StateProvince;
                        mdl.TelephoneNumber = userDetail.TelephoneNumber;
                        mdl.PassportNumber = userDetail.PassportNumber;
                        mdl.UploadedPicturePath = userDetail.UploadedPicturePath;
                        mdl.Desc4 = userDetail.Desc4;
                        mdl.PassportCoptyPath = userDetail.PassportCoptyPath;
                        mdl.DOB = userDetail.DOB;
                        mdl.IssueDate = userDetail.IssueDate;
                        mdl.Nationality = userDetail.Nationality;
                        mdl.ExpiryDate = userDetail.ExpiryDate;
                        mdl.Gender = userDetail.Gender;
                        mdl.GuestOf = userDetail.UserDetailID.ToString();
                        mdl.IsGust = Isguest;
                        mdl.IsActive = true;
                        mdl.IsPublish = true;
                        mdl.CreatedBy = userDetail.CreatedBy;
                        mdl.CreatedOn = DateTime.Now;
                        db.Repository<UserDetail>().Add(mdl);
                        db.SaveChanges();
                    }
                    else
                    {
                        res.TitleID = userDetail.TitleID;
                        res.GivenName = userDetail.GivenName;
                        res.FamilyName = userDetail.FamilyName;
                        res.Email = userDetail.Email;
                        res.FirstLastName = userDetail.FirstLastName;
                        res.AssitantEmail = userDetail.AssitantEmail;
                        res.CountryOfResidanceID = userDetail.CountryOfResidanceID;
                        res.CountryID = userDetail.CountryID;
                        res.Organization = userDetail.Organization;
                        res.OrganizationDesignation = userDetail.OrganizationDesignation;
                        res.OrganizationFunction = userDetail.OrganizationFunction;
                        res.Company = userDetail.Company;
                        res.AddressLine1 = userDetail.AddressLine1;
                        res.AddressLine2 = userDetail.AddressLine2;
                        res.PostalCode = userDetail.PostalCode;
                        res.City = userDetail.City;
                        res.StateProvince = userDetail.StateProvince;
                        res.TelephoneNumber = userDetail.TelephoneNumber;
                        res.PassportNumber = userDetail.PassportNumber;
                        res.UploadedPicturePath = userDetail.UploadedPicturePath != null ? userDetail.UploadedPicturePath : null;
                        res.Desc4 = userDetail.Desc4 != null ? userDetail.Desc4 : null;
                        res.PassportCoptyPath = userDetail.PassportCoptyPath != null ? userDetail.PassportCoptyPath : null;
                        res.DOB = userDetail.DOB;
                        res.IssueDate = userDetail.IssueDate;
                        res.Nationality = userDetail.Nationality;
                        res.ExpiryDate = userDetail.ExpiryDate;
                        res.Gender = userDetail.Gender;
                        res.CreatedBy = userDetail.UpdatedBy;
                        res.UpdateOn = DateTime.Now;
                        db.Repository<UserDetail>().Update(res);
                        db.SaveChanges();
                    }
                }
                else {
                    response.FirstLastName = userDetail.FirstLastName;
                    response.AssitantEmail = userDetail.AssitantEmail;
                    response.CountryOfResidanceID = userDetail.CountryOfResidanceID;
                    response.CountryID = userDetail.CountryID;
                    response.Organization = userDetail.Organization;
                    response.OrganizationDesignation = userDetail.OrganizationDesignation;
                    response.OrganizationFunction = userDetail.OrganizationFunction;
                    response.Company = userDetail.Company;
                    response.AddressLine1 = userDetail.AddressLine1;
                    response.AddressLine2 = userDetail.AddressLine2;
                    response.PostalCode = userDetail.PostalCode;
                    response.City = userDetail.City;
                    response.StateProvince = userDetail.StateProvince;
                    response.TelephoneNumber = userDetail.TelephoneNumber;
                    response.PassportNumber = userDetail.PassportNumber;
                    response.UploadedPicturePath = userDetail.UploadedPicturePath != null ? userDetail.UploadedPicturePath : null;
                    response.Desc4 = userDetail.Desc4 != null ? userDetail.Desc4 : null;
                    response.PassportCoptyPath = userDetail.PassportCoptyPath != null ? userDetail.PassportCoptyPath : null;
                    response.DOB = userDetail.DOB;
                    response.IssueDate = userDetail.IssueDate;
                    response.Nationality = userDetail.Nationality;
                    response.ExpiryDate = userDetail.ExpiryDate;
                    response.UpdatedBy = userDetail.UpdatedBy;
                    response.UpdateOn = DateTime.Now;
                    response.Gender = userDetail.Gender;
                    db.Repository<UserDetail>().Update(response);
                    db.SaveChanges();
                }
                success = true;
            }
            return success;


        }

        public bool ValidateUserWithMeetingDetails(string email)
        {
            using (OCASIAMeetingUOW db = new OCASIAMeetingUOW())
            {
                return db.Repository<UserDetail>().GetAll().Where(el => el.IsActive && el.Email == email && el.Meetings.AllowRegistration == true).FirstOrDefault() == null ? false : true;
            }
        }

        public UserDetailsModel GetUserPersonalDetails(int UserDetailID)
        {
            UserDetailsModel model = new UserDetailsModel();
            using (OCASIAMeetingUOW db = new OCASIAMeetingUOW())
            {
                model = db.Repository<UserDetail>().GetAll().Include(el => el.Meetings).Where(el => el.IsActive && el.UserDetailID == UserDetailID).Select(el =>
                     new UserDetailsModel()
                     {
                         TitleID = el.TitleID,
                         UserDetailID = el.UserDetailID,
                         Gender = el.Gender,
                         FirstLastName = el.FirstLastName,
                         FamilyName = el.FamilyName,
                         MeetingName = el.Meetings.MeetingName,
                         AddressLine1 = el.AddressLine1,
                         AddressLine2 = el.AddressLine2,
                         AssitantEmail = el.AssitantEmail,
                         City = el.City,
                         Company = el.Company,
                         DOB = el.DOB,
                         Email = el.Email,
                         ExpiryDate = el.ExpiryDate,
                         GivenName = el.GivenName,
                         IssueDate = el.IssueDate,
                         Nationality = el.Nationality,
                         Organization = el.Organization,
                         OrganizationDesignation = el.OrganizationDesignation,
                         PassportNumber = el.PassportNumber,
                         PostalCode = el.PostalCode,
                         StateProvince = el.StateProvince,
                         TelephoneNumber = el.TelephoneNumber,
                         Title = el.Titles.TitleName,
                         UploadedPicturePath = el.UploadedPicturePath,
                         Desc4 = el.Desc4,
                         Desc3 = el.PassportCoptyPath != null ? el.PassportCoptyPath.Replace("~/Participant/" + el.Meetings.MeetingName + "/", "") : null,
                         PassportCoptyPath = el.PassportCoptyPath != null ? el.PassportCoptyPath.Replace("~/", "") : null,
                         CountryID = el.CountryID == null ? 0 : el.CountryID.Value,
                         CountryOfResidanceID = el.CountryOfResidanceID == null ? 0 : el.CountryOfResidanceID.Value
                     }).FirstOrDefault();



                if (model != null)
                {
                    if (model.CountryID != 0)
                        model.CountryName = db.Repository<Mst_Country>().GetAll().Where(el => el.CountryID == model.CountryID).Select(el => el.CountryName).FirstOrDefault();
                    if (model.CountryOfResidanceID != 0)
                        model.CountryOfResidance = db.Repository<Mst_Country>().GetAll().Where(el => el.CountryID == model.CountryOfResidanceID).Select(el => el.CountryName).FirstOrDefault();

#if DEBUG
                    model.UploadedPicturePath = model.UploadedPicturePath != null ? model.UploadedPicturePath.Replace("~", "..") : null;
                    model.Desc4 = model.Desc4 != null ? model.Desc4.Replace("~", "..") : null;
                    model.Desc3 = model.PassportCoptyPath != null ? model.PassportCoptyPath.Replace("~/Participant/" + model.MeetingName + "/", "") : null;
                    model.PassportCoptyPath = model.PassportCoptyPath != null ? model.PassportCoptyPath.Replace("~/", "") : null;
#else
                    model.UploadedPicturePath = model.UploadedPicturePath?.Replace("~", CommonOperations.FilePath);
                    model.PassportCoptyPath = model.PassportCoptyPath?.Replace("~", CommonOperations.FilePath);
                    model.PassportCoptyPath = model.PassportCoptyPath?.Replace("~", CommonOperations.FilePath);
#endif

                }
            }
            return model;
        }



        #endregion

        #region Travel Operations
        /// <summary>
        /// Step 3: add travel details
        /// </summary>
        /// <param name="userDetail">Travel Details</param>
        /// <returns></returns>
        public bool UpdateTravelDetails(TravelDetailsModel travelDetail)
        {
            bool success = false;

            using (OCASIAMeetingUOW db = new OCASIAMeetingUOW())
            {
                TravelDetail record = new TravelDetail();

                if (travelDetail.TravelDetailID != 0)
                    record = db.Repository<TravelDetail>().GetAll().Where(el => el.IsActive && el.TravelDetailID == travelDetail.TravelDetailID).Select(el => el).FirstOrDefault();

                if (record == null)
                    return false;

                record.UserDetailID = travelDetail.UserDetailID;
                record.Address = travelDetail.Address;
                record.ArrivalDate = travelDetail.ArrivalDate;
                record.ArrivalAirport = travelDetail.ArrivalAirport;
                record.ArrivalFlightNumber = travelDetail.ArrivalFlightNumber;
                record.ArrivalTime = travelDetail.ArrivalTime;
                record.Comments = travelDetail.Comments;
                record.CreatedBy = travelDetail.CreatedBy;
                record.CreatedOn = DateTime.Now;
                record.DepartureDate = travelDetail.DepartureDate;
                record.DeparturelAirport = travelDetail.DeparturelAirport;
                record.DepartureTime = travelDetail.DepartureTime;
                record.NoCheckInBages = travelDetail.NoCheckInBages;
                record.IsActive = true;
                record.NoCheckInBages = travelDetail.NoCheckInBages;
                record.PhoneNumber = travelDetail.PhoneNumber;
                record.Desc1 = travelDetail.Desc1;
                record.Desc2 = travelDetail.Desc2;
                record.Desc3 = travelDetail.Desc3;

                if (travelDetail.TravelDetailID == 0)
                    db.Repository<TravelDetail>().Add(record);
                else
                    db.Repository<TravelDetail>().Update(record);
                db.SaveChanges();
                success = true;
            }
            return success;
        }

        public TravelDetailsModel GetTravelDetails(int userDetailsID)
        {
            TravelDetailsModel model;
            bool IsreadOnly = CheckSubmitted(userDetailsID);
            using (OCASIAMeetingUOW db = new OCASIAMeetingUOW())
            {
                model = db.Repository<TravelDetail>().GetAll().Where(el => el.IsActive && el.UserDetailID == userDetailsID)
                    .Select(el => new TravelDetailsModel()
                    {
                        ReadOnly = IsreadOnly,
                        UserDetailID = el.UserDetailID,
                        TravelDetailID = el.TravelDetailID,
                        ArrivalAirport = el.ArrivalAirport,
                        ArrivalDate = el.ArrivalDate,
                        ArrivalFlightNumber = el.ArrivalFlightNumber,
                        NoCheckInBages = el.NoCheckInBages,
                        ArrivalTime = el.ArrivalTime,
                        DepartureDate = el.DepartureDate,
                        DeparturelAirport = el.DeparturelAirport,
                        DepartureTime = el.DepartureTime,
                        Comments = el.Comments
                    }).FirstOrDefault();
            }
            return model;
        }

        #endregion

        private bool CheckSubmitted(int userDetailsID)
        {
            using (OCASIAMeetingUOW db = new OCASIAMeetingUOW())
            {
                var v = db.Repository<UserDetail>().GetAll().Where(el => el.IsActive && el.UserDetailID == userDetailsID && el.IsSubmitted == true).FirstOrDefault();
                if (v == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Step 4 : Submitting or cancllation
        /// </summary>
        /// <param name="userDetailsId"></param>
        /// <param name="isCanclled"></param>
        /// <returns></returns>
        public bool UpdateUserConfirmtion(int userDetailsId, bool isSubmitted, bool isCanclled = false)
        {
            bool success = false;

            using (OCASIAMeetingUOW db = new OCASIAMeetingUOW())
            {
                UserDetail response = db.Repository<UserDetail>().GetAll().Where(el => el.IsActive && el.Meetings.AllowRegistration && el.UserDetailID == userDetailsId).Select(el => el).FirstOrDefault();
                if (response == null)
                    return success;

                response.IsCanclled = isCanclled;
                response.IsSubmitted = isSubmitted;

                db.Repository<UserDetail>().Update(response);
                db.SaveChanges();

                success = true;
            }
            return success;
        }





        #region Guest Deatils
        public List<CollectionDetails> GetListOfGuests(int userDetailsID)
        {
            List<CollectionDetails> model = new List<CollectionDetails>();
            using (OCASIAMeetingUOW db = new OCASIAMeetingUOW())
            {
                model = db.Repository<UserDetail>().GetAll().Where(el => el.IsActive && el.IsGust == true && el.GuestOf == userDetailsID.ToString()).Select(el => new CollectionDetails() { ID = el.UserDetailID, Name = el.GivenName }).ToList();
            }
            return model;

        }

        public bool AddGuests()
        {
            throw new NotImplementedException();
        }

        public List<UserDetailsModel> GetGuestDetails(int userDetailsID)
        {
            List<UserDetailsModel> model = new List<UserDetailsModel>();
            using (OCASIAMeetingUOW db = new OCASIAMeetingUOW())
            {
                model = db.Repository<UserDetail>().GetAll().Where(el => el.IsActive && el.IsGust == true && el.GuestOf == userDetailsID.ToString()).Select(el =>
                        new UserDetailsModel()
                        {
                            UserDetailID = el.UserDetailID,
                            GuestOf = el.GuestOf,
                            AddressLine1 = el.AddressLine1,
                            AddressLine2 = el.AddressLine2,
                            AssitantEmail = el.AssitantEmail,
                            City = el.City,
                            Company = el.Company,
                            Desc4 = el.Desc4,
                            DOB = el.DOB,
                            Email = el.Email,
                            ExpiryDate = el.ExpiryDate,
                            FamilyName = el.FamilyName,
                            FirstLastName = el.FirstLastName,
                            Gender = el.Gender,
                            GivenName = el.GivenName,
                            IsGust = el.IsGust == null ? false : el.IsGust.Value,
                            IssueDate = el.IssueDate,
                            Nationality = el.Nationality,
                            Organization = el.Organization,
                            OrganizationDesignation = el.OrganizationDesignation,
                            OrganizationFunction = el.Organization,
                            PassportCoptyPath = el.PassportCoptyPath,
                            PassportNumber = el.PassportNumber,
                            PostalCode = el.PostalCode,
                            StateProvince = el.StateProvince,
                            TelephoneNumber = el.TelephoneNumber,
                            Title = el.Titles.TitleName,
                            UploadedPicturePath = el.UploadedPicturePath,
                            CountryID = el.CountryID == null ? 0 : el.CountryID.Value,
                            CountryOfResidanceID = el.CountryOfResidanceID == null ? 0 : el.CountryOfResidanceID.Value
                        }).ToList();
                model.ForEach(el =>
                 {
                     if (el.CountryID != 0)
                         el.CountryName = db.Repository<Mst_Country>().GetAllReffByID(inner => inner.CountryID == el.CountryID).Select(inner => inner.CountryName).FirstOrDefault();
                     if (el.CountryOfResidanceID != 0)
                         el.CountryOfResidance = db.Repository<Mst_Country>().GetAllReffByID(inner => inner.CountryID == el.CountryOfResidanceID).Select(inner => inner.CountryName).FirstOrDefault();

                 });

            }
            return model;
        }
        #endregion

        #region Meeing related
        public int GetCurrentMeetingID()
        {
            using (OCASIAMeetingUOW db = new OCASIAMeetingUOW())
            {
                var result = db.Repository<Meeting>().GetAll().Where(el => el.IsActive && el.AllowRegistration).Select(el => el).OrderByDescending(el => el.CreatedBy).FirstOrDefault();
                if (result == null)
                    return 0;
                if ((result.ApplicationStartDate != null && result.ApplicationStartDate.HasValue && DateTime.Compare(result.ApplicationStartDate.Value.Date, DateTime.Today.Date) < 0 &&
result.ApplicationEndDate != null && result.ApplicationEndDate.HasValue && DateTime.Compare(result.ApplicationEndDate.Value.Date, DateTime.Today.Date) >= 0)
                )
                    return result.MeetingID;
                else
                    return -1;
            }
        }

        public MeetingPage ValidateMeetingUrl(int meetingID = 0)
        {
            using (meetingContext = new OCASIAMeetingContext())
            {
                var result = meetingContext.Meetings.Where(el => el.MeetingID == meetingID && el.IsActive).FirstOrDefault();
                if (result == null)
                    return MeetingPage.DoesNotExists;
                else if (result.AllowRegistration == false)
                {
                    MeetingName = result.MeetingName;
                    return MeetingPage.Experired;
                }
                else
                if (
                    result.ApplicationStartDate != null && result.ApplicationStartDate.HasValue && DateTime.Compare(result.ApplicationStartDate.Value.Date, DateTime.Today.Date) < 0 &&
result.ApplicationEndDate != null && result.ApplicationEndDate.HasValue && DateTime.Compare(result.ApplicationEndDate.Value.Date, DateTime.Today.Date) >= 0)
                {
                    MeetingName = result.MeetingName;
                    Tabs = result.RegistrationTabs;


                    return MeetingPage.Active;
                }
                else
                    return MeetingPage.Experired;

            }
        }

        public bool AccessKeyValidation(string accesskey, int meetingID)
        {
            using (meetingContext = new OCASIAMeetingContext())
            {
                var result = meetingContext.Invitations.Where(el => el.IsActive && el.IsPublish && el.InvitationAccessKeyName == accesskey && el.MeetingID == meetingID).FirstOrDefault();
                if (result == null)
                    return false;
                else
                    return true;



            }
        }

        public TabDetailModel GetMeetingInfoTabDetils(int meetingID, int tabIDs = 1)
        {
            using (meetingContext = new OCASIAMeetingContext())
            {
                TabDetailModel model = meetingContext.RegistrationTabDetails.Where(el => el.IsActive && el.MeetingID == meetingID && el.RegistrationTabID == tabIDs).
                    Select(el => new TabDetailModel()
                    {
                        RegistrationTabID = el.RegistrationTabID,
                        MeetingID = el.MeetingID,
                        BasicDescription = el.BasicDescription,
                        Description1 = el.Description1,
                        Description1PicturePath = el.Description1PicturePath,
                        Description2 = el.Description2,
                        Description2PicturePath = el.Description2PicturePath,
                        Description3 = el.Description3,
                        Description3PicturePath = el.Description3PicturePath,
                        Description4 = el.Description4,
                        Description4PicturePath = el.Description4PicturePath
                    ,
                        FileName1 = el.FileName1,
                        FileName1Path = el.FileName1Path,
                        FileName2 = el.FileName2,
                        FileName2Path = el.FileName2Path,
                        FileName3 = el.FileName3,
                        FileName3Path = el.FileName3Path,
                        FileName4 = el.FileName4,
                        FileName4Path = el.FileName4Path
                    }).FirstOrDefault();
                if (model == null)
                    return new TabDetailModel();
#if DEBUG
                model.Description1PicturePath = model.Description2PicturePath = model.Description3PicturePath = model.Description4PicturePath = "../Content/banners.jpg";
                model.FileName1Path = model.FileName2Path = model.FileName3Path = model.FileName4Path = "../Content/banners.jpg";



#else
#endif
                return model;
            }
        }


        #endregion

        #region Preview Details
        public PreviewDetails GetPreviewDetails(int userDetailsID)
        {
            PreviewDetails model = new PreviewDetails();
            using (OCASIAMeetingUOW db = new OCASIAMeetingUOW())
            {
                model.Guests = db.Repository<UserDetail>().GetAll().Where(el => (el.UserDetailID == userDetailsID || el.GuestOf == userDetailsID.ToString()) && el.IsActive).
                    Select(el => new UserDetailsModel()
                    {
                        UserDetailID = el.UserDetailID,
                        GuestOf = el.GuestOf,
                        AddressLine1 = el.AddressLine1,
                        AddressLine2 = el.AddressLine2,
                        AssitantEmail = el.AssitantEmail,
                        City = el.City,
                        Company = el.Company,
                        Desc4 = el.Desc4,
                        DOB = el.DOB,
                        Email = el.Email,
                        ExpiryDate = el.ExpiryDate,
                        FamilyName = el.FamilyName,
                        FirstLastName = el.FirstLastName,
                        Gender = el.Gender,
                        GivenName = el.GivenName,
                        IsGust = el.IsGust == null ? false : el.IsGust.Value,
                        IssueDate = el.IssueDate,
                        Nationality = el.Nationality,
                        Organization = el.Organization,
                        OrganizationDesignation = el.OrganizationDesignation,
                        OrganizationFunction = el.Organization,
                        PassportCoptyPath = el.PassportCoptyPath,
                        PassportNumber = el.PassportNumber,
                        PostalCode = el.PostalCode,
                        StateProvince = el.StateProvince,
                        TelephoneNumber = el.TelephoneNumber,
                        Title = el.Titles.TitleName,
                        UploadedPicturePath = el.UploadedPicturePath,
                        CountryID = el.CountryID == null ? 0 : el.CountryID.Value,
                        CountryOfResidanceID = el.CountryOfResidanceID == null ? 0 : el.CountryOfResidanceID.Value
                    }).ToList();
                if (model.Guests == null && model.Guests.Count == 0)
                    return null;
                if (model.Guests.Where(el => el.IsGust != true && !string.IsNullOrEmpty(el.PassportNumber)).Count() > 0)
                    model.IsPersonalFilled = true;

                if (model.Guests.Where(el => el.IsGust == true && !string.IsNullOrEmpty(el.PassportNumber)).Count() > 0)
                    model.IsGuestsFilled = true;
                model.Travel = db.Repository<TravelDetail>().GetAll().Where(el => el.UserDetailID == userDetailsID && el.IsActive).Select(el =>
                      new TravelDetailsModel()
                      {
                          TravelDetailID = el.TravelDetailID,
                          NoCheckInBages = el.NoCheckInBages,
                          PhoneNumber = el.PhoneNumber,
                          Address = el.Address,
                          ArrivalAirport = el.ArrivalAirport,
                          ArrivalTime = el.ArrivalTime,
                          Comments = el.Comments,
                          DepartureDate = el.DepartureDate,
                          DeparturelAirport = el.DeparturelAirport,
                          DepartureTime = el.DepartureTime,
                          ArrivalDate = el.ArrivalDate,
                          ArrivalFlightNumber = el.ArrivalFlightNumber,
                          ReadOnly = el.UserDetails.IsSubmitted == null ? false : el.UserDetails.IsSubmitted.Value
                      }).FirstOrDefault();
                if (model.Travel != null)
                    model.IsTravelFilled = true;
                model.ShowSubmitButton = model.Travel?.ReadOnly == null ? true : false;

                model.Guests.ForEach(el =>
                {
                    el.ReadOnly = model.Travel?.ReadOnly == null ? false : true;
                    if (el.CountryID != 0)
                        el.CountryName = db.Repository<Mst_Country>().GetAllReffByID(inner => inner.CountryID == el.CountryID).Select(inner => inner.CountryName).FirstOrDefault();
                    if (el.CountryOfResidanceID != 0)
                        el.CountryOfResidance = db.Repository<Mst_Country>().GetAllReffByID(inner => inner.CountryID == el.CountryOfResidanceID).Select(inner => inner.CountryName).FirstOrDefault();

#if DEBUG
                    el.UploadedPicturePath = "../Content/banners.jpg";
                    el.Desc4 = "../Content/banners.jpg";
                    el.PassportCoptyPath = "../Content/banners.jpg";
#else
                    el.UploadedPicturePath = model.UploadedPicturePath?.Replace("~", CommonOperations.FilePath);
                    el.PassportCoptyPath = model.PassportCoptyPath?.Replace("~", CommonOperations.FilePath);
                    el.PassportCoptyPath = model.PassportCoptyPath?.Replace("~", CommonOperations.FilePath);
#endif

                });
            }
            return model;
        }
        #endregion

    }
}

