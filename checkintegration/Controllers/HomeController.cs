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


namespace checkintegration.Controllers
{
    public class HomeController : Controller
    {
      
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Homepage()
        {
            return View();
        }

        [HttpGet]
        public ActionResult LogIn()
        {
            if (Session["UserId"] != null)
            {
                int u=(int)Session["UserId"];
                var db = new checkintegration.Models.InsureEntities();
                var user = db.Registrations.Where(m => m.UserID == u).Select(m => m.RoleId).FirstOrDefault();
                if (user == 1)
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else if (user == 2)
                {
                    return RedirectToAction("Homepage", "Home");
                }
                else
                {
                    return RedirectToAction("Surveyor_Index", "Surveyor");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(Models.Registration userr)
        {
            //if (ModelState.IsValid)  
            //{  
          //  InsureEntities db = new InsureEntities();
            using (var db = new checkintegration.Models.InsureEntities())
            {
                var user = db.Registrations.FirstOrDefault(u => u.UserName == userr.UserName && u.Password == userr.Password);
                if (user != null)
                {
                   if(user.RoleId==1)
                   {
                       return RedirectToAction("Dashboard", "Admin");
                   }
                   else if(user.RoleId==2)
                   {
                       FormsAuthentication.SetAuthCookie(userr.UserName, false);
                       Session["UserId"] = user.UserID;
                       return RedirectToAction("Homepage", "Home");
                   }
                   else
                   {
                       Session["CustId"] = user.UserID;
                       FormsAuthentication.SetAuthCookie(userr.UserName, false);
                       return RedirectToAction("Surveyor_Index", "Surveyor");
           
                   }
                }
            }
           
            return View(userr);
        }

        [HttpGet]
        public ActionResult Accident_Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Accident_Register(Models.AccidentInfo ss)
        {
            try
            {
              // Registration rg=new Registration();
                ss.CustId = (int)Session["UserId"];
                InsureEntities db1= new InsureEntities();
                var a = db1.Status_surveyor.Where(m => m.Status == 1).Select(m=>m.SurvId).ToArray();
                var rand = new Random();
                var sid = a[rand.Next(a.Length)];
                
                ss.SurveyorId = sid;
                ss.Status = 0;
                        
                if (ModelState.IsValid)
                {
                    using (InsureEntities db = new InsureEntities())
                    {

                        
                        db.AccidentInfoes.Add(ss);
                        db.SaveChanges();
                        ModelState.Clear();
                        //Session["UserID"] = user.UserID;
                        ViewBag.Error = "Sucessfully Submitted";
                        ss= null;
                        return RedirectToAction("Homepage", "Home");


                    }
                }
                else
                {
                    /* using (InsureEntities2 db = new InsureEntities2())
                     {

                         db.FeedbackMasters.Add(fm);
                         db.SaveChanges();
                         ModelState.Clear();
                         //Session["UserID"] = user.UserID;
                         ViewBag.Error = "Sucessfully Submitted";
                         fm = null;
                         return RedirectToAction("Login", "Home");


                     }
                     */
                    // ModelState.AddModelError("", "Data is not correct");
                    return RedirectToAction("Register", "Home");
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
                return View();
                // throw;
            }
           /* catch(Exception e)
            {
                Console.Write(e);
                return View();

            }*/
            


        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Models.Registration user,FormCollection fc)
        {
            try
            {
                
                if (ModelState.IsValid)
                {
                    user.RoleId = 2;

                    using (InsureEntities db = new InsureEntities())
                    {

                     /*  var newUser = db.Registrations.Create();
                       // newUser.UserID = user.UserID;
                        newUser.FirstName = user.FirstName;
                        newUser.LastName = user.LastName;
                        newUser.UserName = user.UserName;
                        newUser.EmailId = user.EmailId;
                        newUser.Password = user.Password;
                        newUser.ConfirmPassword = user.ConfirmPassword;
                        newUser.Gender = user.Gender;
                       // newUser.RoleId = user.RoleId;
                        newUser.Address = user.Address;
                        newUser.Pincode = user.Pincode;
                        newUser.PhoneNumber = user.PhoneNumber;                     
                       */
                        var username = db.Registrations.Where(u => u.UserName == user.UserName).FirstOrDefault();
                        var em = db.Registrations.Where(u => u.EmailId == user.EmailId).FirstOrDefault();
                        if(username!=null)
                        {

                            ViewBag.Error = "UserName already Exist";
                            return View("Register", user);
                        }
                        
                        else if (em != null)
                        {
                            ViewBag.Error = "EmailId already Exist";
                            return View("Register", user);
                        }
                        else
                        {
                            //user.FirstName
                            user.Gender = fc["type"].ToString();
                            db.Registrations.Add(user);
                            db.SaveChanges();
                            ModelState.Clear();
                            Session["UserID"] = user.UserID;
                            ViewBag.Error = "Sucessfully Submitted";
                            user = null;
                            return RedirectToAction("Index", "Home");
                    
                            
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

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            Session.RemoveAll();
            return RedirectToAction("Index", "Home");
        }

    [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult ForgotPassword(checkintegration.Models.Sendmail okay)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    InsureEntities db = new InsureEntities();
                    var email = db.Registrations.Where(a => a.EmailId == okay.To).FirstOrDefault();
                    if (email != null)
                    {
                        var pass = db.Registrations.Where(a => a.EmailId == okay.To).Select(a => new { Password = a.Password }).Single();
                        MailMessage mail = new MailMessage();
                        mail.To.Add(okay.To);
                        mail.From = new MailAddress("jethalalgada004@gmail.com");
                        mail.Subject = "Forgot Password";
                        string Body = "Your Password is:" + pass.Password;
                        mail.Body = Body;
                        mail.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new System.Net.NetworkCredential("jethalalgada004@gmail.com", "jethalal004"); // Enter seders User name and password  
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                        ViewBag.Error = "Password has been sent to registered mail id";
                        return View("LogIn");
                    }
                    else
                    {
                        ViewBag.Error = "Email not Found";
                        return View(okay);
                    }
                }
                else
                {
                    ViewBag.Error = "Some Error Occured";
                    return View(okay);
                }
            }
            catch (Exception e)
            {
                ViewBag.Error = "Error Occured";
                return View("ForgotPassword", okay);
            }
        }

        [HttpGet]
        public ActionResult Change_Password()
        {
            return View();
        }
        [HttpPost]
       // string OldPassword, string NewPassword, string NewConfirmPassword,string UserID
        public ActionResult Change_Password(Models.Registration user)
        {
           
                InsureEntities db = new InsureEntities();
                //LoginMst obj = new LoginMst();
                var lst = db.Registrations.Where(P => P.EmailId == user.EmailId && P.Password == user.Password).SingleOrDefault();
                var ls = db.Registrations.Where(P => P.Password == user.NewPassword).SingleOrDefault();
                //bool IsUserExist = db.LoginMsts.Where(P => P.SrNo == UserID && P.Password==OldPassword).Any();
                if (lst != null)
                {
                    ViewBag.message = "Changing done";
                    if (ls != null)
                    {
                        ViewBag.Error = "New Password already Exist";
                       return RedirectToAction("Change_Password", "Home");
                    }
                    else
                    {
                         var ab = db.Registrations.Where(P => P.EmailId == user.EmailId && P.Password == user.Password).SingleOrDefault();
                      //  var cd = db.Registrations.Where(P => P.Password == user.NewPassword).SingleOrDefault();
                        Registration rg=new Registration();
                        rg.UserID = ab.UserID;
                        rg.FirstName = ab.FirstName;
                        rg.LastName = ab.LastName;
                        rg.UserName = ab.UserName;
                        rg.EmailId = ab.EmailId;
                        rg.Password = user.NewPassword;
                        rg.ConfirmPassword = user.NewConfirmPassword;
                        rg.Gender = ab.Gender;
                        rg.RoleId = ab.RoleId;
                        rg.Address = ab.Address;
                        rg.Pincode = ab.Pincode;
                        rg.PhoneNumber = ab.PhoneNumber;

                        db.Entry(ab).State = EntityState.Detached;
                        db.Entry(rg).State = EntityState.Modified;
                       /* lst.Password = user.NewPassword;
                        lst.ConfirmPassword = user.NewConfirmPassword;*/
                        
                        ViewBag.message = "Changing password successflly done";
                       // db.Registrations.Add(user);
                        try
                        {
                            db.SaveChanges();
                        }
                         catch (DbEntityValidationException ex)
                         {
                            foreach (var entityValidationErrors in ex.EntityValidationErrors)
                            {
                                foreach (var validationError in entityValidationErrors.ValidationErrors)
                                {
                                    Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                                }
                            }
                         }
                      /*  System.Diagnostics.Debug.Write("Hello via Debug!"+lst.Password);
                        return RedirectToAction("Change_Password", "Home");*/
                        return Json(ViewBag.message);

                    }
                    //return 0;
                }
                else
                {
                    ViewBag.Error = "Email Id or Password is wrong!!";
                    return RedirectToAction("Change_Password", "Home"); ;
                }
            
            /*catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
            }*/
                return View();
        }
      /*  private bool IsValid(string username, string password)
        {
            bool IsValid = false;

            using (var db = new checkintegration.Models.InsureEntities())
            {
             var  user = db.Registrations.FirstOrDefault(u => u.UserName == username && u.Password == password);
                if (user != null)
                {
                    IsValid = true;
                }
            }
            return IsValid;
        }*/

        //private int RoleId(string email, string password)
        //{
        //    int IsValid = 0;

        //    using (var db = new checkintegration.Models.InsureEntities())
        //    {
        //        var user = db.Table_Reg.FirstOrDefault(u => u.Email_Id == email && u.Password == password);
        //        if (user != null)
        //        {
        //            IsValid = user.roleId;
        //        }
        //    }
        //    return IsValid;
        //}

        [HttpGet]
        public ActionResult ContactUs()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult ContactUs(Models.FeedbackMaster fm)
        {
            try
            {
                fm.Date = DateTime.Now;
                fm.CustId = (System.Int32)Session["UserId"];
                if (ModelState.IsValid)
                {
                    
                            

                    using (InsureEntities db = new InsureEntities())
                    {

                       /* var newUser = db.FeedbackMasters.Create();
                        newUser.CustId = fm.UserId;
                      //  newUser.Date = fm.Date;
                        newUser.feedback = fm.feedback;*/
                       
                            db.FeedbackMasters.Add(fm);
                            db.SaveChanges();
                            ModelState.Clear();
                            //Session["UserID"] = user.UserID;
                            ViewBag.Error = "Sucessfully Submitted";
                            fm = null;
                            return RedirectToAction("Login", "Home");


                    }
                }
                else
                {
                   /* using (InsureEntities2 db = new InsureEntities2())
                    {

                        db.FeedbackMasters.Add(fm);
                        db.SaveChanges();
                        ModelState.Clear();
                        //Session["UserID"] = user.UserID;
                        ViewBag.Error = "Sucessfully Submitted";
                        fm = null;
                        return RedirectToAction("Login", "Home");


                    }
                    */
                   // ModelState.AddModelError("", "Data is not correct");
                    return RedirectToAction("Register", "Home");
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
                return View();
               // throw;
            }
            catch(Exception e)
            {
                Console.Write(e);
                return View();

            }
            

        }


        public ActionResult Aboutus()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

    }
}
