using LegendaryExplorer.DialogueEditor.DialogueEditorExperiments;
using LegendaryExplorer.Misc.ExperimentsTools;
using LegendaryExplorerCore.Packages.CloningImportingAndRelinking;
using LegendaryExplorer.Tools.TlkManagerNS;
using LegendaryExplorer.UserControls.ExportLoaderControls;
using LegendaryExplorerCore.Dialogue;
using LegendaryExplorerCore.Kismet;
using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.Unreal;
using System;
using System.Collections.Generic;
using System.Windows;
using static LegendaryExplorer.Misc.ExperimentsTools.SharedMethods;
using static LegendaryExplorer.Misc.ExperimentsTools.PackageAutomations;
using static LegendaryExplorer.Misc.ExperimentsTools.DialogueAutomations;
using static LegendaryExplorer.Misc.ExperimentsTools.SequenceAutomations;
using LegendaryExplorerCore.Unreal.BinaryConverters;

namespace LegendaryExplorer.Mods
{
    public static class EmilyReturns
    {
        public static void Patch(IMEPackage pcc)
        {
            switch (pcc.FileNameNoExtension)
            {
                case "BioA_CitHub_Presidium_Global_LOC_INT":
                    BioA_CitHub_Presidium_Global_LOC_INT(pcc);
                    break;
                case "BioD_CitHub":
                    BioD_CitHub(pcc);
                    break;
                case "BioD_CitHubPV":
                    BioD_CitHub(pcc);
                    break;
                case "BioD_CitHub_Dock":
                    BioD_CitHub_Dock(pcc);
                    break;
                case "BioD_Nor":
                    BioD_Nor(pcc);
                    break;
                case "BioD_Nor_100CabinConv":
                    BioD_Nor_100CabinConv(pcc);
                    break;
                case "BioD_Nor_310LiaraOfficeCon":
                    BioD_Nor_310LiaraOfficeCon(pcc);
                    break;
                case "BioD_Nor_405Engineering":
                    BioD_Nor_405Engineering(pcc);
                    break;
                case "BioD_Nor_420StarCargo_LOC_INT":
                    BioD_Nor_420StarCargo_LOC_INT(pcc);
                    break;
                case "BioD_Nor_420StarCargoConv_LOC_INT":
                    BioD_Nor_420StarCargoConv_LOC_INT(pcc);
                    break;
                case "BioD_End001_420HubStreet1":
                    BioD_End001_420HubStreet1(pcc);
                    break;
                case "BioD_End001_435CommRoom_LOC_INT":
                    BioD_End001_435CommRoom_LOC_INT(pcc);
                    break;
                case "BioD_End001_436CRAllers_LOC_INT":
                    BioD_End001_436CRAllers_LOC_INT(pcc);
                    break;
                default:
                    break;
            }

            MessageBox.Show($"File successfully patched.");
        }

        private static void BioA_CitHub_Presidium_Global_LOC_INT(IMEPackage pcc)
        {
            // Prepare the conversation
            ExportEntry news_ann = pcc.GetUExport(2);
            ConversationExtended news_annConv = GetLoadedConversation(news_ann);

            // Prepare the FaceFX controls
            FaceFXAnimSetEditorControl FXA_news_ann_reporter_F_Control = GetLoadedFXAControl(pcc, 698);
            FaceFXAnimSetEditorControl FXA_news_ann_reporter_M_Control = GetLoadedFXAControl(pcc, 699);
            FaceFXAnimSetEditorControl FXA_news_ann_Owner_F_Control = GetLoadedFXAControl(pcc, 701);
            FaceFXAnimSetEditorControl FXA_news_ann_Owner_M_Control = GetLoadedFXAControl(pcc, 702);

            // 676202 -> 71172316 | "For the Alliance News Network..."
            DialogueNodeExtended node846851 = GetNode(news_annConv, 846851);
            ReplaceLineAndAudio(pcc, node846851, "71172316", TlkAudioMap[71172316], FXA_news_ann_reporter_F_Control, FXA_news_ann_reporter_M_Control);
            WriteNode(news_ann, node846851);

            DialogueNodeExtended nodeE40 = GetNodeByIndex(news_annConv, 40, false);
            ChangeNodeLink(pcc.Game, nodeE40, 37, 40, "", 676227, EReplyCategory.REPLY_CATEGORY_DEFAULT);
            WriteNode(news_ann, nodeE40);

            DialogueNodeExtended nodeE43 = GetNodeByIndex(news_annConv, 43, false);
            RemoveNodeLink(nodeE43, 40);
            WriteNode(news_ann, nodeE43);

            // 677865 -> 71172317 | "Stay tuned..."
            DialogueNodeExtended node848980 = GetNode(news_annConv, 848980);
            ReplaceLineAndAudio(pcc, node848980, "71172317", TlkAudioMap[71172317], FXA_news_ann_Owner_F_Control, FXA_news_ann_Owner_M_Control);
            WriteNode(news_ann, node848980);
        }

