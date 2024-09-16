using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TetsTask.DataAccess;
using TetsTask.Models;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace TetsTask.Controllers
{
    public class PersonController : Controller
    {
        private readonly AppDbContext _context;

        public PersonController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var persons = await _context.Person.ToListAsync();
            if (persons == null)
            {
                persons = new List<Person>();
            }
            return View(persons);
        }

        [HttpPost]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Invalid file type. Please upload a CSV file.");
            }

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                }))
                {
                    var records = csv.GetRecords<Person>().ToList();

                    if (!records.Any())
                    {
                        return BadRequest("No valid records found in the CSV file.");
                    }

                    _context.Person.AddRange(records);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: /Person/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var person = await _context.Person.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: /Person/Edit/{id}
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DateOfBirth,IsMarried,Phone,Salary")] Person person)
        {
            if (!ModelState.IsValid)
            {
                return View(person);
            }

            var existingPerson = await _context.Person.FindAsync(id);
            if (existingPerson == null)
            {
                return NotFound();
            }

            existingPerson.Name = person.Name;
            existingPerson.DateOfBirth = person.DateOfBirth;
            existingPerson.IsMarried = person.IsMarried;
            existingPerson.Phone = person.Phone;
            existingPerson.Salary = person.Salary;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        // GET: /Person/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var person = await _context.Person.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: /Person/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _context.Person.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }

            _context.Person.Remove(person);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
    }
}
