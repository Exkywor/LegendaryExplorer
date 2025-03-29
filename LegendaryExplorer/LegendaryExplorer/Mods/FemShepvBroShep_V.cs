using LegendaryExplorer.Misc.ExperimentsTools;
using LegendaryExplorer.UserControls.ExportLoaderControls;
using LegendaryExplorerCore.Dialogue;
using LegendaryExplorerCore.Helpers;
using LegendaryExplorerCore.Kismet;
using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.Packages.CloningImportingAndRelinking;
using LegendaryExplorerCore.Unreal;
using LegendaryExplorerCore.Unreal.BinaryConverters;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static LegendaryExplorer.Misc.ExperimentsTools.DialogueAutomations;
using static LegendaryExplorer.Misc.ExperimentsTools.PackageAutomations;
using static LegendaryExplorer.Misc.ExperimentsTools.SequenceAutomations;
using static LegendaryExplorer.Misc.ExperimentsTools.SharedMethods;

namespace LegendaryExplorer.Mods
{
    public static class FemShepvBroShep_V
    {
        public static readonly string ModPath = $@"G:\My Drive\Modding\Mass Effect\mods\Counter Clone\delivery\FemShep v BroShep Duel of the Shepards LE - Vanilla VS\DLC_MOD_FSvBSLE_V\CookedPCConsole";

        public static void Patch(IMEPackage pcc, string modName = "base")
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
                    BioD_Cit004_272MaleClone(pcc, modName);
                    break;
                case "BioD_Cit004_273FemClone":
                    BioD_Cit004_273FemClone(pcc, modName);
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

        public static void BatchPatch(string path = null, string modName = "base")
        {
            if (string.IsNullOrEmpty(path)) { path = ModPath; }

            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                if (Path.GetExtension(file) != ".pcc") { continue; }
                using MEPackage pcc = (MEPackage)MEPackageHandler.OpenMEPackage(file);
                Patch(pcc, modName);
                pcc.Save();
            }
        }

