﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCCapstoneBGS.Controllers
{
    [HandleError]
    public class EntitiesController : Controller
    {
        IDataProvider _IDataProvider;
        public EntitiesController()
        {
            _IDataProvider = new DataProvider();
        }


        string Layout_ADashboard= "~/TerraTech/TerraShared/AdministratorDashboard.cshtml";

        string Layout_CU = "~/TerraTech/TerraShared/CommunityUser.cshtml";
        string Layout_CUDashboard = "~/TerraTech/TerraShared/CommunityUser.cshtml";

        public ActionResult Administrator(int UpdatedStatusID=0)
        {
            ViewBag.Title = LabelStruct.Administrator.AdministratorHomepage;
            ViewBag.VBLayout = Layout_ADashboard;
            var LandNumber = _IDataProvider.GetHomeDashboard(DateTime.Now.Year, 1);
            var WaterNumber = _IDataProvider.GetHomeDashboard(DateTime.Now.Year, 2);
            var Users = _IDataProvider.GetDashboard();
            var Progress = _IDataProvider.GetHomeDashboardProgress(DateTime.Now.Year);



            string tempEnvironmentalConcernCount = string.Empty;
            string tempEnvironmentalConcern = string.Empty;          

            _IDataProvider.CHART_Display(StoredProcedureEnum.CHART_EnvironmentalConcern.ToString(),DateTime.Now.Year,0,out tempEnvironmentalConcernCount, out tempEnvironmentalConcern);

            ViewBag.ECCount = tempEnvironmentalConcernCount.Trim();
            ViewBag.ECName = tempEnvironmentalConcern.Trim();

            string tempStatusCount = string.Empty;
            string tempStatus = string.Empty;

            _IDataProvider.CHART_Display(StoredProcedureEnum.CHART_OverallStatusUpdate.ToString(), DateTime.Now.Year, 0, out tempStatusCount, out tempStatus);

            ViewBag.SCount = tempStatusCount.Trim();
            ViewBag.SStatus = tempStatus.Trim();

            ViewBag.DATETIMENOW = DateTime.Now.Date.ToLongDateString() + " - " + DateTime.Now.TimeOfDay;

            ViewBag.VBLand = LandNumber;
            ViewBag.VBWater = WaterNumber;
            ViewBag.VBUsers = Users;
            ViewBag.VBProgress = Progress;

            //var ggka = _IDataProvider.GetCaseReport(5).ToList();

            //string x = string.Empty;
            //foreach (var m in ggka)
            //{
            //    x += "[" + m.XCoordinates + ' ' + m.YCoordinates + "]";

            //}

            //ViewBag.DUMMY = x;

            //var commaSeparated = string.Join(",", _IDataProvider.GetCaseReport(5).Select(mmm => "["+ mmm.XCoordinates+","+mmm.YCoordinates+"]"));
            //ViewBag.DUMMY2 = commaSeparated;


            const string quote = "\"";
            //var commaSeparated = string.Join(",", _IDataProvider.GetCaseReport(5).Select(mmm => "[" + mmm.XCoordinates + "," + mmm.YCoordinates + "]"));

            var commaSeparated = string.Join(",", _IDataProvider.GetCaseReport(UpdatedStatusID).
                Select(
                mmm => "["
                + quote
                //   + "<center><img src='https://i.ytimg.com/vi/EXtNpsj1-0w/hqdefault.jpg' style='width:150px; height:100px;'></center>"
                + "Case No: " + mmm.CaseReportID
                + "<br />Reported on: " + mmm.DateReported
                + "<br />Updated on: " + mmm.UpdatedStatusDate
                + "<br />Type: " + mmm.Concern
                + "<br />City: " + mmm.CaseLocation + " [" + mmm.XCoordinates + "," + mmm.YCoordinates + "]"
                + quote
                + "," + mmm.XCoordinates + "," + mmm.YCoordinates + "]"
                ));
            ViewBag.DUMMY2 = commaSeparated;

            return View();
        }
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            if (((int?)Session["UserInformationID"]) != null)
            {
                try
                {
                    int userId = (int)Session["UserInformationID"];
                    Session.Abandon();
                }
                catch
                {
                    throw;
                }
            }
            else
            {
                //DO NOTHING
            }
            return RedirectToAction("Login", "Entities");
        }

        #region CombinedFunctionalities

        int SESSION_UserInformationID;
       
        public ActionResult Authorise(UserInformation user)
        {
            var userDetail = _IDataProvider.GetUserInformation().Where(x => x.Username == user.Username && x.Password == user.Password).FirstOrDefault();

            if (userDetail == null)
            {
                return View("Login");
            }
            else
            {
                var UserInformationID = userDetail.UserInformationID;
                var UserTypeID = userDetail.UserTypeID;
                var Password = userDetail.Password;
                var Username = userDetail.Username;
                SESSION_UserInformationID = userDetail.UserInformationID;
                Session["UserInformationID"] = UserInformationID;
                Session["Username"] = Username;
                Session["UserTypeID"] = UserTypeID;
                Session["Password"] = Password;

                if (UserTypeID == 1)
                {
                    return RedirectToAction("Administrator", "Entities");
                }

                else if (UserTypeID == 2)
                {

                    return RedirectToAction("CommunityUser", "Entities");
                }

                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }


        }

        public ActionResult Register(UserInformation user)
        {
            _IDataProvider.InsertUserInformation(2,user.Username,user.Password,user.Email,user.GivenName,user.MaidenName,user.FamilyName);
            return View("Login");
        }

        public ActionResult Leaderboard()
        {
            ViewBag.DATETIMENOW = DateTime.Now.Date.ToLongDateString() + " - " + DateTime.Now.TimeOfDay;
            ViewBag.Title = LabelStruct.Others.Leaderboards;
            ViewBag.VBLayout = Layout_ADashboard;
            return View();
        }

        #endregion

        #region Administrator
        public ActionResult Accounts()
        {
            ViewBag.SESSIONID = SESSION_UserInformationID;
            ViewBag.Title = LabelStruct.Administrator.Volunteers;
            ViewBag.VBLayout = Layout_ADashboard;
            ViewBag.DATETIMENOW = DateTime.Now.Date.ToLongDateString() + " - " + DateTime.Now.TimeOfDay;

       
            return View();
        }

        public ActionResult MonthlyReports(int Year = 0, int Month = 0)
        {
            ViewBag.Title = LabelStruct.Administrator.MonthlyReports;
            ViewBag.VBLayout = Layout_ADashboard;

            string tempEnvironmentalConcernCount = string.Empty;
            string tempEnvironmentalConcern = string.Empty;

            _IDataProvider.CHART_Display(StoredProcedureEnum.CHART_EnvironmentalConcern_MonthYear.ToString(), Year, Month, out tempEnvironmentalConcernCount, out tempEnvironmentalConcern);

            ViewBag.ECCount = tempEnvironmentalConcernCount.Trim();
            ViewBag.ECName = tempEnvironmentalConcern.Trim();

            string tempStatusCount = string.Empty;
            string tempStatus = string.Empty;

            _IDataProvider.CHART_Display(StoredProcedureEnum.CHART_OverallStatusUpdate_MonthYear.ToString(), Year, Month, out tempStatusCount, out tempStatus);

            ViewBag.SCount = tempStatusCount.Trim();
            ViewBag.SStatus = tempStatus.Trim();

            ViewBag.DATETIMENOW = DateTime.Now.Date.ToLongDateString() + " - " + DateTime.Now.TimeOfDay;
            ViewBag.CURRENT_YEAR = Year;
            ViewBag.CURRENT_MONTH = Month;

            return View();
        }


        public ActionResult YearlyReports(int Year=0, int Month =0)
        {
            ViewBag.Title = LabelStruct.Administrator.YearlyReports;
            ViewBag.VBLayout = Layout_ADashboard;

            string tempEnvironmentalConcernCount = string.Empty;
            string tempEnvironmentalConcern = string.Empty;

            _IDataProvider.CHART_Display(StoredProcedureEnum.CHART_EnvironmentalConcern.ToString(), Year, Month, out tempEnvironmentalConcernCount, out tempEnvironmentalConcern);

            ViewBag.ECCount = tempEnvironmentalConcernCount.Trim();
            ViewBag.ECName = tempEnvironmentalConcern.Trim();

            string tempStatusCount = string.Empty;
            string tempStatus = string.Empty;

            _IDataProvider.CHART_Display(StoredProcedureEnum.CHART_OverallStatusUpdate.ToString(), Year, Month, out tempStatusCount, out tempStatus);

            ViewBag.SCount = tempStatusCount.Trim();
            ViewBag.SStatus = tempStatus.Trim();

            ViewBag.DATETIMENOW = DateTime.Now.Date.ToLongDateString() + " - " + DateTime.Now.TimeOfDay;
            ViewBag.CURRENT_YEAR = Year;
            return View();
        }


   

        public ActionResult Twitter()
        {
            ViewBag.Title = LabelStruct.Administrator.Twitter;
            ViewBag.VBLayout = Layout_ADashboard;
            ViewBag.DATETIMENOW = DateTime.Now.Date.ToLongDateString() + " - " + DateTime.Now.TimeOfDay;

            var LandNumber = _IDataProvider.GetHomeDashboard(DateTime.Now.Year, 1);
            var WaterNumber = _IDataProvider.GetHomeDashboard(DateTime.Now.Year, 2);
            var Users = _IDataProvider.GetDashboard();
            var Progress = _IDataProvider.GetHomeDashboardProgress(DateTime.Now.Year);


            String areaReport="";
            foreach (var dataArea in _IDataProvider.GetAreaDetailsPerMonthYear(DateTime.Now.Month, DateTime.Now.Year))
            {
                areaReport += dataArea.CaseLocation + " = Land: " + dataArea.L_Completed + " Water: " + dataArea.W_Completed + "\n";
            }

            ViewBag.AreaTweet = "Completed Reports per Area: \n"+areaReport;
            
            
            string sentence = string.Empty;

            if (LandNumber > 2)
            {
                sentence = "are";
            }

            ViewBag.UpdateStatus =
                "As of " + DateTime.Now.ToString() + "\nStatus Report:\n" +
                "Completed Land Concern: " + LandNumber + "\n" +
                "Completed Water Concern: " + WaterNumber + "\n" +
                "Pending Cases: " + Progress + "\n" +
                "all of which are taken from the submitted data of " + Users + " users";
            return View();
        }

        public ActionResult Submitted()
        {
            ViewBag.Title = LabelStruct.Administrator.Submitted;
            ViewBag.VBLayout = Layout_ADashboard;
            ViewBag.DATETIMENOW = DateTime.Now.Date.ToLongDateString() + " - " + DateTime.Now.TimeOfDay;

            string tempEnvironmentalConcernCount = string.Empty;
            string tempEnvironmentalConcern = string.Empty;

            _IDataProvider.CHART_Display(StoredProcedureEnum.CHART_Status.ToString(), DateTime.Now.Year, 1, out tempEnvironmentalConcernCount, out tempEnvironmentalConcern);

            ViewBag.ECCount = tempEnvironmentalConcernCount.Trim();
            ViewBag.ECName = tempEnvironmentalConcern.Trim();


            string tempStatusCount = string.Empty;
            string tempStatus = string.Empty;

            _IDataProvider.CHART_Display(StoredProcedureEnum.CHART_Status_LW.ToString(), DateTime.Now.Year, 1, out tempStatusCount, out tempStatus);

            ViewBag.SCount = tempStatusCount.Trim();
            ViewBag.SStatus = tempStatus.Trim();


            return View();
        }

        public ActionResult Accepted()
        {
            ViewBag.Title = LabelStruct.Administrator.Accepted;
            ViewBag.VBLayout = Layout_ADashboard;
            ViewBag.DATETIMENOW = DateTime.Now.Date.ToLongDateString() + " - " + DateTime.Now.TimeOfDay;


            string tempEnvironmentalConcernCount = string.Empty;
            string tempEnvironmentalConcern = string.Empty;

            _IDataProvider.CHART_Display(StoredProcedureEnum.CHART_Status.ToString(), DateTime.Now.Year, 3, out tempEnvironmentalConcernCount, out tempEnvironmentalConcern);

            ViewBag.ECCount = tempEnvironmentalConcernCount.Trim();
            ViewBag.ECName = tempEnvironmentalConcern.Trim();


            string tempStatusCount = string.Empty;
            string tempStatus = string.Empty;

            _IDataProvider.CHART_Display(StoredProcedureEnum.CHART_Status_LW.ToString(), DateTime.Now.Year, 3, out tempStatusCount, out tempStatus);

            ViewBag.SCount = tempStatusCount.Trim();
            ViewBag.SStatus = tempStatus.Trim();



            return View();
        }

        public ActionResult Rejected()
        {
            ViewBag.Title = LabelStruct.Administrator.Rejected;
            ViewBag.VBLayout = Layout_ADashboard;
            ViewBag.DATETIMENOW = DateTime.Now.Date.ToLongDateString() + " - " + DateTime.Now.TimeOfDay;

            string tempEnvironmentalConcernCount = string.Empty;
            string tempEnvironmentalConcern = string.Empty;

            _IDataProvider.CHART_Display(StoredProcedureEnum.CHART_Status.ToString(), DateTime.Now.Year, 2, out tempEnvironmentalConcernCount, out tempEnvironmentalConcern);

            ViewBag.ECCount = tempEnvironmentalConcernCount.Trim();
            ViewBag.ECName = tempEnvironmentalConcern.Trim();


            string tempStatusCount = string.Empty;
            string tempStatus = string.Empty;

            _IDataProvider.CHART_Display(StoredProcedureEnum.CHART_Status_LW.ToString(), DateTime.Now.Year, 2, out tempStatusCount, out tempStatus);

            ViewBag.SCount = tempStatusCount.Trim();
            ViewBag.SStatus = tempStatus.Trim();

            return View();
        }

        public ActionResult InProgress()
        {
            ViewBag.Title = LabelStruct.Administrator.InProgress;
            ViewBag.VBLayout = Layout_ADashboard;
            ViewBag.DATETIMENOW = DateTime.Now.Date.ToLongDateString() + " - " + DateTime.Now.TimeOfDay;

            string tempEnvironmentalConcernCount = string.Empty;
            string tempEnvironmentalConcern = string.Empty;

            _IDataProvider.CHART_Display(StoredProcedureEnum.CHART_Status.ToString(), DateTime.Now.Year, 4, out tempEnvironmentalConcernCount, out tempEnvironmentalConcern);

            ViewBag.ECCount = tempEnvironmentalConcernCount.Trim();
            ViewBag.ECName = tempEnvironmentalConcern.Trim();


            string tempStatusCount = string.Empty;
            string tempStatus = string.Empty;

            _IDataProvider.CHART_Display(StoredProcedureEnum.CHART_Status_LW.ToString(), DateTime.Now.Year, 4, out tempStatusCount, out tempStatus);

            ViewBag.SCount = tempStatusCount.Trim();
            ViewBag.SStatus = tempStatus.Trim();

            return View();
        }

        public ActionResult Completed()
        {
            ViewBag.Title = LabelStruct.Administrator.Completed;
            ViewBag.VBLayout = Layout_ADashboard;
            ViewBag.DATETIMENOW = DateTime.Now.Date.ToLongDateString() + " - " + DateTime.Now.TimeOfDay;

            string tempEnvironmentalConcernCount = string.Empty;
            string tempEnvironmentalConcern = string.Empty;

            _IDataProvider.CHART_Display(StoredProcedureEnum.CHART_Status.ToString(), DateTime.Now.Year, 5, out tempEnvironmentalConcernCount, out tempEnvironmentalConcern);

            ViewBag.ECCount = tempEnvironmentalConcernCount.Trim();
            ViewBag.ECName = tempEnvironmentalConcern.Trim();

            string tempStatusCount = string.Empty;
            string tempStatus = string.Empty;

            _IDataProvider.CHART_Display(StoredProcedureEnum.CHART_Status_LW.ToString(), DateTime.Now.Year, 5, out tempStatusCount, out tempStatus);

            ViewBag.SCount = tempStatusCount.Trim();
            ViewBag.SStatus = tempStatus.Trim();

            return View();
        }
        #endregion

        #region CommunityUser

        public ActionResult CommunityUser(int UpdatedStatusID=0)
        {
            ViewBag.VBLayout = Layout_CUDashboard;
            //ViewBag.[Pangalan na gusto mo] = Value na gusto mo;
            //    var PASS_SESSION = Session["Username"];
            int PASS_UserInformationID = Convert.ToInt32(Session["UserInformationID"]);

            ViewBag.Title = LabelStruct.CommunityUser.CommunityUserHomepage;
            ViewBag.DATETIMENOW = DateTime.Now.Date.ToLongDateString() + " - " + DateTime.Now.TimeOfDay;

            var UserDataBoard = _IDataProvider.GetLeaderboards_Year(5, DateTime.Now.Year).Where(x => x.UserInformationID == PASS_UserInformationID);

            ViewBag.OverallScore = Convert.ToInt32(UserDataBoard.Select(p => p.Points).FirstOrDefault());
            ViewBag.LandScore = Convert.ToInt32(UserDataBoard.Select(p => p.LandPoints).FirstOrDefault());
            ViewBag.WaterScore = Convert.ToInt32(UserDataBoard.Select(p => p.WaterPoints).FirstOrDefault());
            ViewBag.AreaScore = Convert.ToInt32(UserDataBoard.Select(p => p.Areas).FirstOrDefault());

            //var LandNumber = _IDataProvider.GetHomeDashboard(DateTime.Now.Year, 1);
            //var WaterNumber = _IDataProvider.GetHomeDashboard(DateTime.Now.Year, 2);
            //var Users = _IDataProvider.GetDashboard();
            //var Progress = _IDataProvider.GetHomeDashboardProgress(DateTime.Now.Year);

            //ViewBag.VBLand = LandNumber;
            //ViewBag.VBWater = WaterNumber;
            //ViewBag.VBUsers = Users;
            //ViewBag.VBProgress = Progress;


            const string quote = "\"";
            //var commaSeparated = string.Join(",", _IDataProvider.GetCaseReport(5).Select(mmm => "[" + mmm.XCoordinates + "," + mmm.YCoordinates + "]"));

            var commaSeparated = string.Join(",", _IDataProvider.GetCaseReport(UpdatedStatusID)
                .
                Where(x=>x.UserInformationID == PASS_UserInformationID)
                .
                Select(
                mmm => "["
                +quote
             //   + "<center><img src='https://i.ytimg.com/vi/EXtNpsj1-0w/hqdefault.jpg' style='width:150px; height:100px;'></center>"
                +"Case No: "+mmm.CaseReportID
                +"<br />Reported on: "+mmm.DateReported
                +"<br />Updated on: "+mmm.UpdatedStatusDate
                +"<br />Type: "+mmm.Concern
                +"<br />City: "+mmm.CaseLocation+" ["+mmm.XCoordinates+","+mmm.YCoordinates+"]"
                +quote
                +","+mmm.XCoordinates+","+mmm.YCoordinates+"]"
                ));
            ViewBag.DUMMY2 = commaSeparated;




         
            return View();
        }

        public ActionResult SubmitReport()
        {
            ViewBag.VBLayout = Layout_CU;
            ViewBag.DATETIMENOW = DateTime.Now.Date.ToLongDateString() + " - " + DateTime.Now.TimeOfDay;

            ViewBag.Title = LabelStruct.CommunityUser.SubmitReport;
            return View();
        }

        [HttpPost]
        public ActionResult SubmitReport(CaseReport UI, HttpPostedFileBase image1)
        {
            _IDataProvider.InsertCaseReport(UI, image1);
            ViewBag.VBLayout = Layout_CU;
            ViewBag.DATETIMENOW = DateTime.Now.Date.ToLongDateString() + " - " + DateTime.Now.TimeOfDay;
            ViewBag.Title = LabelStruct.CommunityUser.SubmitReport;
            return View();
        }


        public ActionResult ViewStatus()
        {
            ViewBag.VBLayout = Layout_CU;
            ViewBag.DATETIMENOW = DateTime.Now.Date.ToLongDateString() + " - " + DateTime.Now.TimeOfDay;

            ViewBag.Title = LabelStruct.CommunityUser.ViewStatus;
            return View();
        }
        public ActionResult Achievements()
        {
            ViewBag.VBLayout = Layout_CU;
            ViewBag.DATETIMENOW = DateTime.Now.Date.ToLongDateString() + " - " + DateTime.Now.TimeOfDay;
            
            _IDataProvider.GetLeaderboards_Year(5, DateTime.Now.Year);
            ViewBag.Title = LabelStruct.CommunityUser.Achievements;
            return View();
        }
        #endregion

        #region OtherFunctionalities
        public ActionResult ForgotPassword()
        {

            return View();
        }

        public ActionResult DummyPage1()
        {
            return View();
        }

        public ActionResult DummyPage2()
        {
            return View();
        }

        public ActionResult DummyPage3()
        {
            return View();
        }
        #endregion

    }
}