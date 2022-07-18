using backaramis.Modelsdtos.Recibos;

namespace backaramis.Interfaces
{
    public interface IRecibosService
    {
        Task<ReciboDto> Insert(ReciboInsertDto Recibo);
        Task<PaymentIntentResponeDto> CreatePaymentIntent(PaymentIntentDto PaymentIntent,int id);
        Task<CancelIntentPayDto> CancelPaymentIntent(string PaymentIntent,int id);
        Task<StateIntentPayDto> StatePaymentIntent(string paymentIntentId,int id);
    }
}