        public static void BatchPatchPatches()
        {
            foreach (var (modName, _) in FemShepvBroShep_V.Files_Patches)
            {
                string destPath = $"G:\\My Drive\\Modding\\Mass Effect\\mods\\Counter Clone\\delivery\\FemShep v BroShep Duel of the Shepards LE - Vanilla VS\\Patches\\{modName}\\";
                BatchPatch(destPath, modName);

                // Clean files
                destPath = $"G:\\My Drive\\Modding\\Mass Effect\\mods\\Counter Clone\\delivery\\FemShep v BroShep Duel of the Shepards LE - Vanilla VS\\Patches\\{modName}\\Clean\\";
                BatchPatch(destPath, modName);
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

        private static void BioD_Cit004_272MaleClone(IMEPackage pcc, string modName)
        {
            Edit_BioD_Cit004_27XClone(pcc, 12128, 12127, 12124, 12125, 11731, 12129, 12664, false, modName);
        }

        private static void BioD_Cit004_273FemClone(IMEPackage pcc, string modName)
        {
            Edit_BioD_Cit004_27XClone(pcc, 12128, 12127, 12125, 12124, 11731, 12129, 12665, true, modName);
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

        private static void Edit_BioD_Cit004_27XClone(IMEPackage pcc, int tint0Idx, int tint1Idx, int copyActor0Idx, int copyActor1Idx, int pawnObjIdx, int levelIsLiveIdx, int clonePawnIdx, bool isFemale, string modName)
        {
            if (modName == "CAT6")
            {
                clonePawnIdx = isFemale ? 12665 : 12669;

                string path = $@"G:\My Drive\Modding\Mass Effect\mods\Counter Clone\project\files for automatic patching\BioD_Cit004_272MaleClone_Clean.pcc";
                using MEPackage patchedPCC = (MEPackage)MEPackageHandler.OpenMEPackage(path);
                ExportEntry persistentLevel = (ExportEntry)GetPersistentLevel(pcc);

                // Import my file's StuntActor, because trying to get it to work in Sil's one is a headache
                EntryImporter.ImportAndRelinkEntries(EntryImporter.PortingOption.CloneAllDependencies, patchedPCC.FindExport($"TheWorld.PersistentLevel.SFXPawn_CloneM_0"), pcc,
                    persistentLevel, true, new RelinkerOptionsPackage(), out IEntry stuntActor);

                // Add the actor to the PersistentLevel
                Level levelBinary = ObjectBinary.From<Level>(persistentLevel);
                levelBinary.Actors.Remove(12138);
                levelBinary.Actors.Add(stuntActor.UIndex);
                persistentLevel.WriteBinary(levelBinary);
            }

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
                if (modName == "CAT6")
                {
                    pcc.GetUExport(12138).RemoveProperty("Tag"); // Remove tag from old pawn that we won't use
                    pcc.GetUExport(12778).RemoveProperty("SkeletalMesh");
                    pcc.GetUExport(12778).RemoveProperty("SkeletalMesh"); // For some reason, this needs to be done twice on them
                    pcc.GetUExport(12779).RemoveProperty("SkeletalMesh");
                    pcc.GetUExport(12779).RemoveProperty("SkeletalMesh");
                    pcc.GetUExport(12737).RemoveProperty("SkeletalMesh");
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

        public static readonly List<string> Files = [
            FN.BioD_Cit002_000Global,
            FN.BioD_Cit002_700Exit,
            FN.BioD_Cit002_700Exit_LOC_INT,
            FN.BioD_Cit003,
            //"BioD_Cit003_150AtriumConvo.pcc",
            FN.BioD_Cit003_815Final_RR2,
            FN.BioD_Cit004_210CICIntro,
            //"BioD_Cit004_250Elevator.pcc",
            FN.BioD_Cit004_272MaleClone,
            FN.BioD_Cit004_273FemClone,
            FN.BioD_Cit004_290FightScene,
            FN.BioP_Cit003,
            FN.BioP_Cit004
            ];

        public static readonly List<string> Files_Clean = [
            FN.BioD_Cit002_700Exit,
            FN.BioD_Cit003_815Final_RR2,
            FN.BioD_Cit003,
            FN.BioD_Cit004_210CICIntro,
            FN.BioD_Cit004_272MaleClone,
            FN.BioD_Cit004_273FemClone,
            ];

        public static readonly Dictionary<string, List<string>> Files_Patches = new()
        {
            {"BrooksWraith", [FN.BioD_Cit004_290FightScene]},
            //{"BrooksWraith_MiriMod", [FN.BioD_Cit004_290FightScene]},
            //{"BrooksWraith_ReducedPlot", [FN.BioD_Cit004_290FightScene]},
            //{"BrooksWraith_ReducedPlot_MiriMod", [FN.BioD_Cit004_290FightScene]},
            {"CAT6", [FN.BioD_Cit004_210CICIntro, FN.BioD_Cit004_272MaleClone, FN.BioD_Cit004_273FemClone]},
            {"LE3DP", [FN.BioD_Cit002_000Global, FN.BioD_Cit003, FN.BioD_Cit003_815Final_RR2]},
            //{"LE3DP_MiriMod", [FN.BioD_Cit003]},
            //{"LE3DP_VirSav", [FN.BioD_Cit003]},
            //{"LE3DP_VirSav_MiriMod", [FN.BioD_Cit003]},
            {"MiriMod", [FN.BioD_Cit002_700Exit_LOC_INT, FN.BioD_Cit003, FN.BioD_Cit004_290FightScene]},
            {"PV", [FN.BioD_Cit002_700Exit]},
            {"PV2", [FN.BioD_Cit002_000Global]},
            {"ReducedPlot", [FN.BioD_Cit004_290FightScene]},
            //{"ReducedPlot_MiriMod", [FN.BioD_Cit004_290FightScene]},
            {"VirSav", [FN.BioD_Cit003]},
            //{"VirSav_MiriMod", [FN.BioD_Cit003]},
        };

        public static readonly Dictionary<string, string> ModPaths = new()
        {
            {"BrooksWraith", "D:\\Programs\\ME3TweaksModManager\\mods\\LE3\\Brooks Uses The Wraith\\DLC_MOD_FEMME_WRAITH\\CookedPCConsole\\"},
            {"CAT6", "D:\\Programs\\ME3TweaksModManager\\mods\\LE3\\CAT6 Retrofits\\DLC_MOD_CAT6Retro\\CookedPCConsole\\"},
            {"LE3DP", "D:\\Programs\\ME3TweaksModManager\\mods\\LE3\\LE3 Diversification Project\\DLC_MOD_CITProject\\CookedPCConsole\\"},
            {"MiriMod", "D:\\Programs\\ME3TweaksModManager\\mods\\LE3\\Miranda Mod (LE3)\\Citadel\\"},
            {"PV", "D:\\Programs\\ME3TweaksModManager\\mods\\LE3\\Project Variety (LE3)\\DLC_MOD_ProjectVariety\\CookedPCConsole\\"},
            {"PV2", "D:\\Programs\\ME3TweaksModManager\\mods\\LE3\\Project Variety (LE3)\\DLC_MOD_ProjectVariety2\\CookedPCConsole\\"},
            {"ReducedPlot", "D:\\Programs\\ME3TweaksModManager\\mods\\LE3\\Reduced Plot Armor\\DLC_MOD_ReducedPlotArmor\\CookedPCConsole\\"},
            {"Squad", "D:\\Programs\\ME3TweaksModManager\\mods\\LE3\\EGM Squadmate Pack (Standalone)\\DLC_MOD_EGM_Squad\\CookedPCConsole\\"},
            {"VirSav", "D:\\Programs\\ME3TweaksModManager\\mods\\LE3\\Virmire Savior Mod\\DLC_MOD_VirmireSavior\\CookedPCConsole\\"},


            // Patches for mod combinations
            // NOTE: Commented out elements mean that a patch is needed, but the mod creators have not made one between the mods
            //{"BrooksWraith_MiriMod", ""},
            //{"BrooksWraith_ReducedPlot", ""},
            //{"BrooksWraith_ReducedPlot_MiriMod", ""},
            //{"LE3DP_MiriMod", ""}, ???
            //{"LE3DP_VirSav", "D:\\Programs\\ME3TweaksModManager\\mods\\LE3\\Virmire Savior Mod\\Compatibility\\LE3DP\\"}, ???
            //{"LE3DP_VirSav_MiriMod", ""}, ???
            //{"ReducedPlot_MiriMod", ""},
            //{"VirSav_MiriMod", ""},
        };

        public static class FN
        {
            public const string BioD_Cit002_000Global = "BioD_Cit002_000Global.pcc";
            public const string BioD_Cit002_700Exit = "BioD_Cit002_700Exit.pcc";
            public const string BioD_Cit002_700Exit_LOC_INT = "BioD_Cit002_700Exit_LOC_INT.pcc";
            public const string BioD_Cit003 = "BioD_Cit003.pcc";
            public const string BioD_Cit003_110Atrium_H_LOC_INT = "BioD_Cit003_110Atrium_H_LOC_INT.pcc";
            public const string BioD_Cit003_150AtriumConvo = "BioD_Cit003_150AtriumConvo.pcc";
            public const string BioD_Cit003_150AtriumConvo_LOC_INT = "BioD_Cit003_150AtriumConvo_LOC_INT.pcc";
            public const string BioD_Cit003_200HallEnter_LOC_INT = "BioD_Cit003_200HallEnter_LOC_INT.pcc";
            public const string BioD_Cit003_300TopMen_LOC_INT = "BioD_Cit003_300TopMen_LOC_INT.pcc";
            public const string BioD_Cit003_400Tubes_LOC_INT = "BioD_Cit003_400Tubes_LOC_INT.pcc";
            public const string BioD_Cit003_410Tubes_H_LOC_INT = "BioD_Cit003_410Tubes_H_LOC_INT.pcc";
            public const string BioD_Cit003_450Ladder_LOC_INT = "BioD_Cit003_450Ladder_LOC_INT.pcc";
            public const string BioD_Cit003_580MechDoor_LOC_INT = "BioD_Cit003_580MechDoor_LOC_INT.pcc";
            public const string BioD_Cit003_600MechEvent_LOC_INT = "BioD_Cit003_600MechEvent_LOC_INT.pcc";
            public const string BioD_Cit003_700FinalFloor_LOC_INT = "BioD_Cit003_700FinalFloor_LOC_INT.pcc";
            public const string BioD_Cit003_710Final_H_LOC_INT = "BioD_Cit003_710Final_H_LOC_INT.pcc";
            public const string BioD_Cit003_800FinalBldg_LOC_INT = "BioD_Cit003_800FinalBldg_LOC_INT.pcc";
            public const string BioD_Cit003_815Final_RR2 = "BioD_Cit003_815Final_RR2.pcc";
            public const string BioD_Cit003_850FinalBldg_fl2_LOC_INT = "BioD_Cit003_850FinalBldg_fl2_LOC_INT.pcc";
            public const string BioD_Cit003_900Trap = "BioD_Cit003_900Trap.pcc";
            public const string BioD_Cit003_900Trap_LOC_INT = "BioD_Cit003_900Trap_LOC_INT.pcc";
            public const string BioD_Cit004_100Exterior_LOC_INT = "BioD_Cit004_100Exterior_LOC_INT.pcc";
            public const string BioD_Cit004_210CICIntro = "BioD_Cit004_210CICIntro.pcc";
            public const string BioD_Cit004_210CICIntro_LOC_INT = "BioD_Cit004_210CICIntro_LOC_INT.pcc";
            public const string BioD_Cit004_220CIC_LOC_INT = "BioD_Cit004_220CIC_LOC_INT.pcc";
            public const string BioD_Cit004_250Elevator = "BioD_Cit004_250Elevator.pcc";
            public const string BioD_Cit004_250Elevator_LOC_INT = "BioD_Cit004_250Elevator_LOC_INT.pcc";
            public const string BioD_Cit004_260CloneIntro_LOC_INT = "BioD_Cit004_260CloneIntro_LOC_INT.pcc";
            public const string BioD_Cit004_270ShuttleBay1_LOC_INT = "BioD_Cit004_270ShuttleBay1_LOC_INT.pcc";
            public const string BioD_Cit004_272MaleClone = "BioD_Cit004_272MaleClone.pcc";
            public const string BioD_Cit004_273FemClone = "BioD_Cit004_273FemClone.pcc";
            public const string BioD_Cit004_290FightScene = "BioD_Cit004_290FightScene.pcc";
            public const string BioD_Cit004_290FightScene_LOC_INT = "BioD_Cit004_290FightScene_LOC_INT.pcc";
            public const string BioD_Cit004_295BrooksEnd_LOC_INT = "BioD_Cit004_295BrooksEnd_LOC_INT.pcc";
            public const string BioP_Cit003 = "BioP_Cit003.pcc";
            public const string BioP_Cit004 = "BioP_Cit004.pcc";
        }
    }
}
