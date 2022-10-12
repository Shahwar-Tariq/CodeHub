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
    public class QuizController : Controller
    {

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
        public ActionResult ReadQuiz(string course_code, int? topic_id)
        {
            try
            {
                List<order> orde = new List<order>();
                List<question> que = new List<question>();
                quiz quz = new quiz();
                if (topic_id == null)
                {

                    var cors = conn.courses.Where(x => x.course_code == course_code).FirstOrDefault();
                    List<topic> topc = conn.topics.Where(x => x.course_code == course_code).OrderBy(x => x.topic_number).ToList();
                    if (topc.Count() != 0)
                    {
                        int id = topc[0].id;
                        quz = conn.quizs.Where(x => x.topic_id == id).FirstOrDefault();
                        if (quz != null)
                        {
                            int quzid = quz.id;
                            que = conn.questions.Where(x => x.quiz_id == quzid).ToList();
                        }
                        else
                        {
                            TempData["message"] = AlertMessages.show("There is no quiz!", "error");
                            return RedirectToAction("Index");
                        }
                    }
                    if (Session["id"] != null)
                    {
                        var id = Convert.ToInt32(Session["id"].ToString());
                        orde = conn.orders.SqlQuery("select * from [order] where [order].user_id = " + id).ToList();
                    }

                    var topicContent = new ViewModel.courseQuizViewModel()
                    {
                        crs = cors,
                        tpc = topc,
                        qz = quz,
                        que = que,
                        ordrr = orde,

                    };
                    return View(topicContent);
                }
                else
                {
                    var cors = conn.courses.Where(x => x.course_code == course_code).FirstOrDefault();
                    List<topic> topc = conn.topics.Where(x => x.course_code == course_code).OrderBy(x => x.topic_number).ToList();
                    var qus = conn.quizs.Where(x => x.topic_id == topic_id).SingleOrDefault();

                    quz = conn.quizs.Where(x => x.topic_id == topic_id).FirstOrDefault();
                    if (quz != null)
                    {
                        int quzid = quz.id;
                        que = conn.questions.Where(x => x.quiz_id == quzid).ToList();
                    }
                    else
                    {
                        quz = new quiz();
                    }
                    if (Session["id"] != null)
                    {
                        var id = Convert.ToInt32(Session["id"].ToString());
                        orde = conn.orders.SqlQuery("select * from [order] where [order].user_id = " + id).ToList();
                    }
                    var topicContent = new ViewModel.courseQuizViewModel()
                    {
                        crs = cors,
                        tpc = topc,
                        qz = quz,
                        que = que,
                        ordrr = orde,


                    };
                    return View(topicContent);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                 return RedirectToAction("Index");
            }
        }
        public ActionResult Result(int[] question, string[] answers)
        {
            try {
                List<QuizResult> res = new List<QuizResult>();
                int true_answers = 0;
                int false_answers = 0;
                for (int i = 0; i < question.Length; i++)
                {
                    int qid = question[i];
                    question que = conn.questions.Single(td => td.id == qid);
                    if (answers[i] == que.answer)
                    {
                        true_answers += 1;

                    }
                    else
                    {
                        false_answers += 1;
                    }
                    QuizResult r = new QuizResult();
                    r.question_name = que.question_name;
                    r.correct_option = que.answer;
                    r.selected_option = answers[i];
                    res.Add(r);

                }
                QuestionResultViewModel que_res = new QuestionResultViewModel();
                que_res.correct = true_answers;
                que_res.wrong = false_answers;
                que_res.results = res;
                return View(que_res);
            }
            catch (Exception ex)
            {
                TempData["message"] = AlertMessages.show("Something Went Wrong! Please Try Again." + ex.Message, "error");
                return RedirectToAction("Result");
            }
        }
    }
}