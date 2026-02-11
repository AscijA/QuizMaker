using System.ComponentModel.DataAnnotations;

namespace QuizMaker.Infrastructure;

public class DatabaseSettings {


    [Required(ErrorMessage = "Database connection string is missing!")]
    public string Default { get; set; } = string.Empty;
}