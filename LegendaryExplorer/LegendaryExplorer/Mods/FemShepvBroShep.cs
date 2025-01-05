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
    public static class FemShepvBroShep
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
                case "BioD_Cit003_110Atrium_H_LOC_INT":
                    BioD_Cit003_110Atrium_H_LOC_INT(pcc);
                    break;
                case "BioD_Cit003_150AtriumConvo":
                    BioD_Cit003_150AtriumConvo(pcc);
                    break;
                case "BioD_Cit003_150AtriumConvo_LOC_INT":
                    BioD_Cit003_150AtriumConvo_LOC_INT(pcc);
                    break;
                case "BioD_Cit003_200HallEnter_LOC_INT":
                    BioD_Cit003_200HallEnter_LOC_INT(pcc);
                    break;
                case "BioD_Cit003_300TopMen_LOC_INT":
                    BioD_Cit003_300TopMen_LOC_INT(pcc);
                    break;
                case "BioD_Cit003_400Tubes_LOC_INT":
                    BioD_Cit003_400Tubes_LOC_INT(pcc);
                    break;
                case "BioD_Cit003_410Tubes_H_LOC_INT":
                    BioD_Cit003_410Tubes_H_LOC_INT(pcc);
                    break;
                case "BioD_Cit003_450Ladder_LOC_INT":
                    BioD_Cit003_450Ladder_LOC_INT(pcc);
                    break;
                case "BioD_Cit003_580MechDoor_LOC_INT":
                    BioD_Cit003_580MechDoor_LOC_INT(pcc);
                    break;
                case "BioD_Cit003_600MechEvent_LOC_INT":
                    BioD_Cit003_600MechEvent_LOC_INT(pcc);
                    break;
                case "BioD_Cit003_700FinalFloor_LOC_INT":
                    BioD_Cit003_700FinalFloor_LOC_INT(pcc);
                    break;
                case "BioD_Cit003_710Final_H_LOC_INT":
                    BioD_Cit003_710Final_H_LOC_INT(pcc);
                    break;
                case "BioD_Cit003_800FinalBldg_LOC_INT":
                    BioD_Cit003_800FinalBldg_LOC_INT(pcc);
                    break;
                case "BioD_Cit003_815Final_RR2":
                    BioD_Cit003_815Final_RR2(pcc);
                    break;
                case "BioD_Cit003_850FinalBldg_fl2_LOC_INT":
                    BioD_Cit003_850FinalBldg_fl2_LOC_INT(pcc);
                    break;
                case "BioD_Cit003_900Trap":
                    BioD_Cit003_900Trap(pcc);
                    break;
                case "BioD_Cit003_900Trap_LOC_INT":
                    BioD_Cit003_900Trap_LOC_INT(pcc);
                    break;
                case "BioD_Cit004_100Exterior_LOC_INT":
                    BioD_Cit004_100Exterior_LOC_INT(pcc);
                    break;
                case "BioD_Cit004_210CICIntro":
                    BioD_Cit004_210CICIntro(pcc);
                    break;
                case "BioD_Cit004_210CICIntro_LOC_INT":
                    BioD_Cit004_210CICIntro_LOC_INT(pcc);
                    break;
                case "BioD_Cit004_220CIC_LOC_INT":
                    BioD_Cit004_220CIC_LOC_INT(pcc);
                    break;
                case "BioD_Cit004_250Elevator":
                    BioD_Cit004_250Elevator(pcc);
                    break;
                case "BioD_Cit004_250Elevator_LOC_INT":
                    BioD_Cit004_250Elevator_LOC_INT(pcc);
                    break;
                case "BioD_Cit004_260CloneIntro_LOC_INT":
                    BioD_Cit004_260CloneIntro_LOC_INT(pcc);
                    break;
                case "BioD_Cit004_270ShuttleBay1_LOC_INT":
                    BioD_Cit004_270ShuttleBay1_LOC_INT(pcc);
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
                case "BioD_Cit004_290FightScene_LOC_INT":
                    BioD_Cit004_290FightScene_LOC_INT(pcc);
                    break;
                case "BioD_Cit004_295BrooksEnd_LOC_INT":
                    BioD_Cit004_295BrooksEnd_LOC_INT(pcc);
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
            string path = $@"G:\My Drive\Modding\Mass Effect\mods\Counter Clone\delivery\FemShep v BroShep Duel of the Shepards LE\DLC_MOD_FSvBSLE\CookedPCConsole";

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
            // Prepare the conversation
            ExportEntry deaddealer_m = pcc.GetUExport(237);
            ConversationExtended deaddealer_mConv = GetLoadedConversation(deaddealer_m);

            // Swap the booleans and nodes
            ChangeAndWriteNodePlotCheck(deaddealer_m, GetNodeByIndex(deaddealer_mConv, 19, false), true, 71173000, -1);
            ChangeAndWriteNodePlotCheck(deaddealer_m, GetNodeByIndex(deaddealer_mConv, 21, false), true, 71173000, -1);
            ChangeAndWriteNodePlotCheck(deaddealer_m, GetNodeByIndex(deaddealer_mConv, 23, false), true, 71173000, -1);
            ChangeAndWriteNodePlotCheck(deaddealer_m, GetNodeByIndex(deaddealer_mConv, 25, false), true, 71173000, -1);
            ChangeAndWriteNodePlotCheck(deaddealer_m, GetNodeByIndex(deaddealer_mConv, 26, false), true, 71173000, -1);
            ChangeAndWriteNodePlotCheck(deaddealer_m, GetNodeByIndex(deaddealer_mConv, 29, false), true, 71173000, -1);

            //SwapAndWriteFirstTwoNodesAndPlots(deaddealer_m, deaddealer_mConv, GetNodeByIndex(deaddealer_mConv, 11, true));
            //SwapAndWriteFirstTwoNodesAndPlots(deaddealer_m, deaddealer_mConv, GetNodeByIndex(deaddealer_mConv, 12, true));
            //SwapAndWriteFirstTwoNodesAndPlots(deaddealer_m, deaddealer_mConv, GetNodeByIndex(deaddealer_mConv, 13, true));
            //SwapAndWriteFirstTwoNodesAndPlots(deaddealer_m, deaddealer_mConv, GetNodeByIndex(deaddealer_mConv, 16, true));
            //SwapAndWriteFirstTwoNodesAndPlots(deaddealer_m, deaddealer_mConv, GetNodeByIndex(deaddealer_mConv, 19, true));
            //SwapAndWriteFirstTwoNodesAndPlots(deaddealer_m, deaddealer_mConv, GetNodeByIndex(deaddealer_mConv, 20, true));
            //SwapAndWriteNodesPlotChecks(deaddealer_m, GetNodeByIndex(deaddealer_mConv, 26, false), GetNodeByIndex(deaddealer_mConv, 28, false));

            // Remove the vanilla copy methoddeaddealer_mConv
            KismetHelper.RemoveOutputLinks(pcc.GetUExport(774));
            KismetHelper.RemoveOutputLinks(pcc.GetUExport(775));
            KismetHelper.RemoveVariableLinks(pcc.GetUExport(1602));
            KismetHelper.RemoveVariableLinks(pcc.GetUExport(1603));
        }

        private static void BiOD_Cit003(IMEPackage pcc)
        {
            SequenceAutomations.ReplaceObject(
                pcc,
                pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.Pawn_Handling.Set_Clone"),
                CreateSequenceObjectWithProps(pcc, "BioSeqAct_PMCheckConditional",
                [
                    new IntProperty(71173000, "m_nIndex"),
                    new ArrayProperty<StrProperty>("m_aObjComment")
                    {
                        new("FSvBS: Is Not Female Player?")
                    }]),
                pcc.GetUExport(1270)
                );
            // SwapCheckStateOutputs(pcc.GetUExport(1270));

            SkipAndCleanSequenceElement(pcc.GetUExport(8318), null, 0);
            SkipAndCleanSequenceElement(pcc.GetUExport(8319), null, 0);

            (ExportEntry outEvent, ExportEntry _) = InsertEventHandshake(pcc, pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.Pawn_Handling.Set_Clone"),
                pcc.GetUExport(8462), "RE_FSvBS_SetClone", "RE_FSvBS_CloneSet");

            KismetHelper.ChangeOutputLink(pcc.GetUExport(8463), 0, 0, outEvent.UIndex);
        }

        private static void BioD_Cit003_110Atrium_H_LOC_INT(IMEPackage pcc)
        {
            Cit003_lobby_planning_b_dlg(pcc, pcc.GetUExport(1), 243, 244);
        }

        // NATIVE COMPATIBILITY
        private static void BioD_Cit003_150AtriumConvo(IMEPackage pcc)
        {
            ExportEntry sequence = pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence");
            PropertyCollection props = [
                new IntProperty(71173000, "m_nIndex"),
                new ArrayProperty<StrProperty>("m_aObjComment")
                { new("FSvBS: Is Not Female Player?") }
                ];

            SequenceAutomations.ReplaceObject(pcc, sequence, CreateSequenceObjectWithProps(pcc, "BioSeqAct_PMCheckConditional", props), pcc.GetUExport(168));
            SequenceAutomations.ReplaceObject(pcc, sequence, CreateSequenceObjectWithProps(pcc, "BioSeqAct_PMCheckConditional", props), pcc.GetUExport(167));

            // SwapCheckStateOutputs(pcc.GetUExport(168));
            // SwapCheckStateOutputs(pcc.GetUExport(167));
        }

        private static void BioD_Cit003_150AtriumConvo_LOC_INT(IMEPackage pcc)
        {
            Cit003_meet_clone_m(pcc, pcc.GetUExport(434), 2339, 2340, 2341, 2342);
        }

        private static void BioD_Cit003_200HallEnter_LOC_INT(IMEPackage pcc)
        {
            Cit003_lobby_planning_b_dlg(pcc, pcc.GetUExport(2), 258, 259);
        }

        private static void BioD_Cit003_300TopMen_LOC_INT(IMEPackage pcc)
        {
            Cit003_glyph_a(pcc, pcc.GetUExport(1), 633, 634);
            Cit003_mercs_tubes1_a(pcc, pcc.GetUExport(3), 693, 694, 695, 696);
        }

        private static void BioD_Cit003_400Tubes_LOC_INT(IMEPackage pcc)
        {
            Cit003_glyph_a(pcc, pcc.GetUExport(1), 484, 485);
            Cit003_mercs_tubes1_a(pcc, pcc.GetUExport(2), 519, 520, 521, 522);
            Cit003_tubes1_b(pcc.GetUExport(3));
        }

        private static void BioD_Cit003_410Tubes_H_LOC_INT(IMEPackage pcc)
        {
            Cit003_mercs_tubes1_a(pcc, pcc.GetUExport(1), 162, 163, 164, 165);
            Cit003_tubes1_b(pcc.GetUExport(2));
        }

        private static void BioD_Cit003_450Ladder_LOC_INT(IMEPackage pcc)
        {
            Cit003_hench_meetup_m(pcc, pcc.GetUExport(664), 1284, 1285);
        }

        private static void BioD_Cit003_580MechDoor_LOC_INT(IMEPackage pcc)
        {
            Cit003_glyph_a(pcc, pcc.GetUExport(1), 487, 488);
        }

        private static void BioD_Cit003_600MechEvent_LOC_INT(IMEPackage pcc)
        {
            Cit003_mech_event_merc_a(pcc, pcc.GetUExport(2), 176, 177, 178, 179);
        }

        private static void BioD_Cit003_700FinalFloor_LOC_INT(IMEPackage pcc)
        {
            Cit003_glyph_a(pcc, pcc.GetUExport(1), 442, 443);
        }

        private static void BioD_Cit003_710Final_H_LOC_INT(IMEPackage pcc)
        {
            Cit003_glyph_a(pcc, pcc.GetUExport(1), 496, 497);
        }

        // NATIVE COMPATIBILITY
        private static void BioD_Cit003_800FinalBldg_LOC_INT(IMEPackage pcc)
        {
            Cit003_mercs_building2_a(pcc.GetUExport(2));
        }

        private static void BioD_Cit003_815Final_RR2(IMEPackage pcc)
        {
            ExportEntry sequence = pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.Research_Rooms_Floor3.RR_Shepard.Shepard_Wake");
            PropertyCollection props = [
                new IntProperty(71173000, "m_nIndex"),
                new ArrayProperty<StrProperty>("m_aObjComment")
                { new("FSvBS: Is Not Female Player?") }
                ];

            InsertEventHandshake(pcc, sequence, pcc.GetUExport(4924), "RE_FSvBS_SetClone", "RE_FSvBS_CloneSet");

            SequenceAutomations.ReplaceObject(pcc, sequence, CreateSequenceObjectWithProps(pcc, "BioSeqAct_PMCheckConditional", props), pcc.GetUExport(173));
            SequenceAutomations.ReplaceObject(pcc, sequence, CreateSequenceObjectWithProps(pcc, "BioSeqAct_PMCheckConditional", props), pcc.GetUExport(174));

            // SwapCheckStateOutputs(pcc.GetUExport(173));
            // SwapCheckStateOutputs(pcc.GetUExport(174));
            SkipAndCleanSequenceElement(pcc.GetUExport(4916), null, 0);
            SkipAndCleanSequenceElement(pcc.GetUExport(4917), null, 0);
        }

        private static void BioD_Cit003_850FinalBldg_fl2_LOC_INT(IMEPackage pcc)
        {
            Cit003_glyph_a(pcc, pcc.GetUExport(2), 466, 467);
        }

        // NATIVE COMPATIBILITY
        private static void BioD_Cit003_900Trap(IMEPackage pcc)
        {
            SequenceAutomations.ReplaceObject(
                pcc,
                pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.Convo"),
                CreateSequenceObjectWithProps(pcc, "BioSeqAct_PMCheckConditional",
                [
                    new IntProperty(71173000, "m_nIndex"),
                    new ArrayProperty<StrProperty>("m_aObjComment")
                    {
                        new("FSvBS: Is Not Female Player?")
                    }]),
                pcc.GetUExport(269)
                );

            // SwapCheckStateOutputs(pcc.GetUExport(269));
        }

        private static void BioD_Cit003_900Trap_LOC_INT(IMEPackage pcc)
        {
            Cit003_final_trap_m(pcc, pcc.GetUExport(365), 2697, 2698);
            Cit003_locked_door_a(pcc.GetUExport(366));
        }

        private static void BioD_Cit004_100Exterior_LOC_INT(IMEPackage pcc)
        {
            Cit004_shuttle_m(pcc, pcc.GetUExport(106));
        }

        private static void BioD_Cit004_210CICIntro(IMEPackage pcc)
        {
            ExportEntry sequence = pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.Clone_Conversation");

            ExportEntry CheckConditional = CreateSequenceObjectWithProps(pcc, "BioSeqAct_PMCheckConditional",
            [ new IntProperty(71173000, "m_nIndex"),
                new ArrayProperty<StrProperty>("m_aObjComment")
                {
                    new("FSvBS: Is Not Female Player?")
            }]);
            //ExportEntry PMCheck = CreateSequenceObjectWithProps(pcc, "BioSeqAct_PMCheckState",
            //[
            //    new IntProperty(17662, "m_nIndex"),
            //    new ArrayProperty<StrProperty>("m_aObjComment")
            //    {
            //        new("Female Player?")
            //    }
            //]);
            ExportEntry setObjMale = CreateSequenceObjectWithProps(pcc, "SeqAct_SetObject", []);
            ExportEntry setObjFemale = CreateSequenceObjectWithProps(pcc, "SeqAct_SetObject", []);

            KismetHelper.AddObjectsToSequence(sequence, true, [CheckConditional, setObjMale, setObjFemale]);

            ExportEntry stuntMale = pcc.GetUExport(3470);
            ExportEntry stuntFemale = pcc.GetUExport(3474);
            ExportEntry stuntEmpty = pcc.GetUExport(3476);
            ExportEntry teleport = pcc.GetUExport(3441);

            (ExportEntry outEvt, ExportEntry gate) = AddEventHandshake(pcc, sequence, "RE_FSvBS_SetClone", "RE_FSvBS_CloneSet");

            SkipAndCleanSequenceElement(pcc.GetUExport(3455), null, 0);

            // Disconnect the Teleport and connect it to the plot check
            KismetHelper.RemoveOutputLinks(teleport);
            KismetHelper.CreateOutputLink(teleport, "Out", CheckConditional, 0);
            // Connect the plot check to the set objects
            KismetHelper.CreateOutputLink(CheckConditional, "True", setObjFemale, 0);
            KismetHelper.CreateOutputLink(CheckConditional, "False", setObjMale, 0);
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

        private static void BioD_Cit004_210CICIntro_LOC_INT(IMEPackage pcc)
        {
            Cit004_cic_intro_m(pcc, pcc.GetUExport(343), 566, 567, 568, 569);
        }

        private static void BioD_Cit004_220CIC_LOC_INT(IMEPackage pcc)
        {
            Cit004_hamster_a(pcc, pcc.GetUExport(2), 319, 320);
            Cit004_tubes_banter_b(pcc, pcc.GetUExport(3), 323, 324, 321, 322);
        }

        // NATIVE COMPATIBILITY
        private static void BioD_Cit004_250Elevator(IMEPackage pcc)
        {
            PropertyCollection props = [
                new IntProperty(71173001, "m_nIndex"),
                new ArrayProperty<StrProperty>("m_aObjComment")
                {
                    new("FSvBS: Is Not Male Player?")
                }];

            ExportEntry check44 = pcc.GetUExport(44);
            ExportEntry check45 = pcc.GetUExport(45);
            ExportEntry check46 = pcc.GetUExport(46);
            ExportEntry check47 = pcc.GetUExport(47);

            SharedMethods.AppendProperties(check44, props);
            SharedMethods.AppendProperties(check45, props);
            SharedMethods.AppendProperties(check46, props);
            SharedMethods.AppendProperties(check47, props);

            //SwapCheckStateOutputs(check44);
            //SwapCheckStateOutputs(check45);
            //SwapCheckStateOutputs(check46);
            //SwapCheckStateOutputs(check47);
        }

        // NATIVE COMPATIBILITY
        private static void BioD_Cit004_250Elevator_LOC_INT(IMEPackage pcc)
        {
            Cit004_elevator_ride_b(pcc.GetUExport(1));
        }

        private static void BioD_Cit004_260CloneIntro_LOC_INT(IMEPackage pcc)
        {
            Cit004_elevator_d(pcc, pcc.GetUExport(394));
        }

        private static void BioD_Cit004_270ShuttleBay1_LOC_INT(IMEPackage pcc)
        {
            Cit004_normandyfly_c(pcc.GetUExport(1));
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

            PropertyCollection props = [
                new IntProperty(71173001, "m_nIndex"),
                new ArrayProperty<StrProperty>("m_aObjComment")
                {
                    new("FSvBS: Is Not Male Player?")
                }];

            ExportEntry check386 = pcc.GetUExport(386);
            ExportEntry check387 = pcc.GetUExport(387);
            SharedMethods.AppendProperties(check386, props);
            SharedMethods.AppendProperties(check387, props);

            //SwapCheckStateOutputs(pcc.GetUExport(387));
            //SwapCheckStateOutputs(pcc.GetUExport(386));
        }

        // NATIVE COMPATIBILITY
        private static void BioD_Cit004_290FightScene_LOC_INT(IMEPackage pcc)
        {
            Cit004_climax_choice_m(pcc.GetUExport(26));
        }

        // NATIVE COMPATIBILITY
        private static void BioD_Cit004_295BrooksEnd_LOC_INT(IMEPackage pcc)
        {
            Cit004_epilogue_m(pcc.GetUExport(665));
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
        private static void Cit003_lobby_planning_b_dlg(IMEPackage pcc, ExportEntry convObj, int fxaPlayerFIdx, int fxaPlayerMIdx)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);

            // Prepare the FaceFX controls
            FaceFXAnimSetEditorControl FXA_lobby_planning_b_Player_F_Control = GetLoadedFXAControl(pcc, fxaPlayerFIdx);
            FaceFXAnimSetEditorControl FXA_lobby_planning_b_Player_M_Control = GetLoadedFXAControl(pcc, fxaPlayerMIdx);

            DialogueNodeExtended nodeR1 = GetNodeByIndex(conv, 1, true);
            DialogueNodeExtended nodeR2 = GetNodeByIndex(conv, 2, true);
            ReplaceLineAndAudio(pcc, nodeR1, "71174524", TlkAudioMap[24], FXA_lobby_planning_b_Player_F_Control, FXA_lobby_planning_b_Player_M_Control);
            ReplaceLineAndAudio(pcc, nodeR2, "71174525", TlkAudioMap[25], FXA_lobby_planning_b_Player_F_Control, FXA_lobby_planning_b_Player_M_Control);
            WriteNodes(convObj, nodeR1, nodeR2);
        }

        private static void Cit003_meet_clone_m(IMEPackage pcc, ExportEntry convObj, int fxaCloneFemFIdx, int fxaCloneFemMIdx, int fxaCloneMaleFIdx, int fxaCloneMaleMIdx)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);

            // Prepare the FaceFX controls
            FaceFXAnimSetEditorControl FXA_clone_female_F_Control = GetLoadedFXAControl(pcc, fxaCloneFemFIdx);
            FaceFXAnimSetEditorControl FXA_clone_female_M_Control = GetLoadedFXAControl(pcc, fxaCloneFemMIdx);
            FaceFXAnimSetEditorControl FXA_clone_male_F_Control = GetLoadedFXAControl(pcc, fxaCloneMaleFIdx);
            FaceFXAnimSetEditorControl FXA_clone_male_M_Control = GetLoadedFXAControl(pcc, fxaCloneMaleMIdx);


            BatchChangeAndWriteNodesPlotCheck(convObj, true, 71173000, -1, GetNodesByIndex(conv, false,
                [1, 4, 12, 15, 29, 31, 43, 47, 51, 53, 55, 57, 69, 82, 87, 91, 95, 99, 103, 107, 111, 115, 119, 123, 126, 129, 141, 145, 152, 154, 163]));

            //BatchSwapAndWriteFirstTwoNodesAndPlots(convObj, conv,
            //    GetNodesByIndex(conv, true, [0, 3, 11, 14, 28, 29, 31]));
            //BatchSwapAndWriteFirstTwoNodes(convObj, conv,
            //    GetNodesByIndex(conv, true, [32, 33, 34, 35, 36, 37, 38, 39, 40]));
            //BatchSwapAndWriteFirstTwoNodesAndPlots(convObj, conv,
            //    GetNodesByIndex(conv, true, [43, 46, 47, 48, 49, 51, 63, 67, 71, 75, 79, 83, 87, 91, 95, 99, 103, 70]));
            //BatchSwapAndWriteFirstTwoNodes(convObj, conv,
            //    GetNodesByIndex(conv, true, [74, 78, 82, 86, 90, 94, 98, 102]));
            //BatchSwapAndWriteFirstTwoNodesAndPlots(convObj, conv,
            //    GetNodesByIndex(conv, true, [106, 109, 111]));
            //BatchSwapAndWriteFirstTwoNodes(convObj, conv,
            //    GetNodesByIndex(conv, true, [112, 113, 114, 115, 116, 117, 118, 119, 120]));
            //BatchSwapAndWriteFirstTwoNodesAndPlots(convObj, conv,
            //    GetNodesByIndex(conv, true, [124, 131, 134, 143]));

            // "It's time the understudy..."
            DialogueNodeExtended nodeE53 = GetNodeByIndex(conv, 53, false);
            DialogueNodeExtended nodeE54 = GetNodeByIndex(conv, 54, false);
            ReplaceLineAndAudioAndFXA(pcc, nodeE54, "71174518", TlkAudioMap[18], FXA_clone_male_F_Control, FXA_clone_male_M_Control);
            ReplaceLineAndAudioAndFXA(pcc, nodeE53, "71174519", TlkAudioMap[19], FXA_clone_female_F_Control, FXA_clone_female_M_Control);

            // "Because I don't have..."
            DialogueNodeExtended nodeE82 = GetNodeByIndex(conv, 82, false);
            DialogueNodeExtended nodeE84 = GetNodeByIndex(conv, 84, false);
            ReplaceLineAndAudio(pcc, nodeE84, "71174521", TlkAudioMap[21], FXA_clone_male_F_Control, FXA_clone_male_M_Control);
            ReplaceLineAndAudio(pcc, nodeE82, "71174520", TlkAudioMap[20], FXA_clone_female_F_Control, FXA_clone_female_M_Control);

            // "They will when I'm flying..."
            DialogueNodeExtended nodeE141 = GetNodeByIndex(conv, 141, false);
            DialogueNodeExtended nodeE142 = GetNodeByIndex(conv, 142, false);
            ReplaceLineAndAudio(pcc, nodeE142, "71174523", TlkAudioMap[23], FXA_clone_male_F_Control, FXA_clone_male_M_Control);
            ReplaceLineAndAudio(pcc, nodeE141, "71174522", TlkAudioMap[22], FXA_clone_female_F_Control, FXA_clone_female_M_Control);

            WriteNodes(convObj, nodeE53, nodeE54, nodeE82, nodeE84, nodeE141, nodeE142);
        }

        private static void Cit003_glyph_a(IMEPackage pcc, ExportEntry convObj, int fxaPlayerFIdx, int fxaPlayerMIdx)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);

            // Prepare the FaceFX controls
            FaceFXAnimSetEditorControl FXA_player_F_Control = GetLoadedFXAControl(pcc, fxaPlayerFIdx);
            FaceFXAnimSetEditorControl FXA_player_M_Control = GetLoadedFXAControl(pcc, fxaPlayerMIdx);

            // "... looks like me."
            DialogueNodeExtended nodeR3 = GetNodeByIndex(conv, 3, true);
            DialogueNodeExtended nodeR4 = GetNodeByIndex(conv, 4, true);
            ReplaceLineAndAudio(pcc, nodeR3, "71174526", TlkAudioMap[26], FXA_player_F_Control, FXA_player_M_Control);
            ReplaceLineAndAudio(pcc, nodeR4, "71174527", TlkAudioMap[27], FXA_player_F_Control, FXA_player_M_Control);

            // "Go find..."
            DialogueNodeExtended nodeR13 = GetNodeByIndex(conv, 13, true);
            DialogueNodeExtended nodeR14 = GetNodeByIndex(conv, 14, true);
            DialogueNodeExtended nodeE49 = GetNodeByIndex(conv, 49, false);
            DialogueNodeExtended nodeE50 = GetNodeByIndex(conv, 50, false);
            ReplaceLineAndAudio(pcc, nodeR13, "71174528", TlkAudioMap[28], FXA_player_F_Control, FXA_player_M_Control);
            ReplaceLineAndAudio(pcc, nodeR14, "71174529", TlkAudioMap[29], FXA_player_F_Control, FXA_player_M_Control);
            ChangeNodeLink(pcc.Game, nodeE49, 13, 14);
            ChangeNodeLink(pcc.Game, nodeE50, 14, 13);

            WriteNodes(convObj, nodeR3, nodeR4, nodeR13, nodeR14, nodeE49, nodeE50);

            BatchChangeAndWriteNodesPlotCheck(convObj, true, 71173000, -1, GetNodesByIndex(conv, false, [47, 49, 80, 87]));
            //BatchSwapAndWriteFirstTwoNodesAndPlots(convObj, conv, GetNodesByIndex(conv, true, [11, 12, 35, 38]));
        }

        private static void Cit003_mercs_tubes1_a(IMEPackage pcc, ExportEntry convObj, int fxaCloneFemFIdx, int fxaCloneFemMIdx, int fxaCloneMaleFIdx, int fxaCloneMaleMIdx)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);

            // Prepare the FaceFX controls
            FaceFXAnimSetEditorControl FXA_clone_female_F_Control = GetLoadedFXAControl(pcc, fxaCloneFemFIdx);
            FaceFXAnimSetEditorControl FXA_clone_female_M_Control = GetLoadedFXAControl(pcc, fxaCloneFemMIdx);
            FaceFXAnimSetEditorControl FXA_clone_male_F_Control = GetLoadedFXAControl(pcc, fxaCloneMaleFIdx);
            FaceFXAnimSetEditorControl FXA_clone_male_M_Control = GetLoadedFXAControl(pcc, fxaCloneMaleMIdx);

            // Swap the nodes
            BatchChangeAndWriteNodesPlotCheck(convObj, true, 71173000, -1, GetNodesByIndex(conv, false, [1, 7, 12]));
            // BatchSwapAndWriteFirstTwoNodesAndPlots(convObj, conv, GetNodesByIndex(conv, true, [0, 1, 4]));

            // Replace lines
            // "Eliminate them..."
            DialogueNodeExtended nodeE1 = GetNodeByIndex(conv, 1, false);
            DialogueNodeExtended nodeE2 = GetNodeByIndex(conv, 2, false);
            ReplaceLineAndAudio(pcc, nodeE2, "71174531", TlkAudioMap[31], FXA_clone_male_F_Control, FXA_clone_male_M_Control);
            ReplaceLineAndAudio(pcc, nodeE1, "71174530", TlkAudioMap[30], FXA_clone_female_F_Control, FXA_clone_female_M_Control);
            WriteNodes(convObj, nodeE1, nodeE2);
        }

        private static void Cit003_tubes1_b(ExportEntry convObj)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);

            // Swap the nodes
            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 12, false), true, 71173000, -1);
            //SwapAndWriteFirstTwoNodesAndPlots(convObj, conv, GetNodeByIndex(conv, 2, true));
        }

        private static void Cit003_hench_meetup_m(IMEPackage pcc, ExportEntry convObj, int fxaPlayerFIdx, int fxaPlayerMIdx)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);

            // Prepare the FaceFX controls
            FaceFXAnimSetEditorControl FXA_player_F_Control = GetLoadedFXAControl(pcc, fxaPlayerFIdx);
            FaceFXAnimSetEditorControl FXA_player_M_Control = GetLoadedFXAControl(pcc, fxaPlayerMIdx);

            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 19, false), true, -1, -1);
            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 20, false), true, 71173000, -1);
            SwapAndWriteFirstTwoNodes(convObj, conv, GetNodeByIndex(conv, 8, true));

            // "Eliminate them..."
            DialogueNodeExtended nodeR9 = GetNodeByIndex(conv, 9, true);
            DialogueNodeExtended nodeR10 = GetNodeByIndex(conv, 10, true);
            ReplaceLineAndAudio(pcc, nodeR9, "71174540", TlkAudioMap[40], FXA_player_F_Control, FXA_player_M_Control);
            ReplaceLineAndAudio(pcc, nodeR10, "71174541", TlkAudioMap[41], FXA_player_F_Control, FXA_player_M_Control);
            WriteNodes(convObj, nodeR9, nodeR10);
        }
        private static void Cit003_mech_event_merc_a(IMEPackage pcc, ExportEntry convObj, int fxaCloneFemFIdx, int fxaCloneFemMIdx, int fxaCloneMaleFIdx, int fxaCloneMaleMIdx)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);

            // Prepare the FaceFX controls
            FaceFXAnimSetEditorControl FXA_clone_female_F_Control = GetLoadedFXAControl(pcc, fxaCloneFemFIdx);
            FaceFXAnimSetEditorControl FXA_clone_female_M_Control = GetLoadedFXAControl(pcc, fxaCloneFemMIdx);
            FaceFXAnimSetEditorControl FXA_clone_male_F_Control = GetLoadedFXAControl(pcc, fxaCloneMaleFIdx);
            FaceFXAnimSetEditorControl FXA_clone_male_M_Control = GetLoadedFXAControl(pcc, fxaCloneMaleMIdx);

            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 1, false), true, 71173000, -1);
            //SwapAndWriteFirstTwoNodesAndPlots(convObj, conv, GetNodeByIndex(conv, 0, true));

            // "Keep Shepard off my back..."
            DialogueNodeExtended nodeE1 = GetNodeByIndex(conv, 1, false);
            DialogueNodeExtended nodeE2 = GetNodeByIndex(conv, 2, false);
            ReplaceLineAndAudio(pcc, nodeE2, "71174533", TlkAudioMap[33], FXA_clone_male_F_Control, FXA_clone_male_M_Control);
            ReplaceLineAndAudio(pcc, nodeE1, "71174532", TlkAudioMap[32], FXA_clone_female_F_Control, FXA_clone_female_M_Control);
            WriteNodes(convObj, nodeE1, nodeE2);
        }

        private static void Cit003_mercs_building2_a(ExportEntry convObj)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);

            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 1, false), true, 71173000, -1);
            //SwapAndWriteFirstTwoNodesAndPlots(convObj, conv, GetNodeByIndex(conv, 0, true));
        }

        private static void Cit003_final_trap_m(IMEPackage pcc, ExportEntry convObj, int fxaPlayerFIdx, int fxaPlayerMIdx)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);

            // Prepare the FaceFX controls
            FaceFXAnimSetEditorControl FXA_player_F_Control = GetLoadedFXAControl(pcc, fxaPlayerFIdx);
            FaceFXAnimSetEditorControl FXA_player_M_Control = GetLoadedFXAControl(pcc, fxaPlayerMIdx);

            BatchChangeAndWriteNodesPlotCheck(convObj, true, 71173000, -1, GetNodesByIndex(conv, false, [32, 35, 37, 68, 85, 89, 92, 94, 101]));
            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 95, false), true, -1, -1); // This path for some reason has a bool even though it's the only path
            //BatchSwapAndWriteFirstTwoNodesAndPlots(convObj, conv, GetNodesByIndex(conv, true, [4, 7, 8, 37, 56, 60, 63, 64, 72,]));
            //SwapAndWriteFirstTwoNodes(convObj, conv, GetNodeByIndex(conv, 65, true));

            // "Then I'm going to mount..."
            DialogueNodeExtended nodeR92 = GetNodeByIndex(conv, 92, true);
            DialogueNodeExtended nodeR93 = GetNodeByIndex(conv, 93, true);
            ReplaceLineAndAudio(pcc, nodeR92, "71174534", TlkAudioMap[34], FXA_player_F_Control, FXA_player_M_Control);
            ReplaceLineAndAudio(pcc, nodeR93, "71174535", TlkAudioMap[35], FXA_player_F_Control, FXA_player_M_Control);

            // "... I should go"
            DialogueNodeExtended nodeR79 = GetNodeByIndex(conv, 79, true);
            DialogueNodeExtended nodeR80 = GetNodeByIndex(conv, 80, true);
            ReplaceLineAndAudio(pcc, nodeR79, "71174536", TlkAudioMap[36], FXA_player_F_Control, FXA_player_M_Control);
            ReplaceLineAndAudio(pcc, nodeR80, "71174537", TlkAudioMap[37], FXA_player_F_Control, FXA_player_M_Control);

            // "I'm more confident than..."
            DialogueNodeExtended nodeR83 = GetNodeByIndex(conv, 83, true);
            DialogueNodeExtended nodeR84 = GetNodeByIndex(conv, 84, true);
            ReplaceLineAndAudio(pcc, nodeR83, "71174538", TlkAudioMap[38], FXA_player_F_Control, FXA_player_M_Control);
            ReplaceLineAndAudio(pcc, nodeR84, "71174539", TlkAudioMap[39], FXA_player_F_Control, FXA_player_M_Control);

            WriteNodes(convObj, nodeR92, nodeR93, nodeR79, nodeR80, nodeR83, nodeR84);
        }

        private static void Cit003_locked_door_a(ExportEntry convObj)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);

            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 2, false), true, 71173000, -1);
            //SwapAndWriteFirstTwoNodesAndPlots(convObj, conv, GetNodeByIndex(conv, 1, true));
        }

        private static void Cit004_shuttle_m(IMEPackage pcc, ExportEntry convObj)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);

            DialogueNodeExtended nodeE43 = GetNodeByIndex(conv, 43, false);
            ChangeNodeLink(pcc.Game, nodeE43, 45, 37, "We'lll stop her", 779731, EReplyCategory.REPLY_CATEGORY_DISAGREE); // Temp swap
            ChangeNodeLink(pcc.Game, nodeE43, 46, 45, "We'll stop him", 779732, EReplyCategory.REPLY_CATEGORY_DISAGREE);
            ChangeNodeLink(pcc.Game, nodeE43, 37, 46, "We'lll stop her", 779731, EReplyCategory.REPLY_CATEGORY_DISAGREE);

            WriteNode(convObj, nodeE43);

            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 14, false), true, -1, -1);
            ChangeAndWriteNodeTransition(convObj, GetNodeByIndex(conv, 14, false), 7051, -1);
            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 15, false), true, 71173000, -1);
            ChangeAndWriteNodeTransition(convObj, GetNodeByIndex(conv, 15, false), 7050, -1);
            SwapAndWriteFirstTwoNodes(convObj, conv, GetNodeByIndex(conv, 13, true));

            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 27, false), true, -1, -1);
            ChangeAndWriteNodeTransition(convObj, GetNodeByIndex(conv, 27, false), 7051, -1);
            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 35, false), true, 71173000, -1);
            ChangeAndWriteNodeTransition(convObj, GetNodeByIndex(conv, 35, false), 7050, -1);

            SwapAndWriteFirstTwoNodes(convObj, conv, GetNodeByIndex(conv, 26, true));
        }

        private static void Cit004_cic_intro_m(IMEPackage pcc, ExportEntry convObj, int fxaCloneFemFIdx, int fxaCloneFemMIdx, int fxaCloneMaleFIdx, int fxaCloneMaleMIdx)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);

            // Prepare the FaceFX controls
            FaceFXAnimSetEditorControl FXA_clone_female_F_Control = GetLoadedFXAControl(pcc, fxaCloneFemFIdx);
            FaceFXAnimSetEditorControl FXA_clone_female_M_Control = GetLoadedFXAControl(pcc, fxaCloneFemMIdx);
            FaceFXAnimSetEditorControl FXA_clone_male_F_Control = GetLoadedFXAControl(pcc, fxaCloneMaleFIdx);
            FaceFXAnimSetEditorControl FXA_clone_male_M_Control = GetLoadedFXAControl(pcc, fxaCloneMaleMIdx);

            // Swap start nodes
            //ArrayProperty<IntProperty> m_StartingList = convObj.GetProperty<ArrayProperty<IntProperty>>("m_StartingList");
            //m_StartingList[0].Value = 2;
            //m_StartingList[1].Value = 0;
            //convObj.WriteProperty(m_StartingList);

            // Swap the bools
            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 0, false), true, 71173001, -1); // If Not Male
            //DialogueNodeExtended nodeE0 = GetNodeByIndex(conv, 0, false);
            //DialogueNodeExtended nodeE2 = GetNodeByIndex(conv, 2, false);
            //SwapNodesPlotChecks(nodeE0, nodeE2);
            //WriteNodes(convObj, nodeE0, nodeE2);


            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 5, false), true, 71173001, -1); // If Not Male
            //SwapAndWriteFirstTwoNodesAndPlots(convObj, conv, GetNodeByIndex(conv, 4, true));

            // "... Where?"
            DialogueNodeExtended nodeE1 = GetNodeByIndex(conv, 1, false);
            DialogueNodeExtended nodeE3 = GetNodeByIndex(conv, 3, false);
            ReplaceLineAndAudio(pcc, nodeE1, "71174543", TlkAudioMap[43], FXA_clone_male_F_Control, FXA_clone_male_M_Control);
            ReplaceLineAndAudio(pcc, nodeE3, "71174542", TlkAudioMap[42], FXA_clone_female_F_Control, FXA_clone_female_M_Control);

            // "Armory! Slow..."
            DialogueNodeExtended nodeE7 = GetNodeByIndex(conv, 7, false);
            DialogueNodeExtended nodeE10 = GetNodeByIndex(conv, 10, false);
            ReplaceLineAndAudioAndFXA(pcc, nodeE7, "71174545", TlkAudioMap[45], FXA_clone_male_F_Control, FXA_clone_male_M_Control);
            ReplaceLineAndAudioAndFXA(pcc, nodeE10, "71174544", TlkAudioMap[44], FXA_clone_female_F_Control, FXA_clone_female_M_Control);

            WriteNodes(convObj, [nodeE1, nodeE3, nodeE7, nodeE10]);

            // NOT NEEDED
            // Get the clone gesture tracks to remove the extra gesture
            /*MatineeHelper.TryGetInterpGroup(nodeE7.Interpdata, "Clone", out ExportEntry E7Clone);
            MatineeHelper.TryGetInterpTrack(E7Clone, "BioEvtSysTrackGesture", out ExportEntry E7CloneGesture);
            MatineeHelper.TryGetInterpGroup(nodeE10.Interpdata, "Clone", out ExportEntry E10Clone);
            MatineeHelper.TryGetInterpTrack(E10Clone, "BioEvtSysTrackGesture", out ExportEntry E10CloneGesture);

            // Remove nodeE7 extra gesture
            ArrayProperty<StructProperty> m_aGestures = E7CloneGesture.GetProperty<ArrayProperty<StructProperty>>("m_aGestures");
            ArrayProperty<StructProperty> m_aTrackKeys = E7CloneGesture.GetProperty<ArrayProperty<StructProperty>>("m_aTrackKeys");
            m_aGestures.RemoveAt(1);
            m_aTrackKeys.RemoveAt(1);
            E7CloneGesture.WriteProperties([m_aGestures, m_aTrackKeys]);
            // Remove nodeE10 extra gesture
            m_aGestures = E10CloneGesture.GetProperty<ArrayProperty<StructProperty>>("m_aGestures");
            m_aTrackKeys = E10CloneGesture.GetProperty<ArrayProperty<StructProperty>>("m_aTrackKeys");
            m_aGestures.RemoveAt(1);
            m_aTrackKeys.RemoveAt(1);
            E10CloneGesture.WriteProperties([m_aGestures, m_aTrackKeys]);
            */

            // Swap Brook's line about Shepard
            DialogueNodeExtended nodeR7 = GetNodeByIndex(conv, 7, true);
            DialogueNodeExtended nodeE9 = GetNodeByIndex(conv, 9, false);
            ChangeNodeLink(pcc.Game, nodeR7, 9, 6);
            ChangeNodeLink(pcc.Game, nodeE9, 8, 6);
            DialogueNodeExtended nodeR5 = GetNodeByIndex(conv, 5, true);
            DialogueNodeExtended nodeE6 = GetNodeByIndex(conv, 6, false);
            ChangeNodeLink(pcc.Game, nodeR5, 6, 9);
            ChangeNodeLink(pcc.Game, nodeE6, 6, 8);
            WriteNodes(convObj, [nodeR7, nodeE9, nodeR5, nodeE6]);
        }

        private static void Cit004_hamster_a(IMEPackage pcc, ExportEntry convObj, int fxaPlayerFIdx, int fxaPlayerMIdx)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);

            // Prepare the FaceFX controls
            FaceFXAnimSetEditorControl FXA_player_F_Control = GetLoadedFXAControl(pcc, fxaPlayerFIdx);
            FaceFXAnimSetEditorControl FXA_player_M_Control = GetLoadedFXAControl(pcc, fxaPlayerMIdx);

            // "... messed up with my hamster, guys"
            DialogueNodeExtended nodeR3 = GetNodeByIndex(conv, 3, true);
            DialogueNodeExtended nodeR4 = GetNodeByIndex(conv, 4, true);
            ReplaceLineAndAudio(pcc, nodeR3, "71174547", TlkAudioMap[47], FXA_player_F_Control, FXA_player_M_Control);
            ReplaceLineAndAudio(pcc, nodeR4, "71174546", TlkAudioMap[46], FXA_player_F_Control, FXA_player_M_Control);

            // "Should we check on my fish?"
            DialogueNodeExtended nodeR8 = GetNodeByIndex(conv, 8, true);
            DialogueNodeExtended nodeR9 = GetNodeByIndex(conv, 9, true);
            ReplaceLineAndAudio(pcc, nodeR8, "71174549", TlkAudioMap[49], FXA_player_F_Control, FXA_player_M_Control);
            ReplaceLineAndAudio(pcc, nodeR9, "71174548", TlkAudioMap[48], FXA_player_F_Control, FXA_player_M_Control);

            // "We should probably deal with..."
            DialogueNodeExtended nodeR12 = GetNodeByIndex(conv, 12, true);
            DialogueNodeExtended nodeR14 = GetNodeByIndex(conv, 14, true);
            ReplaceLineAndAudio(pcc, nodeR12, "71174551", TlkAudioMap[51], FXA_player_F_Control, FXA_player_M_Control);
            ReplaceLineAndAudio(pcc, nodeR14, "71174550", TlkAudioMap[50], FXA_player_F_Control, FXA_player_M_Control);

            // "Should we check on the rest of my stuff?"
            DialogueNodeExtended nodeR11 = GetNodeByIndex(conv, 11, true);
            DialogueNodeExtended nodeR13 = GetNodeByIndex(conv, 13, true);
            ReplaceLineAndAudio(pcc, nodeR11, "71174553", TlkAudioMap[53], FXA_player_F_Control, FXA_player_M_Control);
            ReplaceLineAndAudio(pcc, nodeR13, "71174552", TlkAudioMap[52], FXA_player_F_Control, FXA_player_M_Control);

            // "This is from my cabin."
            DialogueNodeExtended nodeR17 = GetNodeByIndex(conv, 17, true);
            DialogueNodeExtended nodeR18 = GetNodeByIndex(conv, 18, true);
            ReplaceLineAndAudio(pcc, nodeR17, "71174555", TlkAudioMap[55], FXA_player_F_Control, FXA_player_M_Control);
            ReplaceLineAndAudio(pcc, nodeR18, "71174554", TlkAudioMap[54], FXA_player_F_Control, FXA_player_M_Control);

            WriteNodes(convObj, nodeR3, nodeR4, nodeR8, nodeR9, nodeR12, nodeR14, nodeR11, nodeR13, nodeR17, nodeR18);
        }

        private static void Cit004_tubes_banter_b(IMEPackage pcc, ExportEntry convObj, int fxaMercFIdx, int fxaMercMIdx, int fxaMercLieuFIdx, int fxaMercLieuMIdx)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);

            // Prepare the FaceFX controls
            FaceFXAnimSetEditorControl FXA_merc_F_Control = GetLoadedFXAControl(pcc, fxaMercFIdx);
            FaceFXAnimSetEditorControl FXA_merc_M_Control = GetLoadedFXAControl(pcc, fxaMercMIdx);
            FaceFXAnimSetEditorControl FXA_mercLieu_F_Control = GetLoadedFXAControl(pcc, fxaMercLieuFIdx);
            FaceFXAnimSetEditorControl FXA_mercLieu_M_Control = GetLoadedFXAControl(pcc, fxaMercLieuMIdx);

            // "Hey, what'd..."
            DialogueNodeExtended nodeE19 = GetNodeByIndex(conv, 19, false);
            DialogueNodeExtended nodeE20 = GetNodeByIndex(conv, 20, false);
            ChangeNodePlotCheck(nodeE19, true, 71173001, -1);
            ReplaceLineAndAudio(pcc, nodeE20, "71174556", TlkAudioMap[56], FXA_merc_F_Control, FXA_merc_M_Control);
            ReplaceLineAndAudio(pcc, nodeE19, "71174557", TlkAudioMap[57], FXA_merc_F_Control, FXA_merc_M_Control);

            // "said 'slow..."
            DialogueNodeExtended nodeE22 = GetNodeByIndex(conv, 22, false);
            DialogueNodeExtended nodeE23 = GetNodeByIndex(conv, 23, false);
            ReplaceLineAndAudio(pcc, nodeE22, "71174558", TlkAudioMap[58], FXA_mercLieu_F_Control, FXA_mercLieu_M_Control);
            ReplaceLineAndAudio(pcc, nodeE23, "71174559", TlkAudioMap[59], FXA_mercLieu_F_Control, FXA_mercLieu_M_Control);

            WriteNodes(convObj, nodeE19, nodeE20, nodeE22, nodeE23);

            // SwapAndWriteFirstTwoNodesAndPlots(convObj, conv, GetNodeByIndex(conv, 4, true));
        }

        private static void Cit004_elevator_ride_b(ExportEntry convObj)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);

            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 38, false), true, 71173001, -1);
            //SwapAndWriteFirstTwoNodesAndPlots(convObj, conv, GetNodeByIndex(conv, 21, true));
        }

        private static void Cit004_elevator_d(IMEPackage pcc, ExportEntry convObj)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);
            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 3, false), true, 71173001, -1);
            //SwapAndWriteFirstTwoNodesAndPlots(convObj, conv, GetNodeByIndex(conv, 1, true));
            // SwapAndWriteFirstTwoNodes(convObj, conv, GetNodeByIndex(conv, 2, true));
            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 9, true), true, 71173000, -1);

            // Reroute dialogue between clone and Shepard
            DialogueNodeExtended nodeE6 = GetNodeByIndex(conv, 6, false);
            DialogueNodeExtended nodeE3 = GetNodeByIndex(conv, 3, false);
            ChangeNodeLink(pcc.Game, nodeE6, 5, 3);
            ChangeNodeLink(pcc.Game, nodeE3, 3, 5);
            DialogueNodeExtended nodeR5 = GetNodeByIndex(conv, 5, true);
            DialogueNodeExtended nodeR3 = GetNodeByIndex(conv, 3, true);
            ChangeNodeLink(pcc.Game, nodeR5, 7, 4);
            ChangeNodeLink(pcc.Game, nodeR3, 4, 7);
            DialogueNodeExtended nodeE7 = GetNodeByIndex(conv, 7, false);
            DialogueNodeExtended nodeE4 = GetNodeByIndex(conv, 4, false);
            ChangeNodeLink(pcc.Game, nodeE7, 6, 4);
            ChangeNodeLink(pcc.Game, nodeE4, 4, 6);
            DialogueNodeExtended nodeR6 = GetNodeByIndex(conv, 6, true);
            DialogueNodeExtended nodeR4 = GetNodeByIndex(conv, 4, true);
            ChangeNodeLink(pcc.Game, nodeR6, 8, 5);
            ChangeNodeLink(pcc.Game, nodeR4, 5, 8);

            WriteNodes(convObj, [nodeE6, nodeE3, nodeR5, nodeR3, nodeE7, nodeE4, nodeR6, nodeR4]);
        }

        private static void Cit004_normandyfly_c(ExportEntry convObj)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);

            // Swap start nodes
            //ArrayProperty<IntProperty> m_StartingList = convObj.GetProperty<ArrayProperty<IntProperty>>("m_StartingList");
            //m_StartingList[0].Value = 2;
            //m_StartingList[1].Value = 0;
            //convObj.WriteProperty(m_StartingList);
            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 0, false), true, 71173001, -1);

            BatchChangeAndWriteNodesPlotCheck(convObj, true, 71173001, -1, GetNodesByIndex(conv, false, [5, 12]));
            //SwapAndWriteFirstTwoNodesAndPlots(convObj, conv, GetNodeByIndex(conv, 4, true));
            //SwapAndWriteFirstTwoNodesAndPlots(convObj, conv, GetNodeByIndex(conv, 11, true));

            // Reroute grunts between clone and Shepard
            DialogueNodeExtended nodeE0 = GetNodeByIndex(conv, 0, false);
            DialogueNodeExtended nodeE2 = GetNodeByIndex(conv, 2, false);
            ChangeNodeLink(MEGame.LE3, nodeE0, 0, 2);
            ChangeNodeLink(MEGame.LE3, nodeE2, 2, 0);
            DialogueNodeExtended nodeR0 = GetNodeByIndex(conv, 0, true);
            DialogueNodeExtended nodeR2 = GetNodeByIndex(conv, 2, true);
            ChangeNodeLink(MEGame.LE3, nodeR0, 1, 3);
            ChangeNodeLink(MEGame.LE3, nodeR2, 3, 1);

            WriteNodes(convObj, [nodeE0, nodeE2, nodeR0, nodeR2]);
        }

        private static void Cit004_climax_choice_m(ExportEntry convObj)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);

            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 1, false), true, 71173001, -1);
            //DialogueNodeExtended E1 = GetNodeByIndex(conv, 1, false);
            //ChangeNodePlotCheck(E1, true, 12, -1);
            //WriteNode(convObj, E1);


            BatchChangeAndWriteNodesPlotCheck(convObj, true, 71173001, -1, GetNodesByIndex(conv, false, [18, 20]));
            //SwapAndWriteFirstTwoNodesAndPlots(convObj, conv, GetNodeByIndex(conv, 17, true));
            //SwapAndWriteFirstTwoNodesAndPlots(convObj, conv, GetNodeByIndex(conv, 18, true));
        }

        private static void Cit004_epilogue_m(ExportEntry convObj)
        {
            ConversationExtended conv = GetLoadedConversation(convObj);

            ChangeAndWriteNodePlotCheck(convObj, GetNodeByIndex(conv, 18, false), true, 71173001, -1);
            //SwapAndWriteFirstTwoNodesAndPlots(convObj, conv, GetNodeByIndex(conv, 15, true));
        }

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

        public static readonly string XMLPath = @"G:\My Drive\Modding\Mass Effect\mods\Counter Clone\project\audio";

        /// <summary>
        /// Map of TLK IDs and their respective line audio infos.
        /// </summary>
        public static readonly Dictionary<int, LineAudioInfo> TlkAudioMap = new()
        {
            { 18, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 21974, 21974, "A89C0700", "A89C0700",
                @$"{XMLPath}\fxa json\FXA_71174518_M.json", @$"{XMLPath}\fxa json\FXA_71174518_M.json")
            },
            { 19, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 19428, 19428, "7EF20700", "7EF20700",
                @$"{XMLPath}\fxa json\FXA_71174519_F.json", @$"{XMLPath}\fxa json\FXA_71174519_F.json")
            },
            { 20, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 34132, 34132, "52930600", "52930600", @$"", @$"")
            },
            { 21, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 35577, 35577, "59080600", "59080600", @$"", @$"")
            },
            { 22, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 15128, 15128, "90610700", "90610700", @$"", @$"")
            },
            { 23, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 18666, 18666, "A6180700", "A6180700", @$"", @$"")
            },
            { 24, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 22587, 22587, "FCF90400", "FCF90400", @$"", @$"")
            },
            { 25, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 22089, 22089, "37520500", "37520500", @$"", @$"")
            },
            { 26, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 7563, 7563, "09EB0200", "09EB0200", @$"", @$"")
            },
            { 27, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 9710, 9710, "94080300", "94080300", @$"", @$"")
            },
            { 28, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 8466, 8466, "822E0300", "822E0300", @$"", @$"")
            },
            { 29, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 11651, 11651, "944F0300", "944F0300", @$"", @$"")
            },
            { 30, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 22294, 22294, "623E0800", "623E0800", @$"", @$"")
            },
            { 31, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 25281, 25281, "78950800", "78950800", @$"", @$"")
            },
            { 32, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 12919, 12919, "80A80500", "80A80500", @$"", @$"")
            },
            { 33, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 11618, 11618, "F7DA0500", "F7DA0500", @$"", @$"")
            },
            { 34, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 23784, 23784, "6D210200", "6D210200", @$"", @$"")
            },
            { 35, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 27828, 27828, "557E0200", "557E0200", @$"", @$"")
            },
            { 36, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 21418, 21418, "4C720000", "4C720000", @$"", @$"")
            },
            { 37, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 29260, 29260, "00000000", "00000000", @$"", @$"")
            },
            { 38, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 38083, 38083, "AA8C0100", "AA8C0100", @$"", @$"")
            },
            { 39, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 50868, 50868, "F6C50000", "F6C50000", @$"", @$"")
            },
            { 40, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 41650, 41650, "177D0300", "177D0300", @$"", @$"")
            },
            { 41, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 558559, 558559, "C91F0400", "C91F0400", @$"", @$"")
            },
            { 42, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 11666, 11666, "E4180900", "E4180900", @$"", @$"")
            },
            { 43, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 8363, 8363, "39F80800", "39F80800", @$"", @$"")
            },
            { 44, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 10911, 10911, "F4750900", "F4750900",
                @$"{XMLPath}\fxa json\FXA_71174544.json", @$"{XMLPath}\fxa json\FXA_71174544.json")
            },
            { 45, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 12158, 12158, "76460900", "76460900",
                @$"{XMLPath}\fxa json\FXA_71174545.json", @$"{XMLPath}\fxa json\FXA_71174545.json")
            },
            { 46, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 27986, 27986, "D2220A00", "D2220A00", @$"", @$"")
            },
            { 47, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 33343, 33343, "93A00900", "93A00900", @$"", @$"")
            },
            { 48, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 24795, 24795, "D7130B00", "D7130B00", @$"", @$"")
            },
            { 49, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 33715, 33715, "24900A00", "24900A00", @$"", @$"")
            },
            { 50, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 28720, 28720, "8DD20C00", "8DD20C00", @$"", @$"")
            },
            { 51, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 25189, 25189, "28700C00", "28700C00", @$"", @$"")
            },
            { 52, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 27932, 27932, "0C030C00", "0C030C00", @$"", @$"")
            },
            { 53, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 36442, 36442, "B2740B00", "B2740B00", @$"", @$"")
            },
            { 54, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 29217, 29217, "7CAA0D00", "7CAA0D00", @$"", @$"")
            },
            { 55, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 26559, 26559, "BD420D00", "BD420D00", @$"", @$"")
            },
            { 56, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 34398, 34398, "FD990E00", "FD990E00", @$"", @$"")
            },
            { 57, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 32096, 32096, "9D1C0E00", "9D1C0E00", @$"", @$"")
            },
            { 58, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 32386, 32386, "E6910F00", "E6910F00", @$"", @$"")
            },
            { 59, new LineAudioInfo(
                "DLC_MOD_FSvBSLE_Audio", "DLC_MOD_FSvBSLE_Audio", 29067, 29067, "5B200F00", "5B200F00", @$"", @$"")
            },
        };

        public static readonly List<string> Files = [
            "BioD_Cit002_000Global.pcc",
            "BioD_Cit002_700Exit.pcc",
            "BioD_Cit002_700Exit_LOC_INT.pcc",
            "BioD_Cit003.pcc",
            "BioD_Cit003_110Atrium_H_LOC_INT.pcc",
            "BioD_Cit003_150AtriumConvo.pcc",
            "BioD_Cit003_150AtriumConvo_LOC_INT.pcc",
            "BioD_Cit003_200HallEnter_LOC_INT.pcc",
            "BioD_Cit003_300TopMen_LOC_INT.pcc",
            "BioD_Cit003_400Tubes_LOC_INT.pcc",
            "BioD_Cit003_410Tubes_H_LOC_INT.pcc",
            "BioD_Cit003_450Ladder_LOC_INT.pcc",
            "BioD_Cit003_580MechDoor_LOC_INT.pcc",
            "BioD_Cit003_600MechEvent_LOC_INT.pcc",
            "BioD_Cit003_700FinalFloor_LOC_INT.pcc",
            "BioD_Cit003_710Final_H_LOC_INT.pcc",
            "BioD_Cit003_800FinalBldg_LOC_INT.pcc",
            "BioD_Cit003_815Final_RR2.pcc",
            "BioD_Cit003_850FinalBldg_fl2_LOC_INT.pcc",
            "BioD_Cit003_900Trap.pcc",
            "BioD_Cit003_900Trap_LOC_INT.pcc",
            "BioD_Cit004_100Exterior_LOC_INT.pcc",
            "BioD_Cit004_210CICIntro.pcc",
            "BioD_Cit004_210CICIntro_LOC_INT.pcc",
            "BioD_Cit004_220CIC_LOC_INT.pcc",
            "BioD_Cit004_250Elevator.pcc",
            "BioD_Cit004_250Elevator_LOC_INT.pcc",
            "BioD_Cit004_260CloneIntro_LOC_INT.pcc",
            "BioD_Cit004_270ShuttleBay1_LOC_INT.pcc",
            "BioD_Cit004_272MaleClone.pcc",
            "BioD_Cit004_273FemClone.pcc",
            "BioD_Cit004_290FightScene.pcc",
            "BioD_Cit004_290FightScene_LOC_INT.pcc",
            "BioD_Cit004_295BrooksEnd_LOC_INT.pcc",
            "BioP_Cit003.pcc",
            "BioP_Cit004.pcc"
            ];

        public static readonly List<string> Files_Clean = [
            "BioD_Cit002_700Exit.pcc",
            "BioD_Cit003_815Final_RR2.pcc",
            "BioD_Cit003.pcc",
            "BioD_Cit004_210CICIntro.pcc",
            "BioD_Cit004_272MaleClone.pcc",
            "BioD_Cit004_273FemClone.pcc",
            ];
    }
}
