﻿using AutoMapper;
using backaramis.Helpers;
using backaramis.Interfaces;
using backaramis.Models;
using backaramis.Modelsdtos.Recibos;
using Microsoft.AspNetCore.Mvc;

namespace backaramis.Controllers
{
    [Route("[controller]")]
    [ApiController]

    public class RecibosController : ControllerBase
    {
        private readonly IRecibosService _recibosService;
        private readonly IGenericService<Point> _genericPoint;
        private readonly ILoggService _loggService;
        private readonly IMapper _mapper;
        private readonly SecurityService _securityService;
        private readonly string _userName;

        public RecibosController(
            IRecibosService recibosService,
            IGenericService<Point> genericPoint,
            ILoggService loggService,
            IMapper mapper,
            SecurityService securityService
            )
        {
            _recibosService = recibosService;
            _genericPoint = genericPoint;
            _loggService = loggService;
            _mapper = mapper;
            _securityService = securityService;
            _userName = _securityService.GetUserAuthenticated();
        }

        [HttpPost("InsertRecibo")]
        public async Task<ReciboDto?> InsertRecibo([FromBody] ReciboInsertDto model)
        {

            model.Operador = _userName;
            model.Fecha = DateTime.Now;

            try
            {
                ReciboDto? data = await _recibosService.Insert(model);
                _loggService.Log($"InsertRecibo {data.Id}", "Recibos", "Insert", _userName);
                return data;
            }
            catch (Exception)
            {
                _loggService.Log($"ErrorInsertRecibo {model.Cliente}", "Recibos", "Insert", _userName);
                // return error message if there was an exception
                return null;
            }
        }


        [HttpPost]
        [Route("CreatePaymentIntent/{id}")]
        public async Task<PaymentIntentResponeDto> CreatePaymentIntent([FromBody] PaymentIntentDto paymentIntentDto, int id)
        {
            PaymentIntentResponeDto? data = await _recibosService.CreatePaymentIntent(paymentIntentDto, id);
            return data;

        }

        [HttpDelete]
        [Route("CancelPaymentIntent/{paymentIntent}/{id}")]
        public async Task<CancelIntentPayDto> CancelPaymentIntent(string paymentIntent, int id)
        {
            CancelIntentPayDto? data = await _recibosService.CancelPaymentIntent(paymentIntent, id);
            return data;

        }

        [HttpGet]
        [Route("StatePaymentIntent/{paymentIntent}/{id}")]
        public async Task<StateIntentPayDto> StatePaymentIntent(string paymentIntent, int id)
        {
            StateIntentPayDto? data = await _recibosService.StatePaymentIntent(paymentIntent, id);
            return data;

        }

        [HttpGet]
        [Route("GetRecibo/{id}")]
        public IActionResult GetRecibo(int id)
        {
            ReciboDto? data = _recibosService.GetRecibo(id);
            return Ok(data);

        }

        [HttpGet]
        [Route("GetPoints")]
        public IActionResult GetPoints()
        {
            List<Point>? data = _genericPoint.Get();
            return Ok(data);
        }

    }
}
