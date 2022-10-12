using CodeHub.Models;
using CodeHub.ViewModel;
using CodeHub.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CodeHub.Controllers
{
    public class AuthenticationController : Controller
    {
        // GET: Authentication

        code_hubEntities7 conn = new code_hubEntities7();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
                if(Session["id"]== null)
                {
                    return View("Login");
                }
                else
                {
                    TempData["message"] = AlertMessages.show("Your Are Login", "error");
                    return Redirect("/CodeHub/Index");
                }       
        }
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            try
            {
                 user u = conn.users.SingleOrDefault(utd => utd.email == email && utd.password == password);             
                var active = u.IsActive;
                if (u != null && active == true)
                {
                    Session["email"] = u.email;
                    Session["id"] = u.user_id;
                    Session["role"] = u.role;
                    Session["first_name"] = u.first_name;
                    Session["last_name"] = u.last_name;
                    Session["skills"] = u.skills;
                    if (u.role == "Expert")
                    {

                        return Redirect("/Expert");

                    }
                    else if (u.role == "admin")
                    {

                        return Redirect("/Admin");

                    }
                    else
                    {
                        return Redirect("/CodeHub/Index");

                    }

                }
            }
            catch(Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again."+ex.Message, "error");
                return RedirectToAction("Login");
            }
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
        //public ActionResult Signup()
        //{
        //    return View();
        //}
        [HttpGet]
        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]

        public ActionResult Signup(string first_name, string last_name, string email, string password, string role, bool IsActive, string ansone, string anstwo)
        {
            try
            {
                user u = new user();
                if (email != "")
                {
                    u.email = email;
                    var isEmailAlreadyExists = conn.users.Any(x => x.email == email);
                    if (isEmailAlreadyExists)
                    {
                        TempData["message"] = AlertMessages.show("Email cannot be same", "error");
                        return RedirectToAction("Signup");
                    }
                }
                else
                {
                    TempData["message"] = AlertMessages.show("Please Enter Email!", "error");
                    return RedirectToAction("Signup");
                }
                if (first_name != "")
                {
                    u.first_name = first_name;
                }
                else
                {
                    TempData["message"] = AlertMessages.show("Please Enter First Name!", "error");
                    return RedirectToAction("Signup");
                }
                if (last_name != "")
                {
                    u.last_name = last_name;
                }
                else
                {
                    TempData["message"] = AlertMessages.show("Please Enter Last Name!", "error");
                    return RedirectToAction("Signup");
                }
                if (password != "" && password.Length >= 8)
                {
                    u.password = password;
                }
                else
                {
                    TempData["message"] = AlertMessages.show("Please Enter Valid password!", "error");
                    return RedirectToAction("Signup");
                }
                if (ansone != "")
                {
                    u.ansone = ansone;
                }
                else
                {
                    TempData["message"] = AlertMessages.show("Enter your answer!", "error");
                    return RedirectToAction("Signup");
                }
                if (anstwo != "")
                {
                    u.anstwo = anstwo;
                }
                else
                {
                    TempData["message"] = AlertMessages.show("Enter your answer!", "error");
                    return RedirectToAction("Signup");
                }
                u.role = role;
                u.IsActive = IsActive;
                conn.users.Add(u);
                conn.SaveChanges();
                TempData["message"] = AlertMessages.show("Account Created! Please Login.", "success");
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("Signup");
            }

        }
        public ActionResult ForgetPassword()
        {
            try
            {
                List<user> u = conn.users.SqlQuery("Select * from [user]").ToList();
                return View(u);
            }
            catch(Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ForgetPassword");
            }
        }
        [HttpPost]
        public ActionResult ForgetPassword(string email, string ansone, string anstwo)
        {
            try
            {
                var query = conn.users.Where(x => x.email == email).FirstOrDefault();
                if (query != null)
                {
                    if (query.ansone == ansone && query.anstwo == anstwo)
                    {
                        TempData["message"] = AlertMessages.show("Answers are correct.", "success");
                        return RedirectToAction("ResetPassword");
                    }

                    TempData["message"] = AlertMessages.show("Enter valid information.", "error");
                    return RedirectToAction("ForgetPassword");
                }
                else
                {
                    TempData["message"] = AlertMessages.show("Account not updated! Please Try Again. Enter valid information.", "error");
                    return RedirectToAction("ForgetPassword");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ForgetPassword");
            }
            return View();
        }
        public ActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(user u)
        {
            try
            {
                var query = conn.users.Where(x => x.email == u.email).FirstOrDefault();
                if (query != null)
                {
                    query.email = u.email;
                    query.password = u.password;

                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Password updated successfully.", "success");
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["message"] = AlertMessages.show("Account not updated! Please Try Again. Enter valid email.", "error");
                    return RedirectToAction("ResetPassword");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ResetPassword");
            }
            return View();
        }
        public ActionResult Error404()
        {
            return View();
        }
    }
}