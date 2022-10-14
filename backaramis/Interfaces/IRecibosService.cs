using backaramis.Modelsdtos.Recibos;
using Microsoft.AspNetCore.Mvc;

namespace backaramis.Interfaces
{
    public interface IRecibosService
    {
        Task<int> Insert(ReciboInsertDto Recibo);
        Task<PaymentIntentResponeDto>? CreatePaymentIntent(PaymentIntentDto PaymentIntent, int id);
        Task<CancelIntentPayDto> CancelPaymentIntent(string PaymentIntent, int id);
        Task<StateIntentPayDto> StatePaymentIntent(string paymentIntentId, int id);
        Task Getpaymentintentlist(int id);
        FileStreamResult GetReciboReport(int id);
    }
}
