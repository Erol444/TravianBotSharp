#if !(TRAVIAN_OFFICIAL || TTWARS)

#error You forgot to define Travian version here

#endif

using MainCore.Enums;

namespace MainCore
{
    public static class VersionDetector
    {
        public static VersionEnums GetVersion()
        {
#if TRAVIAN_OFFICIAL
            return VersionEnums.TravianOfficial;
#elif TTWARS
            return VersionEnums.TTWars;
#endif
        }
    }
}