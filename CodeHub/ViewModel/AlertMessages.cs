using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CodeHub.ViewModel
{
    public class AlertMessages
    {
        public static string show(string message, string type)
        {
            string alert = "";
            if(type== "error")
            {
                alert = "<div class='alert alert-danger' role='alert'>"+message+"</div>";
            }
            else
            {
                alert = "<div class='alert alert-success' role='alert'>" + message + "</div>";
            }
            return alert;
        }
    }
}