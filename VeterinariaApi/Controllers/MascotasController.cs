using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaApi.Data;
using VeterinariaApi.Models;

namespace VeterinariaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MascotasController : ControllerBase
{
    private readonly VeterinariaContext _context;

    public MascotasController(VeterinariaContext context)
    {
        _context = context;
    }

    // GET: api/mascotas?especie=Perro&duenoId=1&nombre=ro&edadMin=1&edadMax=6
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Mascota>>> GetMascotas(
        [FromQuery] string? especie,
        [FromQuery] int? duenoId,
        [FromQuery] string? nombre,
        [FromQuery] int? edadMin,
        [FromQuery] int? edadMax)
    {
        if (edadMin.HasValue && edadMax.HasValue && edadMin > edadMax)
            return BadRequest(new { mensaje = "edadMin no puede ser mayor que edadMax." });   // 400

        var query = _context.Mascotas
            .AsNoTracking()
            .Include(m => m.Dueno)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(especie))
            query = query.Where(m => m.Especie == especie);

        if (duenoId.HasValue)
            query = query.Where(m => m.DuenoId == duenoId.Value);

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(m => m.Nombre.Contains(nombre));

        if (edadMin.HasValue)
            query = query.Where(m => m.Edad >= edadMin.Value);

        if (edadMax.HasValue)
            query = query.Where(m => m.Edad <= edadMax.Value);

        var mascotas = await query
            .OrderBy(m => m.Nombre)
            .ToListAsync();

        return Ok(mascotas);                                 // 200
    }

    // GET: api/mascotas/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Mascota>> GetMascota(int id)
    {
        var mascota = await _context.Mascotas
            .AsNoTracking()
            .Include(m => m.Dueno)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (mascota == null)
            return NotFound(new { mensaje = $"No existe la mascota con id {id}." });   // 404

        return Ok(mascota);                                  // 200
    }

    // POST: api/mascotas
    [HttpPost]
    public async Task<ActionResult<Mascota>> PostMascota(Mascota mascota)
    {
        // Clave foránea inexistente -> 400
        var duenoExiste = await _context.Duenos.AnyAsync(d => d.Id == mascota.DuenoId);
        if (!duenoExiste)
        {
            ModelState.AddModelError(nameof(Mascota.DuenoId), $"No existe el dueño con id {mascota.DuenoId}.");
            return ValidationProblem(ModelState);            // 400
        }

        // Regla de negocio 3: un dueño no puede tener dos mascotas con el mismo nombre
        var nombreRepetido = await _context.Mascotas
            .AnyAsync(m => m.DuenoId == mascota.DuenoId && m.Nombre == mascota.Nombre);
        if (nombreRepetido)
            return Conflict(new { mensaje = $"El dueño ya tiene una mascota llamada {mascota.Nombre}." });   // 409

        mascota.Id = 0;          // lo genera la base de datos
        mascota.Dueno = null;    // solo se usa la clave foránea DuenoId

        _context.Mascotas.Add(mascota);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMascota), new { id = mascota.Id }, mascota);   // 201
    }

    // PUT: api/mascotas/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutMascota(int id, Mascota mascota)
    {
        if (id != mascota.Id)
            return BadRequest(new { mensaje = "El id de la ruta no coincide con el id del cuerpo." });   // 400

        var actual = await _context.Mascotas.FindAsync(id);
        if (actual == null)
            return NotFound(new { mensaje = $"No existe la mascota con id {id}." });   // 404

        var duenoExiste = await _context.Duenos.AnyAsync(d => d.Id == mascota.DuenoId);
        if (!duenoExiste)
        {
            ModelState.AddModelError(nameof(Mascota.DuenoId), $"No existe el dueño con id {mascota.DuenoId}.");
            return ValidationProblem(ModelState);            // 400
        }

        var nombreRepetido = await _context.Mascotas
            .AnyAsync(m => m.DuenoId == mascota.DuenoId && m.Nombre == mascota.Nombre && m.Id != id);
        if (nombreRepetido)
            return Conflict(new { mensaje = $"El dueño ya tiene otra mascota llamada {mascota.Nombre}." });  // 409

        actual.Nombre = mascota.Nombre;
        actual.Especie = mascota.Especie;
        actual.Raza = mascota.Raza;
        actual.Edad = mascota.Edad;
        actual.Peso = mascota.Peso;
        actual.DuenoId = mascota.DuenoId;

        await _context.SaveChangesAsync();
        return NoContent();                                  // 204
    }

    // DELETE: api/mascotas/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMascota(int id)
    {
        var mascota = await _context.Mascotas.FindAsync(id);
        if (mascota == null)
            return NotFound(new { mensaje = $"No existe la mascota con id {id}." });   // 404

        _context.Mascotas.Remove(mascota);
        await _context.SaveChangesAsync();
        return NoContent();                                  // 204
    }
}