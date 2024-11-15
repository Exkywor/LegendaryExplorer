using System;
using System.Collections.Generic;
using System.Linq;
using LegendaryExplorerCore.Gammtek.IO;
using LegendaryExplorerCore.Helpers;
using LegendaryExplorerCore.Unreal;
using LegendaryExplorerCore.Unreal.BinaryConverters;
using static LegendaryExplorerCore.Unreal.UnrealFlags;

namespace LegendaryExplorerCore.Packages
{
    public class PackageDiff
    {
        public IMEPackage PackageA;
        public IMEPackage PackageB;

        public readonly List<IEntry> AOnlyEntries = [];
        public readonly List<IEntry> BOnlyEntries = [];

        public readonly List<string> AOnlyNames = [];
        public readonly List<string> BOnlyNames = [];

        public readonly List<EntryDiff> ChangedEntries = [];

        private PackageDiff(){}

        public class EntryDiff(IEntry a, IEntry b) : IDiff<IEntry>
        {
            public virtual IEntry A { get; } = a;
            public virtual IEntry B { get; } = b;
            public bool IsDifferent { get; protected init; } = true;
        }

        public class ImportDiff : EntryDiff, IDiff<ImportEntry>
        {
            public override ImportEntry A => (ImportEntry)base.A;
            public override ImportEntry B => (ImportEntry)base.B;

            public ImportDiff(ImportEntry a, ImportEntry b) : base(a, b)
            {
                PackageFileNameDiff = new Diff<NameReference>(new NameReference(a.PackageFile, a.PackageFileNameNumber), new NameReference(b.PackageFile, b.PackageFileNameNumber));
                ClassNameDiff = new Diff<NameReference>(new NameReference(a.ClassName, a.ClassNameNumber), new NameReference(b.ClassName, b.ClassNameNumber));
                LinkDiff = new Diff<string>(a.ParentInstancedFullPath, b.ParentInstancedFullPath, StringComparer.OrdinalIgnoreCase);
                ObjectNameDiff = new Diff<NameReference>(a.ObjectName, b.ObjectName);

                IsDifferent = PackageFileNameDiff.IsDifferent | ClassNameDiff.IsDifferent | LinkDiff.IsDifferent | ObjectNameDiff.IsDifferent;
            }

            public Diff<NameReference> PackageFileNameDiff;
            public Diff<NameReference> ClassNameDiff;
            public Diff<string> LinkDiff;
            public Diff<NameReference> ObjectNameDiff;
        }

        public class ExportDiff : EntryDiff, IDiff<ExportEntry>
        {
            public override ExportEntry A => (ExportEntry)base.A;
            public override ExportEntry B => (ExportEntry)base.B;
            
            public readonly Diff<string> SuperClassDiff;
            public readonly Diff<string> LinkDiff;
            public readonly Diff<NameReference> ObjectNameDiff;
            public readonly Diff<string> ArchetypeDiff;
            public readonly Diff<EObjectFlags> ObjectFlagsDiff;
            public readonly Diff<EExportFlags> ExportFlagsDiff;
            //public readonly Diff<Guid> PackageGuidDiff;
            public readonly Diff<EPackageFlags> PackageFlagsDiff;
            public readonly Diff<int> NetIndexDiff;

            public readonly PropertyCollection DiffedProps;

            public readonly List<(Diff<NameReference>, string)> BinNameDiffs;
            public readonly List<(Diff<string>, string)> BinEntryDiffs;
            public readonly bool HasMiscBinDiffs;

