using CodeHub.ViewModels;
using CodeHub.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeHub.Models;
using System.Diagnostics;
using System.Collections;

namespace CodeHub.Controllers
{
    public class html
    {
        public string data;
    }
    public class CodeHubController : Controller
    {
        // GET: Users
        code_hubEntities7 conn = new code_hubEntities7();
        List<cart> li = new List<cart>();
        public ActionResult Index()
        {
            try
            {
                CourseCartIndexViewModel cvm = new CourseCartIndexViewModel();
                if (Session["id"] != null)
                {
                    var id_ = Convert.ToInt32(Session["id"].ToString());
                    cvm.corse = conn.courses.SqlQuery("select * from course ").ToList();
                    cvm.orddet = conn.order_details.SqlQuery("select * from course full outer join order_details on course.course_code = order_details.course_code left join [order] on [order].id = order_details.order_id  where [order].user_id = " + id_).ToList();

                    cvm.cartitem = conn.cart_item.SqlQuery("select * from cart_item ").ToList();
                    if (Session["id"] != null)
                    {
                        var id = Convert.ToInt32(Session["id"].ToString());
                        cvm.ord = conn.orders.SqlQuery("select * from [order] where [order].user_id = " + id).ToList();
                    }
                    return View(cvm);
                }
                else
                {
                    cvm.corse = conn.courses.SqlQuery("select * from course ").ToList();
                    cvm.cartitem = conn.cart_item.SqlQuery("select * from cart_item ").ToList();
                    if (Session["id"] != null)
                    {
                        var id = Convert.ToInt32(Session["id"].ToString());
                        cvm.ord = conn.orders.SqlQuery("select * from [order] where [order].user_id = " + id).ToList();
                    }
                    return View(cvm);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("Index");
            }
        }
        public ActionResult ViewCart()
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    int uid = Convert.ToInt32(Session["id"].ToString());
                    var t = conn.carts.Where(i => i.user_id == uid).SingleOrDefault();
                    var cri = conn.cart_item.Where(i => i.cart_id == t.id).ToList();


                    t.quantity = cri.Count();
                    t.total = 0;
                    foreach (var k in cri)
                    {
                        t.total = t.total + Convert.ToDouble(k.price);
                    }
                    conn.SaveChanges();
                    return View(cri);

                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewCart");
            }

        }
        public ActionResult BuyCourse(string course_code)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    int uid = Convert.ToInt32(Session["id"].ToString());
                    var c = from cart in conn.carts
                            where cart.user_id == uid
                            select cart;
                    if (c.Count() == 0)
                    {
                        cart crt = new cart();
                        crt.user_id = uid;
                        crt.quantity = null;
                        crt.total = null;

                        conn.carts.Add(crt);
                        conn.SaveChanges();

                        cart_item ci = new cart_item();
                        ci.cart_id = crt.id;
                        var crs = conn.courses.Where(i => i.course_code == course_code).SingleOrDefault();
                        ci.course_code = crs.course_code;
                        ci.price = Convert.ToInt32(crs.price);

                        conn.cart_item.Add(ci);
                        conn.SaveChanges();

                    }
                    else
                    {
                        var t = conn.carts.Where(i => i.user_id == uid).SingleOrDefault();
                        cart_item ci = new cart_item();
                        ci.cart_id = t.id;
                        var crs = conn.courses.Where(i => i.course_code == course_code).Distinct().SingleOrDefault();
                        ci.course_code = crs.course_code;
                        ci.price = Convert.ToInt32(crs.price);

                        conn.cart_item.Add(ci);
                        conn.SaveChanges();

                    }

