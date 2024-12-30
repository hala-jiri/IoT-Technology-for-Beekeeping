namespace ApiaryDataCollector
{
    using AutoMapper;
    using ApiaryDataCollector.Models;
    using ApiaryDataCollector.Models.DTO;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapování z ApiaryDto na Apiary (entita)
            CreateMap<ApiaryDto, Apiary>()
                .ForMember(dest => dest.ApiaryNumber, opt => opt.MapFrom(src => src.ApiaryNumber))
                .ForMember(dest => dest.Measurements, opt => opt.MapFrom(src => new List<ApiaryMeasurement>
                {
                new ApiaryMeasurement
                {
                    MeasurementDate = src.ReportDate, // Používáme ReportDate z ApiaryDto pro všechna měření pro Apiary
                    Humidity = src.Humidity,
                    Temperature = src.Temperature,
                    LightIntensity = src.LightIntensity
                }
                }))
                .ForMember(dest => dest.Hives, opt => opt.MapFrom(src => src.Hives)); // Hives budou mapovány níže

            // Mapování z HiveDto na Hive (entita)
            CreateMap<HiveDto, Hive>()
                .ForMember(dest => dest.HiveNumber, opt => opt.MapFrom(src => src.HiveNumber))
                .ForMember(dest => dest.Measurements, opt => opt.MapFrom((src, dest, _, context) =>
                {
                    // Tady dostáváme ApiaryDto (který je v contextu), abychom použili ReportDate
                    var apiaryDto = context.Items["ApiaryDto"] as ApiaryDto;
                    return new List<HiveMeasurement>
                    {
                    new HiveMeasurement
                    {
                        MeasurementDate = apiaryDto?.ReportDate ?? DateTime.UtcNow, // Používáme ReportDate z ApiaryDto nebo aktuální čas
                        Weight = src.Weight,
                        Temperature = src.Temperature,
                        Humidity = src.Humidity
                    }
                    };
                }))
                .ForMember(dest => dest.InspectionReports, opt => opt.MapFrom(src => new List<InspectionReport>())); // Inspekční zprávy, pokud jsou

        }
    }
}
