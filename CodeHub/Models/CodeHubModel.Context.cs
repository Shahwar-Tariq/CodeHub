﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class code_hubEntities7 : DbContext
    {
        public code_hubEntities7()
            : base("name=code_hubEntities7")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AssignCourse> AssignCourses { get; set; }
        public virtual DbSet<cart> carts { get; set; }
        public virtual DbSet<cart_item> cart_item { get; set; }
        public virtual DbSet<content> contents { get; set; }
        public virtual DbSet<course> courses { get; set; }
        public virtual DbSet<favorite_courses> favorite_courses { get; set; }
        public virtual DbSet<feedback> feedbacks { get; set; }
        public virtual DbSet<feedback_reply> feedback_reply { get; set; }
        public virtual DbSet<order> orders { get; set; }
        public virtual DbSet<order_details> order_details { get; set; }
        public virtual DbSet<question> questions { get; set; }
        public virtual DbSet<quiz> quizs { get; set; }
        public virtual DbSet<topic> topics { get; set; }
        public virtual DbSet<user> users { get; set; }
    }
}