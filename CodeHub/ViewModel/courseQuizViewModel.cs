using CodeHub.Models;
using CodeHub.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeHub.ViewModel
{
    public class courseQuizViewModel
    {
        public List<question> que { get; set; }
        public course crs { get; set; }
        public List<topic> tpc { get; set; }
        public quiz qz { get; set; }
        public order ord { get; set; }

        public List<order> ordrr { get; set; }
    }
}