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
    
    public partial class feedback
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public feedback()
        {
            this.feedback_reply = new HashSet<feedback_reply>();
        }
    
        public int id { get; set; }
        public Nullable<int> send_by { get; set; }
        public string details { get; set; }
        public string feedbackfor { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<feedback_reply> feedback_reply { get; set; }
        public virtual user user { get; set; }
    }
}