// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices
{
    internal static class MethodImpl
    {
        internal const MethodImplOptions AggressiveInlining =
#if NETSTANDARD  || NET45_OR_GREATER
            MethodImplOptions.AggressiveInlining;
#else
            (MethodImplOptions)256;
#endif
    }
}