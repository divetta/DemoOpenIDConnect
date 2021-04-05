using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Helpers;
using SimpleApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleApi.Controllers
{
    [Route("api/heroes")]
    [ApiController]
    public class HeroesController : ControllerBase
    {
        private readonly HeroContext _context;

        public HeroesController(HeroContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        [HttpGet]
        [ClaimRequirement("Hero-ReadOnly")]
        public async Task<ActionResult<IEnumerable<Hero>>> GetTodoItems()
        {
            return await _context.Heroes.ToListAsync();
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        [ClaimRequirement("Hero-ReadOnly")]
        public async Task<ActionResult<Hero>> GetTodoItem(long id)
        {
            var todoItem = await _context.Heroes.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        #region snippet_Update
        // PUT: api/TodoItems/5
        [HttpPut("{id}")]
        [ClaimRequirement("Hero-Write")]
        public async Task<IActionResult> PutTodoItem(long id, Hero todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HeroExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        #endregion

        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        #region snippet_Create
        // POST: api/TodoItems
        [HttpPost]
        [ClaimRequirement("Hero-Write")]
        public async Task<ActionResult<Hero>> PostTodoItem(Hero todoItem)
        {
            _context.Heroes.Add(todoItem);
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }
        #endregion

        #region snippet_Delete
        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        [ClaimRequirement("Hero-Write")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.Heroes.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.Heroes.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        private bool HeroExists(long id)
        {
            return _context.Heroes.Any(e => e.Id == id);
        }
    }
}
