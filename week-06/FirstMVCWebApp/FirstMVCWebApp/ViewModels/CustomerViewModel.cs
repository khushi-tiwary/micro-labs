
using System.ComponentModel.DataAnnotations;

namespace CustomerMvcDemo.ViewModels;

public class CustomerViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, MinimumLength = 3,
        ErrorMessage = "Name must be between 3 and 50 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Enter a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Enter a valid phone number")]
    public string Phone { get; set; } = string.Empty;
}
