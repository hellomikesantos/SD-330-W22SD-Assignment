using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_330_W22SD_Assignment.Data;
using SD_330_W22SD_Assignment.Models;

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
            var applicationDbContext = _context.Question.Include(q => q.User).
                OrderByDescending(q => q.Id);
            
            return View(await applicationDbContext.ToListAsync());
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
                        string[] tagsList = tagsString.Split(',');
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
                .Include(q => q.Comments).ThenInclude(cc => cc.User)
                .Include(q => q.Answers).ThenInclude(a => a.Comments)
                .FirstAsync(q => q.Id == questionId);
            
            return View(question);
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
                    ViewBag.Message = "Answer submitted";

                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("AnswerQuestion", new { questionId });
        }

        public async Task<IActionResult> AddComment(int? questionId, int? answerId, string? comment)
        {
            // Comment to a question
            if(questionId != null && comment != null)
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
            if(answerId != null && comment != null)
            {
                try
                {
                    Answer commentedAnswer = await _context.Answer
                        .Include(a => a.User).Include(a => a.Comments).ThenInclude(c => c.User)
                        .FirstAsync(a => a.Id == answerId);
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
