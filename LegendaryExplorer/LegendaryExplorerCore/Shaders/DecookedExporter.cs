using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.Packages.CloningImportingAndRelinking;
using LegendaryExplorerCore.Unreal;
using LegendaryExplorerCore.Unreal.ObjectInfo;

namespace LegendaryExplorerCore.Shaders
{
    public static class DecookedExporter
    {
        /// <summary>
        /// Converts a decooked object back into cooked form. Returns the ported export. You can then port this object to whatever you need.
        /// </summary>
        /// <param name="targetPackage">Package to place object into. If not specified, a temporary memory package is generated instead</param>
        /// <param name="sourceObject">The object to port out of the package. It should be from a decooked package.</param>
        /// <returns></returns>
        public static IEntry CookObjectToPackage(ExportEntry sourceObject, IMEPackage targetPackage = null, Action<ExportEntry> specialAction = null)
        {
            IMEPackage result = targetPackage ?? MEPackageHandler.CreateMemoryEmptyPackage($@"{sourceObject.ObjectName.Instanced}.pcc", sourceObject.Game);

            var rop = new RelinkerOptionsPackage(new PackageCache())
            {
                PortImportsMemorySafe = true
            };

            EntryExporter.ExportExportToPackage(sourceObject, result, out var portedExp, null, rop);
            if (portedExp is ExportEntry exp)
            {
                specialAction?.Invoke(exp);
            }

            return portedExp;
        }

    }
}