        private static void BioD_CitHub(IMEPackage pcc)
        {
            string filename = "BioD_CitHub_Dock_Emily";
            AddStreamingKismet(pcc, filename);
            StreamFile(pcc, filename, "BioD_CitHub_Dock");
        }

        private static void BioD_CitHub_Dock(IMEPackage pcc)
        {
            // Update the object referencing Allers name
            ExportEntry nameObj = pcc.GetUExport(12494);
            nameObj.WriteProperty(new StringRefProperty(71172315, "m_srValue"));

            // Replace the conversation object
            ReplaceObjectWithEventHandshake(pcc, pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.SEQ_War_Reporter"),
                pcc.GetUExport(11617), "ConvPlay_citprs_emily_intro_m", "ConvEnd_citprs_emily_intro_m");
        }

        private static void BioD_Nor(IMEPackage pcc)
        {
            // Nor_100CabinConv
            AddStreamingKismet(pcc, "BioD_Nor_100CabinConv_Emily");
            StreamFile(pcc, "BioD_Nor_100CabinConv_Emily", "BioD_Nor_100CabinConv");

            // Nor_310LiaraOfficeCon
            // AddStreamingKismet(pcc, "BioD_Nor_310LiaraOfficeCon_Emily");
            // StreamFile(pcc, "BioD_Nor_310LiaraOfficeCon_Emily", "BioD_Nor_310LiaraOfficeCon");

            // Nor_405Engineering
            // AddStreamingKismet(pcc, "BioD_Nor_405Engineering_Emily");
            // StreamFile(pcc, "BioD_Nor_405Engineering_Emily", "BioD_Nor_405Engineering");
        }

        private static void BioD_Nor_100CabinConv(IMEPackage pcc)
        {
            // RELATIONSHIP 3
            ExportEntry allers_char_moment_r3 = pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.Intercomm.Allers_Char_Moment_R3");
            ExportEntry prevNodeR3 = pcc.GetUExport(206);

            // Replace the interp object
            (ExportEntry outEvtR3, ExportEntry _) = ReplaceObjectWithEventHandshake(pcc, allers_char_moment_r3,
                pcc.GetUExport(8253), "ConvPlay_nor_emily_relationship3_d_pre", "ConvEnd_nor_emily_relationship3_d_pre");
            // Create a remote event for loading
            ExportEntry loadEvtR3 = CreateSequenceObjectWithProps(pcc, "SeqAct_ActivateRemoteEvent", new PropertyCollection()
            {
                new NameProperty("ConvLoad_nor_emily_relationship3_d", "EventName")
            });
            KismetHelper.AddObjectToSequence(loadEvtR3, allers_char_moment_r3);
            // We clear the outputs just in case
            KismetHelper.RemoveOutputLinks(prevNodeR3);
            // Add the links
            KismetHelper.CreateOutputLink(prevNodeR3, "Out", outEvtR3);
            KismetHelper.CreateOutputLink(prevNodeR3, "Out", loadEvtR3);

            // Replace the conversation object
            ReplaceObjectWithEventHandshake(pcc, allers_char_moment_r3,
                pcc.GetUExport(8570), "ConvPlay_nor_emily_relationship3_d", "ConvEnd_nor_emily_relationship3_d");


            // RELATIONSHIP 4
            ExportEntry allers_char_moment_r4 = pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.Intercomm.Allers_Char_Moment_R4");
            ExportEntry prevNodeR4 = pcc.GetUExport(208);

            // Replace the interp object
            (ExportEntry outEvtR4, ExportEntry _) = ReplaceObjectWithEventHandshake(pcc, allers_char_moment_r4,
                pcc.GetUExport(8254), "ConvPlay_nor_emily_relationship4_d_pre", "ConvEnd_nor_emily_relationship4_d_pre");
            // Create a remote event for loading
            ExportEntry loadEvtR4 = CreateSequenceObjectWithProps(pcc, "SeqAct_ActivateRemoteEvent", new PropertyCollection()
            {
                new NameProperty("ConvLoad_nor_emily_relationship4_d", "EventName")
            });
            KismetHelper.AddObjectToSequence(loadEvtR4, allers_char_moment_r4);
            // We clear the outputs just in case
            KismetHelper.RemoveOutputLinks(prevNodeR4);
            // Add the links
            KismetHelper.CreateOutputLink(prevNodeR4, "Out", outEvtR4);
            KismetHelper.CreateOutputLink(prevNodeR4, "Out", loadEvtR4);

            // Replace the conversation object
            ReplaceObjectWithEventHandshake(pcc, allers_char_moment_r4,
                pcc.GetUExport(8571), "ConvPlay_nor_emily_relationship4_d", "ConvEnd_nor_emily_relationship4_d");
        }

