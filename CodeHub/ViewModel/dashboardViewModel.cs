using CodeHub.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeHub.ViewModel
{
    public class dashboardViewModel
    {
        public List<course> crs { get; set; }
        public List<favorite_courses> fav_crs { get; set; }
    }
}