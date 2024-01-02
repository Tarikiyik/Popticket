using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Models;

namespace WebProject.ViewModels
{
    public class Homepage
    {
        public List<Movie> OnTheaters { get; set; }
        public List<Movie> Upcoming { get; set; }
        public List<Movie> Featured { get; set; }
    }
}