        private static void BioD_Nor_310LiaraOfficeCon(IMEPackage pcc)
        {
            ReplaceObjectWithEventHandshake(pcc, pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.Generic_Ambients"),
                pcc.GetUExport(553), "ConvPlay_norhen_lia_emily_break_ambs_a", "ConvEnd_norhen_lia_emily_break_ambs_a");
        }

        // OLD METHOD, before changing to KK's method
        /*private static void BioD_Nor_405Engineering(IMEPackage pcc)
        {
            // ENGINEERS
            ExportEntry engineers = pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.Engineering_Ambients.Engineers");
            (_, ExportEntry gateEng) = ReplaceObjectWithEventHandshake(pcc, engineers,
                pcc.GetUExport(979), "ConvPlay_nor_engineers_emily_a", "ConvEnd_nor_engineers_emily_a");

            // Create a remote event for handling ambient fail
            ExportEntry failEvtEng = CreateSequenceObjectWithProps(pcc, "SeqEvent_RemoteEvent", new PropertyCollection()
            {
                new NameProperty("ConvFail_nor_engineers_emily_a", "EventName")
            });
            KismetHelper.AddObjectToSequence(failEvtEng, engineers);
            // Connect the fail event
            KismetHelper.CreateOutputLink(failEvtEng, "Out", gateEng);

            // EXIT_AMBIENTS
            ExportEntry exit_ambient = pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.Engineering_Ambients.Exit_Ambient");
            ExportEntry prevNodeExit = pcc.GetUExport(305);
            KismetHelper.RemoveOutputLinks(prevNodeExit);

            // Create the remote event
            ExportEntry activatREExit = CreateSequenceObjectWithProps(pcc, "SeqAct_ActivateRemoteEvent", new PropertyCollection()
            {
                new NameProperty("ConvPlay_nor_engineers_emily_a", "EventName")
            });
            KismetHelper.AddObjectToSequence(activatREExit, exit_ambient);
            // Connect the event
            KismetHelper.CreateOutputLink(prevNodeExit, "Out", activatREExit);

            // MAKING_OUT
            ExportEntry making_out = pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.Ken_Gabby_Making_Out");
            ExportEntry prevNodeMakingOut = pcc.GetUExport(315);
            KismetHelper.RemoveOutputLinks(prevNodeMakingOut);

            // Create the remote event
            ExportEntry activatREMakingOut = CreateSequenceObjectWithProps(pcc, "SeqAct_ActivateRemoteEvent", new PropertyCollection()
            {
                new NameProperty("ConvPlay_nor_engineers_emily_a", "EventName")
            });
            KismetHelper.AddObjectToSequence(activatREMakingOut, making_out);
            // Connect the event
            KismetHelper.CreateOutputLink(prevNodeMakingOut, "Out", activatREMakingOut);

            ReplaceObjectWithEventHandshake(pcc, making_out,
                pcc.GetUExport(992), "ConvPlay_nor_engineers_emily_a", "ConvEnd_nor_engineers_emily_a");

            // ADAMS
            ReplaceObjectWithEventHandshake(pcc, pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.Adams"),
                pcc.GetUExport(193), "ConvInterrupt_nor_engineers_emily_a", "ConvInterruptEnd_nor_engineers_emily_a");

            // GABBY
            ReplaceObjectWithEventHandshake(pcc, pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.Gabby"),
                pcc.GetUExport(199), "ConvInterrupt_nor_engineers_emily_a", "ConvInterruptEnd_nor_engineers_emily_a");

            // KEN
            ReplaceObjectWithEventHandshake(pcc, pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.Ken"),
                pcc.GetUExport(201), "ConvInterrupt_nor_engineers_emily_a", "ConvInterruptEnd_nor_engineers_emily_a");
        }
        */