                    return RedirectToAction("ViewCart");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewCart");
            }

        }
        public ActionResult RemoveItem(int id)
        {
            try
            {
                cart_item c = conn.cart_item.Single(ctd => ctd.id == id);
                conn.cart_item.Remove(c);
                conn.SaveChanges();
                TempData["message"] = AlertMessages.show("Item removed successfully" , "error");
                return RedirectToAction("ViewCart");
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewCart");
            }

        }
        public ActionResult Checkout()
        {
            try
            {
                int uid = Convert.ToInt32(Session["id"].ToString());
                var t = conn.carts.Where(i => i.user_id == uid).SingleOrDefault();
                var cri = conn.cart_item.Where(i => i.cart_id == t.id).ToList();
                return View(cri);
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("Checkout");
            }

        }
        public ActionResult Thankyou()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Thankyou(string transactionID, string status, string amount_paid, string frstname, string lstname)
        {

            order o = new order();
            int uid = Convert.ToInt32(Session["id"].ToString());
            o.first_name = frstname;
            o.last_name = lstname;
            o.payed_amount = Convert.ToDouble(amount_paid);
            o.payment_id = transactionID;
            o.status = status;
            o.user_id = uid;

            conn.orders.Add(o);
            conn.SaveChanges();

            var crs = conn.carts.Where(x => x.user_id == uid).FirstOrDefault();
            List<cart_item> cartcourses = crs.cart_item.ToList();

            foreach (var course in cartcourses)
            {
                order_details ord = new order_details();
                ord.course_code = course.course_code;
                ord.order_id = o.id;
                ord.price = o.payed_amount;

                conn.order_details.Add(ord);
                conn.SaveChanges();
                conn.cart_item.Remove(course);
                conn.SaveChanges();
            }

            var i = "success";
            return Json(i, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Dashboard()
        {
            if (Session["id"] == null)
            {
                return Redirect("/Authentication/Login");
            }
            else
            {
                
                dashboardViewModel dvm = new dashboardViewModel();
                int uid = Convert.ToInt32(Session["id"]);
               dvm.crs = conn.courses.SqlQuery("select * from course inner join order_details on course.course_code = order_details.course_code inner join [order] on [order].id = order_details.order_id where [order].status = 'COMPLETED' and [order].user_id = " + uid).ToList();
                dvm.fav_crs = conn.favorite_courses.SqlQuery("select * from favorite_courses where user_id = "+uid).ToList();
                return View(dvm);
            }
        }
        public ActionResult RemoveFavorite(int id)
        {
            try
            {
                favorite_courses c = conn.favorite_courses.Single(ctd => ctd.id == id);
                conn.favorite_courses.Remove(c);
                conn.SaveChanges();
                TempData["message"] = AlertMessages.show("Favorite Course removed successfully", "error");
                return RedirectToAction("Dashboard");
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("Dashboard");
            }

        }
        public ActionResult Feedback()
        {
            try
            {
                courseFeedback crs_fb = new courseFeedback();
                crs_fb.crs = conn.courses.SqlQuery("select * from course ").ToList();
                crs_fb.fbs = conn.feedbacks.SqlQuery("select * from feedback ").ToList();
                crs_fb.urs = conn.users.SqlQuery("select * from [user] ").ToList();
                crs_fb.fbr = conn.feedback_reply.SqlQuery("select * from feedback_reply ").ToList();
                crs_fb.fbsr = conn.feedback_reply.SqlQuery("select * from feedback inner join feedback_reply on feedback_reply.feedback_id = feedback.id inner join [user] on [user].user_id = feedback_reply.replied_by").ToList();
                return View(crs_fb);
            }
            catch(Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("Feedback");
            }
        }
        [HttpPost]
        public ActionResult Feedback(string feedback, string feedbackfor)
        {
            try
            {
                feedback fb = new feedback();
               var send_id = Convert.ToInt32(Session["id"].ToString());
                var isFeedbackAlreadyExists = conn.feedbacks.Any(x => x.send_by == send_id && x.feedbackfor == feedbackfor);
                if (isFeedbackAlreadyExists)
                {
                    TempData["message"] = AlertMessages.show("You can give only one feedback to one course! please choose another course.", "error");
                    return RedirectToAction("Feedback");
                }
                else
                {
                    fb.send_by = Convert.ToInt32(Session["id"].ToString());
                }
                if (feedback != "")
                {
                    fb.details = feedback;
                }
                else
                {
                    TempData["message"] = AlertMessages.show("Enter Feedback", "error");
                    return RedirectToAction("Feedback");
                }

                if (feedbackfor != "")
                {
                    fb.feedbackfor = feedbackfor;
                }
                else
                {
                    TempData["message"] = AlertMessages.show("Select a value from dropdown" , "error");
                    return RedirectToAction("Feedback");
                }              
                conn.feedbacks.Add(fb);               
                conn.SaveChanges();
                TempData["message"] = AlertMessages.show("Feedback added successfully", "Success");
                return RedirectToAction("Feedback");
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("Feedback");
            }
            
        }
        public ActionResult Quizes()
        {
            return View();
        }
        public ActionResult AboutUs()
        {
            try
            {
                AboutUsViewModel cvm = new AboutUsViewModel();
                if (Session["id"] != null)
                {
                    var id_ = Convert.ToInt32(Session["id"].ToString());
                    cvm.uzr = conn.users.SqlQuery("select * from  [user]  where [user].role!='Student' and [user].IsActive='True'").ToList();
                    cvm.corse = conn.courses.SqlQuery("select * from course ").ToList();
                    cvm.orddet = conn.order_details.SqlQuery("select * from course full outer join order_details on course.course_code = order_details.course_code left join [order] on [order].id = order_details.order_id  where [order].user_id = " + id_).ToList();

                    cvm.cartitem = conn.cart_item.SqlQuery("select * from cart_item ").ToList();
                    if (Session["id"] != null)
                    {
                        var id = Convert.ToInt32(Session["id"].ToString());
                        cvm.ord = conn.orders.SqlQuery("select * from [order] where [order].user_id = " + id).ToList();
                    }
                    return View(cvm);
                }
                else
                {
                    cvm.uzr = conn.users.SqlQuery("select * from  [user]  where [user].role!='Student' and [user].IsActive='True'").ToList();
                    cvm.corse = conn.courses.SqlQuery("select * from course ").ToList();
                    cvm.cartitem = conn.cart_item.SqlQuery("select * from cart_item ").ToList();
                    if (Session["id"] != null)
                    {
                        var id = Convert.ToInt32(Session["id"].ToString());
                        cvm.ord = conn.orders.SqlQuery("select * from [order] where [order].user_id = " + id).ToList();
                    }
                    return View(cvm);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("Index");
            }
        }
        public ActionResult search(string search)
        {
        
                try
                {
                    CourseCartIndexViewModel cvm = new CourseCartIndexViewModel();
                    if (Session["id"] != null)
                    {
                        var id_ = Convert.ToInt32(Session["id"].ToString());
                        cvm.corse = conn.courses.SqlQuery("select * from course where course_name like '" + search + "%'").ToList();
                    if(cvm.corse== null)
                    {
                        return View(cvm);
                    }
                    cvm.orddet = conn.order_details.SqlQuery("select * from course full outer join order_details on course.course_code = order_details.course_code left join [order] on [order].id = order_details.order_id  where [order].user_id = " + id_).ToList();

                        cvm.cartitem = conn.cart_item.SqlQuery("select * from cart_item ").ToList();
                        if (Session["id"] != null)
                        {
                            var id = Convert.ToInt32(Session["id"].ToString());
                            cvm.ord = conn.orders.SqlQuery("select * from [order] where [order].user_id = " + id).ToList();
                        }
                        return View(cvm);
                    }
                    else
                    {
                        cvm.corse = conn.courses.SqlQuery("select * from course where course_name like '" + search + "%'").ToList();
                    if(cvm.corse==null)
                    {
                        return View(cvm);
                    }
                        cvm.cartitem = conn.cart_item.SqlQuery("select * from cart_item ").ToList();
                        if (Session["id"] != null)
                        {
                            var id = Convert.ToInt32(Session["id"].ToString());
                            cvm.ord = conn.orders.SqlQuery("select * from [order] where [order].user_id = " + id).ToList();
                        }
                        return View(cvm);
                    }
                }
                catch (Exception ex)
                {
                    TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                    return RedirectToAction("Index");
                }
            }
        public ActionResult ContactUs()
        {
            return View();
        }
        public ActionResult compiler()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Compiler(string lang, string code)
        {
            string output = "";
            if (lang == "java")
            {
                string targetFolder = Server.MapPath("/");
                string targetPath = @Path.Combine(targetFolder, "CodeFiles\\Program.java");

                using (StreamWriter sw = new StreamWriter(targetPath))
                {
                    sw.Write(code);
                }
                Process process = new Process();
                try
                {
                  
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.Start();
                    string cmd = "javac " + targetPath + " && " + "java " + targetPath;
                   
                    process.StandardInput.WriteLine(cmd);
                    process.StandardInput.Flush();
                    process.StandardInput.Close();
         
                    output = process.StandardOutput.ReadToEnd();
                    string[]  outp = output.Split('\n');
                    var segment = new ArraySegment<string>(outp, 4, outp.Length - 4 - 2);
                    outp = segment.ToArray();
                    output = String.Join(" ", outp);
                    outp = output.Split('\r');
                    output = String.Join("<br />", outp);



                }
                catch (Exception ex)
                {
                  
                    output = process.StandardOutput.ReadToEnd();
                }
                
            }
           else if (lang == "python")
            {
                string targetFolder = Server.MapPath("/");
                string targetPath = @Path.Combine(targetFolder, "CodeFiles\\file.py");

                using (StreamWriter sw = new StreamWriter(targetPath))
                {
                    sw.Write(code);
                }
                Process process = new Process();
                try
                {
                   
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.Start();
                    string cmd = targetPath;

                    process.StandardInput.WriteLine(cmd);
                    process.StandardInput.Flush();
                    process.StandardInput.Close();

                    output = process.StandardOutput.ReadToEnd();
                    string[] outp = output.Split('\n');
                    var segment = new ArraySegment<string>(outp, 4, outp.Length - 4 - 2);
                    outp = segment.ToArray();
                    output = String.Join(" ", outp);
                    outp = output.Split('\r');
                    output = String.Join("<br />", outp);

                }
                catch (Exception ex)
                {

                    output = process.StandardOutput.ReadToEnd();
                }

            }
            else if (lang == "c++")
            {
                
                string targetFolder = Server.MapPath("/");
                string folder = @Path.Combine(targetFolder, "CodeFiles");
                string targetPath =  "cplus.cpp";
                string exePath = "a.exe";

                var s = code.Replace("$1", "<");
                s = s.Replace("$2", ">");
                code = s;

                using (StreamWriter sw = new StreamWriter(folder+"\\"+targetPath))
                {
                    sw.Write(code);
                }
                Process process = new Process();
                try
                {

                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.Start();
                    string cmd = "D: && cd " + folder + " && g++ " + targetPath + " && " + exePath;

                    //d:
                    //cd CodeHub\CodeHub\CodeFiles
                    //g++cplus.cpp && a.exe

                    process.StandardInput.WriteLine(cmd);
                    process.StandardInput.Flush();
                    process.StandardInput.Close();

                    output = process.StandardOutput.ReadToEnd();
                    string[] outp = output.Split('\n');
                    var segment = new ArraySegment<string>(outp, 4, outp.Length - 4 - 1);
                    outp = segment.ToArray();
                    output = String.Join(" ", outp);
                    outp = output.Split('\r');
                    output = String.Join("<br />", outp);

                }
                catch (Exception ex)
                {

                    output = process.StandardOutput.ReadToEnd();
                }

            }
            return Json(output, JsonRequestBehavior.AllowGet);
        }

    }
}