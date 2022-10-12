using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeHub.ViewModels;
using CodeHub.Models;

namespace CodeHub.ViewModel
{
    public class UserCourseViewModel
    {
        public List<course> crs = new List<course>();
        public List<user> usr = new List<user>();
        public AssignCourse assigncrs = new AssignCourse();
        public course crses = new course();
    }
}