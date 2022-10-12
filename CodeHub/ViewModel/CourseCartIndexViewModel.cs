using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeHub.ViewModels;
using CodeHub.Models;

namespace CodeHub.ViewModel
{
    public class CourseCartIndexViewModel
    {
        public List<course> corse { get; set; }
        public List<cart_item> cartitem { get; set; }
        public List<order> ord { get; set; }

        public List<order_details> orddet { get; set; }
    }
}