        private static void BioD_Nor_405Engineering(IMEPackage pcc)
        {
            ExportEntry engineers = pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence.Engineering_Ambients.Engineers");

            ExportEntry startAmbientConv = pcc.GetUExport(979);
            ExportEntry finishSequence = pcc.GetUExport(606);
            ExportEntry checkPokerCond = pcc.GetUExport(235);
            ExportEntry checkTraynorCond = pcc.GetUExport(236);
            ExportEntry executeTransition = pcc.GetUExport(297);

            // Create new objects
            ExportEntry compBool0 = CreateSequenceObjectWithProps(pcc, "SeqCond_CompareBool", []);
            ExportEntry setBool0 = CreateSequenceObjectWithProps(pcc, "SeqAct_SetBool", []);
            ExportEntry objTrue0 = CreateSequenceObjectWithProps(pcc, "SeqVar_Bool", [
                new IntProperty(1, "bValue")]);
            ExportEntry bypassBool = CreateSequenceObjectWithProps(pcc, "SeqVar_Bool", [
                new ArrayProperty<StrProperty>([
                    new StrProperty("Bypass Allers Poker")]
                    , "m_aObjComment")]);
            ExportEntry plotBool20574 = CreateSequenceObjectWithProps(pcc, "BioSeqVar_StoryManagerBool", [
                new IntProperty(20574, "m_nIndex")]);

            ExportEntry checkAllersCond = CreateSequenceObjectWithProps(pcc, "BioSeqAct_PMCheckConditional", [
                new EnumProperty("EBioAutoSet", pcc.Game, "Conditional"),
                new IntProperty(1442, "m_nIndex"),
                new EnumProperty("EBioRegionAutoSet", pcc.Game, "Region"),
                new EnumProperty("EBioPlotAutoSet", pcc.Game, "Plot")
                ]);
            ExportEntry setBool1 = CreateSequenceObjectWithProps(pcc, "SeqAct_SetBool", []);
            ExportEntry setBool2 = CreateSequenceObjectWithProps(pcc, "SeqAct_SetBool", []);
            ExportEntry objTrue1 = CreateSequenceObjectWithProps(pcc, "SeqVar_Bool", [
                new IntProperty(1, "bValue")]);
            ExportEntry objFalse0 = CreateSequenceObjectWithProps(pcc, "SeqVar_Bool", [
                new IntProperty(0, "bValue")]);

            KismetHelper.AddObjectsToSequence(engineers, true, [
                compBool0, setBool0, bypassBool, objTrue0, plotBool20574,
                checkAllersCond, setBool1, setBool2, objTrue1, objFalse0
                ]);

            // FIRST PART
            // Connect the outputs
            KismetHelper.RemoveOutputLinks(startAmbientConv);
            KismetHelper.CreateOutputLink(startAmbientConv, "Out", compBool0);
            KismetHelper.CreateOutputLink(startAmbientConv, "Failed", compBool0);
            KismetHelper.CreateOutputLink(compBool0, "True", setBool0);
            KismetHelper.CreateOutputLink(compBool0, "False", finishSequence);
            KismetHelper.CreateOutputLink(setBool0, "Out", finishSequence);
            // Connect the variable links
            KismetHelper.CreateVariableLink(compBool0, "Bool", bypassBool);
            KismetHelper.CreateVariableLink(setBool0, "Target", plotBool20574);
            KismetHelper.CreateVariableLink(setBool0, "Value", objTrue0);

            // SECOND PART
            // Connect the outputs
            KismetHelper.RemoveOutputLinks(checkPokerCond);
            KismetHelper.CreateOutputLink(checkPokerCond, "True", checkAllersCond);
            KismetHelper.CreateOutputLink(checkPokerCond, "False", checkTraynorCond);
            KismetHelper.CreateOutputLink(checkAllersCond, "True", setBool1);
            KismetHelper.CreateOutputLink(checkAllersCond, "False", executeTransition);
            KismetHelper.CreateOutputLink(setBool1, "Out", setBool2);
            KismetHelper.CreateOutputLink(setBool2, "Out", executeTransition);
            // Connect the variable links
            KismetHelper.CreateVariableLink(setBool1, "Target", bypassBool);
            KismetHelper.CreateVariableLink(setBool1, "Value", objTrue1);
            KismetHelper.CreateVariableLink(setBool2, "Target", plotBool20574);
            KismetHelper.CreateVariableLink(setBool2, "Value", objFalse0);
        }

