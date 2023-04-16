using DocumentFormat.OpenXml.InkML;
using LegendaryExplorer.Dialogs;
using LegendaryExplorer.Tools.TlkManagerNS;
using LegendaryExplorer.UserControls.ExportLoaderControls;
using LegendaryExplorerCore.Dialogue;
using LegendaryExplorerCore.Kismet;
using LegendaryExplorerCore.Matinee;
using LegendaryExplorerCore.Misc;
using LegendaryExplorerCore.Packages;
using LegendaryExplorerCore.Packages.CloningImportingAndRelinking;
using LegendaryExplorerCore.Unreal;
using LegendaryExplorerCore.Unreal.ObjectInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace LegendaryExplorer.DialogueEditor.DialogueEditorExperiments
{
    /// <summary>
    /// Class for Exkywor's preset buttons and stuff
    /// </summary>
    class DialogueEditorExperimentsE
    {
        #region Update Native Node String Ref
        // Changes the node's lineref and the parts of the FXA, WwiseStream, and referencing VOs that include it so it doesn't break
        public static void UpdateNativeNodeStringRef(DialogueEditorWindow dew)
        {
            DialogueNodeExtended node = dew.SelectedDialogueNode;

            if (dew.Pcc != null && node != null)
            {
                // Need to check if currStringRef exists
                int currStrRefID = node.LineStrRef;
                if (currStrRefID < 1)
                {
                    MessageBox.Show("The selected node does not have a Line String Ref, which is required in order to programatically replace the required elements.", "Warning", MessageBoxButton.OK);
                    return;
                }

                int newStrRefID = promptForID("New line string ref:", "Not a valid line string ref.");
                if (newStrRefID < 1)
                {
                    return;
                }

                if (currStrRefID == newStrRefID)
                {
                    MessageBox.Show("New StringRef matches the existing one.", "Warning", MessageBoxButton.OK);
                    return;
                }

                string currStringRef = currStrRefID.ToString();
                string newStringRef = newStrRefID.ToString();

                updateFaceFX(dew.FaceFXAnimSetEditorControl_M, currStringRef, newStringRef);
                updateFaceFX(dew.FaceFXAnimSetEditorControl_F, currStringRef, newStringRef);

                updateWwiseStream(node.WwiseStream_Male, currStringRef, newStringRef);
                updateWwiseStream(node.WwiseStream_Female, currStringRef, newStringRef);

                updateVOReferences(dew.Pcc, node.WwiseStream_Male, currStringRef, newStringRef);
                updateVOReferences(dew.Pcc, node.WwiseStream_Female, currStringRef, newStringRef);

                node.LineStrRef = newStrRefID;
                node.Line = TLKManagerWPF.GlobalFindStrRefbyID(node.LineStrRef, dew.Pcc);

                UpdateVOAndComment(node);
                dew.RecreateNodesToProperties(dew.SelectedConv);
                dew.ForceRefreshCommand.Execute(null);

                MessageBox.Show($"The node now points to {newStringRef}.", "Success", MessageBoxButton.OK);
            }
        }

        private static void updateVOReferences(IMEPackage pcc, ExportEntry wwiseStream, string oldRef, string newRef)
        {
            if (wwiseStream == null)
            {
                return;
            }

            var entry = pcc.GetEntry(wwiseStream.UIndex);

            var references = entry.GetEntriesThatReferenceThisOne();
            foreach (KeyValuePair<IEntry, List<string>> reference in references)
            {
                if (reference.Key.ClassName != "WwiseEvent")
                {
                    continue;
                }
                ExportEntry refEntry = (ExportEntry)pcc.GetEntry(reference.Key.UIndex);
                refEntry.ObjectNameString = refEntry.ObjectNameString.Replace(oldRef, newRef);
            }

        }

        private static void updateWwiseStream(ExportEntry wwiseStream, string oldRef, string newRef)
        {
            if (wwiseStream is null)
            {
                return;
            }

            // Pads the string refs so they have the required minimum length
            newRef = newRef.PadLeft(8, '0');
            oldRef = oldRef.PadLeft(8, '0');

            wwiseStream.ObjectNameString = wwiseStream.ObjectNameString.Replace(oldRef, newRef);
        }

        private static void updateFaceFX(FaceFXAnimSetEditorControl fxa, string oldRef, string newRef)
        {
            if (fxa.SelectedLine == null || fxa == null)
            {
                return;
            }

            var FaceFX = fxa.FaceFX;
            var SelectedLine = fxa.SelectedLine;

            if (SelectedLine.Path != null)
            {
                SelectedLine.Path = SelectedLine.Path.Replace(oldRef, newRef);
            }
            if (SelectedLine.ID != null)
            {
                SelectedLine.ID = newRef;
            }
            // Change FaceFX name
            if (SelectedLine.NameAsString != null)
            {
                string newName = SelectedLine.NameAsString.Replace(oldRef, newRef);
                if (FaceFX.Names.Contains(newName))
                {
                    SelectedLine.NameIndex = FaceFX.Names.IndexOf(newName);
                    SelectedLine.NameAsString = newName;
                }
                else
                {
                    FaceFX.Names.Add(newName);
                    SelectedLine.NameIndex = FaceFX.Names.Count - 1;
                    SelectedLine.NameAsString = newName;
                }
            }

            fxa.SaveChanges();
        }

        private static string promptForRef(string msg, string err)
        {
            if (PromptDialog.Prompt(null, msg) is string stringRef)
            {
                int intRef;
                if (string.IsNullOrEmpty(stringRef) || !int.TryParse(stringRef, out intRef))
                {
                    MessageBox.Show(err, "Warning", MessageBoxButton.OK);
                    return null;
                }
                return intRef.ToString();
            }
            return null;
        }
        #endregion

        #region Clone Node And Sequence
        /// <summary>
        /// Clones a Dialogue Node and its related Sequence, while giving it a unique id.
        /// </summary>
        /// <param name="dew">Dialogue Editor Window instance.</param>
        public static void CloneNodeAndSequence(DialogueEditorWindow dew)
        {
            DialogueNodeExtended selectedDialogueNode = dew.SelectedDialogueNode;

            if (dew.Pcc != null && selectedDialogueNode != null)
            {
                // Need to check if the node has associated data
                if (selectedDialogueNode.Interpdata == null)
                {
                    MessageBox.Show("The selected node does not have an InterpData associated with it.", "Warning", MessageBoxButton.OK);
                    return;
                }

                int newID = promptForID("New node ExportID:", "Not a valid ExportID.");
                if (newID == 0) { return; }

                if (selectedDialogueNode.ExportID.Equals(newID))
                {
                    MessageBox.Show("New ExportID matches the existing one.", "Warning", MessageBoxButton.OK);
                    return;
                }

                ExportEntry oldInterpData = selectedDialogueNode.Interpdata;

                // Get the Interp linked to the InterpData
                IEnumerable<KeyValuePair<IEntry, List<string>>> interpDataReferences = oldInterpData.GetEntriesThatReferenceThisOne()
                    .Where(entry => entry.Key.ClassName == "SeqAct_Interp");
                if (interpDataReferences.Count() > 1)
                {
                    MessageBox.Show("The selected Node's InterpData is linked to Interps. Please ensure it's only linked to one.", "Warning", MessageBoxButton.OK);
                }
                ExportEntry oldInterp = (ExportEntry)interpDataReferences.First().Key;

                // Get the/a ConvNode linked to the Interp
                ExportEntry oldConvNode = SeqTools.FindOutboundConnectionsToNode(oldInterp, SeqTools.GetAllSequenceElements(oldInterp).OfType<ExportEntry>())
                    .FirstOrDefault(entry => entry.ClassName == "BioSeqEvt_ConvNode");

                // Get the/a EndCurrentConvNode that the Interp outputs to
                ExportEntry oldEndNode = SeqTools.GetOutboundLinksOfNode(oldInterp).Select(outboundLink =>
                {
                    IEnumerable<SeqTools.OutboundLink> links = outboundLink.Where(link => link.LinkedOp.ClassName == "BioSeqAct_EndCurrentConvNode");
                    if (links.Any()) { return (ExportEntry)links.First().LinkedOp; } else { return null; }
                }).ToList().FirstOrDefault();

                ExportEntry sequence = SeqTools.GetParentSequence(oldInterpData);

                // Clone the Intero and Interpdata objects
                ExportEntry newInterp = cloneObject(oldInterp, sequence);
                ExportEntry newInterpData = EntryCloner.CloneTree(oldInterpData);
                KismetHelper.AddObjectToSequence(newInterpData, sequence, true);

                // Clone and link the Conv and End objects, if they exist
                ExportEntry newConvNode = null;
                if (oldConvNode != null)
                {
                    newConvNode = cloneObject(oldConvNode, sequence);
                    KismetHelper.CreateOutputLink(newConvNode, "Started", newInterp, 0);
                }

                if (oldEndNode != null)
                {
                    ExportEntry newEndNode = cloneObject(oldEndNode, sequence);
                    KismetHelper.CreateOutputLink(newInterp, "Completed", newEndNode, 0);
                    KismetHelper.CreateOutputLink(newInterp, "Reversed", newEndNode, 0);
                }

                // Save existing varLinks, minus the Data one
                List<SeqTools.VarLinkInfo> varLinks = SeqTools.GetVariableLinksOfNode(oldInterp);
                foreach (SeqTools.VarLinkInfo link in varLinks)
                {
                    if (link.LinkDesc == "Data") { link.LinkedNodes = new(); }
                }
                SeqTools.WriteVariableLinksToNode(newInterp, varLinks);
                KismetHelper.CreateVariableLink(newInterp, "Data", newInterpData);

                // Write the new nodeID
                if (newConvNode != null)
                {
                    IntProperty m_nNodeID = new(newID, "m_nNodeID");
                    newConvNode.WriteProperty(m_nNodeID);
                }

                // Clone and select the cloned node
                dew.NodeAddCommand.Execute(selectedDialogueNode.IsReply ? "CloneReply" : "CloneEntry");
                int index = selectedDialogueNode.IsReply ? dew.SelectedConv.ReplyList.Count : dew.SelectedConv.EntryList.Count;
                DialogueNodeExtended node = selectedDialogueNode.IsReply ? dew.SelectedConv.ReplyList[index - 1] : dew.SelectedConv.EntryList[index - 1];

                // Set the ExportID
                StructProperty prop = node.NodeProp;
                var nExportID = new IntProperty(newID, "nExportID");
                prop.Properties.AddOrReplaceProp(nExportID);
                dew.RecreateNodesToProperties(dew.SelectedConv);
                dew.ForceRefreshCommand.Execute(null);

                MessageBox.Show($"Node cloned and given the ExportID: {newID}.", "Success", MessageBoxButton.OK);
            }
        }
        #endregion

        #region Link Nodes
        /// <summary>
        /// Links all audio nodes in the conversation without an ExportID to the free ConvNodes in the sequence.
        /// </summary>
        /// <param name="dew">Current Dialogue Editor instance.</param>
        public static void LinkNodesFree(DialogueEditorWindow dew)
        {
            if (dew.Pcc == null || dew.SelectedConv == null) { return; }

            int convNodeIDBase = promptForInt("New ExportIDs base for extra IDs that may be need to be created:",
                "Not a valid base. It must be positive integer", -1, "New NodeID range");
            if (convNodeIDBase == -1) { return; }

            ConversationExtended conversation = dew.SelectedConv;

            HashSet<int> usedIDs = new();
            List<DialogueNodeExtended> nodes = new();
            List<DialogueNodeExtended> remainingNodes = new();

            List<DialogueNodeExtended> entryNodes = FilterAudioNodes(conversation.EntryList, el => el.ExportID < 1, usedIDs);
            List<DialogueNodeExtended> replyNodes = FilterAudioNodes(conversation.ReplyList, el => el.ExportID < 1, usedIDs);

            nodes.AddRange(entryNodes);
            nodes.AddRange(replyNodes);

            List<(int, ExportEntry, ExportEntry, int)> elements = GetConvNodeElements((ExportEntry)dew.SelectedConv.Sequence, conversation, usedIDs);

            // Assign ExportIDs to the dialogue nodes, and write the new StrRefIDs to the VOElements tracks
            for (int i = 0; i < nodes.Count; i++)
            {
                if (i >= elements.Count)
                {
                    // Store a list of nodes that couldn't get an ExportID
                    int remainingCount = nodes.Count - elements.Count;
                    if (remainingCount > 0)
                    {
                        remainingNodes.AddRange(nodes.GetRange(i, remainingCount));
                    }
                    break;
                }

                DialogueNodeExtended node = nodes[i];
                (int exportID, ExportEntry VOElements, ExportEntry interp, int _) = elements[i];

                // Write the new ExportID
                node.NodeProp.Properties.AddOrReplaceProp(new IntProperty(exportID, "nExportID"));
                // Update the Interp comment
                interp.WriteProperty(GenerateObjComment(node.Line));
                // Write the StringRef
                VOElements.WriteProperty(new IntProperty(node.LineStrRef, "m_nStrRefID"));

                usedIDs.Add(exportID); // Mark the ExportID as used
            }

            // Create the sequence objects for any nodes that are left without an ExportID
            if (remainingNodes.Any())
            {
                CreateNodesSequence(dew.Pcc, conversation, convNodeIDBase, nodes, usedIDs);
            }

            dew.RecreateNodesToProperties(dew.SelectedConv);
            dew.ForceRefreshCommand.Execute(null);

            // Code to check how many uses each ExportID has, to catch duplicates
            //Dictionary<int, int> test = new();
            //foreach (DialogueNodeExtended node in nodes)
            //{
            //    int exportID = node.NodeProp.GetProp<IntProperty>("nExportID");
            //    if (test.TryGetValue(exportID, out int val))
            //    {
            //        test[exportID] = val++;
            //    } else
            //    {
            //        test[exportID] = 1;
            //    }
            //}

            MessageBox.Show("Linked all nodes without an ExportID.", "Success", MessageBoxButton.OK);
        }

        /// <summary>
        /// Links all audio nodes in the conversation without an ExportID to the free ConvNodes that have a matching StringRef in the sequence.
        /// </summary>
        /// <param name="dew">Current Dialogue Editor instance.</param>
        public static void LinkNodesStrRef(DialogueEditorWindow dew)
        {
            if (dew.Pcc == null || dew.SelectedConv == null) { return; }

            int convNodeIDBase = 0;

            bool createObjsForNotMatched = MessageBoxResult.Yes == MessageBox.Show(
                "Generate new sequence objects with a basic VO track and new ExportIDs for nodes that don't have a match?",
                "Generate new sequence objects", MessageBoxButton.YesNo);

            if (createObjsForNotMatched)
            {
                convNodeIDBase = promptForInt("New ExportIDs base for new IDs that may be needed:",
                    "Not a valid base. It must be positive integer", -1, "New NodeID range");
                if (convNodeIDBase == -1) { return; }
            }

            ConversationExtended conversation = dew.SelectedConv;

            HashSet<int> usedIDs = new();
            List<DialogueNodeExtended> nodes = new();
            List<DialogueNodeExtended> notMatchedNodes = new();
            List<string> notMatchedNodesNames = new(); // Used for the result message

            List<DialogueNodeExtended> entryNodes = FilterAudioNodes(conversation.EntryList, el => el.ExportID < 1, usedIDs);
            List<DialogueNodeExtended> replyNodes = FilterAudioNodes(conversation.ReplyList, el => el.ExportID < 1, usedIDs);

            nodes.AddRange(entryNodes);
            nodes.AddRange(replyNodes);

            // Key: StrRefID, Val: (ExportID, Interp)
            Dictionary<int, (int, ExportEntry)> exportIDs = new();
            foreach (var el in GetConvNodeElements((ExportEntry)dew.SelectedConv.Sequence, conversation, usedIDs))
            {
                // We do it like this instead of using ToDictionary to avoid errors with duplicate keys
                (int ExportID, ExportEntry _, ExportEntry interp, int StrRefID) = el;
                exportIDs[StrRefID] = (ExportID, interp);
            }

            // Assign ExportIDs to the dialogue nodes that match the StrRefID
            foreach (DialogueNodeExtended node in nodes)
            {
                if (exportIDs.TryGetValue(node.LineStrRef, out (int, ExportEntry) el))
                {
                    (int exportID, ExportEntry interp) = el;

                    // Write the new ExportID
                    node.NodeProp.Properties.AddOrReplaceProp(new IntProperty(exportID, "nExportID"));
                    // Update the Interp comment
                    interp.WriteProperty(GenerateObjComment(node.Line));

                    usedIDs.Add(exportID); // Mark the ExportID as used
                }
                else
                {
                    notMatchedNodes.Add(node);
                    notMatchedNodesNames.Add(node.IsReply ? $"R{node.NodeCount}" : $"E{node.NodeCount}");
                }
            }

            // Create the sequence objects for any nodes that are left without an ExportID
            if (createObjsForNotMatched)
            {
                CreateNodesSequence(dew.Pcc, conversation, convNodeIDBase, notMatchedNodes, usedIDs);
                // Clear the not matched nodes
                notMatchedNodes = new();
                notMatchedNodesNames = new();
            }

            dew.RecreateNodesToProperties(dew.SelectedConv);
            dew.ForceRefreshCommand.Execute(null);

            string message = $"{nodes.Count - notMatchedNodes.Count} nodes matched.";
            if (notMatchedNodesNames.Any())
            {
                message = $"{message} The following nodes' StrRefIDs were not found in any InterpData: \n{string.Join(", ", notMatchedNodesNames)}";
            }

            MessageBox.Show(message, "Success", MessageBoxButton.OK);
        }

        /// <summary>
        /// Wrapper for CreateNodesSequence so it can used as an experiment.
        /// </summary>
        /// <param name="dew">Current Dialogue Editor instance.</param>
        public static void CreateNodesSequenceExperiment(DialogueEditorWindow dew)
        {
            if (dew.Pcc == null || dew.SelectedConv == null) { return; }

            int convNodeIDBase = promptForInt("New ExportIDs base:", "Not a valid base. It must be positive integer", -1, "New NodeID range");
            if (convNodeIDBase == -1) { return; }

            HashSet<int> usedIDs = new();
            List<DialogueNodeExtended> nodes = new();

            List<DialogueNodeExtended> entryNodes = FilterAudioNodes(dew.SelectedConv.EntryList, el => el.ExportID < 1, usedIDs);
            List<DialogueNodeExtended> replyNodes = FilterAudioNodes(dew.SelectedConv.ReplyList, el => el.ExportID < 1, usedIDs);

            nodes.AddRange(entryNodes);
            nodes.AddRange(replyNodes);

            CreateNodesSequence(dew.Pcc, dew.SelectedConv, convNodeIDBase, nodes, usedIDs);

            dew.RecreateNodesToProperties(dew.SelectedConv);
            dew.ForceRefreshCommand.Execute(null);

            string txtCount = nodes.Count == 1 ? "one audio node" : $"{nodes.Count} nodes";
            MessageBox.Show($"Successfully created the sequence objects for {txtCount}.", "Success", MessageBoxButton.OK);
        }


        /// <summary>
        /// Create the basic sequence objects for the selected audio node, if it doesn't have an ExportID.
        /// </summary>
        /// <param name="dew">Current Dialogue Editor instance.</param>
        public static void CreateSelectedNodeSequence(DialogueEditorWindow dew)
        {
            if (dew.Pcc == null || dew.SelectedDialogueNode == null) { return; }

            int exportID = promptForInt("New ExportID. If you input 0, a new ID will be generated:", "Not a valid ID. It must be positive integer", -1, "New NodeID");
            if (exportID == -1) { return; }

            DialogueNodeExtended node = dew.SelectedDialogueNode;

            string faceFX = node.FaceFX_Female ?? (node.FaceFX_Male ?? "");
            string errMsg = "";
            if (string.IsNullOrEmpty(faceFX) || !faceFX.Contains($"{node.LineStrRef}") || node.LineStrRef == -1)
            {
                errMsg = "The selected node does not contain valid audio data. Check it contains a LineStrRef, and that its FaceFX exists and points to it.";
            }
            if (node.ReplyType != EReplyTypes.REPLY_STANDARD)
            {
                errMsg = "The node type is not REPLY_STANDARD.";
            }
            if (node.Interpdata != null)
            {
                errMsg = "The selected node already points to an InterpData.";
            }
            if (!string.IsNullOrEmpty(errMsg))
            {
                MessageBox.Show(errMsg, "Warning", MessageBoxButton.OK);
                return;
            }

            // If the provided ID is 0, generate an ID not in use in the conversation
            List<int> newExportIDs = new();
            if (exportID == 0)
            {
                HashSet<int> usedIDs = new();
                List<DialogueNodeExtended> nodes = new();

                List<DialogueNodeExtended> entryNodes = FilterAudioNodes(dew.SelectedConv.EntryList, el => el.ExportID < 1, usedIDs);
                List<DialogueNodeExtended> replyNodes = FilterAudioNodes(dew.SelectedConv.ReplyList, el => el.ExportID < 1, usedIDs);

                nodes.AddRange(entryNodes);
                nodes.AddRange(replyNodes);

                newExportIDs = GenerateIDs(100, 1, usedIDs);
                exportID = newExportIDs.First();
            }

            // Write the new ExportID
            node.NodeProp.Properties.AddOrReplaceProp(new IntProperty(exportID, "nExportID"));

            // Create the required sequence elements and add it to the new exports list
            List<ExportEntry> newExports = CreateDialogueNodeSequence(dew.Pcc, exportID, dew.SelectedConv.BioConvo.GetProp<IntProperty>("m_nResRefID").Value,
                node.LineStrRef, node.Line);

            if (newExports.Any())
            {
                KismetHelper.AddObjectsToSequence((ExportEntry)dew.SelectedConv.Sequence, false, newExports.ToArray());
            }

            dew.RecreateNodesToProperties(dew.SelectedConv);
            dew.ForceRefreshCommand.Execute(null);

            MessageBox.Show($"Successfully created the sequence objects.", "Success", MessageBoxButton.OK);
        }

        /// <summary>
        /// Create the basic sequence objects for all the audio nodes that don't have an ExportID.
        /// </summary>
        /// <param name="pcc">Pcc to operate on.</param>
        /// <param name="conversation">Conversation to create the objects for.</param>
        /// <param name="convNodeIDBase">Base ID for the new ExportIDs.</param>
        /// <param name="nodes">Nodes to generate the sequence objects for.</param>
        /// <param name="usedIDs">ExportIDs in use.</param>

        public static void CreateNodesSequence(IMEPackage pcc, ConversationExtended conversation, int convNodeIDBase,
            List<DialogueNodeExtended> nodes, HashSet<int> usedIDs)
        {
            List<int> newExportIDs = GenerateIDs(convNodeIDBase, nodes.Count, usedIDs);
            List<ExportEntry> newExports = new(); // Sequence objects to add

            for (int i = 0; i < nodes.Count; i++)
            {
                DialogueNodeExtended node = nodes[i];
                int exportID = newExportIDs[i];

                // Write the new ExportID
                node.NodeProp.Properties.AddOrReplaceProp(new IntProperty(exportID, "nExportID"));

                // Create the required sequence elements and add it to the new exports list
                newExports.AddRange(CreateDialogueNodeSequence(pcc, exportID, conversation.BioConvo.GetProp<IntProperty>("m_nResRefID").Value,
                    node.LineStrRef, node.Line));
            }

            if (newExports.Any())
            {
                KismetHelper.AddObjectsToSequence((ExportEntry)conversation.Sequence, false, newExports.ToArray());
            }
        }

        /// <summary>
        /// Generate a list of IDs starting at base, of the given length, and skip IDs that are in the usedIDs list.
        /// </summary>
        /// <param name="baseID">Base num for the list.</param>
        /// <param name="length">Target length of the list.</param>
        /// <param name="usedIDs">IDs to skip.</param>
        /// <returns>Generated IDs.</returns>
        private static List<int> GenerateIDs(int baseID, int length, HashSet<int> usedIDs)
        {
            List<int> ids = new();

            int count = 0;
            while (count < length)
            {
                if (usedIDs != null && !usedIDs.Contains(baseID))
                {
                    ids.Add(baseID);
                    count++;
                }
                baseID++;
            }

            return ids;
        }

        /// <summary>
        /// Create all the required sequence elements for a dialogue node. IT DOES NOT ADD THE EXPORTS TO THE SEQUENCE.
        /// </summary>
        /// <param name="pcc">Pcc to operate on.</param>
        /// <param name="nodeID">Node's ExportID.</param>
        /// <param name="convResRefID">Conversation's ID.</param>
        /// <param name="strRefID">Node's StrRefID.</param>
        /// <param name="line">Text of the Node's StrRefID.</param>
        /// <returns>List of created exports.</returns>
        private static List<ExportEntry> CreateDialogueNodeSequence(IMEPackage pcc, int nodeID, int convResRefID, int strRefID, string line)
        {
            List<ExportEntry> exports = new();

            // Create ConvNode
            ExportEntry convNode = SequenceObjectCreator.CreateSequenceObject(pcc, "BioSeqEvt_ConvNode");
            PropertyCollection convNodeProps = SequenceObjectCreator.GetSequenceObjectDefaults(pcc, "BioSeqEvt_ConvNode", pcc.Game);
            convNodeProps.AddOrReplaceProp(new IntProperty(nodeID, "m_nNodeID"));
            convNodeProps.AddOrReplaceProp(new IntProperty(convResRefID, "m_nConvResRefID"));
            convNode.WriteProperties(convNodeProps);
            exports.Add(convNode);

            // Create Interp
            ExportEntry interp = SequenceObjectCreator.CreateSequenceObject(pcc, "SeqAct_Interp");
            PropertyCollection interpProps = SequenceObjectCreator.GetSequenceObjectDefaults(pcc, "SeqAct_Interp", pcc.Game);
            interpProps.AddOrReplaceProp(new ArrayProperty<StrProperty>("m_aObjComment")
            {
                new StrProperty(line == "No Data" ? "" : line.Length <= 32 ? line : $"{line.AsSpan(0, 29)}...")
            });
            // Add Conversation variable link
            ArrayProperty<StructProperty> variableLinks = interpProps.GetProp<ArrayProperty<StructProperty>>("VariableLinks");
            PropertyCollection props = GlobalUnrealObjectInfo.getDefaultStructValue(pcc.Game, "SeqVarLink", true);
            props.AddOrReplaceProp(new StrProperty("Conversation", "LinkDesc"));
            int index = pcc.FindImport("Engine.SeqVar_Object").UIndex;
            props.AddOrReplaceProp(new ObjectProperty(index, "ExpectedType"));
            props.AddOrReplaceProp(new IntProperty(1, "MinVars"));
            props.AddOrReplaceProp(new IntProperty(255, "MaxVars"));
            variableLinks.Add(new StructProperty("SeqVarLink", props));
            interpProps.AddOrReplaceProp(variableLinks);
            interp.WriteProperties(interpProps);
            exports.Add(interp);

            // Create EndCurrentConvNode
            ExportEntry endNode = SequenceObjectCreator.CreateSequenceObject(pcc, "BioSeqAct_EndCurrentConvNode");
            PropertyCollection endNodeProps = SequenceObjectCreator.GetSequenceObjectDefaults(pcc, "BioSeqAct_EndCurrentConvNode", pcc.Game);
            endNode.WriteProperties(endNodeProps);
            exports.Add(endNode);

            // Create InterpData
            ExportEntry interpData = SequenceObjectCreator.CreateSequenceObject(pcc, "InterpData");
            PropertyCollection interpDataProps = SequenceObjectCreator.GetSequenceObjectDefaults(pcc, "InterpData", pcc.Game);
            interpDataProps.AddOrReplaceProp(new FloatProperty(3, "InterpLength"));
            interpData.WriteProperties(interpDataProps);
            // Add Conversation group and VOElements track with its StrRefID
            ExportEntry conversationGroup = MatineeHelper.AddNewGroupToInterpData(interpData, "Conversation");
            ExportEntry VOElements = MatineeHelper.AddNewTrackToGroup(conversationGroup, "BioEvtSysTrackVOElements");
            VOElements.WriteProperty(new IntProperty(strRefID, "m_nStrRefID"));
            VOElements.WriteProperty(new ArrayProperty<StructProperty>("m_aTrackKeys"));

            exports.Add(interpData);

            // Connect elements
            KismetHelper.CreateOutputLink(convNode, "Started", interp, 0);
            KismetHelper.CreateOutputLink(interp, "Completed", endNode, 0);
            KismetHelper.CreateOutputLink(interp, "Reversed", endNode, 0);
            KismetHelper.CreateVariableLink(interp, "Data", interpData);

            return exports;
        }

        /// <summary>
        /// Update all the Interp comments and VOElements' StrRefIDs that are linked to the audio nodes of the selected conversation.
        /// </summary>
        /// <param name="dew">Current Dialogue Editor instance.</param>
        public static void UpdateVOsAndComments(DialogueEditorWindow dew)
        {
            if (dew.Pcc == null || dew.SelectedConv == null) { return; }

            List<DialogueNodeExtended> nodes = new();
            int updateCount = 0;

            nodes.AddRange(dew.SelectedConv.EntryList);
            nodes.AddRange(dew.SelectedConv.ReplyList);

            foreach (DialogueNodeExtended node in nodes)
            {
                if (IsAudioNode(node) && node.ExportID > 0)
                {
                    UpdateVOAndComment(node);
                    updateCount += 1;
                }
            }

            dew.RecreateNodesToProperties(dew.SelectedConv);
            dew.ForceRefreshCommand.Execute(null);

            string txtCount = updateCount == 1 ? "one audio node" : $"{updateCount} nodes";
            MessageBox.Show($"Successfully updated the StrRefID and Interp comment for {txtCount}.", "Success", MessageBoxButton.OK);
        }

        /// <summary>
        /// Update the Interp comment and VOElement' StrRefID that are linked to the selected audio node.
        /// Wrapper for UpdateVOAndComment so it can used as an experiment.
        /// </summary>
        /// <param name="dew">Current Dialogue Editor instance.</param>
        public static void UpdateVOAndCommentExperiment(DialogueEditorWindow dew)
        {
            if (dew.Pcc == null || dew.SelectedDialogueNode == null) { return; }

            DialogueNodeExtended node = dew.SelectedDialogueNode;

            string faceFX = node.FaceFX_Female ?? (node.FaceFX_Male ?? "");
            string errMsg = "";
            if (string.IsNullOrEmpty(faceFX) || !faceFX.Contains($"{node.LineStrRef}") || node.LineStrRef == -1)
            {
                errMsg = "The selected node does not contain valid audio data. Check it contains a LineStrRef, and that its FaceFX exists and points to it.";
            }
            if (node.ReplyType != EReplyTypes.REPLY_STANDARD)
            {
                errMsg = "The node type is not REPLY_STANDARD.";
            }
            if (node.Interpdata == null)
            {
                errMsg = "The selected node doesn't point to an InterpData.";
            }
            if (!string.IsNullOrEmpty(errMsg))
            {
                MessageBox.Show(errMsg, "Warning", MessageBoxButton.OK);
                return;
            }

            UpdateVOAndComment(node);

            dew.RecreateNodesToProperties(dew.SelectedConv);
            dew.ForceRefreshCommand.Execute(null);

            MessageBox.Show($"Successfully updated the StrRefID and Interp comment for the selected node.", "Success", MessageBoxButton.OK);
        }

        /// <summary>
        /// Update the StrRefID of the node's InterpData and the comment of the Interp linking to it.
        /// </summary>
        /// <param name="node">Node to update</param>
        private static void UpdateVOAndComment(DialogueNodeExtended node)
        {
            ExportEntry interpData = node.Interpdata;

            if (TryGetInterp(interpData, out ExportEntry interp))
            {
                UpdateInterpDataStrRefID(interpData, node.LineStrRef);
                // Update the Interp comment
                interp.WriteProperty(GenerateObjComment(node.Line));
            }
        }

        /// <summary>
        /// Try get the first Interp referencing the InterpData.
        /// </summary>
        /// <param name="interpData">InterpData to search on.</param>
        /// <param name="interp">Referencing interp.</param>
        /// <returns>Whether the Interp was found or not.</returns>
        private static bool TryGetInterp(ExportEntry interpData, out ExportEntry interp)
        {
            interp = null;

            Dictionary<IEntry, List<string>> refs = interpData.GetEntriesThatReferenceThisOne();

            if (refs.Count == 0) { return false; }

            IEntry entry = null;
            foreach (IEntry e in refs.Keys)
            {
                if (e.ClassName == "SeqAct_Interp")
                {
                    entry = e;
                    break;
                }
            }

            interp = (ExportEntry)entry;
            return true;
        }

        /// <summary>
        /// Filter the nodes, by the filter condition, to get only those that have valid audio information.
        /// </summary>
        /// <param name="nodes">Nodes to filter.</param>
        /// <param name="filter">Filter to apply to the audio nodes.</param>
        /// <param name="usedIDs">ExportIDs that exist in all the nodes. Useful for linking IDs or generating new ones.</param>
        /// <returns>Filtered nodes.</returns>
        private static List<DialogueNodeExtended> FilterAudioNodes(ObservableCollectionExtended<DialogueNodeExtended> nodes,
            Func<DialogueNodeExtended, bool> filter,
            HashSet<int> usedIDs = null)
        {
            if (nodes == null) { return null; }

            List<DialogueNodeExtended> filteredNodes = new();
            foreach (DialogueNodeExtended node in nodes)
            {
                if (node.ExportID > 0 && usedIDs != null) { usedIDs.Add(node.ExportID); }

                if (IsAudioNode(node) && filter(node)) { filteredNodes.Add(node); }
            }

            return filteredNodes;
        }

        /// <summary>
        /// Check if the given node is a valid audio node.
        /// CONDITION: Has FaceFX. FaceFX matches LineRef. LineRef is not -1. Reply type is REPLY_STANDARD.
        /// </summary>
        /// <param name="node">Node to check.</param>
        /// <returns>Whether it's an audio node or not.</returns>
        private static bool IsAudioNode(DialogueNodeExtended node)
        {
            // Check that there's at least one FaceFX and store its strRef
            string faceFX = node.FaceFX_Female ?? (node.FaceFX_Male ?? "");

            // Validate that the node is meant to have data (not autocontinues or dialogend)
            // and that has proper audio data (strRef matches the FaceFX)
            return !string.IsNullOrEmpty(faceFX) && node.LineStrRef != -1 && node.ReplyType == EReplyTypes.REPLY_STANDARD && faceFX.Contains($"{node.LineStrRef}");
        }

        /// <summary>
        /// Get a list of ExportIDs, VOElements track, Interp, and StrRefIDs of all the ConvNodes in the sequence.
        /// </summary>
        /// <param name="sequence">Sequence to get the elements from.</param>
        /// <param name="conversation">BioConversation to operate on.</param>
        /// <param name="usedIDs">List of ExportIDs that are already in use.</param>
        /// <returns>List of (ExportID, VOElements track, Interp, StrRefID)</returns>
        private static List<(int, ExportEntry, ExportEntry, int)> GetConvNodeElements(ExportEntry sequence, ConversationExtended conversation, HashSet<int> usedIDs)
        {
            IMEPackage pcc = sequence.FileRef;

            List<(int, ExportEntry, ExportEntry, int)> elements = new();

            List<IEntry> convNodes = SeqTools.GetAllSequenceElements(sequence)
                .Where(el => el.ClassName == "BioSeqEvt_ConvNode").ToList();

            foreach (ExportEntry node in convNodes)
            {
                IntProperty m_nNodeID = node.GetProperty<IntProperty>("m_nNodeID");
                // Skip nodes that don't have an ExportID, or an ExportID that is already in use
                if (m_nNodeID == null || usedIDs.Contains(m_nNodeID.Value)) { continue; }

                // Find the interp data
                ExportEntry interpData = null;
                List<ExportEntry> searchingExports = new() { node };

                ExportEntry seqActInterp = conversation.recursiveFindSeqActInterp(searchingExports, new List<ExportEntry>(), 10);
                if (seqActInterp == null) { continue; }

                ArrayProperty<StructProperty> varLinksProp = seqActInterp.GetProperty<ArrayProperty<StructProperty>>("VariableLinks");

                if (varLinksProp != null)
                {
                    foreach (StructProperty prop in varLinksProp)
                    {
                        string desc = prop.GetProp<StrProperty>("LinkDesc").Value; //ME3/ME2/ME1
                        if (desc == "Data") //ME3/ME1
                        {
                            ArrayProperty<ObjectProperty> linkedVars = prop.GetProp<ArrayProperty<ObjectProperty>>("LinkedVariables");
                            if (linkedVars != null && linkedVars.Any())
                            {
                                int datalink = linkedVars[0].Value;
                                interpData = sequence.FileRef.GetUExport(datalink);
                            }
                            break;
                        }
                    }
                }

                // Only consider as valid ExportIDs that lead to InterpDatas
                if (interpData == null) { continue; }

                // Store the StrRefID in the VOElements track, if one exists
                int strRefID = GetVOStrRefID(interpData, out ExportEntry VOElements);

                // Only consider as valid InterpDatas that contain a VOElements track
                if (VOElements == null) { continue; }

                elements.Add((m_nNodeID.Value, VOElements, seqActInterp, strRefID));
            }

            return elements;
        }

        /// <summary>
        /// Get the StrRefID of the VOElements track and the track itself of an InterpData, if they exist.
        /// </summary>
        /// <param name="interpData">InterpData to find the value on.</param>
        /// <param name="VOTrack">VOElementes track, if it exists.</param>
        /// <returns>StrRefID, if it exists.</returns>
        private static int GetVOStrRefID(ExportEntry interpData, out ExportEntry VOTrack)
        {
            VOTrack = null;
            if (!MatineeHelper.TryGetInterpGroup(interpData, "Conversation", out ExportEntry interpGroup))
            {
                return 0;
            }

            if (!MatineeHelper.TryGetInterpTrack(interpGroup, "BioEvtSysTrackVOElements", out ExportEntry interpTrack))
            {
                return 0;
            }

            IntProperty m_nStrRefID = interpTrack.GetProperty<IntProperty>("m_nStrRefID");
            if (m_nStrRefID == null)
            {
                return 0;
            }

            VOTrack = interpTrack;
            return m_nStrRefID.Value;
        }


        /// <summary>
        /// Update the StrRefID of the VOElements track of the InterpData. Creates any missing element.
        /// </summary>
        /// <param name="interpData">InterpData to update the value on.</param>
        /// <param name="strRefID">StringRefID to set.</param>
        private static void UpdateInterpDataStrRefID(ExportEntry interpData, int strRefID)
        {
            if (!MatineeHelper.TryGetInterpGroup(interpData, "Conversation", out ExportEntry interpGroup))
            {
                interpGroup = MatineeHelper.AddNewGroupToInterpData(interpData, "Conversation");
            }

            if (!MatineeHelper.TryGetInterpTrack(interpGroup, "BioEvtSysTrackVOElements", out ExportEntry interpTrack))
            {
                interpTrack = MatineeHelper.AddNewTrackToGroup(interpGroup, "BioEvtSysTrackVOElements");
            }

            interpTrack.WriteProperty(new IntProperty(strRefID, "m_nStrRefID"));
        }

        /// <summary>
        /// Generate an ObjComment array containing a single comment based on the given line,
        /// concatenating the line at 29 characters and adding an ellipsis at the end
        /// </summary>
        /// <param name="line">Line to use to generate the comment.</param>
        /// <returns>Generated ObjComment array.</returns>
        private static ArrayProperty<StrProperty> GenerateObjComment(string line)
        {
            return new("m_aObjComment")
            {
                new StrProperty(line == "No Data" ? "" : line.Length <= 32 ? line : $"{line.AsSpan(0, 29)}...")
            };
        }
        #endregion

        // HELPER FUNCTIONS
        #region Helper functions
        private static int promptForID(string msg, string err)
        {
            if (PromptDialog.Prompt(null, msg) is string strID)
            {
                int ID;
                if (!int.TryParse(strID, out ID))
                {
                    MessageBox.Show(err, "Warning", MessageBoxButton.OK);
                    return 0;
                }
                return ID;
            }
            return 0;
        }

        /// <summary>
        /// Prompts the user for an int, verifying that the int is valid.
        /// </summary>
        /// <param name="msg">Message to display for the prompt.</param>
        /// <param name="err">Error message to display.</param>
        /// <param name="biggerThan">Number the input must be bigger than. If not provided -2,147,483,648 will be used.</param>
        /// <param name="title">Title for the prompt.</param>
        /// <returns>The input int.</returns>
        private static int promptForInt(string msg, string err, int biggerThan = -2147483648, string title = "")
        {
            if (PromptDialog.Prompt(null, msg, title) is string stringPrompt)
            {
                int intPrompt;
                if (string.IsNullOrEmpty(stringPrompt) || !int.TryParse(stringPrompt, out intPrompt) || !(intPrompt > biggerThan))
                {
                    MessageBox.Show(err, "Warning", MessageBoxButton.OK);
                    return -1;
                }
                return intPrompt;
            }
            return -1;
        }

        // From SequenceEditorWPF.xaml.cs
        private static ExportEntry cloneObject(ExportEntry old, ExportEntry sequence, bool topLevel = true, bool incrementIndex = true)
        {
            //SeqVar_External needs to have the same index to work properly
            ExportEntry exp = EntryCloner.CloneEntry(old, incrementIndex: incrementIndex && old.ClassName != "SeqVar_External");

            KismetHelper.AddObjectToSequence(exp, sequence, topLevel);
            // cloneSequence(exp);
            return exp;
        }

        #endregion
    }
}
