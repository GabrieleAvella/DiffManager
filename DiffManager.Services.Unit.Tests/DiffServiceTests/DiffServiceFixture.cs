namespace DiffManager.Services.Unit.Tests.DiffServiceTests
{
    using System;

    using DiffManager.Models;
    using DiffManager.Services.DataContracts;

    public class DiffServiceFixture : IDisposable
    {
        public DiffServiceFixture()
        {
            AutoMapper.Mapper.Initialize(
                cfg =>
                    {
                        // Service data contracts to Entities
                        cfg.CreateMap<DifferenceForCreationDto, Difference>();
                        cfg.CreateMap<DifferenceForUpdateDto, Difference>();

                        // Service data contracts to Data contracts
                        cfg.CreateMap<DifferenceDto, DifferenceDto>();
                        cfg.CreateMap<DifferenceDto, DifferenceForUpdateDto>();

                        // Data contracts to Service data contracts
                        cfg.CreateMap<DifferenceForCreationDto, DifferenceForCreationDto>();
                        cfg.CreateMap<DifferenceForUpdateDto, DifferenceForUpdateDto>();

                        // Entities to Service data contracts
                        cfg.CreateMap<Difference, DifferenceDto>();
                    });
        }

        public void Dispose()
        {
        }
    }
}
