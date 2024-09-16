using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TetsTask.DataAccess;
using TetsTask.Models;
using System.Linq;
using System.Threading.Tasks;

namespace TetsTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
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
    }
}
