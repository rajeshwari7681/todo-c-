using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController(TodoDbContext db) : ControllerBase
{
    // GET api/todo
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetAll()
    {
        return Ok(await db.Todos.OrderByDescending(t => t.CreatedAt).ToListAsync());
    }

    // GET api/todo/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetById(int id)
    {
        var todo = await db.Todos.FindAsync(id);
        if (todo is null)
            return NotFound(new { message = $"Todo with id {id} not found." });

        return Ok(todo);
    }

    // POST api/todo
    [HttpPost]
    public async Task<ActionResult<TodoItem>> Create([FromBody] CreateTodoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new { message = "Title is required." });

        var todo = new TodoItem
        {
            Title = request.Title,
            Description = request.Description,
            Price = request.Price,
            TimeToComplete = request.TimeToComplete
        };

        db.Todos.Add(todo);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
    }

    // PUT api/todo/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<TodoItem>> Update(int id, [FromBody] UpdateTodoRequest request)
    {
        var todo = await db.Todos.FindAsync(id);
        if (todo is null)
            return NotFound(new { message = $"Todo with id {id} not found." });

        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new { message = "Title is required." });

        todo.Title = request.Title;
        todo.Description = request.Description;
        todo.Price = request.Price;
        todo.IsCompleted = request.IsCompleted;

        await db.SaveChangesAsync();
        return Ok(todo);
    }

    // PATCH api/todo/{id}/complete
    [HttpPatch("{id}/complete")]
    public async Task<ActionResult<TodoItem>> MarkComplete(int id)
    {
        var todo = await db.Todos.FindAsync(id);
        if (todo is null)
            return NotFound(new { message = $"Todo with id {id} not found." });

        todo.IsCompleted = true;
        await db.SaveChangesAsync();
        return Ok(todo);
    }

    // DELETE api/todo/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var todo = await db.Todos.FindAsync(id);
        if (todo is null)
            return NotFound(new { message = $"Todo with id {id} not found." });

        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
