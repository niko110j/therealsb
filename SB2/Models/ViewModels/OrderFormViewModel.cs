using System.ComponentModel.DataAnnotations;

namespace SB2.Models.ViewModels
{
    public class OrderFormViewModel
    {
        public int Id { get; set; }
        // General order information
        [Required]
        public string ClientName { get; set; }

        [Required]
        [EmailAddress]
        public string ClientEmail { get; set; }

        public string? ClientPhone { get; set; }

        public string? SalespersonName { get; set; }

        public string? FilledBy { get; set; }

        public string? Status { get; set; }

        // Booking type (e.g., "print", "radio", etc.)
        
        public string? BookingType { get; set; }

        // Dynamic booking-specific fields from the partial form
        public Dictionary<string?, string?> BookingFields { get; set; } = new();

        public DateTime Created { get; set; }
    }

}
