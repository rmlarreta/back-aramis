namespace backaramis.Modelsdtos.Recibos
{
    public class PaymentIntentDto
    {
        public string Id { get; set; }
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }  
        public AddionalInfo AddionalInfo { get; set; }
        public int Ammount { get; set; }
    }

     public class AddionalInfo
    {
        public string External_reference { get; set; } 
        public bool Print_on_terminal { get; set; }
        public string Ticket_number { get; set; }
    }
}
