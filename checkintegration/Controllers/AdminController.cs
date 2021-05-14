using System;
using System.Data.Entity.Validation;
using System.Web.Security;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using checkintegration.Models;
using System.Net.Mail;
using System.Data.Entity;
using System.IO;

namespace checkintegration.Controllers
{
    public class AdminController : Controller
    {



        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult Dashboard2()
        {
            return View();
        }
         [HttpGet]
        public ActionResult AddServeyor(int id=0)
        {
            InsureEntities db = new InsureEntities();
            var jsondata = db.Registrations.Where(q => q.UserID == id).FirstOrDefault();
            if (jsondata == null)
            {
                jsondata = new Registration();
            }
            return View(jsondata);
        }
        [HttpPost]
        public ActionResult AddServeyor(Models.Registration user)
        {
            try
            {
                
                if (ModelState.IsValid)
                {
                    user.RoleId = 3;
                    Status_surveyor ss = new Status_surveyor();
                    using (InsureEntities db = new InsureEntities())
                    {
                        if(user.UserID ==0)
                        {
                            var username = db.Registrations.Where(u => u.UserName == user.UserName).FirstOrDefault();
                            var em = db.Registrations.Where(u => u.EmailId == user.EmailId).FirstOrDefault();
                            if (username != null)
                            {

                                ViewBag.Error = "UserName already Exist";
                                return View("AddServeyor", user);
                            }

                            else if (em != null)
                            {
                                ViewBag.Error = "EmailId already Exist";
                                return View("AddServeyor", user);
                            }
                            else
                            {
                                db.Registrations.Add(user);
                                db.SaveChanges();
                                ss.SurvId = user.UserID;
                                ss.Status = 1;
                                db.Status_surveyor.Add(ss);
                                db.SaveChanges();
                                ModelState.Clear();
                                ViewBag.Error = "Sucessfully Submitted";
                                user = null;
                                return RedirectToAction("Dashboard", "Admin");
                            }
                        }
                        else
                        {
                            var username = db.Registrations.Where(u => u.UserName == user.UserName && u.UserID != user.UserID).FirstOrDefault();
                            var em = db.Registrations.Where(u => u.EmailId == user.EmailId && u.UserID != user.UserID).FirstOrDefault();
                            if (username != null)
                            {

                                ViewBag.Error = "UserName already Exist";
                                return View("AddServeyor", user);
                            }

                            else if (em != null)
                            {
                                ViewBag.Error = "EmailId already Exist";
                                return View("AddServeyor", user);
                            }
                            else
                            {
                                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                                ModelState.Clear();
                                ViewBag.Error = "Sucessfully Submitted";
                                user = null;
                                return RedirectToAction("Dashboard", "Admin");
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Data is not correct");
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            return View();
        }

        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult ViewCust()
        {
            return View();
        }
        public JsonResult custo()
        {
            InsureEntities db = new InsureEntities();

            var json = db.Registrations.Where(m => m.RoleId == 2).ToList();
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AddorEditCustomer(int id = 0)
        {
            InsureEntities db = new InsureEntities();
            if (id == 0)
            {
                return View(new Registration());
            }
            else
            {
                var data = db.Registrations.Where(m => m.UserID == id).FirstOrDefault();
                return View(data);
            }
        }
        [HttpPost]
        public ActionResult AddorEditCustomer(Registration rm)
        {
            try
            {
                InsureEntities db = new InsureEntities();
                var data = db.Registrations.Where(m => m.UserID == rm.UserID).FirstOrDefault();
                if (data != null)
                {
                    rm.UserName = data.UserName;
                    rm.ConfirmPassword = data.ConfirmPassword;
                    rm.Gender = data.Gender;
                    rm.NewConfirmPassword = data.NewConfirmPassword;
                    rm.Password = data.Password;
                    rm.Pincode = data.Pincode;
                    rm.RoleId = data.RoleId;
                    rm.Address = data.Address;
                    db.Entry(data).State = EntityState.Detached;
                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Updation Error" }, JsonRequestBehavior.AllowGet);
                }
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
        }

        public ActionResult DeleteCustomer(int id)
        {
            //if( Session["UserId"] == null)
            //{
            //    return RedirectToAction("Login","Home");
            //}
            InsureEntities db = new InsureEntities();
            var jsondata = db.Registrations.Where(q => q.UserID == id).FirstOrDefault();
            if (jsondata != null)
            {
                db.Registrations.Remove(jsondata);
                db.SaveChanges();
            }
            return View(jsondata);
        }

        public ActionResult View_Accident()
        {
            return View();
        }

        public ActionResult AcciReg()
        {
            InsureEntities db = new InsureEntities();
           var jsondata=(from ii in db.AccidentInfoes join reg in db.Registrations on ii.SurveyorId equals reg.UserID select new {ii.Latitude,ii.Longitude,ii.Description,ii.CustId,ii.AccidentId,reg.FirstName});
            return Json(new { data = jsondata }, JsonRequestBehavior.AllowGet);

        }
        
      /*  public ActionResult AssignSurveyor(int id=0)
        {
            InsureEntities db = new InsureEntities();
            if (id == 0)
            {
                return View(new AccidentInfo());
            }
            else
            {
                var data = db.AccidentInfoes.Where(m => m.AccidentId == id).FirstOrDefault();
                return View(data);
            }
        }

        public ActionResult AssignSurveyor(AccidentInfo ai)
        {
            try
            {
                InsureEntities db = new InsureEntities();
                var data = db.Registrations.Where(m => m.UserID == rm.UserID).FirstOrDefault();
                if (data != null)
                {
                    rm.UserName = data.UserName;
                    rm.ConfirmPassword = data.ConfirmPassword;
                    rm.Gender = data.Gender;
                    rm.NewConfirmPassword = data.NewConfirmPassword;
                    rm.Password = data.Password;
                    rm.Pincode = data.Pincode;
                    rm.RoleId = data.RoleId;
                    rm.Address = data.Address;
                    db.Entry(data).State = EntityState.Detached;
                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Updation Error" }, JsonRequestBehavior.AllowGet);
                }
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


        }

        public ActionResult Available_Surveyor()
        {
            InsureEntities db = new InsureEntities();
            var a = db.Status_surveyor.Where(m => m.Status == 1);

            return Json(a, JsonRequestBehavior.AllowGet);
        }*/
        public ActionResult View_Surveyor()
        {
            //if( Session["UserId"] == null)
            //{
            //    return RedirectToAction("Login","Home");
            //}
          /*  InsureEntities3 db = new InsureEntities3();
            List<Registration> jsondata = db.Registrations.Where(q => q.RoleId == 3).ToList<Registration>();
            return View(jsondata);*/
            return View();
        }

        public JsonResult GetSurveyor()
        {
            InsureEntities db = new InsureEntities();

            var json = db.Registrations.Where(m => m.RoleId == 3).ToList();
            return Json(new { data = json }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AddorEditSurveyor(int id = 0)
        {
            InsureEntities db = new InsureEntities();
            if (id == 0)
            {
                return View(new Registration());
            }
            else
            {
                var data = db.Registrations.Where(m => m.UserID == id).FirstOrDefault();
                return View(data);
            }
        }
        [HttpPost]
        public ActionResult AddorEditSurveyor(Registration rm)
        {
            try
            {
                InsureEntities db = new InsureEntities();
                var data = db.Registrations.Where(m => m.UserID == rm.UserID).FirstOrDefault();
                if (data != null)
                {
                    rm.UserName = data.UserName;
                    rm.ConfirmPassword = data.ConfirmPassword;
                    rm.Gender = data.Gender;
                    rm.NewConfirmPassword = data.NewConfirmPassword;
                    rm.Password = data.Password;
                    rm.Pincode = data.Pincode;
                    rm.RoleId = data.RoleId;
                    rm.Address = data.Address;
                    db.Entry(data).State = EntityState.Detached;
                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Updation Error" }, JsonRequestBehavior.AllowGet);
                }
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



        }

        public ActionResult DeleteServeyor(int id)
        {
            //if( Session["UserId"] == null)
            //{
            //    return RedirectToAction("Login","Home");
            //}
            InsureEntities db = new InsureEntities();
            var jsondata = db.Registrations.Where(q => q.UserID == id).FirstOrDefault();
            if (jsondata != null)
            {
                db.Registrations.Remove(jsondata);
                db.SaveChanges();
            }
            return View(jsondata);
        }


        public ActionResult View_feedback()
        {
            return View();
        }
        public JsonResult feedbk()
        {
            InsureEntities db = new InsureEntities();
            var  jsondata = (from x in db.FeedbackMasters
                                             select new
                                             {
                                               x.UserId,
                                               x.CustId,
                                               x.Date,
                                               x.feedback


                                             }).ToList();
            return Json(new { data = jsondata }, JsonRequestBehavior.AllowGet);
        }

         [HttpGet]
        public ActionResult AddGarrage()
        {
            return View();
        }
       
        [HttpPost]
         public ActionResult AddGarrage(Models.GarrageMaster gm)
         {
            try
            {
                
                if (ModelState.IsValid)
                {
                   
                    using (InsureEntities db = new InsureEntities())
                    {                 
                       
                            db.GarrageMasters.Add(gm);
                            db.SaveChanges();
                            ModelState.Clear();
                            ViewBag.Error = "Sucessfully Submitted";
                            gm = null;
                            return RedirectToAction("Dashboard", "Admin");     
                        
                     }
                }
                else
                {
                    ModelState.AddModelError("", "Data is not correct");
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                //throw;
            }
            return View();
        }
        public ActionResult gm()
        {
            return View();
        }

        public JsonResult gmadd()
        {
            InsureEntities db = new InsureEntities();
            List<GarrageMaster> jsondata = db.GarrageMasters.ToList<GarrageMaster>();
            return Json(new { data = jsondata }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public ActionResult AddorEditGarrage(int id = 0)
        {
            InsureEntities db = new InsureEntities();
            if (id == 0)
            {
                return View(new GarrageMaster());
            }
            else
            {
                var data = db.GarrageMasters.Where(m => m.Id == id).FirstOrDefault();
                return View(data);
            }
        }
        [HttpPost]
        public ActionResult AddorEditGarrage(GarrageMaster rm)
        {
            try
            {
                InsureEntities db = new InsureEntities();
                var data = db.GarrageMasters.Where(m => m.Id == rm.Id).FirstOrDefault();
                if (data != null)
                {
                    //rm.Id = data.Id;
                    rm.Name = data.Name;
                    rm.Address = data.Address;
                    rm.PhoneNo = data.PhoneNo;
                    rm.OwnerName = data.OwnerName;
                    rm.Description = data.Description;
                    rm.IsAuth = data.IsAuth;
                    rm.CreatedBy = data.CreatedBy;
                    
                    db.Entry(data).State = EntityState.Detached;
                    db.Entry(rm).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Updation Error" }, JsonRequestBehavior.AllowGet);
                }
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



        }

        public ActionResult DeleteGarrage(int id)
        {
            
            InsureEntities db = new InsureEntities();
            var jsondata = db.GarrageMasters.Where(q => q.Id == id).FirstOrDefault();
            if (jsondata != null)
            {
                db.GarrageMasters.Remove(jsondata);
                db.SaveChanges();
            }
            return View(jsondata);
        }

        public ActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Upload(string baseData)
        {
            if (HttpContext.Request.Files.AllKeys.Any())
            {
                for (int i = 0; i <= HttpContext.Request.Files.Count; i++)
                {
                    var file = HttpContext.Request.Files["files" + i];
                    if (file != null)
                    {
                        var fileSavePath =Path.Combine(Server.MapPath("/Files"), file.FileName);
                        file.SaveAs(fileSavePath);
                    }
                }
            }
            return View();
         }

        public ActionResult Download()
        {
              string[] files = Directory.GetFiles(Server.MapPath("/Files"));
              for (int i = 0; i < files.Length; i++)
              {
                   files[i] = Path.GetFileName(files[i]);
              }
              ViewBag.Files = files;
              return View();
        }

        public FileResult DownloadFile(string fileName)
        {
             var filepath = System.IO.Path.Combine(Server.MapPath("/Files/"), fileName);
             return File(filepath, MimeMapping.GetMimeMapping(filepath), fileName);
        }

        public ActionResult UploadReport(int id=0)
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
           rm.AccidentId=id;
           rm.ReportType = fc[1];
           var sid = db.AccidentInfoes.Where(m => m.AccidentId ==id ).Select(m=>m.SurveyorId).FirstOrDefault();
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
                   //return Json(new { data = "fdsdsass" +h}, JsonRequestBehavior.AllowGet);
                   return RedirectToAction("View_Accident", "Admin");
               }
               
              
               return Json(new { data = "Faillll" +h}, JsonRequestBehavior.AllowGet);
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
       }

        public ActionResult SendEmail(checkintegration.Models.Sendmail okay,int id=0)
       {
           InsureEntities db = new InsureEntities();
           AccidentInfo ai = new AccidentInfo();
           Registration rg = new Registration();

           var sid = db.AccidentInfoes.Where(m => m.AccidentId == id).Select(m => m.SurveyorId).FirstOrDefault();
           var smail = db.Registrations.Where(m => m.UserID == sid).Single();

           var cid = db.AccidentInfoes.Where(m => m.AccidentId == id).Select(m => m.CustId).FirstOrDefault();
           var cmail = db.Registrations.Where(m => m.UserID == cid).Single();

           MailMessage mail = new MailMessage();
           mail.To.Add(smail.EmailId);
           mail.From = new MailAddress("jethalalgada004@gmail.com");
           mail.Subject = "Information about your Client";
           string Body = "Client FullName :" + cmail.FirstName+" "+cmail.LastName+"\n"+"\n Client Email ID:"+cmail.EmailId+"\n"+"\nClient Gender:"+cmail.Gender+"\n"+"\nClient Phone number:"+cmail.PhoneNumber+"\n"+"\nClient Address:"+cmail.Address+" "+cmail.Pincode;
           mail.Body = Body;
           mail.IsBodyHtml = true;
           SmtpClient smtp = new SmtpClient();
           smtp.Host = "smtp.gmail.com";
           smtp.Port = 587;
           smtp.UseDefaultCredentials = false;
           smtp.Credentials = new System.Net.NetworkCredential("jethalalgada004@gmail.com", "jethalal004"); // Enter seders User name and password  
           smtp.EnableSsl = true;
           smtp.Send(mail);
           ViewBag.Error = "Information has been sent to registered mail id";

           mail.To.Add(cmail.EmailId);
           mail.From = new MailAddress("jethalalgada004@gmail.com");
           mail.Subject = "Information about your Surveyor";
           string info = "Surveyor FullName :" + smail.FirstName + " " + smail.LastName +"\n"+ "\n Surveyor Email ID:" + smail.EmailId + "\n"+"\nSurveyor Gender:" + smail.Gender +"\n"+ "\nSurveyor Phone number:" + smail.PhoneNumber + "\n"+"\nSurveyor Address:" + smail.Address + " " + smail.Pincode;
           mail.Body = info;
           mail.IsBodyHtml = true;
          // SmtpClient smtp = new SmtpClient();
           smtp.Host = "smtp.gmail.com";
           smtp.Port = 587;
           smtp.UseDefaultCredentials = false;
           smtp.Credentials = new System.Net.NetworkCredential("jethalalgada004@gmail.com", "jethalal004"); // Enter seders User name and password  
           smtp.EnableSsl = true;
           smtp.Send(mail);
           ViewBag.Error = "Information has been sent to registered mail id";
           
           return View("View_Accident");


           
           return View();
       }

        
    }
}

