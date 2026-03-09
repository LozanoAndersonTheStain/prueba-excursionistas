using AutoMapper;
using Excursionistas.Application.DTOs.Request;
using Excursionistas.Application.DTOs.Response;
using Excursionistas.Domain.Entities;
using Excursionistas.Domain.ValueObjects;

namespace Excursionistas.Application.Mappings;

/// <summary>
/// Perfil de AutoMapper encargado de definir las conversiones
/// entre entidades del dominio y DTOs utilizados en la aplicación.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ================================
        // Mapeos relacionados con Element
        // ================================

        // Convierte una entidad de dominio Element en un DTO de respuesta
        CreateMap<Element, ElementResponse>()
            .ForMember(dest => dest.CalorieEfficiency,
                opt => opt.MapFrom(src => src.CalorieEfficiency));

        // Convierte un DTO de creación en una entidad de dominio
        CreateMap<CreateElementRequest, Element>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // El ID lo genera la base de datos
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));

        // Convierte un DTO de actualización en una entidad existente
        CreateMap<UpdateElementRequest, Element>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // ================================
        // Mapeos relacionados con optimización
        // ================================

        // Convierte una entidad de dominio OptimizationResult en un DTO de respuesta
        CreateMap<OptimizationResult, OptimizationResultResponse>()
            .ForMember(dest => dest.SelectedElements,
                opt => opt.MapFrom(src => src.SelectedItems))
            .ForMember(dest => dest.ItemCount,
                opt => opt.MapFrom(src => src.ItemCount))
            .ForMember(dest => dest.AverageEfficiency,
                opt => opt.MapFrom(src => src.AverageEfficiency));
    }
}
