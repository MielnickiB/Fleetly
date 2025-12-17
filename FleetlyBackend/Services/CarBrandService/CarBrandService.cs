using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.CarBrandDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.CarBrandService
{
    public class CarBrandService(FleetlyContext context) : ICarBrandService
    {
        private readonly FleetlyContext _context = context;

        public async Task<PagedResult<CarBrandResponseDto>> GetAll()
        {
            var totalCount = await _context.CarBrands.CountAsync();
            var brands = await _context.CarBrands
                .AsNoTracking()
                .OrderBy(b => b.BrandName)
                .Select(b => b.ToResponseDto())
                .ToListAsync();
            return new PagedResult<CarBrandResponseDto> { Items = brands, TotalCount = totalCount };
        }

        public async Task<CarBrandResponseDto?> GetById(int id)
        {
            var brand = await _context.CarBrands.FindAsync(id);
            return brand?.ToResponseDto();
        }

        public async Task<CarBrandResponseDto> Create(CarBrandCreateDto dto)
        {
            var exists = await BrandExists(dto.BrandName);
            if (exists)
                throw new InvalidOperationException("Podana marka już istnieje.");

            var brand = new CarBrand
            {
                BrandName = dto.BrandName
            };

            _context.CarBrands.Add(brand);
            await _context.SaveChangesAsync();

            return brand.ToResponseDto();
        }

        public async Task<CarBrandResponseDto> Update(int id, CarBrandUpdateDto dto)
        {
            var brand = await _context.CarBrands.FindAsync(id)
                ?? throw new ArgumentException("Nie znaleziono marki.");

            if (dto.BrandName is not null && !string.Equals(brand.BrandName, dto.BrandName, StringComparison.OrdinalIgnoreCase))
            {
                var exists = await BrandExists(dto.BrandName);
                if (exists)
                    throw new InvalidOperationException("Podana marka już istnieje.");

                brand.BrandName = dto.BrandName;
            }

            brand.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return brand.ToResponseDto();
        }

        public async Task<bool> Deactivate(int id)
        {
            var brand = await _context.CarBrands.FindAsync(id) ?? throw new ArgumentException("Nie znaleziono marki.");

            if (!brand.IsActive) throw new InvalidOperationException("Marka jest już nieaktywna.");

            brand.IsActive = false;
            brand.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<bool> BrandExists(string brandName)
        {
            return await _context.CarBrands
                .AnyAsync(b => b.BrandName.ToLower().Equals(brandName.ToLower()));
        }
    }
}
