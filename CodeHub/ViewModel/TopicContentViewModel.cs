using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeHub.ViewModels;
using CodeHub.Models;

namespace CodeHub.ViewModel
{
    public class TopicContentViewModel
    {
        public int next = -1;
        public int pre = -1;
        public List<topic> tpc { get; set; }
        public course crs { get; set; }
        public content cnt { get; set; }
        public question qs { get; set; }
        public order ord { get; set; }
         
        public List<order> ordrr { get; set; }

    }
}