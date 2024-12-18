using LegendaryExplorer.Misc.ExperimentsTools;
using LegendaryExplorer.UserControls.ExportLoaderControls;
using LegendaryExplorerCore.Dialogue;
using LegendaryExplorerCore.Helpers;
using LegendaryExplorerCore.Kismet;
using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.Unreal;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static LegendaryExplorer.Misc.ExperimentsTools.DialogueAutomations;
using static LegendaryExplorer.Misc.ExperimentsTools.PackageAutomations;
using static LegendaryExplorer.Misc.ExperimentsTools.SequenceAutomations;

namespace LegendaryExplorer.Mods
{
    public static class FemShepvBroShep_V
    {
        public static void Patch(IMEPackage pcc)
        {
            switch (pcc.FileNameNoExtension)
            {
                case "BioD_Cit002_000Global":
                    BioD_Cit002_000Global(pcc);
                    break;
                case "BioD_Cit002_700Exit":
                    BioD_Cit002_700Exit(pcc);
                    break;
                case "BioD_Cit002_700Exit_LOC_INT":
                    BioD_Cit002_700Exit_LOC_INT(pcc);
                    break;
                case "BioD_Cit003":
                    BiOD_Cit003(pcc);
                    break;
                case "BioD_Cit003_150AtriumConvo":
                    BioD_Cit003_150AtriumConvo(pcc);
                    break;
                case "BioD_Cit003_815Final_RR2":
                    BioD_Cit003_815Final_RR2(pcc);
                    break;
                case "BioD_Cit004_210CICIntro":
                    BioD_Cit004_210CICIntro(pcc);
                    break;
                case "BioD_Cit004_250Elevator":
                    BioD_Cit004_250Elevator(pcc);
                    break;
                case "BioD_Cit004_272MaleClone":
                    BioD_Cit004_272MaleClone(pcc);
                    break;
                case "BioD_Cit004_273FemClone":
                    BioD_Cit004_273FemClone(pcc);
                    break;
                case "BioD_Cit004_290FightScene":
                    BioD_Cit004_290FightScene(pcc);
                    break;
                case "BioP_Cit003":
                    BioP_Cit003(pcc);
                    break;
                case "BioP_Cit004":
                    BioP_Cit004(pcc);
                    break;
                default:
                    break;
            }
        }

        public static void BatchPatch()
        {
            string path = $@"G:\My Drive\Modding\Mass Effect\mods\Counter Clone\delivery\FemShep v BroShep Duel of the Shepards LE - Vanilla vs\DLC_MOD_FSvBSLE_V\CookedPCConsole";
            string[] files = Directory.GetFiles(path);


            foreach (string file in files)
            {
                if (Path.GetExtension(file) != ".pcc") { continue; }
                using MEPackage pcc = (MEPackage)MEPackageHandler.OpenMEPackage(file);
                Patch(pcc);
                pcc.Save();
            }
        }

        private static void BioD_Cit002_000Global(IMEPackage pcc)
        {
            string filename = "BioD_FSvBS_Dummies";
            AddStreamingKismet(pcc, filename);
            StreamFile(pcc, filename, "BioD_Cit002_700Exit");
        }

        private static void BioD_Cit002_700Exit(IMEPackage pcc)
        {
            InsertEventHandshake(pcc, pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.4-1_LeavingCasino"),
                pcc.GetUExport(3775), "RE_FSvBS_SetClone", "RE_FSvBS_CloneSet");
        }

        private static void BioD_Cit002_700Exit_LOC_INT(IMEPackage pcc)
        {
            // Remove the vanilla copy methoddeaddealer_mConv
            KismetHelper.RemoveOutputLinks(pcc.GetUExport(774));
            KismetHelper.RemoveOutputLinks(pcc.GetUExport(775));
            KismetHelper.RemoveVariableLinks(pcc.GetUExport(1602));
            KismetHelper.RemoveVariableLinks(pcc.GetUExport(1603));
        }

        private static void BiOD_Cit003(IMEPackage pcc)
        {
            SkipAndCleanSequenceElement(pcc.GetUExport(8318), null, 0);
            SkipAndCleanSequenceElement(pcc.GetUExport(8319), null, 0);

            (ExportEntry outEvent, ExportEntry _) = InsertEventHandshake(pcc, pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.Pawn_Handling.Set_Clone"),
                pcc.GetUExport(8462), "RE_FSvBS_SetClone", "RE_FSvBS_CloneSet");

            KismetHelper.ChangeOutputLink(pcc.GetUExport(8463), 0, 0, outEvent.UIndex);
        }

        private static void BioD_Cit003_150AtriumConvo(IMEPackage pcc)
        {
        }