            public ExportDiff(ExportEntry a, ExportEntry b) : base(a, b)
            {
                IMEPackage aPcc = a.FileRef;
                IMEPackage bPcc = b.FileRef;

                SuperClassDiff = CreateEntryDiff(a.SuperClass, b.SuperClass);
                LinkDiff = CreateEntryDiff(a.Parent, b.Parent);
                ObjectNameDiff = new Diff<NameReference>(a.ObjectName, b.ObjectName);
                ArchetypeDiff = CreateEntryDiff(a.Archetype, b.Archetype);
                ObjectFlagsDiff = new Diff<EObjectFlags>(a.ObjectFlags, b.ObjectFlags);
                ExportFlagsDiff = new Diff<EExportFlags>(a.ExportFlags, b.ExportFlags);
                //PackageGuidDiff = new Diff<Guid>(a.PackageGUID, b.PackageGUID);
                PackageFlagsDiff = new Diff<EPackageFlags>(a.PackageFlags, b.PackageFlags);
                NetIndexDiff = new Diff<int>(a.NetIndex, b.NetIndex);

                BinEntryDiffs = [];
                BinNameDiffs = [];

                var aData = a.DataReadOnly;
                var bData = b.DataReadOnly;

                if (a.HasStack != b.HasStack || a.TemplateOwnerClassIdx != b.TemplateOwnerClassIdx)
                {
                    HasMiscBinDiffs = true;
                }
                else if (a.HasStack)
                {
                    StateFrame aStack = StateFrame.FromExport(a);
                    StateFrame bStack = StateFrame.FromExport(b);
                    AddIfDiff(BinEntryDiffs, "Stack Node", CreateEntryDiff(aPcc.GetEntry(aStack.Node), bPcc.GetEntry(bStack.Node)));
                    AddIfDiff(BinEntryDiffs, "Stack StateNode", CreateEntryDiff(aPcc.GetEntry(aStack.StateNode), bPcc.GetEntry(bStack.StateNode)));
                }
                else if (a.TemplateOwnerClassIdx >= 0)
                {
                    int aToci = a.TemplateOwnerClassIdx;
                    int bToci = b.TemplateOwnerClassIdx;
                    AddIfDiff(BinEntryDiffs, "TemplateOwnerClass", CreateEntryDiff(aPcc.GetEntry(EndianReader.ToInt32(aData[aToci..], aPcc.Endian)), bPcc.GetEntry(EndianReader.ToInt32(bData[bToci..], bPcc.Endian))));
                    IEntry parent = a.Parent;
                    while (parent is not null)
                    {
                        if (parent is ExportEntry { IsDefaultObject: true })
                        {
                            AddIfDiff(BinNameDiffs, "TemplateName", new Diff<NameReference>(EndianReader.ToName(aData[(aToci + 4)..], aPcc), EndianReader.ToName(aData[(aToci + 4)..], aPcc)));
                            break;
                        }
                        parent = parent.Parent;
                    }
                }

                PropertyCollection aProps = a.GetProperties();
                PropertyCollection bProps = b.GetProperties();

                bool ObjectComparer(int aUIdx, int bUIdx) => string.Equals(aPcc.GetEntry(aUIdx)?.InstancedFullPath ?? "", bPcc.GetEntry(bUIdx)?.InstancedFullPath ?? "", StringComparison.OrdinalIgnoreCase);

                DiffedProps = aProps.Diff(bProps, objectComparer: ObjectComparer);
                foreach (Property bProp in bProps)
                {
                    if (aProps.GetProp<Property>(bProp.Name, bProp.StaticArrayIndex) is null)
                    {
                        DiffedProps.Add(bProp.DeepClone());
                    }
                }

                if (ObjectBinary.From(a) is ObjectBinary aBin)
                {
                    ObjectBinary bBin = ObjectBinary.From(b);
                    var aUIndexes = new Dictionary<string, int>();
                    var bUIndexes = new Dictionary<string, int>();
                    //using a.Game for both is intentional. We're not interested in differences between games
                    aBin.ForEachUIndex(a.Game, new UIndexAndPropNameCollectorAndZeroer(aUIndexes));
                    bBin.ForEachUIndex(a.Game, new UIndexAndPropNameCollectorAndZeroer(bUIndexes));

                    foreach ((string propName, int aUIndex) in aUIndexes)
                    {
                        if (bUIndexes.Remove(propName, out int bUIndex))
                        {
                            AddIfDiff(BinEntryDiffs, propName, CreateEntryDiff(aPcc.GetEntry(aUIndex), bPcc.GetEntry(bUIndex)));
                        }
                        else
                        {
                            BinEntryDiffs.Add(CreateEntryDiff(aPcc.GetEntry(aUIndex), null), propName);
                        }
                    }
                    foreach ((string propName, int bUIndex) in bUIndexes)
                    {
                        BinEntryDiffs.Add(CreateEntryDiff(null, bPcc.GetEntry(bUIndex)), propName);
                    }
                    //using a.Game for both is intentional. We're not interested in differences between games
                    var aNames = new Dictionary<string, NameReference>();
                    foreach ((NameReference aName, string propName) in aBin.GetNames(a.Game)) aNames[propName] = aName;
                    var bNames = new Dictionary<string, NameReference>();
                    foreach ((NameReference bName, string propName) in bBin.GetNames(a.Game)) bNames[propName] = bName;

                    foreach ((string propName, NameReference aName) in aNames)
                    {
                        if (bNames.Remove(propName, out NameReference bName))
                        {
                            AddIfDiff(BinNameDiffs, propName, new Diff<NameReference>(aName, bName));
                        }
                    }

                    //using aPcc for both is intentional, to force names to serialize the same, and to elimanate structural differences between game versions
                    var aStrippedBin = aBin.ToBytes(aPcc);
                    var bStrippedBin = bBin.ToBytes(aPcc);

                    if (!aStrippedBin.AsSpan().SequenceEqual(bStrippedBin))
                    {
                        HasMiscBinDiffs = true;
                    }
                }

                IsDifferent = SuperClassDiff.IsDifferent | LinkDiff.IsDifferent | ObjectNameDiff.IsDifferent | ArchetypeDiff.IsDifferent
                              | ObjectFlagsDiff.IsDifferent | ExportFlagsDiff.IsDifferent /*| PackageGuidDiff.IsDifferent*/ | PackageFlagsDiff.IsDifferent 
                              | DiffedProps.Count > 0 | BinNameDiffs.Count > 0 | BinEntryDiffs.Count > 0 | HasMiscBinDiffs;

                //if the files are from different games, this will always be different, so it's not meaningful
                if (a.Game == b.Game)
                {
                    IsDifferent |= NetIndexDiff.IsDifferent;
                }
            }
        }

