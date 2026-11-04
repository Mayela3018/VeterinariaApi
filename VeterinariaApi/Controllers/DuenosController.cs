using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaApi.Data;
using VeterinariaApi.Models;

namespace VeterinariaApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DuenosController : ControllerBase
{
    private readonly VeterinariaContext _context;

    public DuenosController(VeterinariaContext context)
    {
        _context = context;
    }

    // GET: api/duenos?nombre=car
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Dueno>>> GetDuenos([FromQuery] string? nombre)
    {
        var query = _context.Duenos.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(d => d.Nombres.Contains(nombre));

        var duenos = await query
            .OrderBy(d => d.Nombres)
            .ToListAsync();

        return Ok(duenos);                                   // 200
    }

    // GET: api/duenos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Dueno>> GetDueno(int id)
    {
        var dueno = await _context.Duenos.FindAsync(id);

        if (dueno == null)
            return NotFound(new { mensaje = $"No existe el dueño con id {id}." });   // 404

        return Ok(dueno);                                    // 200
    }

    // GET: api/duenos/5/mascotas   (ruta anidada)
    [HttpGet("{id}/mascotas")]
    public async Task<ActionResult<IEnumerable<Mascota>>> GetMascotasDelDueno(int id)
    {
        var existe = await _context.Duenos.AnyAsync(d => d.Id == id);
        if (!existe)
            return NotFound(new { mensaje = $"No existe el dueño con id {id}." });   // 404

        var mascotas = await _context.Mascotas
            .AsNoTracking()
            .Where(m => m.DuenoId == id)
            .OrderBy(m => m.Nombre)
            .ToListAsync();

        return Ok(mascotas);                                 // 200
    }

    // POST: api/duenos
    [HttpPost]
    public async Task<ActionResult<Dueno>> PostDueno(Dueno dueno)
    {
        // Regla de negocio 1: el DNI no se puede repetir
        var dniRepetido = await _context.Duenos.AnyAsync(d => d.Dni == dueno.Dni);
        if (dniRepetido)
            return Conflict(new { mensaje = $"Ya existe un dueño registrado con el DNI {dueno.Dni}." });  // 409

        dueno.Id = 0;   // el Id lo genera la base de datos (IDENTITY)

        _context.Duenos.Add(dueno);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDueno), new { id = dueno.Id }, dueno);    // 201
    }

    // PUT: api/duenos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDueno(int id, Dueno dueno)
    {
        if (id != dueno.Id)
            return BadRequest(new { mensaje = "El id de la ruta no coincide con el id del cuerpo." });   // 400

        var actual = await _context.Duenos.FindAsync(id);
        if (actual == null)
            return NotFound(new { mensaje = $"No existe el dueño con id {id}." });   // 404

        // Regla de negocio 1: el DNI no puede pertenecer a otro dueño
        var dniRepetido = await _context.Duenos.AnyAsync(d => d.Dni == dueno.Dni && d.Id != id);
        if (dniRepetido)
            return Conflict(new { mensaje = $"El DNI {dueno.Dni} ya pertenece a otro dueño." });          // 409

        actual.Nombres = dueno.Nombres;
        actual.Dni = dueno.Dni;
        actual.Telefono = dueno.Telefono;
        actual.Email = dueno.Email;

        await _context.SaveChangesAsync();
        return NoContent();                                  // 204
    }

    // DELETE: api/duenos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDueno(int id)
    {
        var dueno = await _context.Duenos.FindAsync(id);
        if (dueno == null)
            return NotFound(new { mensaje = $"No existe el dueño con id {id}." });   // 404

        // Regla de negocio 2: no se elimina un dueño que tiene mascotas
        var tieneMascotas = await _context.Mascotas.AnyAsync(m => m.DuenoId == id);
        if (tieneMascotas)
            return Conflict(new { mensaje = "No se puede eliminar el dueño porque tiene mascotas registradas." });  // 409

        _context.Duenos.Remove(dueno);
        await _context.SaveChangesAsync();
        return NoContent();                                  // 204
    }
}