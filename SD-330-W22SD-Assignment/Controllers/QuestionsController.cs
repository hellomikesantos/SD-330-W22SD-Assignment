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
        public async Task<IActionResult> PostQuestion(string? title, string? body, Question question, string? tags)
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
                }
                catch(Exception ex)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult AnswerQuestion(Question question)
        {
            //Question question = _context.Question.Include(q => q.Answers).
            //    ThenInclude(a => a.User).FirstOrDefault(q => q.Id == questionId);
            
            return View(question);
        }

        [HttpPost]
        public IActionResult AnswerQuestion(int? questionId, string? answer)
        {
            if (questionId != null && answer != null)
            {
                try
                {
                    Question answeredQuestion = _context.Question.First(q => q.Id == questionId);
                    Answer submittedAnswer = new Answer();

                    string userName = User.Identity.Name;
                    ApplicationUser user = _context.Users.First(u => u.UserName == userName);

                    submittedAnswer.QuestionId = (int)questionId;
                    submittedAnswer.Body = answer;
                    submittedAnswer.User = user;
                    submittedAnswer.UserId = user.Id;

                    answeredQuestion.Answers.Add(submittedAnswer);
                    answeredQuestion.IsBeingAnswered = false;
                    _context.SaveChanges();
                }
                catch
                {
                    return RedirectToAction("Index", "Home");

                }
            }
            return RedirectToAction("View");
        }
        //[HttpPost]
        //public async Task<ActionResult> Index(int? questionId)
        //{
        //    if(questionId != null)
        //    {
        //        try
        //        {
        //            Question selected = _context.Question.First(q => q.Id == questionId);
        //            string userName = User.Identity.Name;

        //            ApplicationUser user = _context.Users.First(q => q.UserName == userName);
        //            user.Questions.Add(selected);
        //            selected.User = user;
        //            selected.User.Id = user.Id;
        //        }
        //        catch
        //        {
        //            return RedirectToAction("Error", "Home");
        //        }
        //    }
        //    return View("Index");
        //}

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
