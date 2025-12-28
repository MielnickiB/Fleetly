using Fleetly.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Fleetly.Shared.Dto.OrderDtos
{
    public class OrderCreateDto : OrderBaseDto
    {
        [Required(ErrorMessage = "Typ zlecenia jest wymagany!")]
        public OrderType Type { get; set; }
    }
}
