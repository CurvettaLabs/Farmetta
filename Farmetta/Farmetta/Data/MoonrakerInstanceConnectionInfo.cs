using System.ComponentModel.DataAnnotations.Schema;

namespace Farmetta.Data;

public class MoonrakerInstanceConnectionInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required Uri Uri { get; set; }
    
}