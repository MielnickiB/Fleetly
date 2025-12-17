using Fleetly.Shared.Dto;
using Fleetly.Shared.Dto.BrandModelDtos;
using FleetlyBackend.Data;
using FleetlyBackend.Mappings;
using FleetlyBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace FleetlyBackend.Services.BrandModelService
{
    public class BrandModelService(FleetlyContext context) : IBrandModelService
    {
        private readonly FleetlyContext _context = context;

        public async Task<PagedResult<BrandModelResponseDto>> GetAll()
        {
            var models = await _context.BrandModels
                .AsNoTracking()
                .Include(bm => bm.CarBrand)
                .OrderBy(bm => bm.ModelName)
                .Select(bm => bm.ToResponseDto())
                .ToListAsync();
            var totalCount = await _context.BrandModels.CountAsync();
            return new PagedResult<BrandModelResponseDto> { Items = models, TotalCount = totalCount };
        }

        public async Task<BrandModelResponseDto?> Get(int id)
        {
            var bm = await _context.BrandModels
                .Include(b => b.CarBrand)
                .FirstOrDefaultAsync(x => x.Id == id);

            return bm?.ToResponseDto();
        }

        public async Task<BrandModelResponseDto> Create(BrandModelCreateDto dto)
        {
            var exists = await _context.BrandModels
                .AnyAsync(m => m.ModelName == dto.ModelName && m.CarBrandId == dto.CarBrandId);

            if (exists)
                throw new InvalidOperationException("Model z taką nazwą już istnieje dla tej marki.");

            var brandExists = await _context.CarBrands.AnyAsync(b => b.Id == dto.CarBrandId);
            if (!brandExists)
                throw new InvalidOperationException("Podana marka nie istnieje.");

            var model = new BrandModel
            {
                ModelName = dto.ModelName,
                CarBrandId = dto.CarBrandId
            };

            _context.BrandModels.Add(model);
            await _context.SaveChangesAsync();

            return (await Get(model.Id))!;
        }

        public async Task<BrandModelResponseDto> Update(int id, BrandModelUpdateDto dto)
        {
            var model = await _context.BrandModels.FindAsync(id)
                ?? throw new ArgumentException("Nie znaleziono modelu.");

            var brandExists = await _context.CarBrands.AnyAsync(b => b.Id == dto.CarBrandId);
            if (!brandExists)
                throw new InvalidOperationException("Podana marka nie istnieje.");

            if (dto.ModelName is not null && dto.ModelName != model.ModelName)
            {
                bool exists = await _context.BrandModels
                    .AnyAsync(m =>
                        m.ModelName == dto.ModelName &&
                        m.CarBrandId == dto.CarBrandId &&
                        m.Id != id);

                if (exists)
                    throw new InvalidOperationException("Taki model już istnieje dla tej marki.");

                model.ModelName = dto.ModelName;
            }


            model.CarBrandId = dto.CarBrandId;

            model.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return (await Get(model.Id))!;
        }

        public async Task<bool> Deactivate(int id)
        {
            var model = await _context.BrandModels.FindAsync(id) ?? throw new ArgumentException("Nie znaleziono modelu.");

            if (!model.IsActive) throw new InvalidOperationException("Model jest już nieaktywny.");

            model.IsActive = false;
            model.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
