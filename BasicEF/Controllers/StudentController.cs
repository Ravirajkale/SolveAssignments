using BasicEF.Data_Access_Layer;
using BasicEF.Entites;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace BasicEF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly BasicEFContext _context;

        public StudentController(BasicEFContext context)
        {
            _context = context;
        }

        //api/student
        [HttpGet]
        public async Task<ActionResult<IEnumerable<student>>> GetStudents()
        {
            return await _context.students.ToListAsync();
        }

        //api/student/id
        [HttpGet("{id}")]
        public async Task<ActionResult<student>> GetStudentById(int id)
        {
            var s = await _context.students.FindAsync(id);
            if (s == null)
            {
                return NotFound();
            }
            return s;
        }

        //api/student
        [HttpPost]
        public async Task<ActionResult<student>> AddStudent(student s)
        {
            _context.Add(s);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudents), new { id = s.id }, s);
        }
    }
}
