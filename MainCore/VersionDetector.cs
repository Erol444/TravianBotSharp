#if !(TRAVIAN_OFFICIAL || TTWARS)

#error You forgot to define Travian version here

#endif

namespace MainCore
{
    public static class VersionDetector
    {
        public static bool IsTravianOfficial()
        {
#if TRAVIAN_OFFICIAL
            return true;
#else
            return false;
#endif
        }

        public static bool IsTTWars()
        {
#if TTWARS
            return true;
#else
            return false;
#endif
        }
    }
}