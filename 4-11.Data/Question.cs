using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4_11.Data
{
    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime DatePosted { get; set; }
        public List<QuestionTag> QuestionTag { get; set; }
        public List<Like> Likes { get; set; }
        public List<Answer> Answers { get; set; }
        public int UserId { get; set; }
    }
}
