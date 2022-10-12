using CodeHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeHub.ViewModel
{
    public class AboutUsViewModel
    {
        public List<course> corse { get; set; }
        public List<cart_item> cartitem { get; set; }
        public List<order> ord { get; set; }
        public List<order_details> orddet { get; set; }
        public List<user> uzr { get; set; }
    }
}