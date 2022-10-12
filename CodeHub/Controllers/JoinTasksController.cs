using CodeHub.ViewModels;
using CodeHub.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CodeHub.Models;

namespace CodeHub.Controllers
{
    public class JoinTasksController : Controller
    {
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
        public ActionResult AddTopic()
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    if (Session["role"].ToString() == "Expert")
                    {
                        String id = Session["id"].ToString();
                        AssignCourse query = conn.AssignCourses.SingleOrDefault(x => x.user_id.ToString() == id);
                        if (query == null)
                        {
                            TempData["message"] = AlertMessages.show("Course is not Assign to you yet! ", "error");       
                            return Redirect("/Expert/Index");
                        }
                        try
                        {
                            course c = new course();
                            c.course_name = query.course.course_name;
                            c.course_code = query.course.course_code;
                            List<course> crs = new List<course>();
                            crs.Add(c);
                            return View(crs);
                        }
                        catch(Exception ex)
                        {
                            TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                            return RedirectToAction("AddTopic");
                        }
                    }
                    else
                    {
                        //link query
                        try
                        {
                            return View(from course in conn.courses select course);
                        }
                        catch(Exception ex)
                        {
                            TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                            return RedirectToAction("AddTopic");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                 TempData["message"] = AlertMessages.show("Please Login"+ ex.Message, "error");                        
                return Redirect("/CodeHub/Index");
            }

        }
        [HttpPost]
        public ActionResult AddTopic(string course_code, string topic_name, int topic_num = 0) //optional arrgument
        {
            topic t = new topic();
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                t.course_code = course_code;
                t.topic_number = topic_num;
                if(topic_name!="")
                {
                    t.topic_name = topic_name;
                }
               else
                {
                    TempData["message"] = AlertMessages.show("Please Enter Topic Name! ", "error");
                    return RedirectToAction("AddTopic");
                }
                var isTopicNameAlreadyExists = conn.topics.Any(x => x.topic_name == topic_name && x.course_code == course_code);
                if (isTopicNameAlreadyExists)
                {
                    TempData["message"] = AlertMessages.show("Topic name cannot be same", "error");
                    return RedirectToAction("AddTopic");
                }
                t.user_id = Convert.ToInt32(Session["id"].ToString());
                conn.topics.Add(t);
                conn.SaveChanges();
                TempData["message"] = AlertMessages.show("Topic Added Successfully", "Success");
                return RedirectToAction("AddTopic");
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("AddTopic");
            }            
        }
        public ActionResult ViewTopics()
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    if (Session["role"].ToString() == "Expert")
                    {
                        String id = Session["id"].ToString();
                        AssignCourse query = conn.AssignCourses.SingleOrDefault(x => x.user_id.ToString() == id);
                            if (query == null)
                            {
                            TempData["message"] = AlertMessages.show("Course is not Assign to you yet! ", "error");
                            return Redirect("/Expert/Index");
                        }
                        IEnumerable<topic> t = new List<topic>();
                        t = query.course.topics;
                        return View(t);
                    }
                    else
                    {
                        try
                        {
                            List<topic> t = conn.topics.SqlQuery("select * from topic inner join course on topic.course_code = course.course_code").ToList();
                            return View(t);
                        }
                        catch(Exception ex)
                        {
                            TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                            return RedirectToAction("ViewTopic");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewTopics");
            }
            
        }
        public ActionResult EditTopics(int id)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    try
                    {
                        topic t = conn.topics.Single(td => td.id == id);
                        List<course> c = (from course in conn.courses select course).ToList();
                        ViewBag.courses = c;
                        return View(t);
                    }
                    catch(Exception ex)
                    {
                        TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again. " + ex.Message, "error");
                        return RedirectToAction("ViewTopic");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again. " + ex.Message, "error");
                return RedirectToAction("ViewTopic");
            }
           
        }
        [HttpPost]
        public ActionResult EditTopics(int id, string course_code, int topic_num, string topic_name)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    topic t = conn.topics.Single(td => td.id == id);
                    t.course_code = course_code;
                    t.topic_number = topic_num;

                    try
                    {
                        t.topic_name = topic_name;
                        var isTopicNameAlreadyExists = conn.topics.Any(x => x.topic_name == topic_name && x.course_code == course_code);
                        if (isTopicNameAlreadyExists)
                        {
                            TempData["message"] = AlertMessages.show("Topic name cannot be same", "error");
                            return Redirect("/JoinTasks/EditTopics?id="+id);
                        }
                    }
                    catch (Exception ex)
                    {
                        TempData["message"] = AlertMessages.show("Something went wrong! Please Try Again." + ex.Message, "error");
                        return Redirect("/JoinTasks/EditTopics?id="+id);
                    }
                    t.user_id = Convert.ToInt32(Session["id"].ToString());           
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Topic Updated Successfully", "Success");
                    return RedirectToAction("ViewTopics");

                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewTopics");
            }
            
        }
        public ActionResult DeleteTopic(int id)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    topic t = conn.topics.Single(td => td.id == id);
                    conn.topics.Remove(t);
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Topic Deleted Successfully!", "success");
                    return RedirectToAction("ViewTopics");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewTopics");
            }
            
        }
        public ActionResult AddContent()
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    if (Session["role"].ToString() == "Expert")
                    {
                        String id = Session["id"].ToString();
                        AssignCourse query = conn.AssignCourses.SingleOrDefault(x => x.user_id.ToString() == id);
                        if (query == null)
                        {
                            TempData["message"] = AlertMessages.show("Course is not Assign to you yet! ", "error");
                            return Redirect("/Expert/Index");
                        }
                        course c = new course();
                        c.course_name = query.course.course_name;
                        c.course_code = query.course.course_code;
                        List<course> crs = new List<course>();
                        crs.Add(c);
                        return View(crs);
                    }
                    else
                    {
                        List<course> c = (from course in conn.courses select course).ToList();  
                        return View(c);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return View();
            }
        }

        public JsonResult GetTopic(string course_code)
        {
            List<topic> t = new List<topic>();
            String response = "";
            int id = 0;
            try
            {
                t = (from topic in conn.topics where topic.course_code == course_code select topic).ToList();
                id = t[0].id;
            }
            catch (Exception ex)
            {

                response = "<option value = '0'>No Topic Exists!</option>";
                String[] ss = new string[] { response };
                return Json(ss, JsonRequestBehavior.AllowGet);
            }

            foreach (var a in t)
            {
                response += "<option value=" + a.id + ">" + a.topic_name + "</option>";
            }

            String[] s = new string[] { response };
            return Json(s, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetQuizTopic(string course_code)
        {
            List<topic> t = new List<topic>();
            String response = "";
            String response1 = "";
            int id = 0;
            try
            {
                t = (from topic in conn.topics where topic.course_code == course_code select topic).ToList();
                id = t[0].id;
            }
            catch (Exception ex)
            {

                response = "<option value = '0'>No Topic Exists!</option>";
                response1 = "<option value = '0'>No quiz Exists!</option>";
                String[] ss = new string[] { response, response1 };
                return Json(ss, JsonRequestBehavior.AllowGet);
            }
            List<quiz> q = (from quiz in conn.quizs where quiz.topic_id == id select quiz).ToList();
            if (q.Count == 0)
            {
                response1 = "<option value = '0'>No Quiz Exists!</option>";
            }

            //iss ko yahan iss liye likha hy taky page reload na ho or html bana k reponse k pas ajax main bhejain gy
            foreach (var a in t)
            {
                response += "<option value=" + a.id + ">" + a.topic_name + "</option>";
            }
            foreach (var a in q)
            {
                response1 += "<option value=" + a.id + ">" + a.quiz_name + "</option>";
            }
            String[] s = new string[] { response, response1 };
            return Json(s, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddContent(string course_code, int topic_id, string editor, HttpPostedFileBase fileupload)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    content ct = new content();
                    ct.course_code = course_code;
                        ct.topic_id = topic_id;
                        var isContentAlreadyExists = conn.contents.Any(x => x.topic_id == topic_id);
                        if (isContentAlreadyExists)
                        {
                            TempData["message"] = AlertMessages.show("Content Already Exists in this topic!", "error");
                            return RedirectToAction("AddContent");
                        }
                    if(editor!="")
                    {
                        ct.content_description = editor;
                    }
                    else
                    {
                        TempData["message"] = AlertMessages.show("Please Enter Content!", "error");
                        return RedirectToAction("AddContent");
                    }

                    ct.user_id = Convert.ToInt32(Session["id"].ToString());

                    if (fileupload != null)
                    {

                        string fileName = Path.GetFileName(fileupload.FileName);
                        var isVideoAlreadyExists = conn.contents.Any(x => x.videoname == fileName);
                        if (isVideoAlreadyExists)
                        {
                            TempData["message"] = AlertMessages.show("Video name cannot be same", "error");
                            return RedirectToAction("AddContent");
                        }
                        int fileSize = fileupload.ContentLength;
                        int Size = fileSize / 1000;
                        fileupload.SaveAs(Server.MapPath("~/VideoFileUpload/" + fileName));

                        ct.videoname = fileName;
                        ct.filesize = fileSize;
                    }
                    conn.contents.Add(ct);
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Content added succesfully", "Success");
                    return RedirectToAction("AddContent");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("AddContent");
            }     
        }
        public ActionResult ViewContent()
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    if (Session["role"].ToString() == "Expert")
                    {
                        String id = Session["id"].ToString();
                        AssignCourse query = conn.AssignCourses.SingleOrDefault(x => x.user_id.ToString() == id); // where AssignCourse.user_id.ToString() == Session["id"].ToString() select AssignCourse).ToList().First();
                        if (query == null)
                        {
                            TempData["message"] = AlertMessages.show("Course is not Assign to you yet! ", "error");
                            return Redirect("/Expert/Index");
                        }
                        List<content> content = (from t in conn.contents where query.course_code == t.course_code select t).ToList();
                        return View(content);
                    }
                    else
                    {
                        List<content> t = conn.contents.SqlQuery("select * from content inner join course on content.course_code = course.course_code").ToList();
                        return View(t);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewContent");
            }
        
        }
        public ActionResult EditContent(int id)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    content ct = conn.contents.Single(td => td.id == id);
                    List<course> c = (from course in conn.courses select course).ToList();
                    ViewBag.courses = c;
                    List<topic> t = (from topic in conn.topics select topic).ToList();
                    ViewBag.topics = t;
                    return View(ct);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewContent");
            }
           
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditContent(int id, string course_code, int topic_id, string editor, HttpPostedFileBase fileupload)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    content ct = conn.contents.Single(td => td.id == id);
                    ct.course_code = course_code;
                    ct.topic_id = topic_id;
                   
                    if (editor != "")
                    {
                        ct.content_description = editor;
                    }
                    else
                    {
                        TempData["message"] = AlertMessages.show("Please Enter Content!", "error");
                        return Redirect("/JoinTasks/EditContent?id="+id);
                    }
                    if (fileupload != null)
                    {
                        string fileName = Path.GetFileName(fileupload.FileName);
                        var isVideoAlreadyExists = conn.contents.Any(x => x.videoname == fileName);
                        if (isVideoAlreadyExists)
                        {
                            TempData["message"] = AlertMessages.show("Video name cannot be same", "error");
                            return Redirect("/JoinTasks/EditContent?id="+id);
                        }
                        int fileSize = fileupload.ContentLength;
                        int Size = fileSize / 1000;
                        fileupload.SaveAs(Server.MapPath("~/VideoFileUpload/" + fileName));

                        ct.videoname = fileName;
                        ct.filesize = fileSize;
                    }
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Content updated successfully", "Success");
                    return RedirectToAction("ViewContent");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewContent");
            }
           
        }
        public JsonResult RemoveVideo(string videoName)
        {
            try
            {
                if (Session["id"] == null)
                {
                    string s = AlertMessages.show("Session expire!", "error");
                    return Json(s, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    content ct = conn.contents.Single(td => td.videoname == videoName);
                    ct.videoname = "";
                    conn.SaveChanges();
                  string s = AlertMessages.show("Video Removed Successfully", "Success");
                    return Json(s, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                string s = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return Json(s, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult DeleteContent(int id)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    content ct = conn.contents.Single(td => td.id == id);
                    conn.contents.Remove(ct);
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Content Deleted Successfully!", "success");
                    return RedirectToAction("ViewContent");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewContent");
            }
            
        }
        public String GetQuiz(int topic_id)
        {

            var q = (from quiz in conn.quizs where quiz.topic_id == topic_id select quiz).ToList();
            String response = "";
            if (q.Count == 0)
            {
                response = "<option value = '0'>No Quiz Exist</option>";
            }
            foreach (var a in q)
            {
                response += "<option value=" + a.id + ">" + a.quiz_name + "</option>";
            }


            return response;
        }
        public ActionResult AddQuizzes()
        {

            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    if (Session["role"].ToString() == "Expert")
                    {
                        String id = Session["id"].ToString();
                        AssignCourse query = conn.AssignCourses.SingleOrDefault(x => x.user_id.ToString() == id);
                        if (query == null)
                        {
                            TempData["message"] = AlertMessages.show("Course is not Assign to you yet! ", "error");
                            return Redirect("/Expert/Index");
                        }
                        List<quiz> quiz = (from q in conn.quizs where query.course_code == q.course_code select q).ToList();
                        return View(quiz);
                    }
                    else
                    {
                        List<course> c = (from course in conn.courses select course).ToList();
                        return View(c);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something went wrong! Please Try Again. " + ex.Message, "error");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddQuizzes(string course_code, int topic_id, short quiz_no, string quiz_name)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    quiz q = new quiz();
                    q.course_code = course_code;
                    q.topic_id = topic_id;
                    var isQuizAlreadyExists = conn.quizs.Any(x => x.topic_id == topic_id && x.course_code == course_code);
                    if (isQuizAlreadyExists)
                    {
                        TempData["message"] = AlertMessages.show("Quiz Already Exists in This Topics!", "error");
                        return RedirectToAction("AddQuizzes");
                    }
                    q.quiz_number = quiz_no;

                    if (quiz_name != "")
                    {
                        q.quiz_name = quiz_name;
                    }
                    else
                    {
                        TempData["message"] = AlertMessages.show("please Enter Quiz Name", "error");
                        return RedirectToAction("AddQuizzes");
                    }
                    q.user_id = Convert.ToInt32(Session["id"].ToString());
                    conn.quizs.Add(q);
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Quiz Added Successfully", "Success");
                    return RedirectToAction("AddQuizzes");
                }

            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("AddQuizzes");
            }
           
        }
     
        public ActionResult ViewQuizzes()
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    List<quiz> t = conn.quizs.SqlQuery("select * from quiz inner join course on quiz.course_code = course.course_code").ToList();
                    return View(t);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewQuizzes");
            }
           
        }
        public ActionResult EditQuizzes(int id)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    quiz q = conn.quizs.Single(td => td.id == id);
                    List<course> c = (from course in conn.courses select course).ToList();
                    ViewBag.courses = c;
                    List<topic> t = (from topic in conn.topics select topic).ToList();
                    ViewBag.topics = t;
                    return View(q);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewQuizzes");
            }
           
        }
        [HttpPost]
        public ActionResult EditQuizzes(int id, string course_code, int topic_id, short quiz_no, string quiz_name)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    quiz q = conn.quizs.Single(td => td.id == id);
                    q.course_code = course_code;
                    q.topic_id = topic_id;
                    q.quiz_number = quiz_no;
                    if (quiz_name != "")
                    {
                        q.quiz_name = quiz_name;
                    }
                    else
                    {
                        TempData["message"] = AlertMessages.show("please Enter Quiz Name", "error");
                        return Redirect("/JoinTasks/EditQuizzes?id"+id);
                    }
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Quiz Updated Successfully", "Success");
                    return RedirectToAction("ViewQuizzes");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewQuizzes");
            }
            
        }
       
        public ActionResult DeleteQuizzes(int id)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    quiz q = conn.quizs.Single(td => td.id == id);
                    conn.quizs.Remove(q);
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Quiz Deleted Successfully!", "success");
                    return RedirectToAction("ViewQuizzes");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewQuizzes");
            }
            
        }
        public ActionResult AddQuestions()
        {

            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    if (Session["role"].ToString() == "Expert")
                    {
                        String id = Session["id"].ToString();
                        AssignCourse query = conn.AssignCourses.SingleOrDefault(x => x.user_id.ToString() == id);
                        if (query == null)
                        {
                            TempData["message"] = AlertMessages.show("Course is not Assign to you yet! ", "error");
                            return Redirect("/Expert/Index");
                        }
                        List<quiz> quiz = (from q in conn.quizs where query.course_code == q.course_code select q).ToList();
                        return View(quiz);
                    }
                    else
                    {
                        List<course> c = (from course in conn.courses select course).ToList();
                        return View(c);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return View();
            }         
        }
        [HttpPost]  
        public ActionResult AddQuestions( int quiz_id, string question_name, string option1, string option2, string option3, string option4, int answers)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    question que = new question();
                    que.quiz_id = quiz_id;
                    if (question_name != "")
                    {
                        que.question_name = question_name;
                    }
                    else
                    {
                        TempData["message"] = AlertMessages.show("Please Enter Question Name!", "error");
                        return RedirectToAction("AddQuestions");
                    }
                    if(option1!="" || option2!="" || option3!="" || option4!="")
                    { 
                        que.option1 = option1;
                        que.option2 = option2;
                        que.option3 = option3;
                        que.option4 = option4;
                    }
                    else
                    {
                        TempData["message"] = AlertMessages.show("Options can not be Empty!", "error");
                        return RedirectToAction("AddQuestions");
                    }
                    string[] options = { option1, option2, option3, option4 };
                    que.answer = options[answers];
                    conn.questions.Add(que);
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Question Added successfully!", "Success");
                    return RedirectToAction("AddQuestions");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("AddQuestions");
            }         
        }
        public ActionResult ViewQuestions()
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    List<question> q = conn.questions.SqlQuery("select * from question inner join quiz on question.quiz_id = quiz.id").ToList();
                    return View(q);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewQuestions");
            }           
        }
        public ActionResult DeleteQuestion(int id)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    question que = conn.questions.Single(td => td.id == id);
                    conn.questions.Remove(que);
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Question Deleted Successfully!", "Success");
                    return RedirectToAction("ViewQuestions");
                }
             }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("ViewQuestions");
            }         
        }
        public ActionResult Profile(int id)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                   user u = conn.users.Single(td => td.user_id == id);
                    return View(u);
                    //return View(from user in conn.users select user);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("Profile");
            }          
        }
        [HttpPost]
        public ActionResult Profile(int id, string first_name, string last_name, string email, string password, HttpPostedFileBase img, string skills)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    user u = conn.users.Single(td => td.user_id == id);
                    u.first_name = first_name;
                    u.last_name = last_name;
                    u.email = email;
                    u.password = password;
                    u.skills = skills;

                    if (img != null)
                    {
                        u.image_path = img.ToString();
                        string ImageName = Path.GetFileName(img.FileName);
                        var isImageAlreadyExists = conn.courses.Any(x => x.image == ImageName);
                        if (isImageAlreadyExists)
                        {
                            TempData["message"] = AlertMessages.show("Image already Exists,Please Rename Your Image!", "error");
                            return Redirect("/JoinTasks/Profile?id=" + id);
                        }
                        img.SaveAs(Server.MapPath("~/Images/profile_images/" + ImageName));
                        u.image_path = ImageName;
                    }
                 else
                    {

                    }
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Profile Updated", "success");
                    return Redirect("/JoinTasks/Profile?id="+id);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return Redirect("/JoinTasks/Profile?id="+id);
            }            
        }

        public JsonResult Remove_Profile_Image(int id)
        {
            try
            {
                user u = conn.users.Single(td => td.user_id == id);
                u.image_path = "";
                conn.SaveChanges();
                string s = AlertMessages.show("Profile Image Removed Successfully", "Success");
                return Json(s, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                string s = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return Json(s, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult Feedback()
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    if (Session["role"].ToString() == "Expert")
                    {
                        List<feedback> fb = conn.feedbacks.SqlQuery("select * from feedback left join [user] on feedback.feedbackfor = [user].skills where feedback.feedbackfor ='"+Session["skills"].ToString()+"'").ToList();
                        return View(fb);
                    }
                    else
                    {
                        List<feedback> fb = conn.feedbacks.SqlQuery("select * from feedback").ToList();
                        return View(fb);
                    }
                }
            }
            catch(Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("Feedback");
            }
        }
        public ActionResult ReplyFeedbacks(int feedback_id)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    feedback fbr = conn.feedbacks.Single(td => td.id == feedback_id);
                    return View(fbr);
                }
            }
            catch(Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return Redirect("/JoinTasks/ReplyFeedbacks?feedback_id=" + feedback_id);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ReplyFeedbacks(int feedback_id,string editor)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    feedback_reply fbr = new feedback_reply();
                    fbr.replied_by = Convert.ToInt32(Session["id"].ToString());
                    fbr.feedback_id = feedback_id;
                    if (editor != "")
                    {
                        fbr.reply = editor;
                    }
                    else
                    {
                        TempData["message"] = AlertMessages.show("Please Enter Something!", "error");
                        return Redirect("/JoinTasks/ReplyFeedbacks?feedback_id=" + feedback_id);
                    }
                    conn.feedback_reply.Add(fbr);
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Feedback Replied successfully", "Success");
                    return RedirectToAction("Feedback");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return Redirect("/JoinTasks/ReplyFeedbacks?feedback_id=" + feedback_id);
            }
        }
        public ActionResult DeleteFeedback(int id)
        {
            try
            {
                if (Session["id"] == null)
                {
                    return Redirect("/Authentication/Login");
                }
                else
                {
                    feedback fb = conn.feedbacks.Single(utd => utd.id == id);
                    conn.feedbacks.Remove(fb);
                    conn.SaveChanges();
                    TempData["message"] = AlertMessages.show("Feedback deleted successfully" , "Success");
                    return RedirectToAction("Feedback");
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("Feedback");
            }
            
        }
    }
}