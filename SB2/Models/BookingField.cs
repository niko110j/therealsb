using NPoco;

[TableName("BookingFields")]
[PrimaryKey("Id", AutoIncrement = true)]
public class BookingField
{
    public int Id { get; set; }

    public int OrderId { get; set; }  // Foreign key to Order

    public string FieldKey { get; set; }

    public string FieldValue { get; set; }
}

