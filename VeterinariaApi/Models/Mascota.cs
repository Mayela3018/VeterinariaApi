using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinariaApi.Models;

[Table("Mascotas")]
public partial class Mascota
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la mascota es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La especie es obligatoria.")]
    [StringLength(30, ErrorMessage = "La especie no puede superar los 30 caracteres.")]
    public string Especie { get; set; } = null!;

    [StringLength(50, ErrorMessage = "La raza no puede superar los 50 caracteres.")]
    public string? Raza { get; set; }

    [Range(0, 40, ErrorMessage = "La edad debe estar entre 0 y 40 años.")]
    public int Edad { get; set; }

    [Range(typeof(decimal), "0.01", "999.99",
        ParseLimitsInInvariantCulture = true,
        ConvertValueInInvariantCulture = true,
        ErrorMessage = "El peso debe estar entre 0.01 y 999.99 kg.")]
    [Column(TypeName = "decimal(5, 2)")]
    public decimal Peso { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un DuenoId válido.")]
    public int DuenoId { get; set; }

    // Admite nulo (Dueno?) para que en el POST solo se envíe el DuenoId
    [ForeignKey(nameof(DuenoId))]
    [InverseProperty(nameof(Models.Dueno.Mascotas))]
    public virtual Dueno? Dueno { get; set; }
}