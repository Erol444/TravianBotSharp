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