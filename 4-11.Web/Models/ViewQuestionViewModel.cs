using _4_11.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _4_11.Web.Models
{
    public class ViewQuestionViewModel
    {
        public Question Question { get; set; }
        public List<User> Users { get; set; }
        public bool IsAuthenticated { get; set; }
        public List<Like> Likes { get; set; }
        public User CurrentUser { get; set; }

    }
}
