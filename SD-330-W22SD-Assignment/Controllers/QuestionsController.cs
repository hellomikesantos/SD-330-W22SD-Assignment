using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_330_W22SD_Assignment.Data;
using SD_330_W22SD_Assignment.Models;
using SD_330_W22SD_Assignment.Models.ViewModels;

namespace SD_330_W22SD_Assignment.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestionsController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Questions
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Question.Include(q => q.User)
                .Include(q => q.Votes)
                .Include(q => q.User)
                    .ThenInclude(u => u.Reputation)
                .Include(q => q.Answers)
                .OrderByDescending(q => q.Id);
            
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Index(int? questionId, string? vote)
        {
            if(questionId != null)
            {
                try
                {
                    Question question = await _context.Question
                        .Include(q => q.Votes)
                            .ThenInclude(v => v.User)
                        .Include(q => q.User)
                            .ThenInclude(u => u.Reputation)
                        .FirstAsync(q => q.Id == questionId);

                    string userName = User.Identity.Name;
                    ApplicationUser user = await _context.Users
                        .Include(u => u.Votes)
                        .FirstAsync(q => q.UserName == userName);

                    Vote newVote = new Vote();

                    if (question.User.Id == user.Id)
                    {
                        switch (vote)
                        {
                            case "up":
                                if (question.Votes.Any(v => v.UserId == user.Id && v.UpVote))
                                {
                                    return RedirectToAction("Index");
                                }
                                break;
                            case "down":
                                if (question.Votes.Any(v => v.UserId == user.Id && v.DownVote))
                                {
                                    return RedirectToAction("Index");
                                }
                            break;
                        };
                    }
                    else if(question.User.Id != user.Id)
                    {
                        switch (vote)
                        {
                            case "up":
                                if (question.Votes.Any(v => v.User.Id == user.Id && v.UpVote))
                                {
                                    return RedirectToAction("Index");
                                }
                                newVote.UpVote = true;
                                foreach(Vote v in question.Votes)
                                {
                                    if (v.User.Id == user.Id && v.DownVote)
                                    {
                                        question.Votes.Remove(v);
                                    }
                                }

                                break;
                            case "down":
                                if (question.Votes.Any(v => v.User.Id == user.Id && v.DownVote))
                                {
                                    return RedirectToAction("Index");
                                }
                                newVote.DownVote = true;
                                foreach (Vote v in question.Votes)
                                {
                                    if (v.User.Id == user.Id && v.UpVote)
                                    {
                                        question.Votes.Remove(v);
                                    }
                                }

                                break;
                        };

                        

                        newVote.User = user;
                        newVote.UserId = user.Id;

                        question.Votes.Add(newVote);
                        user.Votes.Add(newVote);

                        question.User.Reputation.Score = 5 * (question.Votes
                            .Where(v => v.UpVote).Count() - question.Votes
                            .Where(v => v.DownVote).Count());
                        _context.SaveChanges();
                    }
                }
                catch
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index");
            
        }

 
        public IActionResult PostQuestion()
        {
            Question question = new Question();
            return View(question);
        }

        [HttpPost]
        public async Task<IActionResult> PostQuestion(string? title, string? body, string? tagsString)
        {
            if (title != null && body != null)
            {
                try
                {
                    Question q = new Question();
                    q.Title = title;
                    q.Body = body;
                    q.CreatedDate = DateTime.Today;

                    string userName = User.Identity.Name;

                    ApplicationUser user = await _context.Users.FirstAsync(u => u.UserName == userName);
                    q.User = user;
                    q.User.Id = user.Id;
                    user.Questions.Add(q);

                    _context.Question.Add(q);
                    
                    _context.SaveChanges();
                    if(tagsString != null)
                    {
                        foreach(string tagString in tagsString.Split(','))
                        {
                            //Question_Tag qTag = new Question_Tag();
                            //qTag.Tag.Name = tagString;
                            //qTag.TagId = qTag.Tag.Id;
                            //qTag.Tag.QuestionTags.Add(qTag);

                            //qTag.Question = q;
                            //qTag.QuestionId = q.Id;
                            //qTag.Question.QuestionTags.Add(qTag);
                            //_context.QuestionTag.Add(qTag);
                            //_context.SaveChanges();

                            //Tag tagDb = await _context.Tag.FirstAsync(t => t.Id == tag.Id);
                            //tagDb.Name = tagString;
                            //q.Tags.Add(tagDb);
                            //tagDb.Questions.Add(q);
                            //_context.SaveChanges();
                        }
                    }
                }
                catch(Exception ex)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index");
        }


        
        public async Task<IActionResult> AnswerQuestion(int questionId)
        {
            //List<Question> questionsList = _context.Question.ToList();

            Question question = await _context.Question
                .Include(q => q.Comments).ThenInclude(c => c.User)
                .Include(q => q.Answers)
                    .ThenInclude(a => a.User)
                    //.ThenInclude(a => a.AnswerAndVote)
                    //.ThenInclude(av => av.Vote)
                .FirstAsync(q => q.Id == questionId);
            return View(question);
        }

        public async Task<IActionResult> MyPosts()
        {
            string userName = User.Identity.Name;
            ApplicationUser user = await _context.Users
                .Include(u => u.Questions)
                    .ThenInclude(q => q.Answers)
                    .ThenInclude(a => a.User)
                .Include(u => u.Answers)
                .FirstAsync(u => u.UserName == userName);
            return View(user);
        }

        
        public async Task<IActionResult> MyPost(int? questionId)
        {
            Question question = await _context.Question
                .Include(q => q.Answers)
                    .ThenInclude(a => a.User)
                    .ThenInclude(a => a.CommentsToAnswer)
                .Include(q => q.Comments)
                .Include(q => q.CorrectAnswer)
                .FirstAsync(q => q.Id == questionId);
            

            return View(question);
        }

        [HttpPost]
        public async Task<IActionResult> MyPost(int? answerId, int? questionId)
        {
            if(answerId != null && questionId != null)
            {
                try
                {
                    Answer answer = await _context.Answer
                        .Include(a => a.CorrectAnswer)
                        .FirstAsync(a => a.Id == answerId);

                    Question question = await _context.Question
                        .Include(q => q.CorrectAnswer)
                        .FirstAsync(q => q.Id == questionId);

                    string userName = User.Identity.Name;
                    ApplicationUser user = await _context.Users.FirstAsync(u => u.UserName == userName);

                    if(user.Id != question.User.Id)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        question.CorrectAnswer.Answer = answer;
                        question.CorrectAnswer.Answer.Id = answer.Id;

                        _context.SaveChanges();
                    }   
                }
                catch(Exception ex)
                {
                    return RedirectToAction("Error", new { ex });
                }
            }
            

            return RedirectToAction("MyPost", new { questionId });
        }

        [HttpPost]
        public async Task<IActionResult> AnswerQuestion(int? questionId, string? answer)
        {
            if (questionId != null && answer != null)
            {
                try
                {
                    Question answeredQuestion = await _context.Question.Include(q => q.Answers).ThenInclude(a => a.User).FirstAsync(q => q.Id == questionId);
                    Answer submittedAnswer = new Answer();

                    string userName = User.Identity.Name;
                    ApplicationUser user = await _context.Users.FirstAsync(u => u.UserName == userName);

                    submittedAnswer.QuestionId = (int)questionId;
                    submittedAnswer.Body = answer;
                    submittedAnswer.User = user;
                    submittedAnswer.UserId = user.Id;

                    answeredQuestion.Answers.Add(submittedAnswer);
                    int count = answeredQuestion.Answers.Count();

                    answeredQuestion.IsBeingAnswered = false;

                    user.Answers.Add(submittedAnswer);

                    _context.Answer.Add(submittedAnswer);
                    

                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", new { ex });
                }
            }
            return RedirectToAction("AnswerQuestion", new { questionId });
        }

        public async Task<IActionResult> AddComment(int? questionId, int? answerId, string? comment)
        {
            // Comment to a question
            if(questionId != null && comment != null && answerId == null)
            {
                try
                {
                    Question commentedQuestion = await _context.Question
                        .Include(q => q.Comments).ThenInclude(c => c.User)
                        .Include(q => q.Answers).ThenInclude(a => a.User)
                        .FirstAsync(q => q.Id == questionId);

                    CommentToQuestion submittedComment = new CommentToQuestion();
                    submittedComment.Body = comment;

                    string userName = User.Identity.Name;
                    ApplicationUser user = await _context.Users.FirstAsync(u => u.UserName == userName);
                    submittedComment.User = user;
                    submittedComment.UserId = user.Id;
                    submittedComment.Question = commentedQuestion;
                    submittedComment.QuestionId = commentedQuestion.Id;                    

                    commentedQuestion.Comments.Add(submittedComment);
                    
                    _context.SaveChanges();

                    return RedirectToAction("AnswerQuestion", new { questionId });
                }
                catch
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            // Comment to an Answer
            if(answerId != null && comment != null && questionId != null)
            {
                try
                {
                    Question selectedQuestion = await _context.Question.Include(q => q.Answers)
                        .ThenInclude(a => a.Comments).ThenInclude(c => c.User)
                        .FirstAsync(q => q.Id == questionId);
                    //Answer commentedAnswer = await _context.Answer
                    //    .Include(a => a.User).Include(a => a.Comments).ThenInclude(c => c.User)
                    //    .FirstAsync(a => a.Id == answerId);

                    Answer commentedAnswer = selectedQuestion.Answers.First(a => a.Id == answerId);
                    CommentToAnswer submittedComment = new CommentToAnswer();
                    submittedComment.Body = comment;

                    string userName = User.Identity.Name;
                    ApplicationUser user = await _context.Users.FirstAsync(u => u.UserName == userName);
                    submittedComment.User = user;
                    submittedComment.UserId = user.Id;
                    submittedComment.Answer = commentedAnswer;
                    submittedComment.AnswerId = commentedAnswer.Id;

                    commentedAnswer.Comments.Add(submittedComment);
                    
                    

                    _context.SaveChanges();

                    return RedirectToAction("AnswerQuestion", new { questionId });

                }
                catch
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("AnswerQuestion", new { questionId });
        }

        public async Task<IActionResult> VoteAnswer(int? answerId, string? vote, int questionId)
        {
            if(answerId != null && vote != null)
            {
                try
                {
                    Answer answer = await _context.Answer
                        .Include(a => a.AnswerAndVote)
                        .ThenInclude(av => av.Vote)
                        .ThenInclude(v => v.User)
                        .FirstAsync(a => a.Id == answerId);

                    AnswerAndVote av = new AnswerAndVote();
                    Vote newVote = new Vote();
                    string userName = User.Identity.Name;
                    ApplicationUser user = await _context.Users.FirstAsync(u => u.UserName == userName);

                    if(answer.User.Id == user.Id)
                    {
                        switch (vote)
                        {
                            case "up":
                                if (answer.AnswerAndVote.Any(av => av.Vote.UserId == user.Id && av.Vote.UpVote))
                                {
                                    return RedirectToAction("AnswerQuestion", new { questionId });
                                }
                                break;
                            case "down":
                                if (answer.AnswerAndVote.Any(av => av.Vote.UserId == user.Id && av.Vote.DownVote))
                                {
                                    return RedirectToAction("AnswerQuestion", new { questionId });
                                }
                                break;
                                //default:
                                //    return RedirectToAction("AnswerQuestion", new { questionId });
                        };
                    }
                    else
                    {
                        _ = vote == "up" ? newVote.UpVote = true : false;
                        _ = vote == "down" ? newVote.DownVote = true : false;

                        av.Answer = answer;
                        av.AnswerId = answer.Id;
                        av.Vote = newVote;
                        av.VoteId = newVote.Id;

                        answer.AnswerAndVote.Add(av);

                        _context.SaveChanges();
                    }
                    
                }
                catch
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("AnswerQuestion", new { questionId });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ExceptionMessage = message });
        }












        // GET: Questions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Question == null)
            {
                return NotFound();
            }

            var question = await _context.Question
                .Include(q => q.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // GET: Questions/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Body,UserId")] Question question)
        {
            if (ModelState.IsValid)
            {
                _context.Add(question);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", question.UserId);
            return View(question);
        }

        // GET: Questions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Question == null)
            {
                return NotFound();
            }

            var question = await _context.Question.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", question.UserId);
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Body,UserId")] Question question)
        {
            if (id != question.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(question);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", question.UserId);
            return View(question);
        }

        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Question == null)
            {
                return NotFound();
            }

            var question = await _context.Question
                .Include(q => q.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Question == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Question'  is null.");
            }
            var question = await _context.Question.FindAsync(id);
            if (question != null)
            {
                _context.Question.Remove(question);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionExists(int id)
        {
          return (_context.Question?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
