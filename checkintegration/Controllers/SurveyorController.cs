using System;
using System.Data.Entity.Validation;
using System.Web.Security;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using System.Net.Mail;
using checkintegration.Models;
using System.IO;

namespace checkintegration.Controllers
{
    public class SurveyorController : Controller
    {
        //
        // GET: /Surveyor/

     /*   public ActionResult Surveyor_Index()
        {
            return View();
        }*/
        [HttpGet]
        public ActionResult Surveyor_Index()
        {
            Status_surveyor ss = new Status_surveyor();
            InsureEntities ii = new InsureEntities();
            int id=(int)Session["CustId"];
            
           var aa = ii.Status_surveyor.Where(m => m.SurvId == id).FirstOrDefault();
            ss.Status=aa.Status;
           if (aa != null)
           {
               return View(ss);
           }

            
            return View();
        }
        [HttpPost]
        public ActionResult SurveyorRes(string Option)
        {
            // Let's get all states that we need for a DropDownList

            Status_surveyor ss = new Status_surveyor();
            int aa = (int)Session["CustId"];
            ss.SurvId = aa;
            int s;
            string Text="";
            if (Option == "true")
            {
                s = 1;
                Text="You Are Available";
            }
            else
            {
                s = 0;
                Text="You Are Not Available";
            }
            ss.Status = s;
            
            InsureEntities ii = new InsureEntities();
            var a=ii.Status_surveyor.Where(m => m.SurvId == aa).FirstOrDefault();
            ss.ID = a.ID;
            if (a != null)
            {
                ii.Entry(a).State = EntityState.Detached;
                ii.Entry(ss).State = EntityState.Modified;
                ii.SaveChanges();
            }
            return Json("" + Text,JsonRequestBehavior.AllowGet);
        }

       /* public ActionResult AcciReg()
        {
            InsureEntities db = new InsureEntities();
            var jsondata = (from ii in db.AccidentInfoes join reg in db.Registrations on ii.SurveyorId equals reg.UserID select new { ii.Latitude, ii.Longitude, ii.Description, ii.CustId, ii.AccidentId, reg.FirstName });
            return Json(new { data = jsondata }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult UploadReport(int id = 0)
        {
            InsureEntities db = new InsureEntities();
            if (id == 0)
            {

                return View();
            }
            else
            {
                var data = db.AccidentInfoes.Where(m => m.AccidentId == id).FirstOrDefault();
                return View(data);
            }

        }
        [HttpPost]
        public ActionResult UploadReport(FormCollection fc, HttpPostedFileBase file)
        {
            InsureEntities db = new InsureEntities();
            ReportMst rm = new ReportMst();
            var id = Convert.ToInt32(fc[0]);
            rm.AccidentId = id;
            rm.ReportType = fc[1];
            var sid = db.AccidentInfoes.Where(m => m.AccidentId == id).Select(m => m.SurveyorId).FirstOrDefault();
            var name = db.Registrations.Where(m => m.UserID == sid).Select(m => m.EmailId).FirstOrDefault();


            rm.SurveyorId = sid;
            string h = "";
            // E:\project_shown\checkintegration\checkintegration\Content\FilePath\
            try
            {
                if (file != null)
                {

                    string path = Path.Combine(Server.MapPath("~/Content/FilePath/"), Path.GetFileName(file.FileName));
                    string n = Path.GetFileName(file.FileName);
                    file.SaveAs(path);
                    rm.FilePath = "~/Content/FilePath/" + n;
                    db.ReportMsts.Add(rm);
                    db.SaveChanges();
                    return Json(new { data = "fdsdsass" + h }, JsonRequestBehavior.AllowGet);
                }


                return Json(new { data = "Faillll" + h }, JsonRequestBehavior.AllowGet);
            }
            catch (DbEntityValidationException e)
            {
                string ms = "";
                foreach (var eve in e.EntityValidationErrors)
                {
                    ms += "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:" +
                        eve.Entry.Entity.GetType().Name + eve.Entry.State;
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ms += "- Property: \"{0}\", Error: \"{1}\"" +
                            ve.PropertyName + ve.ErrorMessage;
                    }
                }
                return Json(new { success = false, message = ms }, JsonRequestBehavior.AllowGet);

            }
        }*/
      
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

    }
}
