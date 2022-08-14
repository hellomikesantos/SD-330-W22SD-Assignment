using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SD_330_W22SD_Assignment.Data;
using SD_330_W22SD_Assignment.Models;
using SD_330_W22SD_Assignment.Models.ViewModels;

namespace SD_330_W22SD_Assignment.Controllers
{
    [Authorize]
    public class QuestionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager { get; set; }
        private RoleManager<IdentityRole> _roleManager { get; set; }
        public QuestionsController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult AddRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(string? roleName)
        {
            if (roleName != null)
            {
                try
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                    await _context.SaveChangesAsync();

                    ViewBag.Message = $"Added new role '{roleName}'";
                    return View();
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", new { ex.Message });
                }
            }
            return View();
        }

        public IActionResult AddUserToRole()
        {
            UserAndRole vm = new UserAndRole(
            _context.Users.ToList(),
            _context.Roles.ToList());
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToRole(string userId, string roleId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            IdentityRole role = await _roleManager.FindByIdAsync(roleId);

            bool isInRole = await _userManager.IsInRoleAsync(user, role.Name);

            if (!isInRole)
            {
                await _userManager.AddToRoleAsync(user, role.Name);
                await _context.SaveChangesAsync();
            }

            UserAndRole vm = new UserAndRole(
            _context.Users.ToList(),
            _context.Roles.ToList());

            vm.Message = $"Added {user.UserName} to role {role.Name}";
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminPage()
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
        public async Task<IActionResult> AdminPage(int questionId)
        {
            // delete questions from admin page
            Question question = await _context.Question
                       .FirstAsync(q => q.Id == questionId);

            _context.Question.Remove(question);
            await _context.SaveChangesAsync();
            return RedirectToAction("AdminPage");
        }

        // GET: Questions
        [AllowAnonymous]
        public async Task<IActionResult> Index(int page = 1)
        {
            var applicationDbContext = _context.Question.Include(q => q.User)
                .Include(q => q.Votes)
                .Include(q => q.User)
                    .ThenInclude(u => u.Reputation)
                .Include(q => q.Answers)
                .Include(q => q.Tags)
                .OrderByDescending(q => q.Id);
            int pageSize = 10;
            int questionCount = applicationDbContext.Count();

            Pager pager = new Pager(questionCount, page, pageSize);
            int skips = (page - 1) * pageSize;
            var currentItems = applicationDbContext.Skip(skips).Take(pager.PageCount);
            ViewBag.Pager = pager;

            return View(await currentItems.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> IndexAnonymous()
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
            if (questionId != null)
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
                    else if (question.User.Id != user.Id)
                    {
                        switch (vote)
                        {
                            case "up":
                                if (question.Votes.Any(v => v.User.Id == user.Id && v.UpVote))
                                {
                                    return RedirectToAction("Index");
                                }
                                newVote.UpVote = true;
                                foreach (Vote v in question.Votes)
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
                catch (Exception ex)
                {
                    return RedirectToAction("Error", new { ex.Message });
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
        public async Task<IActionResult> PostQuestion(string? title, string? body, string? tags)
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
                    if (tags != null)
                    {
                        foreach (string tagString in tags.Split(','))
                        {
                            Tag newTag = new Tag();
                            newTag.Name = tagString;
                            q.Tags.Add(newTag);
                            newTag.Questions.Add(q);

                            _context.Tag.Add(newTag);
                            _context.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", new { ex.Message });
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> TagQuestion(int? tagId)
        {
            Tag tag = await _context.Tag
                .Include(t => t.Questions)
                    .ThenInclude(q => q.User)
                    .ThenInclude(u => u.Reputation)
                .FirstAsync(t => t.Id == tagId);
            return View(tag);
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
            if (answerId != null && questionId != null)
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

                    if (user.Id != question.User.Id)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        CorrectAnswer correctAnswer = new CorrectAnswer();
                        correctAnswer.Answer = answer;
                        correctAnswer.AnswerId = answer.Id;
                        correctAnswer.Question = question;
                        correctAnswer.QuestionId = question.Id;
                        question.CorrectAnswer.Answer = answer;
                        question.CorrectAnswer.Answer.Id = answer.Id;

                        _context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", new { ex.Message });
                }
            }
            return RedirectToAction("MyPost", new { questionId });
        }

        public async Task<IActionResult> AnswerQuestion(int questionId)
        {

            Question question = await _context.Question
                .Include(q => q.Comments).ThenInclude(c => c.User)
                .Include(q => q.Answers)
                    .ThenInclude(a => a.User)
                //.ThenInclude(a => a.AnswerAndVote)
                //.ThenInclude(av => av.Vote)
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


                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", new { ex.Message });
                }
            }
            return RedirectToAction("AnswerQuestion", new { questionId });
        }

        public async Task<IActionResult> AddComment(int? questionId, int? answerId, string? comment)
        {
            // Comment to a question
            if (questionId != null && comment != null && answerId == null)
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
                catch (Exception ex)
                {
                    return RedirectToAction("Error", new { ex.Message });
                }
            }

            // Comment to an Answer
            if (answerId != null && comment != null && questionId != null)
            {
                try
                {
                    Question selectedQuestion = await _context.Question
                        .Include(q => q.Answers)
                            .ThenInclude(a => a.Comments).ThenInclude(c => c.User)
                        .Include(q => q.Comments)
                            .ThenInclude(c => c.User)
                        .FirstAsync(q => q.Id == questionId);

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
                    _context.CommentToAnswer.Add(submittedComment);

                    _context.SaveChanges();

                    return RedirectToAction("AnswerQuestion", new { questionId });

                }
                catch (Exception ex)
                {
                    return RedirectToAction("Error", new { ex.Message });
                }
            }
            return RedirectToAction("AnswerQuestion", new { questionId });
        }

        public async Task<IActionResult> VoteAnswer(int? answerId, string? vote, int questionId)
        {
            if (answerId != null && vote != null)
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

                    if (answer.User.Id == user.Id)
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
                catch (Exception ex)
                {
                    return RedirectToAction("Error", new { ex.Message });
                }
            }
            return RedirectToAction("AnswerQuestion", new { questionId });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, ExceptionMessage = message });
        }
    }
}
