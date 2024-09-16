using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TetsTask.DataAccess;
using TetsTask.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace TetsTask.Controllers
{
    public class PersonController : Controller
    {
        private readonly AppDbContext _context;

        public PersonController(AppDbContext context)
        {
            _context = context;
        }

        // Метод для отображения списка людей (index.cshtml)
        public async Task<IActionResult> Index()
        {
            var persons = await _context.Person.ToListAsync();
            if (persons == null)
            {
                persons = new List<Person>(); // Убедитесь, что модель не равна null
            }
            return View(persons);
        }

        // Метод для загрузки CSV файла
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
                using (var csv = new CsvHelper.CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null
                }))
                {
                    var records = csv.GetRecords<Person>().ToList();

                    foreach (var record in records)
                    {
                        record.Id = 0; // Устанавливаем Id в 0 для автогенерации в базе данных
                    }

                    if (!records.Any())
                    {
                        return BadRequest("No valid records found in the CSV file.");
                    }

                    _context.Person.AddRange(records);
                    await _context.SaveChangesAsync();
                }
            }
            catch
            {
                // Исключение не обрабатывается
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: /Person/Edit/{id} - Открываем страницу редактирования
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

        // POST: /Person/Edit/{id} - Обрабатываем изменения
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

        // GET: /Person/Delete/{id} - Открываем страницу удаления
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

        // POST: /Person/Delete/{id} - Обрабатываем удаление
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

            // Переадресация на Home/Index после удаления
            return RedirectToAction("Index", "Home");
        }
    }
}
