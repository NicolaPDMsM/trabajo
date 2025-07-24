namespace U1_evaluacion_sumativa.Helpers;
using System.Reflection;
using System.Runtime.Loader;

public class CustomAssemblyLoadContext : AssemblyLoadContext
{
    public IntPtr LoadUnmanagedLibrary(string absolutePath)
    {
        return LoadUnmanagedDllFromPath(absolutePath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        return null;
    }
}
