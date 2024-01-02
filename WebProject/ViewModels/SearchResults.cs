using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Models;

namespace WebProject.ViewModels
{
    public class SearchResults
    {
        public string Query { get; set; }
        public List<Movie> Movies { get; set; }
    }
}