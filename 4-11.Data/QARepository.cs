using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _4_11.Data
{
    public class QARepository
    {
        private readonly string _connectionString;

        public QARepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<Question> GetQuestions()
        {
            using var context = new QADataContext(_connectionString);
            return context.Questions.Include(q=>q.Answers).Include(q=>q.Likes).Include(q => q.QuestionTag).ThenInclude(qt => qt.Tag).ToList();          
        }

        public List<User> GetUsers()
        {
            using var context = new QADataContext(_connectionString);
            return context.Users.Include(u=>u.Questions).ToList();
        }


        public Question GetQuestionById(int id)
        {
            using var context = new QADataContext(_connectionString);
            return context.Questions.Include(q => q.Answers).Include(q=>q.Likes).Include(q => q.QuestionTag).ThenInclude(qt => qt.Tag).FirstOrDefault(q => q.Id == id);
        }
        public List<Like> GetLikesByQuestion(int id)
        {
            using var context = new QADataContext(_connectionString);
            return context.Likes.Where(l=>l.QuestionId==id).ToList();
        }
        private Tag GetTag(string name)
        {
            using var ctx = new QADataContext(_connectionString);
            return ctx.Tags.FirstOrDefault(t => t.Name == name);
        }
        private int AddTag(string name)
        {
            using var ctx = new QADataContext(_connectionString);
            var tag = new Tag { Name = name };
            ctx.Tags.Add(tag);
            ctx.SaveChanges();
            return tag.Id;
        }

        public void AddQuestion(Question question, IEnumerable<string> tags)
        {
            using var context = new QADataContext(_connectionString);
            context.Questions.Add(question);
            context.SaveChanges();
            foreach (string tag in tags)
            {
                Tag t = GetTag(tag);
                int tagId;
                if (t == null)
                {
                    tagId = AddTag(tag);
                }
                else
                {
                    tagId = t.Id;
                }
                context.QuestionTags.Add(new QuestionTag
                {
                    QuestionId = question.Id,
                    TagId = tagId
                });
            }
            context.SaveChanges();

        }
        public void AddTagsForQuestion(Question question, List<Tag> tags)
        {
            using var context = new QADataContext(_connectionString);
            foreach(Tag t in tags)
            {
                context.Tags.Add(t);
                QuestionTag qt = new();
                qt.Question = question;
                qt.QuestionId = question.Id;
                qt.Tag = t;
                qt.TagId = t.Id;
                context.QuestionTags.Add(qt);

            }
        }
        public void AddAnswer(Answer answer)
        {
            using var context = new QADataContext(_connectionString);
            context.Answers.Add(answer);
            context.SaveChanges();
            context.Questions.FirstOrDefault(q => q.Id == answer.QuestionId).Answers.Add(answer);
            context.SaveChanges();
        }
        public void AddUser(User user, string password)
        {
            using var context = new QADataContext(_connectionString);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            context.Users.Add(user);
            context.SaveChanges();
        }
        public User Login(string email, string password)
        {
            var user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isValid ? user : null;

        }
        public User GetByEmail(string email)
        {
            using var context = new QADataContext(_connectionString);
            return context.Users.FirstOrDefault(u => u.Email == email);         
        }
        public void AddLike(Like like)
        {
            using var context = new QADataContext(_connectionString);
            //context.Questions.Attach(question);
            //context.Entry(question).State = EntityState.Modified;
            context.Likes.Add(like);
            context.SaveChanges();
        }
    }
}