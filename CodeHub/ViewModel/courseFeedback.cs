using CodeHub.Models;
using CodeHub.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeHub.ViewModel
{
    public class courseFeedback
    {
        public List<course> crs { get; set; }
        public List<feedback> fbs { get; set; }
        public List<feedback_reply> fbsr { get; set; }
        public List<feedback_reply> fbr { get; set; }

        public List<user> urs { get; set; }

    }
}