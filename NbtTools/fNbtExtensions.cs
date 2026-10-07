using fNbt;

namespace NbtTools
{
    // Provides extension methods to facilitate transitioning from SharpNBT to fNbt.
    internal static class fNbtExtensions
    {
        public static bool ContainsKey(this NbtCompound compound, string key)
        {
            return compound[key] != null;
        }
    }
}
