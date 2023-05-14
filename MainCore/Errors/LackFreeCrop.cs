using FluentResults;

namespace MainCore.Errors
{
    public class LackFreeCrop : Error
    {
        public LackFreeCrop() : base("Cannot build because of lack of freecrop ( < 6 )")
        {
        }
    }
}