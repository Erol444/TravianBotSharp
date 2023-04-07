using MainCore.DependencyInjector;
using MainCore.Enums;
using System;

namespace MainCore.DependencyInjectior
{
    public static class InjectorFactory
    {
        public static IInjector GetInjector(VersionEnums version)
        {
            return version switch
            {
                VersionEnums.TravianOfficial => new TravianOfficialInjector(),
                VersionEnums.TTWars => new TTWarsInjector(),
                _ => throw new Exception($"Version not supported {version}"),
            };
        }
    }
}