using Microsoft.AspNetCore.Mvc;
using MOVIFY.Data.Data;
using MOVIFY.Model;
using System.Linq;

namespace MOVIFY.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TotalMovies = _context.Movies.Count();
            ViewBag.TotalCategories = _context.Categories.Count();
            ViewBag.TotalDirectors = _context.Directors.Count();

            var finalData = _context.Categories
                .Select(c => new {
                    Name = c.CategoryName,
                    Count = c.Movies.Count
                })
                .Where(c => c.Count > 0)
                .OrderByDescending(c => c.Count)
                .Take(6)
                .ToList();

            ViewBag.CategoryLabels = finalData.Select(c => c.Name).ToArray();
            ViewBag.CategoryCounts = finalData.Select(c => c.Count).ToArray();

            ViewBag.Todos = _context.TodoItems.OrderByDescending(t => t.Id).ToList();

            return View();
        }

        [HttpPost]
        public IActionResult AddTodo(string description)
        {
            if (!string.IsNullOrWhiteSpace(description))
            {
                _context.TodoItems.Add(new TodoItem { Description = description, IsCompleted = false });
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ToggleTodo(int id)
        {
            var todo = _context.TodoItems.Find(id);
            if (todo != null)
            {
                todo.IsCompleted = !todo.IsCompleted;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteTodo(int id)
        {
            var todo = _context.TodoItems.Find(id);
            if (todo != null)
            {
                _context.TodoItems.Remove(todo);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}