        private static void BioD_Nor_420StarCargo_LOC_INT(IMEPackage pcc)
        {
            // Prepare the conversation
            ExportEntry inter2_i = pcc.GetUExport(2);
            ConversationExtended inter2_iConv = GetLoadedConversation(inter2_i);

            // Prepare the FaceFX controls
            FaceFXAnimSetEditorControl FXA_inter2_i_Player_F_Control = GetLoadedFXAControl(pcc, 497);
            FaceFXAnimSetEditorControl FXA_inter2_i_Player_M_Control = GetLoadedFXAControl(pcc, 498);
            FaceFXAnimSetEditorControl FXA_inter2_i_nor_crew_male1_F_Control = GetLoadedFXAControl(pcc, 493);
            FaceFXAnimSetEditorControl FXA_inter2_i_nor_crew_male1_M_Control = GetLoadedFXAControl(pcc, 494);

            // 670834 -> 71172310 | "Lose the piece..."
            DialogueNodeExtended node824810 = GetNode(inter2_iConv, 828410);
            ReplaceLineAndAudioAndFXA(pcc, node824810, "71172310", TlkAudioMap[71172310], FXA_inter2_i_Player_F_Control, FXA_inter2_i_Player_M_Control);
            WriteNode(inter2_i, node824810);

            // 670819 -> 71172309 | "Thanks, Commander"
            DialogueNodeExtended node828396 = GetNode(inter2_iConv, 828396);
            ReplaceLineAndAudioAndFXA(pcc, node828396, "71172309", TlkAudioMap[71172309], FXA_inter2_i_nor_crew_male1_F_Control, FXA_inter2_i_nor_crew_male1_M_Control);
            UpdateNodeLength(node828396, 1.5499983f);
            WriteNode(inter2_i, node828396);
        }