        public readonly struct Diff<T> : IDiff<T>
        {
            public T A { get; }
            public T B { get; }
            public bool IsDifferent { get; }

            public Diff(T a, T b, IEqualityComparer<T> comparer = null)
            {
                A = a;
                B = b;
                IsDifferent = !(comparer ?? EqualityComparer<T>.Default).Equals(A, B);
            }
        }

        public interface IDiff<out T>
        {
            T A { get; }
            T B { get; }
            bool IsDifferent { get; }
        }

        private readonly struct UIndexAndPropNameCollectorAndZeroer(Dictionary<string, int> uindexAndPropNames) : IUIndexAction
        {
            public void Invoke(ref int uIndex, string propName)
            {
                uindexAndPropNames[propName] = uIndex;
                uIndex = 0;
            }
        }

        public static PackageDiff Create(IMEPackage packageA, IMEPackage packageB)
        {
            var packageDiff = new PackageDiff
            {
                PackageA = packageA,
                PackageB = packageB
            };

            List<string> aNameList = packageA.Names.ToList();

            EntryTree packageATree = packageA.Tree;
            EntryTree packageBTree = packageB.Tree;
            try
            {
                foreach (TreeNode<IEntry, int> root in packageATree.Roots)
                {
                    CompareEntryAndDescendants(root);
                    void CompareEntryAndDescendants(TreeNode<IEntry, int> node)
                    {
                        (IEntry entryA, List<int> children) = node;
                        if (packageB.FindEntry(entryA.InstancedFullPath) is not IEntry entryB)
                        {
                            packageDiff.AOnlyEntries.AddRange(packageATree.FlattenTreeOf(entryA));
                            return;
                        }
                        if (entryA is ExportEntry exportA)
                        {
                            if (entryB is ExportEntry exportB)
                            {
                                if (!exportA.ClassName.CaseInsensitiveEquals(exportB.ClassName))
                                {
                                    packageDiff.ChangedEntries.Add(new EntryDiff(entryA, entryB));
                                }
                                else
                                {
                                    var exportDiff = new ExportDiff(exportA, exportB);
                                    if (exportDiff.IsDifferent)
                                    {
                                        packageDiff.ChangedEntries.Add(exportDiff);
                                    }
                                }
                            }
                            else
                            {
                                packageDiff.ChangedEntries.Add(new EntryDiff(entryA, entryB));
                            }
                        }
                        else
                        {
                            var importA = (ImportEntry)entryA;
                            if (entryB is ImportEntry importB)
                            {
                                var importDiff = new ImportDiff(importA, importB);
                                if (importDiff.IsDifferent)
                                {
                                    packageDiff.ChangedEntries.Add(importDiff);
                                }
                            }
                            else
                            {
                                packageDiff.ChangedEntries.Add(new EntryDiff(entryA, entryB));
                            }
                        }
                        foreach (IEntry child in entryB.GetChildren())
                        {
                            if (packageA.FindEntry(child.InstancedFullPath) is null)
                            {
                                packageDiff.BOnlyEntries.AddRange(packageBTree.FlattenTreeOf(child));
                            }
                        }
                        foreach (int uIndex in children)
                        {
                            CompareEntryAndDescendants(packageATree[uIndex]);
                        }
                    }
                }
            }
            finally
            {
                //Diffing may have added names to packageA
                packageA.restoreNames(aNameList);
            }

            foreach (TreeNode<IEntry, int> root in packageBTree.Roots)
            {
                if (packageA.FindEntry(root.Data.InstancedFullPath) is null)
                {
                    packageDiff.BOnlyEntries.AddRange(packageBTree.FlattenTreeOf(root.Data));
                }
            }

            var aNames = new HashSet<string>(packageA.Names);
            var bNames = new HashSet<string>(packageB.Names);
            foreach (string name in bNames)
            {
                if (!aNames.Remove(name))
                {
                    packageDiff.BOnlyNames.Add(name);
                }
            }
            foreach (string name in aNames)
            {
                packageDiff.AOnlyNames.Add(name);
            }

            return packageDiff;
        }

        private static Diff<string> CreateEntryDiff(IEntry a, IEntry b)
        {
            return new Diff<string>(a?.InstancedFullPath ?? "", b?.InstancedFullPath ?? "", StringComparer.OrdinalIgnoreCase);
        }

        private static void AddIfDiff<T>(List<(Diff<T>, string)> list, string name, Diff<T> diff)
        {
            if (diff.IsDifferent)
            {
                list.Add(diff, name);
            }
        }
    }
}