        private static void BioD_Cit003_815Final_RR2(IMEPackage pcc)
        {
            InsertEventHandshake(pcc, pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.Research_Rooms_Floor3.RR_Shepard.Shepard_Wake"),
                pcc.GetUExport(4924), "RE_FSvBS_SetClone", "RE_FSvBS_CloneSet");

            SkipAndCleanSequenceElement(pcc.GetUExport(4916), null, 0);
            SkipAndCleanSequenceElement(pcc.GetUExport(4917), null, 0);
        }

        private static void BioD_Cit004_210CICIntro(IMEPackage pcc)
        {
            ExportEntry sequence = pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.Clone_Conversation");

            ExportEntry PMCheck = CreateSequenceObjectWithProps(pcc, "BioSeqAct_PMCheckState",
            [
                new IntProperty(17662, "m_nIndex"),
                new ArrayProperty<StrProperty>("m_aObjComment")
                {
                    new("Female Player?")
                }
            ]);
            ExportEntry setObjMale = CreateSequenceObjectWithProps(pcc, "SeqAct_SetObject", []);
            ExportEntry setObjFemale = CreateSequenceObjectWithProps(pcc, "SeqAct_SetObject", []);

            KismetHelper.AddObjectsToSequence(sequence, true, [PMCheck, setObjMale, setObjFemale]);

            ExportEntry stuntMale = pcc.GetUExport(3470);
            ExportEntry stuntFemale = pcc.GetUExport(3474);
            ExportEntry stuntEmpty = pcc.GetUExport(3476);
            ExportEntry teleport = pcc.GetUExport(3441);

            (ExportEntry outEvt, ExportEntry gate) = AddEventHandshake(pcc, sequence, "RE_FSvBS_SetClone", "RE_FSvBS_CloneSet");

            SkipAndCleanSequenceElement(pcc.GetUExport(3455), null, 0);

            // Disconnect the Teleport and connect it to the plot check
            KismetHelper.RemoveOutputLinks(teleport);
            KismetHelper.CreateOutputLink(teleport, "Out", PMCheck, 0);
            // Connect the plot check to the set objects
            KismetHelper.CreateOutputLink(PMCheck, "True", setObjFemale, 0);
            KismetHelper.CreateOutputLink(PMCheck, "False", setObjMale, 0);
            // Connec the set objects to the vars
            KismetHelper.CreateVariableLink(setObjMale, "Target", stuntEmpty);
            KismetHelper.CreateVariableLink(setObjMale, "Value", stuntMale);
            KismetHelper.CreateVariableLink(setObjFemale, "Target", stuntEmpty);
            KismetHelper.CreateVariableLink(setObjFemale, "Value", stuntFemale);
            // Connect the set objects to the handshake
            KismetHelper.CreateOutputLink(setObjMale, "Out", outEvt, 0);
            KismetHelper.CreateOutputLink(setObjFemale, "Out", outEvt, 0);
            // Connect the handshake to set active
            KismetHelper.CreateOutputLink(gate, "Out", pcc.GetUExport(146), 0);

            // Remove the reference to the old clone creation sequence
            ArrayProperty<ObjectProperty> sequenceObjects = sequence.GetProperty<ArrayProperty<ObjectProperty>>("SequenceObjects");
            sequenceObjects.TryRemove(objRef => objRef.Value == 3455, out ObjectProperty _);
            sequence.WriteProperty(sequenceObjects);
        }

        private static void BioD_Cit004_250Elevator(IMEPackage pcc)
        {
        }

        private static void BioD_Cit004_272MaleClone(IMEPackage pcc)
        {
            Edit_BioD_Cit004_27XClone(pcc, 12128, 12127, 12124, 12125, 11731, 12129, 12664, false);
        }

        private static void BioD_Cit004_273FemClone(IMEPackage pcc)
        {
            Edit_BioD_Cit004_27XClone(pcc, 12128, 12127, 12125, 12124, 11731, 12129, 12665, true);
        }

        private static void BioD_Cit004_290FightScene(IMEPackage pcc)
        {
            ExportEntry mainSequence = pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence");
            InsertEventHandshake(pcc, mainSequence, pcc.GetUExport(2413), "RE_FSvBS_SetPawnClone", "RE_FSvBS_CloneSet");
        }

        private static void BioP_Cit003(IMEPackage pcc)
        {
            string filename = "BioD_FSvBS_Dummies";
            AddStreamingKismet(pcc, filename);
            StreamFile(pcc, filename, "BioD_Cit003");
        }

        private static void BioP_Cit004(IMEPackage pcc)
        {
            string filename = "BioD_FSvBS_Dummies";
            AddStreamingKismet(pcc, filename);
            StreamFile(pcc, filename, "BioD_Cit004");
        }

