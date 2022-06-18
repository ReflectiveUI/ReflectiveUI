using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dto = ValuedTime.App.Models.Dto;
using ValuedTime.Domain.Abstractions;
using ValuedTime.Domain.Aggregates;

namespace ValuedTime.App.Services;

public class LifeValueService
{
    private readonly IRepository<LifeValue> _lifeValueRepository;
    private readonly IGuidFactory _guidFactory;

    public LifeValueService(IRepository<LifeValue> lifeValueRepository,
        IGuidFactory guidFactory)
    {
        _lifeValueRepository = lifeValueRepository;
        _guidFactory = guidFactory;
    }

    public async Task<Dto.LifeValue> AddLifeValue(Dto.CreateLifeValueCommand command)
    {
        var lv = await _lifeValueRepository.AddAsync(
            new LifeValue(new LifeValueId(_guidFactory.NewGuid()), command.Name));

        await _lifeValueRepository.SaveChangesAsync();

        return new Dto.LifeValue(lv.Id.Value, lv.Name);
    }

    public async Task<List<Dto.LifeValue>> GetAllLifeValues()
    {
        var values = await _lifeValueRepository.ListAsync();
        return values.Select(lv => new Dto.LifeValue(lv.Id.Value, lv.Name))
            .ToList();
    }
}
