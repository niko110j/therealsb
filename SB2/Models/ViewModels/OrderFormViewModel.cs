namespace SB2.Models.ViewModels
{
    public class OrderFormViewModel
    {
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientPhone { get; set; }

        public string SalespersonName { get; set; }
        public string FilledBy { get; set; }

        // Selected booking type
        public string SelectedBookingType { get; set; }

        // --- Print Order ---
        public int? PrintQuantity { get; set; }
        public decimal? PrintUnitPrice { get; set; }

        // --- Radio Order ---
        public DateTime? RadioBroadcastDate { get; set; }
        public int? RadioDuration { get; set; }

        // --- Digital Order ---
        public string DigitalPlatform { get; set; }
        public decimal? DigitalBudget { get; set; }

        // Add other common or specific fields as needed
    }

}
