﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Data;
using ToDoApp.Models;

namespace ToDoApp.Controllers
{
    public class NoteController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;


        public NoteController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Notes
        public async Task<IActionResult> Index()
        {
            // Buradaki yapı ile kullanıcıya özel sayfa getiriyoruz
            var userId = _userManager.GetUserId(User);
            var notes = await _context.Notes          //DbSet<Note>'a erişim
                .Where(n => n.UserId == userId)
                .ToListAsync();

            return View(notes);
        }

        // GET: Notes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var note = await _context.Notes.FirstOrDefaultAsync(m => m.Id == id);

            if (note == null) return NotFound();

            if (note.UserId != _userManager.GetUserId(User)) return Forbid(); // veya return NotFound(); güvenlik için

            return View(note);
        }

        // GET: Notes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Notes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content")] Note note)
        {
            if (ModelState.IsValid)
            {
                note.CreatedAt = DateTime.Now;
                note.UserId = _userManager.GetUserId(User); // 👈 kullanıcı ID’si

                _context.Add(note);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(note);
        }

        // GET: Notes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var note = await _context.Notes.FindAsync(id);
            if (note == null || note.UserId != _userManager.GetUserId(User)) return Forbid();

            return View(note);
        }

        // POST: Notes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,UserId")] Note note)
        {
            var userId = _userManager.GetUserId(User);
            var existingNote = await _context.Notes.FindAsync(id);

            if (existingNote == null || existingNote.UserId != userId) return NotFound();

            if (id != note.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // 🔥 Değişiklik kontrolü
                if (existingNote.Title == note.Title && existingNote.Content == note.Content)
                {
                    TempData["WarningMessage"] = "⚠️ Herhangi bir değişiklik yapılmadı.";
                    return View(note); // Sayfada kal
                }

                try
                {
                    // Sadece değiştirilebilir alanları (varsa) güncelle
                    existingNote.Title = note.Title;
                    existingNote.Content = note.Content;

                    _context.Update(existingNote);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "✅ Not başarıyla güncellendi!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoteExists(note.Id)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }
            return View(note);
        }

        // GET: Notes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var note = await _context.Notes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // POST: Notes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note != null)
            {
                _context.Notes.Remove(note);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NoteExists(int id)
        {
            return _context.Notes.Any(e => e.Id == id);
        }
    }
}
