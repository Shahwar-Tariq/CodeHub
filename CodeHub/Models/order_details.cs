//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CodeHub.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class order_details
    {
        public int id { get; set; }
        public Nullable<int> order_id { get; set; }
        public string course_code { get; set; }
        public Nullable<double> price { get; set; }
    
        public virtual course course { get; set; }
        public virtual order order { get; set; }
    }
}