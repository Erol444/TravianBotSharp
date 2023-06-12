using MainCore.DependencyInjector;
using MainCore.Enums;
using System;
using WPFUI.DependencyInjector;

namespace WPFUI.DependencyInjectior
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

        public static IInjector GetUIInjector() => new UIInjector();
    }
}