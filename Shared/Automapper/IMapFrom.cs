using AutoMapper;

namespace Dipterv.Shared.Automapper
{
    public interface IMapFrom<T>
    {
        void Mapping(Profile profile)
            => profile.CreateMap(typeof(T), GetType());
    }
}
