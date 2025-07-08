using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using KutuphaneOtomasyon.Web.Models; // Namespace for your Personel model
using System.Diagnostics; // Added for Debug.WriteLine

namespace KutuphaneOtomasyon.Web.Controllers
{
    public class PersonellerController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Constructor: Injects ApplicationDbContext using Dependency Injection
        public PersonellerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Personeller
        // Lists all personnel
        public async Task<IActionResult> Index()
        {
            var personeller = await _context.Personel.ToListAsync();
            return View(personeller);
        }

        // GET: Personeller/Details/5
        // Displays details of a specific personnel
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personel = await _context.Personel
                .FirstOrDefaultAsync(m => m.PersonelID == id);
            if (personel == null)
            {
                return NotFound();
            }

            return View(personel);
        }

        // GET: Personeller/Create
        // Displays the form for creating new personnel
        public IActionResult Create()
        {
            return View();
        }

        // POST: Personeller/Create
        // Saves new personnel to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Bind only properties that come directly from the form and are not set manually
        public async Task<IActionResult> Create([Bind("PersonelID,Ad,Soyad,KullaniciAdi")] Personel personel, string Sifre)
        {
            // Set SifreHash and Rol BEFORE ModelState.IsValid check
            // This ensures that [Required] validation for these fields passes.
            personel.SifreHash = "GEÇİCİ_SİFRE_HASH_" + Sifre; // Placeholder, replace with proper hashing later
            personel.Rol = "Standart"; // Default role assignment

            // Debugging: If model validation fails, log errors to debug output
            if (!ModelState.IsValid)
            {
                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        Debug.WriteLine($"Model Hata (Create): {error.ErrorMessage}");
                        if (error.Exception != null)
                        {
                            Debug.WriteLine($"Model Hata Detay (Create): {error.Exception.Message}");
                        }
                    }
                }
                TempData["ErrorMessage"] = "Personel eklenirken bazı hatalar oluştu. Lütfen bilgileri kontrol edin.";
                return View(personel); // Return view with validation errors
            }

            // If model validation is successful, proceed with saving
            try
            {
                _context.Add(personel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Personel başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // Catch and display database-related errors
                var postgresEx = ex.InnerException as Npgsql.PostgresException;
                if (postgresEx != null)
                {
                    ModelState.AddModelError("", $"Veritabanı Hatası: {postgresEx.MessageText}");
                    Debug.WriteLine($"DbUpdateException (Create): {postgresEx.MessageText}");
                }
                else
                {
                    ModelState.AddModelError("", "Kayıt sırasında bir veritabanı hatası oluştu.");
                    Debug.WriteLine($"DbUpdateException (Create): {ex.Message}");
                }
                TempData["ErrorMessage"] = "Personel eklenirken bir veritabanı hatası oluştu.";
                return View(personel); // Return view with database error
            }
        }

        // GET: Personeller/Edit/5
        // Displays the form for editing existing personnel
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personel = await _context.Personel.FindAsync(id);
            if (personel == null)
            {
                return NotFound();
            }
            return View(personel);
        }

        // POST: Personeller/Edit/5
        // Updates existing personnel in the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        // For Edit, SifreHash is expected to come from a hidden input, and Rol from a visible input.
        public async Task<IActionResult> Edit(int id, [Bind("PersonelID,Ad,Soyad,KullaniciAdi,SifreHash,Rol")] Personel personel)
        {
            if (id != personel.PersonelID)
            {
                return NotFound();
            }

            // Debugging: If model validation fails, log errors to debug output
            if (!ModelState.IsValid)
            {
                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        Debug.WriteLine($"Model Hata (Edit): {error.ErrorMessage}");
                        if (error.Exception != null)
                        {
                            Debug.WriteLine($"Model Hata Detay (Edit): {error.Exception.Message}");
                        }
                    }
                }
                TempData["ErrorMessage"] = "Personel güncellenirken bazı hatalar oluştu. Lütfen bilgileri kontrol edin.";
                return View(personel); // Return view with validation errors
            }

            try
            {
                _context.Update(personel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Personel başarıyla güncellendi.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonelExists(personel.PersonelID))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Rethrow other unexpected exceptions
                }
            }
            catch (DbUpdateException ex)
            {
                // Catch and display database-related errors
                var postgresEx = ex.InnerException as Npgsql.PostgresException;
                if (postgresEx != null)
                {
                    ModelState.AddModelError("", $"Veritabanı Hatası: {postgresEx.MessageText}");
                    Debug.WriteLine($"DbUpdateException (Edit): {postgresEx.MessageText}");
                }
                else
                {
                    ModelState.AddModelError("", "Güncelleme sırasında bir veritabanı hatası oluştu.");
                    Debug.WriteLine($"DbUpdateException (Edit): {ex.Message}");
                }
                TempData["ErrorMessage"] = "Personel güncellenirken bir veritabanı hatası oluştu.";
                return View(personel); // Return view with database error
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Personeller/Delete/5
        // Displays the confirmation page for deleting personnel
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personel = await _context.Personel
                .FirstOrDefaultAsync(m => m.PersonelID == id);
            if (personel == null)
            {
                return NotFound();
            }

            return View(personel);
        }

        // POST: Personeller/Delete/5
        // Deletes personnel from the database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personel = await _context.Personel.FindAsync(id);
            if (personel != null)
            {
                _context.Personel.Remove(personel);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Personel başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        // Helper method: Checks if a personnel exists
        private bool PersonelExists(int id)
        {
            return _context.Personel.Any(e => e.PersonelID == id);
        }
    }
}