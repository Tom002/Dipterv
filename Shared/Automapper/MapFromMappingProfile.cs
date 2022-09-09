using AutoMapper;
using System.Reflection;

namespace Dipterv.Shared.Automapper
{
    public class MapFromMappingProfile : Profile
    {
        public MapFromMappingProfile(Assembly assembly)
        {
            ApplyMappingsFromAssembly(assembly);
        }

        private void ApplyMappingsFromAssembly(Assembly assembly)
        {
            Dictionary<Type, List<Type>> interfacesByTypes = assembly.GetExportedTypes()
                .Select(t => new { Type = t, Interfaces = t.GetInterfaces() })
                .Where(t => t.Interfaces.Any(i => IsIMapFrom(i)) && !t.Type.IsAbstract)
                .ToDictionary(t => t.Type, t => t.Interfaces.Where(i => IsIMapFrom(i)).ToList());

            foreach (KeyValuePair<Type, List<Type>> type in interfacesByTypes)
            {
                object instance = Activator.CreateInstance(type.Key);

                // default interfész metódusokat a sima GetMethod nem találja meg
                // többszörös IMapFrom megvalósítás kezelése
                foreach (Type @interface in type.Value)
                {
                    MethodInfo method = @interface.GetMethod(nameof(IMapFrom<object>.Mapping));
                    method.Invoke(instance, new object[] { this });
                }
            }
        }

        private bool IsIMapFrom(Type type) =>
            type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IMapFrom<>);
    }
}
