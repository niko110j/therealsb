namespace SB2.Models.ViewModels
{
    public class OrderListItem
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }

        public string SalespersonName { get; set; }

        public string FilledBy { get; set; }

        public string Status { get; set; }
        public string BookingType { get; set; }

        public DateTime Created { get; set; }
    }

}