        private static void BioD_Nor_420StarCargoConv_LOC_INT(IMEPackage pcc)
        {
            // Prepare the conversation
            ExportEntry rel1 =  pcc.GetUExport(79);
            ConversationExtended rel1Conv = GetLoadedConversation(rel1);
            ExportEntry rel2 = pcc.GetUExport(80);
            ConversationExtended rel2Conv = GetLoadedConversation(rel2);
            ExportEntry kickoff = pcc.GetUExport(78);
            ConversationExtended kickoffConv = GetLoadedConversation(kickoff);

            // Prepare the FaceFX controls
            FaceFXAnimSetEditorControl FXA_rel1_Player_F_Control = GetLoadedFXAControl(pcc, 433);
            FaceFXAnimSetEditorControl FXA_rel1_Player_M_Control = GetLoadedFXAControl(pcc, 434);

            FaceFXAnimSetEditorControl FXA_rel2_Owner_F_Control = GetLoadedFXAControl(pcc, 436);
            FaceFXAnimSetEditorControl FXA_rel2_Owner_M_Control = GetLoadedFXAControl(pcc, 437);

            FaceFXAnimSetEditorControl FXA_kickoff_Player_F_Control = GetLoadedFXAControl(pcc, 428);
            FaceFXAnimSetEditorControl FXA_kickoff_Player_M_Control = GetLoadedFXAControl(pcc, 429);

            // 666410 -> 71172310 | "How's your new assignment..."
            DialogueNodeExtended node804301 = GetNode(rel1Conv, 804301);
            ReplaceLineAndAudioAndFXA(pcc, node804301, "71172311", TlkAudioMap[71172311], FXA_rel1_Player_F_Control, FXA_rel1_Player_M_Control);
            WriteNode(rel1, node804301);

            // 695915 -> 71172312 | "There it is..."
            DialogueNodeExtended node695915 = GetNode(rel2Conv, 948979);
            ReplaceLineAndAudioAndFXA(pcc, node695915, "71172312", TlkAudioMap[71172312], FXA_rel2_Owner_F_Control, FXA_rel2_Owner_M_Control);
            WriteNode(rel2, node695915);

            // 666410 -> 71172313 | "Not right now"
            DialogueNodeExtended node954574 = GetNode(kickoffConv, 954574);
            ReplaceLineAndAudioAndFXA(pcc, node954574, "71172313", TlkAudioMap[71172313], FXA_kickoff_Player_F_Control, FXA_kickoff_Player_M_Control);
            WriteNode(kickoff, node954574);

            // 699110 -> 71172314 | "I need you to leave...."
            DialogueNodeExtended node954576 = GetNode(kickoffConv, 954576);
            ReplaceLineAndAudioAndFXA(pcc, node954576, "71172314", TlkAudioMap[71172314], FXA_kickoff_Player_F_Control, FXA_kickoff_Player_M_Control);
            WriteNode(kickoff, node954576);
        }

        private static void BioD_End001_435CommRoom_LOC_INT(IMEPackage pcc)
        {
            // Prepare the conversation
            ExportEntry vid_terminal =  pcc.GetUExport(34);
            ConversationExtended vid_terminalConv = GetLoadedConversation(vid_terminal);

            // Prepare the FaceFX controls
            FaceFXAnimSetEditorControl FXA_vid_terminal_Player_F_Control = GetLoadedFXAControl(pcc, 145);
            FaceFXAnimSetEditorControl FXA_vid_terminal_Player_M_Control = GetLoadedFXAControl(pcc, 146);

            // 704981 -> 71172343 | "Ms. Wong"
            DialogueNodeExtended node979399 = GetNode(vid_terminalConv, 979399);
            ReplaceLineAndAudioAndFXA(pcc, node979399, "71172343", TlkAudioMap[71172343], FXA_vid_terminal_Player_F_Control, FXA_vid_terminal_Player_M_Control);
            WriteNode(vid_terminal, node979399);
        }

        private static void BioD_End001_436CRAllers_LOC_INT(IMEPackage pcc)
        {
            // Prepare the conversation
            // ExportEntry allers =  pcc.GetUExport(38);
            // ConversationExtended conv = GetLoadedConversation(allers);  

            // Prepare the FaceFX controls
            FaceFXAnimSetEditorControl FXA_allers_Player_F_Control = GetLoadedFXAControl(pcc, 185);
            FaceFXAnimSetEditorControl FXA_allers_Player_M_Control = GetLoadedFXAControl(pcc, 186);

            // How's the view
            ReplaceAudioInfo(pcc.GetUExport(495), TlkAudioMap[961432], true);
            ReplaceAudioInfo(pcc.GetUExport(486), TlkAudioMap[961432], false);
            FXA_allers_Player_F_Control.SelectLineByName("FXA_961432_F");
            FXA_allers_Player_M_Control.SelectLineByName("FXA_961432_M");
            ReplaceAnimationFromXml(FXA_allers_Player_F_Control, TlkAudioMap[961432].XMLUri_F);
            ReplaceAnimationFromXml(FXA_allers_Player_M_Control, TlkAudioMap[961432].XMLUri_M);
            // You had me
            ReplaceAudioInfo(pcc.GetUExport(494), TlkAudioMap[961434], true);
            ReplaceAudioInfo(pcc.GetUExport(484), TlkAudioMap[961434], false);
            FXA_allers_Player_F_Control.SelectLineByName("FXA_961434_F");
            FXA_allers_Player_M_Control.SelectLineByName("FXA_961434_M");
            ReplaceAnimationFromXml(FXA_allers_Player_F_Control, TlkAudioMap[961434].XMLUri_F);
            ReplaceAnimationFromXml(FXA_allers_Player_M_Control, TlkAudioMap[961434].XMLUri_M);
        }

