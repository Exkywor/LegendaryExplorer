using DocumentFormat.OpenXml.Office2010.ExcelAc;
using LegendaryExplorer.Tools.Sequence_Editor;
using LegendaryExplorerCore.Kismet;
using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.Packages.CloningImportingAndRelinking;
using LegendaryExplorerCore.Unreal;
using System.Collections.Generic;
using System.Linq;
using static LegendaryExplorer.Misc.ExperimentsTools.SharedMethods;
using static LegendaryExplorer.Misc.ExperimentsTools.PackageAutomations;
using static LegendaryExplorer.Misc.ExperimentsTools.DialogueAutomations;
using static LegendaryExplorer.Misc.ExperimentsTools.SequenceAutomations;

namespace LegendaryExplorer.Mods
{
    public static class Audemus
    {
        public static void LE1Doors(SequenceEditorWPF sew)
        {
            if (sew.Pcc == null || sew.SelectedSequence == null) { return; }

            int INERT_UINDEX = 0;
            int TRIGGER_UINDEX = 0;

            IMEPackage pcc = sew.Pcc;

            ExportEntry sourceInert = pcc.GetUExport(INERT_UINDEX);

            if (sourceInert == null)
            {
                ShowError("The provided uindex does not match a BioInert export.");
                return;
            }

            ExportEntry originatorTrigger = pcc.GetUExport(TRIGGER_UINDEX);

            if (originatorTrigger == null)
            {
                ShowError("The provided uindex does not match a BioInert export.");
                return;
            }

            List<ExportEntry> newObjects = new();

            // CREATE INERTS
            ExportEntry inert1 = EntryCloner.CloneTree(sourceInert);
            ExportEntry inert2 = EntryCloner.CloneTree(sourceInert);

            // Write basic template properties to be tweaked manually
            PropertyCollection inertProps = sourceInert.GetProperties();
            inertProps.AddOrReplaceProp(
                new StructProperty("Vector", new PropertyCollection() { new FloatProperty(0, "X"), new FloatProperty(0, "Y"), new FloatProperty(0, "Z") },
                "Location", true) );
            inertProps.AddOrReplaceProp(new NameProperty("", "Tag"));
            inertProps.AddOrReplaceProp(new BoolProperty(false, "bCollideActors"));
            inertProps.AddOrReplaceProp(new BoolProperty(false, "bCollideWorld"));
            inertProps.AddOrReplaceProp(new BoolProperty(true, "bHidden"));
            inert1.WriteProperties(inertProps);
            inert2.WriteProperties(inertProps);

            // CREATE SEQUENCE OBJECTS TO REFERENCE THOSE INERTS
            ExportEntry poiA_Ref = SequenceObjectCreator.CreateSequenceObject(pcc, "SeqVar_Object");
            ExportEntry poiB_Ref = SequenceObjectCreator.CreateSequenceObject(pcc, "SeqVar_Object");
            poiA_Ref.WriteProperty(new ObjectProperty(inert1, "ObjValue"));
            poiB_Ref.WriteProperty(new ObjectProperty(inert2, "ObjValue"));



        }
    }
}
