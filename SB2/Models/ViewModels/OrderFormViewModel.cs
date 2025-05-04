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
        public int? startDate { get; set; }

        public int? endDate { get; set; }

        public string? newspaper { get; set; }

        public string? placement { get; set; }

        public string? size { get; set; }
        public decimal? priceBySizeAndPlacement { get; set; }

        // --- Radio Order ---

        public string? producerNameAndEmail { get; set; }

        public string? radioArea { get; set; }

        public int? lengthInSeconds { get; set; }

        public int? totalPrice { get; set; }


        // --- Digital Order ---
        public string type { get; set; }

        public int selectedIssue {  get; set; }

        public string comments { get; set; }
        public decimal? DigitalBudget { get; set; }

        // Add other common or specific fields as needed
    }

}
