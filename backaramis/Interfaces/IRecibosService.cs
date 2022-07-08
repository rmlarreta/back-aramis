using backaramis.Modelsdtos.Recibos;

namespace backaramis.Interfaces
{
    public interface IRecibosService
    {
        Task<ReciboDto> Insert(ReciboInsertDto Recibo);
        Task<PaymentIntentDto> CreatePaymentIntent(PaymentIntentDto PaymentIntent);
        Task<CancelIntentPayDto> CancelPaymentIntent(CancelIntentPayDto PaymentIntent);
        Task<StateIntentPayDto> StatePaymentIntent(PaymentIntentDto StateIntentPayDto);
    }
}
