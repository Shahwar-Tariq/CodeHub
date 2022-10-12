using CodeHub.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeHub.ViewModel;
using CodeHub.Models;

namespace CodeHub.Controllers
{
    public class CoursesController : Controller
    {
        // GET: Courses
        code_hubEntities7 conn = new code_hubEntities7();
        public ActionResult Index()
        {
            try
            {
                CourseCartIndexViewModel cvm = new CourseCartIndexViewModel();
                if (Session["id"] != null)
                {
                    var id_ = Convert.ToInt32(Session["id"].ToString());
                    cvm.corse = conn.courses.SqlQuery("select * from course ").ToList();
                    cvm.orddet = conn.order_details.SqlQuery("select * from course full outer join order_details on course.course_code = order_details.course_code left join [order] on [order].id = order_details.order_id  where [order].user_id = "+id_).ToList();
 
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
     
        public ActionResult GetCourse(string course_code , int? topic_id)
        {

            try
            {
            
                List<order> orde = new List<order>();
                content ctn = new content();
                if (topic_id == null)
                {
                    var cors = conn.courses.Where(x => x.course_code == course_code).FirstOrDefault();
                    List<topic> topc = conn.topics.Where(x => x.course_code == course_code).OrderBy(x => x.topic_number).ToList();
                    if (topc.Count() != 0)
                    {
                        int id = topc[0].id;
                       
                        ctn = conn.contents.Where(x => x.topic_id == id).FirstOrDefault();                    
                    }
                    if(Session["id"] != null)
                    {
                        var id = Convert.ToInt32(Session["id"].ToString());
                         orde = conn.orders.SqlQuery("select * from [order] where [order].user_id = " + id).ToList();
                    }

                    var topicContent = new ViewModel.TopicContentViewModel()
                    {
                        crs = cors,
                        tpc = topc,
                        cnt = ctn,
                        ordrr = orde,
                       
                    };
                    return View(topicContent);
                }
                else
                {
                    var cors = conn.courses.Where(x => x.course_code == course_code).Single();
                    List<topic> topc = conn.topics.Where(x => x.course_code == course_code).OrderBy(x => x.topic_number).ToList();
                    var cntnt = conn.contents.SingleOrDefault(x => x.topic_id == topic_id);
                    if (Session["id"] != null)
                    {
                        var id = Convert.ToInt32(Session["id"].ToString());
                        orde = conn.orders.SqlQuery("select * from [order] where [order].user_id = " + id).ToList();
                    }
                    var topicContent = new ViewModel.TopicContentViewModel()
                    {
                        crs = cors,
                        tpc = topc,
                        cnt = cntnt,
                        ordrr = orde,

                    };
                    return View(topicContent);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("GetCourse");
            }

        }

        [HttpPost]
        public ActionResult Favorites_course(string course_code)
        {

            favorite_courses fav = new favorite_courses();
            var id = Session["id"].ToString();
            fav.user_id = Convert.ToInt32(Session["id"].ToString());
            fav.course_code = course_code;
            var isCourseAlreadyExists = conn.favorite_courses.Any(x => x.course_code == course_code &&  x.user_id.ToString() == id );
            if (isCourseAlreadyExists)
            {
                TempData["message"] = AlertMessages.show("Course already Exists in Favorites!", "error");
                return Redirect("/CodeHub/Dashboard");
            }
            conn.favorite_courses.Add(fav);
            conn.SaveChanges();
            TempData["message"] = AlertMessages.show("Course Add To Favorites Successfully!", "success");
            return Redirect("/CodeHub/Dashboard");
        }
    }
}