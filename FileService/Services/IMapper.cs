using AutoMapper;

namespace FileService.Services
{
    public interface IMapper<in TSource, TDestination>
    {
        TDestination Map(TSource source);
    }

    internal class Mapper<TSource, TDestination> : IMapper<TSource, TDestination>
    {
        private readonly IMapper mapper;

        public Mapper(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public TDestination Map(TSource source)
        {
            return mapper.Map<TSource, TDestination>(source);
        }
    }
}