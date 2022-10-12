using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeHub.ViewModels;
using System.IO;
using CodeHub.Models;
using CodeHub.ViewModel;

namespace CodeHub.Controllers
{
    public class ExpertController : Controller
    {
        // GET: Expert
        code_hubEntities7 conn = new code_hubEntities7();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ExpertFeedback()
        {
            return View();
        }
        [HttpGet]
        public ActionResult RequestForJoinAsExpert()
        {
            return View(from user in conn.users select user);
        }
        [HttpPost]
        public ActionResult RequestForJoinAsExpert(string first_name, string last_name, string email, string password, string skills, string role, HttpPostedFileBase file)
        {
            try
            {
                user u = new user();
                    if (file != null)
                    {
                        string _FileName = Path.GetFileName(file.FileName);
                        string _path = Path.Combine(Server.MapPath("~/UploadedFiles"), _FileName);
                        file.SaveAs(_path);
                        u.cv = _FileName;
                    }
                    else
                    {
                        TempData["message"] = AlertMessages.show("Enter your CV ", "error");
                        return RedirectToAction("RequestForJoinAsExpert");
                    }
                    if(first_name != "")
                    {
                        u.first_name = first_name;
                    }
                    else
                    {
                        TempData["message"] = AlertMessages.show("Enter Name", "error");
                        return RedirectToAction("RequestForJoinAsExpert");
                    }
                     if (last_name != "")
                    {
                        u.last_name = last_name;
                    }
                    else
                    {
                        TempData["message"] = AlertMessages.show("Enter Name", "error");
                        return RedirectToAction("RequestForJoinAsExpert");
                    }     
                u.email = email;
                u.password = password;
                u.skills = skills;
                u.role = role;         
                conn.users.Add(u);
                conn.SaveChanges();
                TempData["message"] = AlertMessages.show("Reqest sent successfully", "Success");
                return RedirectToAction("RequestForJoinAsExpert");
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("RequestForJoinAsExpert");
            }
          
        }
   

        public ActionResult TeamMembers()
        {
            try
            {
                List<user> e = conn.users.SqlQuery("select * from  [user]  where [user].role!='Student' and [user].IsActive='True'").ToList();
                return View(e);
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("TeamMembers");
            }
        }


    }
}