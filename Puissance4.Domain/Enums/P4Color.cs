

namespace Puissance4.Domain.Enums
{
    public enum P4Color
    {
        None = 0, Red = 1, Yellow = -1
    }

    public static class P4ColorExtensions
    {
        public static P4Color Switch(this P4Color color)
        {
            return (P4Color)((int)color * -1);
        }
    }
}
