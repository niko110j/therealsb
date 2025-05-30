using NPoco;

[TableName("Orders")]
[PrimaryKey("Id", AutoIncrement = true)]
public class Order
{
    public int Id { get; set; }
    public string ClientName { get; set; }
    public string ClientEmail { get; set; }

    public string SalespersonName { get; set; }

    public string FilledBy { get; set; }

    public string Status { get; set; }
    public string BookingType { get; set; }

    // We'll store a JSON string of dynamic fields here
    public string BookingFields { get; set; }

    public DateTime Created { get; set; }
}