        private static void BioD_End001_420HubStreet1(IMEPackage pcc)
        {
            string path = $@"G:\My Drive\Modding\Mass Effect\mods\Emily Returns\delivery\Emily Returns\Patches\TEB\BioD_End001_420HubStreet1.pcc";
            string seqPath = "TheWorld.PersistentLevel.Main_Sequence";
            using MEPackage patchedPCC = (MEPackage)MEPackageHandler.OpenMEPackage(path);

            // Import the PMCheck for the clothing settings, which in turns brings the rest of the sequence
            ExportEntry sequence = pcc.FindExport("TheWorld.PersistentLevel.Main_Sequence");
            ExportEntry persistentLevel = (ExportEntry)GetPersistentLevel(pcc);
            EntryImporter.ImportAndRelinkEntries(EntryImporter.PortingOption.CloneAllDependencies,
                patchedPCC.FindExport($"{seqPath}.SeqEvent_RemoteEvent_6"), pcc, sequence, true, new RelinkerOptionsPackage(), out _);
            // Import the Aller StuntActor, which brings all the remaining needed elements
            EntryImporter.ImportAndRelinkEntries(EntryImporter.PortingOption.CloneAllDependencies,
                patchedPCC.FindExport($"TheWorld.PersistentLevel.SFXStuntActor_19"), pcc, persistentLevel, true, new RelinkerOptionsPackage(), out IEntry stuntActor);

            // Add the cloned objects into the sequence
            ExportEntry PMCheck = pcc.FindExport($"{seqPath}.BioSeqAct_PMCheckState_9");
            KismetHelper.AddObjectsToSequence(sequence, false,
                pcc.FindExport($"{seqPath}.SeqEvent_RemoteEvent_6"), PMCheck,
                pcc.FindExport($"{seqPath}.SFXSeqAct_SetStuntMeshes_0"), pcc.FindExport($"{seqPath}.SFXSeqAct_SetStuntMeshes_1"),
                pcc.FindExport($"{seqPath}.SFXSeqAct_SetMaterialParameter_0"), pcc.FindExport($"{seqPath}.SFXSeqAct_SetMaterialParameter_1"));
            KismetHelper.CreateOutputLink(pcc.GetUExport(1082), "True", PMCheck, 0);

            // Replace the stuntActor reference
            ExportEntry allersVarObj = pcc.GetUExport(1090);
            allersVarObj.WriteProperty(new ObjectProperty(stuntActor, "ObjValue"));
            // Remove the Allers tag from the old actor
            ExportEntry allersOldActor = pcc.GetUExport(84);
            allersOldActor.RemoveProperty("Tag");

            // Add the actor to the PersistentLevel
            Level levelBinary = ObjectBinary.From<Level>(persistentLevel);
            levelBinary.Actors.Add(stuntActor.UIndex);
            persistentLevel.WriteBinary(levelBinary);
        }


        public static readonly string XMLPath = @"G:\My Drive\Modding\Mass Effect\mods\Emily Returns\project\audio";

