using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Add this line
using System;
using System.Linq;
using System.Threading.Tasks;
using ToDoManagement.Models;

namespace ToDoManagement.Controllers
{
    public class MainTasksController : Controller
    {
        private readonly TodoDbContext _context;
        private readonly ILogger<MainTasksController> _logger;

        public MainTasksController(TodoDbContext context, ILogger<MainTasksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var mainTasks = _context.MainTasks.ToList();
            return View(mainTasks);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MainTask mainTask)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    mainTask.Id = Guid.NewGuid();
                    mainTask.CreatedAt = DateTime.Now;

                    _context.MainTasks.Add(mainTask);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"MainTask created successfully. Id: {mainTask.Id}");
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error creating MainTask: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the MainTask.");
                }
            }

            return View(mainTask);
        }

        // GET: /main-tasks/edit/5
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (!Guid.TryParse(id, out var guidId) || guidId == Guid.Empty)
            {
                return BadRequest();
            }

            var mainTask = await _context.MainTasks.FindAsync(guidId);

            if (mainTask == null)
            {
                return NotFound();
            }

            return View(mainTask);
        }

        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, MainTask mainTask)
        {
            if (!Guid.TryParse(id, out var guidId) || guidId == Guid.Empty)
            {
                return BadRequest();
            }

            if (guidId != mainTask.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTask = _context.MainTasks.Find(guidId);

                    if (existingTask == null)
                    {
                        return NotFound();
                    }

                    existingTask.TaskLabel = mainTask.TaskLabel;
                    existingTask.TaskDescription = mainTask.TaskDescription;
                    existingTask.IsDone = mainTask.IsDone;
                    existingTask.IsPinned = mainTask.IsPinned;

                    _context.Entry(existingTask).State = EntityState.Modified;
                    _context.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MainTaskExists(guidId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                return View(mainTask);
            }
        }

        // GET: /main-tasks/delete/5
        [HttpGet("delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!Guid.TryParse(id, out var guidId) || guidId == Guid.Empty)
            {
                return BadRequest();
            }

            var mainTask = await _context.MainTasks.FindAsync(guidId);

            if (mainTask == null)
            {
                return NotFound();
            }

            return View(mainTask);
        }

        // POST: /main-tasks/delete/5
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            if (!Guid.TryParse(id, out var guidId) || guidId == Guid.Empty)
            {
                return BadRequest();
            }

            var mainTask = _context.MainTasks.FindAsync(guidId);

            if (mainTask == null)
            {
                return NotFound();
            }

            _context.MainTasks.Remove(mainTask.Result);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        private bool MainTaskExists(Guid id)
        {
            return _context.MainTasks.Any(e => e.Id == id);
        }
    }
}
