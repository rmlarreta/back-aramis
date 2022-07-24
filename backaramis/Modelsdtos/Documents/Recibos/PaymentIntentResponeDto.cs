namespace backaramis.Modelsdtos.Recibos
{
    
    public class PaymentIntentResponeDto
    {
        public AddionalInfo? additional_info { get; set; }
        public int amount { get; set; }
        public string? id { get; set; }
        public string? device_id { get; set; }
    }
}