        /// <summary>
        /// Map of TLK IDs and their respective line audio infos.
        /// </summary>
        public static readonly Dictionary<int, LineAudioInfo> TlkAudioMap = new()
        {
            { 71172309, new LineAudioInfo(
                "DLC_MOD_EmilyReturns_Audio", "DLC_MOD_EmilyReturns_Audio", 0, 12194, "", "4A5A0700",
                @$"{XMLPath}\BioD_Nor_420StarCargo\00670819-71172309.xml", @$"{XMLPath}\BioD_Nor_420StarCargo\00670819-71172309.xml")
            },
            { 71172310, new LineAudioInfo(
                "DLC_MOD_EmilyReturns_Audio", "DLC_MOD_EmilyReturns_Audio", 21190, 31840, "EC890700", "B2DC0700",
                @$"{XMLPath}\BioD_Nor_420StarCargo\00670834-71172310_f.xml", @$"{XMLPath}\BioD_Nor_420StarCargo\00670834-71172310_m.xml")
            },
            { 71172311, new LineAudioInfo(
                "DLC_MOD_EmilyReturns_Audio", "DLC_MOD_EmilyReturns_Audio", 15605, 18973, "12590800", "07960800",
                @$"{XMLPath}\BioD_Nor_420StarCargoConv\00666410-71172311_f.xml", @$"{XMLPath}\BioD_Nor_420StarCargoConv\00666410-71172311_m.xml")
            },
            { 71172312, new LineAudioInfo(
                "DLC_MOD_EmilyReturns_Audio", "DLC_MOD_EmilyReturns_Audio", 0, 53762, "", "26B20900",
                @$"{XMLPath}\BioD_Nor_420StarCargoConv\00695915-71172312.xml", @$"{XMLPath}\BioD_Nor_420StarCargoConv\00695915-71172312.xml")
            },
            { 71172313, new LineAudioInfo(
                "DLC_MOD_EmilyReturns_Audio", "DLC_MOD_EmilyReturns_Audio", 7365, 12287, "28840A00", "EDA00A00",
                @$"{XMLPath}\BioD_Nor_420StarCargoConv\00699108-71172313_f.xml", @$"{XMLPath}\BioD_Nor_420StarCargoConv\00699108-71172313_m.xml")
            },
            { 71172314, new LineAudioInfo(
                "DLC_MOD_EmilyReturns_Audio", "DLC_MOD_EmilyReturns_Audio", 16214, 20249, "ECD00A00", "42100B00",
                @$"{XMLPath}\BioD_Nor_420StarCargoConv\00699110-71172314_f.xml", @$"{XMLPath}\BioD_Nor_420StarCargoConv\00699110-71172314_m.xml")
            },
            { 71172316, new LineAudioInfo(
                "DLC_MOD_EmilyReturns_Audio", "DLC_MOD_EmilyReturns_Audio", 0, 39922, "", "00000000", "", "")
            },
            { 71172317, new LineAudioInfo(
                "DLC_MOD_EmilyReturns_Audio", "DLC_MOD_EmilyReturns_Audio", 0, 19425, "", "F29B0000", "", "")
            },
            { 71172343, new LineAudioInfo(
                "DLC_MOD_EmilyReturnsP_Audio", "DLC_MOD_EmilyReturnsP_Audio", 11090, 11653, "852D0000", "00000000",
                @$"{XMLPath}\BioD_End001_435CommRoom\00704981-71172343_f.xml", @$"{XMLPath}\BioD_End001_435CommRoom\00704981-71172343_m.xml")
            },
            { 961432, new LineAudioInfo(
                "DLC_MOD_EmilyReturnsP_Audio", "DLC_MOD_EmilyReturnsP_Audio", 24313, 25127, "D7580000", "0E080100",
                @$"{XMLPath}\BioD_End001_436CRAllers\00961432_f.xml", @$"{XMLPath}\BioD_End001_436CRAllers\00961432_m.xml")
            },
            { 961434, new LineAudioInfo(
                "DLC_MOD_EmilyReturnsP_Audio", "DLC_MOD_EmilyReturnsP_Audio", 20542, 22597, "D0B70000", "356A0100",
                @$"{XMLPath}\BioD_End001_436CRAllers\00961434_f.xml", @$"{XMLPath}\BioD_End001_436CRAllers\00961434_m.xml")
            },
        };
    }
}
