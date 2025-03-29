using System.ComponentModel.DataAnnotations;

public record BanUserDTO([Required] Guid Id, [Required] bool banStatus);