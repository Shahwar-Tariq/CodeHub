using CodeHub.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeHub.ViewModel;
using System.Data.SqlClient;
using CodeHub.Models;

namespace CodeHub.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        code_hubEntities7 conn = new code_hubEntities7();
        public ActionResult Index()
        {
            if (Session["id"] == null)
            {
                return Redirect("/Authentication/Login");
            }
            else
            {
                return View();
            }
        }    
        public ActionResult AddCourses()
        {   
            return View();
        }
        [HttpPost]
        public ActionResult AddCourses(string course_code, string course_name,string type, string summary, float price, HttpPostedFileBase C_image)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    course c = new course();
                    try
                    {
                        if (course_code != "")
                        {
                            c.course_code = course_code;
                        }
                        else
                        {
                            TempData["message"] = AlertMessages.show("Please Enter Course Code!", "error");
                            return RedirectToAction("AddCourses");
                        }
                        var isCourseCodeAlreadyExists = conn.courses.Any(x => x.course_code == course_code);
                        if (isCourseCodeAlreadyExists)
                        {
                            TempData["message"] = AlertMessages.show("Course Code cannot be same", "error");
                            return RedirectToAction("AddCourses");
                        }

                    }
                    catch (SqlException ex)
                    {
                        TempData["message"] = AlertMessages.show("Course Code Already Exists!", "error");
                        return RedirectToAction("AddCourses");
                    }                 
                    try
                    {
                        if (course_name != "")
                        {
                            c.course_name = course_name;
                        }
                        else
                        {
                            TempData["message"] = AlertMessages.show("Please Enter Course Name!", "error");
                            return RedirectToAction("AddCourses");
                        }
                        var isCourseNameAlreadyExists = conn.courses.Any(x => x.course_name == course_name);
                        if (isCourseNameAlreadyExists)
                        {
                            TempData["message"] = AlertMessages.show("Course name cannot be same", "error");
                            return RedirectToAction("AddCourses");
                        }
                    }
                    catch (SqlException ex)
                    {
                        TempData["message"] = AlertMessages.show("Course Name Already Exists!", "error");
                        return RedirectToAction("AddCourses");
                    }
                    c.type = type;
                    c.summary = summary;
                    if(type=="Paid")
                    {
                        if(price<=0)
                        {
                            TempData["message"] = AlertMessages.show("Please Enter Price!", "error");
                            return RedirectToAction("AddCourses");
                        }
                        else
                        {
                            c.price = price;
                        }
                    }              
                    try
                    {
                        c.image = C_image.ToString();
                    }
                    catch (NullReferenceException ex)
                    {
                        TempData["message"] = AlertMessages.show("Please Upload Image!", "error");
                        return RedirectToAction("AddCourses");
                    }
                    if (C_image != null)
                    {
                        string ImageName = Path.GetFileName(C_image.FileName);
                        var isImageAlreadyExists = conn.courses.Any(x => x.image == ImageName);
                        if (isImageAlreadyExists)
                        {
                            TempData["message"] = AlertMessages.show("Image already Exists,Please Rename Your Image!", "error");
                            return RedirectToAction("AddCourses");
                        }
                        C_image.SaveAs(Server.MapPath("~/Images/Course_Images/" + ImageName));
                        c.image = ImageName;
                    }
                    conn.courses.Add(c);
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Course Added Successfully!","success");
                    return RedirectToAction("AddCourses");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("AddCourses");
            }            
        }
        public ActionResult ViewCourses()
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    List<course> c = conn.courses.SqlQuery("select * from course").ToList();
                    return View(c);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewCourses");
            }
        }
        public ActionResult EditCourses(string course_code)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    course c = conn.courses.Single(ctd => ctd.course_code == course_code);
                    return View(c);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewCourses");
            }
        }
        [HttpPost]
        public ActionResult Editcourse(string course_code, string course_name, string type, string summary, float price, HttpPostedFileBase C_image)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    course c = conn.courses.Single(ctd => ctd.course_code == course_code);
                    c.course_code = course_code;
                    if(course_name!="")
                    {
                        c.course_name = course_name;
                    }
                   else
                    {
                        TempData["message"] = AlertMessages.show("Please Enter Course Name!", "error");
                        return Redirect("/Admin/EditCourse?course_code='"+course_code);
                    }
                    c.type = type;
                    c.summary = summary;
                    if (type == "Paid")
                    {
                        if (price <= 0)
                        {
                            TempData["message"] = AlertMessages.show("Please Enter Price!", "error");
                            return Redirect("/Admin/EditCourse?course_code='"+course_code);
                        }
                        else
                        {
                            c.price = price;
                        }
                    }
                    if (C_image != null)
                    {             
                        c.image = C_image.ToString();
                        string ImageName = Path.GetFileName(C_image.FileName);
                        var isImageAlreadyExists = conn.courses.Any(x => x.image == ImageName);
                        if (isImageAlreadyExists)
                        {
                            TempData["message"] = AlertMessages.show("Image already Exists,Please Rename Your Image!", "error");
                            return Redirect("/Admin/EditCourse?course_code='"+course_code);
                        }
                        C_image.SaveAs(Server.MapPath("~/Images/Course_Images/"+ImageName));
                        c.image = ImageName;
                    }
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Course Updated Successfully!", "success");
                    return RedirectToAction("ViewCourses");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewCourses");
            }
        }
        public ActionResult DeleteCourse(string course_code)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    course c = conn.courses.Single(ctd => ctd.course_code == course_code);
                    conn.courses.Remove(c);
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Course Delete Successfully!", "success");
                    return RedirectToAction("ViewCourses");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewCourses");
            }
        }
        public ActionResult AssignCourse()
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    List<course> c = conn.courses.SqlQuery("select course.* from course left join AssignCourse on course.course_code = AssignCourse.course_code where AssignCourse.course_code is NULL").ToList();
                    ViewBag.courses = c;
                    List<user> u = conn.users.Where(l => l.role != "Student" && l.IsActive == true).ToList();
                    var userCourse = new ViewModel.UserCourseViewModel()
                    {
                        crs = c,
                        usr = u,
                    };
                    return View(userCourse);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("AssignCourse");
            }
        }       
        [HttpPost]
        public ActionResult AssignCourse(string course_code, int user_id)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    AssignCourse ac = new AssignCourse();
                    ac.course_code = course_code;
                    try
                    {
                        ac.user_id = user_id;
                    }
                    catch(Exception ex)
                    {
                        TempData["message"] = AlertMessages.show("Please select Expert!" + ex.Message, "error");
                        return RedirectToAction("AssignCourse");
                    }
                    conn.AssignCourses.Add(ac);
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Course Assigned Successfully!", "success");
                    return RedirectToAction("AssignCourse");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("AssignCourse");
            }
        }
        public ActionResult ViewAssignedCourse()
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    List<AssignCourse> ac = conn.AssignCourses.SqlQuery("select * from AssignCourse ").ToList();
                    return View(ac);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewAssignedCourse");
            }
        }
        public ActionResult EditAssignCourses(string course_code)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    AssignCourse ac = conn.AssignCourses.Single(td => td.course_code == course_code);
                    course c = conn.courses.Single(ctd => ctd.course_code == course_code);
                    ViewBag.courses = c;
                    List<user> u = conn.users.Where(l => l.role != "Student" && l.IsActive == true).ToList();
                    var userCourse = new ViewModel.UserCourseViewModel()
                    {
                        crses = c,
                        usr = u,
                        assigncrs = ac
                    };
                    return View(userCourse);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewAssignedCourse");
            }
        }
        [HttpPost]
        public ActionResult EditAssignCourse(string course_code, int user_id)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    AssignCourse ac = conn.AssignCourses.Single(td => td.course_code == course_code);
                    ac.course_code = course_code;
                    ac.user_id = user_id;
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Course Assign Update Successfully!", "success");
                    return RedirectToAction("ViewAssignedCourse");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewAssignedCourse");
            }
        }
        public ActionResult DeleteAssignCourse(string course_code)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    AssignCourse ac = conn.AssignCourses.Single(ctd => ctd.course_code == course_code);
                    conn.AssignCourses.Remove(ac);
                    conn.SaveChanges();
                   TempData["message"] = AlertMessages.show("course Assign Delete Successfully!", "success");             
                    return RedirectToAction("ViewAssignedCourse");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewAssignedCourse");
            }
        }
        public ActionResult AddTeachers()
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    List<user> u = conn.users.Where(utd => utd.IsActive != true && utd.role == "Expert").ToList();
                    return View(u);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("AddTeachers");
            }
           
        }
        public ActionResult DownloadCV(string filePath, int id)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    user file = conn.users.Where(x => x.user_id == id).FirstOrDefault();
                    string path = Server.MapPath("~/UploadedFiles/") + file.cv;
                    byte[] bytes = System.IO.File.ReadAllBytes(path);
                    return File(bytes, "application/octet-stream", file.cv);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("AddTeachers");
            }
            
        }
        public ActionResult AcceptJoinRequest(int id)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    user u = conn.users.SingleOrDefault(utd => utd.user_id == id);
                    u.IsActive = true;
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Expert Added Successfully", "Success");
                    return RedirectToAction("AddTeachers");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("AddTeachers");
            }
            
        }
        public ActionResult DeleteJoinRequest(int id)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    user u = conn.users.Single(utd => utd.user_id == id);
                    conn.users.Remove(u);
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Expert Request Deleted Successfully" ,"Success");
                    return RedirectToAction("AddTeachers");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("AddTeachers");
            }
          
        }
        public ActionResult ViewTeachers()
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    List<user> c = conn.users.SqlQuery("select * from [user] where IsActive = 'True' and role = 'Expert'").ToList();
                    return View(c);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewTeachers");
            }
            
        }
        public ActionResult DeleteExpert(int id)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    user u = conn.users.Single(utd => utd.user_id == id);
                    conn.users.Remove(u);
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Expert Account Deleted Successfully", "Success");
                    return RedirectToAction("ViewTeachers");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewTeachers");
            }
            
        }
        public ActionResult DeactivateExpert(int id)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    user u = conn.users.Single(utd => utd.user_id == id);
                    u.IsActive = false;
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Expert Account Deactivated Successfully", "Success");
                    return RedirectToAction("ViewTeachers");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewTeachers");
            }
            
        }
        public ActionResult ViewPayments()
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    List<order> o = conn.orders.SqlQuery("select * from [order] left join order_details on [order].id = order_details.id ").ToList();
                    
                    return View(o);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewPayments");
            }
            
        }

    }
}