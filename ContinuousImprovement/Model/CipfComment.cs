﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace ContinuousImprovement.Model
{
    public partial class CipfComment
    {
        public int Id { get; set; }
        public string SuggestionId { get; set; }
        public string UserComment { get; set; }
        public string Comment { get; set; }
        public DateTime DateComment { get; set; }
        public DateTime SubmitSuggestionDt { get; set; }
    }
}