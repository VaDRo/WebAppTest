using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class FilmsController : Controller
    {
        private readonly SakilaContext _context;

        public FilmsController(SakilaContext context)
        {
            _context = context;
        }

        // GET: Films
        public async Task<IActionResult> Index(string sortCondition, string searchString)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortCondition) ? "name_desc" : "";
            ViewData["CurrentFilter"] = searchString;

            var sakilaContext = _context.Film.Include(f => f.Language).Include(f => f.OriginalLanguage);
            var films = from f in _context.Film
                        select f;
            if (!String.IsNullOrEmpty(searchString))
            {
                films = films.Where(s => s.Title.Contains(searchString)
                                       || s.Description.Contains(searchString));
            }

            switch (sortCondition)
            {
                case "name_desc":
                    films = films.OrderByDescending(f => f.Title);
                    break;
                default:
                    films = films.OrderBy(f => f.Title);
                    break;
            }

            return View(await films.AsNoTracking().ToListAsync());
        }

        // GET: Films/Details/5
        public async Task<IActionResult> Details(ushort? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Film
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .Include(f => f.FilmActor)
                .ThenInclude(f => f.Actor)
                .Include(f => f.FilmCategory)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.FilmId == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // GET: Films/Create
        public IActionResult Create()
        {
            ViewData["LanguageId"] = new SelectList(_context.Language, "LanguageId", "Name");
            ViewData["OriginalLanguageId"] = new SelectList(_context.Language, "LanguageId", "Name");
            return View();
        }

        // POST: Films/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FilmId,Title,Description,LanguageId,OriginalLanguageId,RentalDuration,RentalRate,Length,ReplacementCost,LastUpdate")] Film film)
        {
            if (ModelState.IsValid)
            {
                _context.Add(film);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LanguageId"] = new SelectList(_context.Language, "LanguageId", "Name", film.LanguageId);
            ViewData["OriginalLanguageId"] = new SelectList(_context.Language, "LanguageId", "Name", film.OriginalLanguageId);
            return View(film);
        }

        // GET: Films/Edit/5
        public async Task<IActionResult> Edit(ushort? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Film.SingleOrDefaultAsync(m => m.FilmId == id);
            if (film == null)
            {
                return NotFound();
            }
            ViewData["LanguageId"] = new SelectList(_context.Language, "LanguageId", "Name", film.LanguageId);
            ViewData["OriginalLanguageId"] = new SelectList(_context.Language, "LanguageId", "Name", film.OriginalLanguageId);
            return View(film);
        }

        // POST: Films/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ushort id, [Bind("FilmId,Title,Description,LanguageId,OriginalLanguageId,RentalDuration,RentalRate,Length,ReplacementCost,LastUpdate")] Film film)
        {
            if (id != film.FilmId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(film);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(film.FilmId))
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
            ViewData["LanguageId"] = new SelectList(_context.Language, "LanguageId", "Name", film.LanguageId);
            ViewData["OriginalLanguageId"] = new SelectList(_context.Language, "LanguageId", "Name", film.OriginalLanguageId);
            return View(film);
        }

        // GET: Films/Delete/5
        public async Task<IActionResult> Delete(ushort? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Film
                .Include(f => f.Language)
                .Include(f => f.OriginalLanguage)
                .SingleOrDefaultAsync(m => m.FilmId == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // POST: Films/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(ushort id)
        {
            var film = await _context.Film.SingleOrDefaultAsync(m => m.FilmId == id);
            _context.Film.Remove(film);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FilmExists(ushort id)
        {
            return _context.Film.Any(e => e.FilmId == id);
        }
    }
}
