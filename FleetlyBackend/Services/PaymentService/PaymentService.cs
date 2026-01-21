using Fleetly.Shared.Enums;
using FleetlyBackend.Data;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using Fleetly.Shared.Dto.InvoiceDtos;

namespace FleetlyBackend.Services.PaymentService
{
    public class PaymentService
    {
        private readonly IConfiguration _config;
        private readonly FleetlyContext _context;

        public PaymentService(IConfiguration config, FleetlyContext context)
        {
            _config = config;
            _context = context;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public async Task<PaymentInitResponseDto> CreateCheckoutSession(int invoiceId, string domain)
        {
            var invoice = await _context.Invoices.Include(i => i.Order).FirstOrDefaultAsync(i => i.Id == invoiceId);

            if (invoice == null || invoice.IsPaid)
            {
                throw new Exception("Faktura nie istnieje lub jest już opłacona.");
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    MethodOfPayment.Blik.ToString().ToLower(),
                    MethodOfPayment.Card.ToString().ToLower()
                },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(invoice.Sum * 100),
                            Currency = "pln",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = $"Faktura za zlecenie #{invoice.OrderId}",
                            },
                        },
                        Quantity = 1,
                    },
                },
                Mode = "payment",
                SuccessUrl = $"{domain}/payment-success?invoiceId={invoiceId}&session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{domain}/payment-failed",
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            invoice.StripeSessionId = session.Id;
            await _context.SaveChangesAsync();

            return new PaymentInitResponseDto
            {
                Url = session.Url,
                SessionId = session.Id
            };
        }

        public async Task<bool> VerifySessionPayment(string sessionId)
        {
            var service = new SessionService();
            var session = await service.GetAsync(sessionId);

            return session.PaymentStatus == "paid";
        }

        public async Task<string> GetSessionPaymentMethod(string sessionId)
        {
            var service = new SessionService();

            var options = new SessionGetOptions();
            options.AddExpand("payment_intent.payment_method");

            var session = await service.GetAsync(sessionId, options);

            var methodType = session.PaymentIntent?.PaymentMethod?.Type;

            return methodType ?? "card";
        }
    }
}
