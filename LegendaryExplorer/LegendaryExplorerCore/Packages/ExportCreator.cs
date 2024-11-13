﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LegendaryExplorerCore.Misc;
using LegendaryExplorerCore.Packages.CloningImportingAndRelinking;
using LegendaryExplorerCore.Unreal;

namespace LegendaryExplorerCore.Packages
{
    public static class ExportCreator
    {
        // This is technically not an export, but it's next to the method that makes sense for it to be next to
        /// <summary>
        /// Creates a package import, if it doesn't already exist as an export.
        /// </summary>
        /// <param name="pcc"></param>
        /// <param name="packageName"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static ImportEntry CreatePackageImport(IMEPackage pcc, NameReference packageName, IEntry parent = null)
        {
            var testName = parent != null ? NameReference.FromInstancedString($"{parent.InstancedFullPath}.{packageName.Instanced}") : packageName;
            var testEntry = pcc.FindImport(testName, "Package");
            if (testEntry != null)
                return testEntry;

            var imp = new ImportEntry(pcc, parent, packageName)
            {
                ClassName = "Package",
                PackageFile = "Core",
            };

            pcc.AddImport(imp);
            return imp;
        }

        /// <summary>
        /// Creates a package export, if it doesn't already exist as an export.
        /// </summary>
        /// <param name="pcc"></param>
        /// <param name="packageName"></param>
        /// <param name="parent"></param>
        /// <param name="relinkResultsAvailable"></param>
        /// <returns></returns>
        public static ExportEntry CreatePackageExport(IMEPackage pcc, NameReference packageName, IEntry parent = null, Action<List<EntryStringPair>> relinkResultsAvailable = null, PackageCache cache = null, bool forcedExport = true)
        {
            var testName = parent != null ? NameReference.FromInstancedString($"{parent.InstancedFullPath}.{packageName.Instanced}") : packageName;
            var testEntry = pcc.FindExport(testName, "Package");
            if (testEntry != null)
                return testEntry;

            var rop = new RelinkerOptionsPackage { ImportExportDependencies = true, Cache = cache };
            var exp = new ExportEntry(pcc, parent, packageName)
            {
                Class = EntryImporter.EnsureClassIsInFile(pcc, "Package", rop)
            };
            relinkResultsAvailable?.Invoke(rop.RelinkReport);
            exp.ObjectFlags |= UnrealFlags.EObjectFlags.Public;
            if (forcedExport)
            {
                exp.ExportFlags |= UnrealFlags.EExportFlags.ForcedExport;
            }

            pcc.AddExport(exp);
            return exp;
        }

        public static ExportEntry CreateExport(IMEPackage pcc, NameReference name, string className, IEntry parent = null, Action<List<EntryStringPair>> relinkResultsAvailable = null,
            bool indexed = true,
            bool createWithStack = false,
            byte[] prePropBinary = null,
            PackageCache cache = null)
        {
            var rop = new RelinkerOptionsPackage() { ImportExportDependencies = true, Cache = cache };
            var exp = new ExportEntry(pcc, parent, indexed ? pcc.GetNextIndexedName(name) : name, prePropBinary)
            {
                Class = EntryImporter.EnsureClassIsInFile(pcc, className, rop)
            };
            if (parent is ImportEntry or ExportEntry { IsForcedExport: true })
            {
                // Import parents will always yield forced export since we are in a different package.
                exp.ExportFlags |= UnrealFlags.EExportFlags.ForcedExport;
            }

            relinkResultsAvailable?.Invoke(rop.RelinkReport);
            pcc.AddExport(exp);

            if (createWithStack)
            {
                exp.SetPrePropBinary(EntryImporter.CreateStack(pcc.Game, exp.Class.UIndex), true);
                exp.ObjectFlags |= UnrealFlags.EObjectFlags.HasStack;
            }
            return exp;
        }

        /// <summary>
        /// Creates an ObjectReferencer in the given package. If one exists, that is returned. If this package is marked as map, it will return null. Otherwise, a new one will be created.
        /// </summary>
        /// <param name="package">Package to create ObjectReferencer in</param>
        /// <param name="cache">Cache to speed up performance</param>
        /// <returns>ObjectReferencer in package; null if a map package</returns>
        public static ExportEntry CreateObjectReferencer(IMEPackage package, PackageCache cache = null)
        {
            if (package.Flags.HasFlag(UnrealFlags.EPackageFlags.Map))
            {
                Debug.WriteLine(@"Map packages do not use ObjectReferencer; to keep objects in memory, add root objects to ExtraReferencedObjects in TheWorld's binary.");
                return null;
            }

            var objRef = package.Exports.FirstOrDefault(x => x.ClassName == "ObjectReferencer" && !x.IsDefaultObject);
            if (objRef != null)
            {
                return objRef;
            }

            var rop = new RelinkerOptionsPackage() { Cache = cache };
            var referencer = new ExportEntry(package, 0, package.GetNextIndexedName("ObjectReferencer"), properties: [new ArrayProperty<ObjectProperty>("ReferencedObjects")])
            {
                Class = EntryImporter.EnsureClassIsInFile(package, "ObjectReferencer", rop)
            };
            package.AddExport(referencer);
            return referencer;
        }
    }
}
