namespace backaramis.Modelsdtos.Recibos
{
    public class EventoDto
    {
        public List<Evento> events { get; set; }

    }

    public class Evento
    {
        public string payment_intent_id { get; set; }
        public string status { get; set; }
        public string created_on { get; set; }
    }

}
