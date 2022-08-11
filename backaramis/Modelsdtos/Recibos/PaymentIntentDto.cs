namespace backaramis.Modelsdtos.Recibos
{
    public class PaymentIntentDto
    {
        public AddionalInfo? additional_info { get; set; }
        public int amount { get; set; }
    }

    public class AddionalInfo
    {
        public string? external_reference { get; set; }
        public bool print_on_terminal { get; set; }
        public string? ticket_number { get; set; }
    }
}
