namespace Fleetly.Shared.Dto.BrandModelDtos
{
    public class BrandModelResponseDto
    {
        public int Id { get; set; }
        public string ModelName { get; set; } = null!;
        public int CarBrandId { get; set; }
        public string CarBrandName { get; set; } = null!;
    }
}
