using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace VeterinariaApi.Models;

[Table("Duenos")]
[Index(nameof(Dni), Name = "UQ_Duenos_Dni", IsUnique = true)]
public partial class Dueno
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Los nombres deben tener entre 3 y 100 caracteres.")]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "El DNI es obligatorio.")]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "El DNI debe tener exactamente 8 dígitos.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI solo debe contener números.")]
    [Unicode(false)]
    public string Dni { get; set; } = null!;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [StringLength(15, ErrorMessage = "El teléfono no puede superar los 15 caracteres.")]
    [RegularExpression(@"^9\d{8}$", ErrorMessage = "El teléfono debe ser un celular de 9 dígitos que empiece con 9.")]
    [Unicode(false)]
    public string Telefono { get; set; } = null!;

    [StringLength(100, ErrorMessage = "El email no puede superar los 100 caracteres.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string? Email { get; set; }

    // [JsonIgnore] evita el ciclo Dueno -> Mascotas -> Dueno -> ... al convertir a JSON
    [JsonIgnore]
    [InverseProperty(nameof(Mascota.Dueno))]
    public virtual ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();
}