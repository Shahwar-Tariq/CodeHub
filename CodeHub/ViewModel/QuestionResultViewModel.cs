using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeHub.ViewModels
{
    public class QuestionResultViewModel
    {
        public List<QuizResult> results = new List<QuizResult>();
        public int correct;
        public int wrong;
    }
}