        // DIALOGUES
        private static void Edit_BioD_Cit004_27XClone(IMEPackage pcc, int tint0Idx, int tint1Idx, int copyActor0Idx, int copyActor1Idx, int pawnObjIdx, int levelIsLiveIdx, int clonePawnIdx, bool isFemale)
        {
            ExportEntry sequence = pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence");

            // Remove the armor tinting and cloning for enemy spawning and replace with event handshake
            ExportEntry tintObj = pcc.GetUExport(tint0Idx);
            KismetHelper.SkipSequenceElement(tintObj, null, 0);
            KismetHelper.RemoveAllLinks(tintObj);

            (ExportEntry outEvt, _) = ReplaceObjectWithEventHandshake(pcc, sequence, pcc.GetUExport(copyActor0Idx), "RE_FSvBS_SetPawnClone", "RE_FSvBS_CloneSet");
            ArrayProperty<StructProperty> variableLinks = outEvt.GetProperty<ArrayProperty<StructProperty>>("VariableLinks");
            StructProperty varLink = variableLinks[0];
            StrProperty linkDesc = varLink.GetProp<StrProperty>("LinkDesc");
            linkDesc.Value = "ClonePawn";
            outEvt.WriteProperty(variableLinks);
            KismetHelper.CreateVariableLink(outEvt, "ClonePawn", pcc.GetUExport(pawnObjIdx));
            outEvt.WriteProperty(new ArrayProperty<StructProperty>([
                new StructProperty("RemoteEventParameter", false, [
                    new NameProperty("ClonePawn", "ParameterName"),
                    new EnumProperty(KismetVarTypes.KVT_Object.ToString(), "KismetVarTypes", pcc.Game, "VariableType")
                    ]
                )], "Parameters"));

            ExportEntry levelIsLive = pcc.GetUExport(levelIsLiveIdx);

            // Create the LevelIsLive event and connect it to the clone pawn object
            ExportEntry outEvtLiL = CreateSequenceObjectWithProps(pcc, "SeqAct_ActivateRemoteEvent", new() { new NameProperty("RE_FSvBS_SetPawnClone", "EventName") });
            ExportEntry clonePawnObj = CreateSequenceObjectWithProps(pcc, "SeqVar_Object", [new ObjectProperty(clonePawnIdx, "ObjValue")]);
            KismetHelper.AddObjectsToSequence(sequence, true, [outEvtLiL, clonePawnObj]);
            variableLinks = outEvtLiL.GetProperty<ArrayProperty<StructProperty>>("VariableLinks");
            varLink = variableLinks[0];
            linkDesc = varLink.GetProp<StrProperty>("LinkDesc");
            linkDesc.Value = "ClonePawn";
            outEvtLiL.WriteProperty(variableLinks);
            KismetHelper.CreateVariableLink(outEvtLiL, "ClonePawn", clonePawnObj);
            outEvtLiL.WriteProperty(new ArrayProperty<StructProperty>([
                new StructProperty("RemoteEventParameter", false, [
                    new NameProperty("ClonePawn", "ParameterName"),
                    new EnumProperty(KismetVarTypes.KVT_Object.ToString(), "KismetVarTypes", pcc.Game, "VariableType")
                    ]
                )], "Parameters"));

            // Remove the old creation code
            ArrayProperty<StructProperty> outputLinks = levelIsLive.GetProperty<ArrayProperty<StructProperty>>("OutputLinks");
            StructProperty outLink = outputLinks[0];
            ArrayProperty<StructProperty> links = outLink.GetProp<ArrayProperty<StructProperty>>("Links");
            links.Values = links.Skip(1).ToList();
            levelIsLive.WriteProperty(outputLinks);

            // Connect the LevelIsLive to the new clone creation code
            KismetHelper.CreateOutputLink(levelIsLive, "Out", outEvtLiL);

            KismetHelper.RemoveAllLinks(pcc.GetUExport(tint1Idx));
            KismetHelper.RemoveAllLinks(pcc.GetUExport(copyActor1Idx));

            // Remove the head and hair meshes from pawn objects
            if (isFemale)
            {
                pcc.GetUExport(12761).RemoveProperty("SkeletalMesh");
                pcc.GetUExport(12761).RemoveProperty("SkeletalMesh"); // For some reason, this needs to be done twice on them
                pcc.GetUExport(12761).RemoveProperty("Materials");
                pcc.GetUExport(12766).RemoveProperty("SkeletalMesh");
                pcc.GetUExport(12766).RemoveProperty("SkeletalMesh");
                pcc.GetUExport(12760).RemoveProperty("SkeletalMesh");
                pcc.GetUExport(12760).RemoveProperty("Materials");
                pcc.GetUExport(12765).RemoveProperty("SkeletalMesh");
            }
            else
            {
                pcc.GetUExport(12763).RemoveProperty("SkeletalMesh");
                pcc.GetUExport(12763).RemoveProperty("SkeletalMesh"); // For some reason, this needs to be done twice on them
                pcc.GetUExport(12767).RemoveProperty("SkeletalMesh");
                pcc.GetUExport(12767).RemoveProperty("SkeletalMesh");
                pcc.GetUExport(12766).RemoveProperty("SkeletalMesh");
            }
        }
    }
}
