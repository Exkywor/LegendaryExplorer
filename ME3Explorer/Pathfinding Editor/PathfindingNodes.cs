﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UMD.HCIL.Piccolo.Nodes;
using ME3Explorer.Pathfinding_Editor;
using ME3Explorer.SequenceObjects;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using ME3ExplorerCore.Packages;
using ME3ExplorerCore.Unreal;

namespace ME3Explorer.PathfindingNodes
{
    public abstract class PathfindingNode : PathfindingNodeMaster
    {
        static readonly Pen blackPen = Pens.Black;
        static Pen halfReachSpecPen = new Pen(Color.FromArgb(90, 90, 90));
        static readonly Pen slotToSlotPen = Pens.DarkOrange;
        static readonly Pen coverSlipPen = Pens.OrangeRed;
        static readonly Pen sfxLadderPen = Pens.Purple;
        static readonly Pen sfxBoostPen = Pens.Blue;
        static readonly Pen sfxJumpDownPen = Pens.Maroon;
        static readonly Pen sfxLargeBoostPen = Pens.DeepPink;
        internal SText val;

        public List<Volume> Volumes = new List<Volume>();
        public List<ExportEntry> ReachSpecs = new List<ExportEntry>();

        public abstract Color GetDefaultShapeColor();
        public abstract PPath GetDefaultShape();

        protected PathfindingNode(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
        {
            try
            {
                pcc = p;
                g = grapheditor;
                index = idx;
                export = pcc.GetUExport(index);
                comment = new SText(GetComment(), commentColor, false);
                comment.X = 0;
                comment.Y = 65 - comment.Height;
                comment.Pickable = false;

                var volsArray = export.GetProperty<ArrayProperty<StructProperty>>("Volumes");
                if (volsArray != null)
                {
                    foreach (var volumestruct in volsArray)
                    {
                        Volumes.Add(new Volume(volumestruct));
                    }
                }

                this.AddChild(comment);
                this.Pickable = true;
            }
            catch (Exception e)
            {
                Debugger.Break();
            }

            Bounds = new RectangleF(0, 0, 50, 50);
            SetShape();
            TranslateBy(x, y);
        }

        /// <summary>
        /// Refreshes reachspecs after one has been removed from this node.
        /// </summary>
        public void RefreshConnectionsAfterReachspecRemoval(List<PathfindingNodeMaster> graphNodes)
        {
            var edgesToRemove = new List<PathfindingEditorEdge>();
            foreach (PathfindingEditorEdge edge in Edges)
            {
                //Remove remote connections
                if (edge.EndPoints[0] != this && edge.EndPoints[0] is PathfindingNode pn0)
                {
                    var removed = pn0.Edges.Remove(edge);
                    if (removed)
                    {
                        Debug.WriteLine("Removed edge during refresh on endpoint 0");
                    }
                }
                if (edge.EndPoints[1] != this && edge.EndPoints[1] is PathfindingNode pn1)
                {
                    pn1.Edges.Remove(edge);
                    Debug.WriteLine("Removed edge during refresh on endpoint 1");
                }

                if (edge.IsOneWayOnly())
                {
                    edge.Pen.DashStyle = DashStyle.Dash;
                }
                else if (!edge.HasAnyOutboundConnections())
                {
                    edgesToRemove.Add(edge);
                }
            }
            //Debug.WriteLine("Remaining edges: " + Edges.Count);
            g.edgeLayer.RemoveChildren(edgesToRemove);
            //Edges.Clear();

            //CreateConnections(graphNodes);
            Edges.ForEach(PathingGraphEditor.UpdateEdgeStraight);
        }

        /// <summary>
        /// Creates the reachspec connections from this pathfinding node to others.
        /// </summary>
        public override void CreateConnections(List<PathfindingNodeMaster> graphNodes)
        {
            ReachSpecs = (SharedPathfinding.GetReachspecExports(export));
            foreach (ExportEntry spec in ReachSpecs)
            {
                Pen penToUse = blackPen;
                switch (spec.ObjectName.Name)
                {
                    case "SlotToSlotReachSpec":
                        penToUse = slotToSlotPen;
                        break;
                    case "CoverSlipReachSpec":
                        penToUse = coverSlipPen;
                        break;
                    case "SFXLadderReachSpec":
                        penToUse = sfxLadderPen;
                        break;
                    case "SFXLargeBoostReachSpec":
                        penToUse = sfxLargeBoostPen;
                        break;
                    case "SFXBoostReachSpec":
                        penToUse = sfxBoostPen;
                        break;
                    case "SFXJumpDownReachSpec":
                        penToUse = sfxJumpDownPen;
                        break;
                }
                //Get ending
                PropertyCollection props = spec.GetProperties();
                ExportEntry otherEndExport = SharedPathfinding.GetReachSpecEndExport(spec, props);

                /*
                if (props.GetProp<StructProperty>("End") is StructProperty endProperty &&
                    endProperty.GetProp<ObjectProperty>(SharedPathfinding.GetReachSpecEndName(spec)) is ObjectProperty otherNodeValue)
                {
                    othernodeidx = otherNodeValue.Value;
                }*/

                if (otherEndExport != null)
                {
                    bool isTwoWay = false;
                    PathfindingNodeMaster othernode = graphNodes.FirstOrDefault(x => x.export == otherEndExport);
                    if (othernode != null)
                    {
                        //Check for returning reachspec for pen drawing. This is going to incur a significant performance penalty...
                        var othernodeSpecs = SharedPathfinding.GetReachspecExports(otherEndExport);
                        foreach (var path in othernodeSpecs)
                        {
                            if (SharedPathfinding.GetReachSpecEndExport(path) == export)
                            {
                                isTwoWay = true;
                                break;
                            }
                        }

                        //var 
                        //    PropertyCollection otherSpecProperties = possibleIncomingSpec.GetProperties();

                        //    if (otherSpecProperties.GetProp<StructProperty>("End") is StructProperty endStruct)
                        //    {
                        //        if (endStruct.GetProp<ObjectProperty>(SharedPathfinding.GetReachSpecEndName(possibleIncomingSpec)) is ObjectProperty incomingTargetIdx)
                        //        {
                        //            if (incomingTargetIdx.Value == export.UIndex)
                        //            {
                        //                isTwoWay = true;
                        //                break;
                        //            }
                        //        }
                        //    }
                        //}

                        //if (othernode != null)
                        //{
                        var radius = props.GetProp<IntProperty>("CollisionRadius");
                        var height = props.GetProp<IntProperty>("CollisionHeight");

                        bool penCloned = false;
                        if (radius != null && height != null && (radius >= ReachSpecSize.MINIBOSS_RADIUS || height >= ReachSpecSize.MINIBOSS_HEIGHT))
                        {
                            penCloned = true;
                            penToUse = (Pen)penToUse.Clone();

                            if (radius >= ReachSpecSize.BOSS_RADIUS && height >= ReachSpecSize.BOSS_HEIGHT)
                            {
                                penToUse.Width = 3;
                            }
                            else
                            {
                                penToUse.Width = 2;
                            }
                        }
                        if (!isTwoWay)
                        {
                            if (!penCloned)
                            {
                                penToUse = (Pen)penToUse.Clone();
                                penCloned = true;
                            }
                            penToUse.DashStyle = DashStyle.Dash;
                        }

                        if (!penCloned)
                        {
                            //This will prevent immutable modifications later if we delete or modify reachspecs without a full
                            //graph redraw
                            penToUse = (Pen)penToUse.Clone();
                            penCloned = true;
                        }

                        PathfindingEditorEdge edge = new PathfindingEditorEdge
                        {
                            Pen = penToUse,
                            EndPoints = { [0] = this, [1] = othernode },
                            OutboundConnections = { [0] = true, [1] = isTwoWay }
                        };
                        if (!Edges.Any(x => x.DoesEdgeConnectSameNodes(edge)) && !othernode.Edges.Any(x => x.DoesEdgeConnectSameNodes(edge)))
                        {
                            //Only add edge if neither node contains this edge
                            Edges.Add(edge);
                            othernode.Edges.Add(edge);
                            g.edgeLayer.AddChild(edge);
                        }
                    }
                }
            }
        }

        public void SetShape()
        {
            if (shape != null)
            {
                RemoveChild(shape);
            }
            bool addVal = val == null;
            if (val == null)
            {
                val = new SText(index.ToString());

                if (Properties.Settings.Default.PathfindingEditorShowNodeSizes)
                {
                    StructProperty maxPathSize = export.GetProperty<StructProperty>("MaxPathSize");
                    if (maxPathSize != null)
                    {
                        float height = maxPathSize.GetProp<FloatProperty>("Height");
                        float radius = maxPathSize.GetProp<FloatProperty>("Radius");

                        if (radius >= 135)
                        {
                            val.Text += "\nBS";
                        }
                        else if (radius >= 90)
                        {
                            val.Text += "\nMB";
                        }
                        else
                        {
                            val.Text += "\nM";
                        }
                    }
                }
                val.Pickable = false;
                val.TextAlignment = StringAlignment.Center;
            }
            outlinePen = new Pen(GetDefaultShapeColor()); //Pen's can't be static and used in more than one place at a time.
            shape = GetDefaultShape();
            shape.Pen = Selected ? selectedPen : outlinePen;
            shape.Brush = pathfindingNodeBrush;
            shape.Pickable = false;
            val.X = 50f / 2 - val.Width / 2;
            val.Y = 50f / 2 - val.Height / 2;
            AddChild(0, shape);
            if (addVal)
            {
                AddChild(val);
            }
        }


        /// <summary>
        /// Class for storing Volumes information. THIS IS NOT A NODE TYPE.
        /// </summary>
        public class Volume
        {
            public int ActorUIndex;
            public FGuid ActorReference;
            public Volume(StructProperty volumestruct)
            {
                ActorReference = new FGuid(volumestruct.GetProp<StructProperty>("Guid"));
                ActorUIndex = volumestruct.GetProp<ObjectProperty>("Actor").Value;
            }
        }
    }
    [DebuggerDisplay("PathNode - {" + nameof(UIndex) + "}")]
    public class PathNode : PathfindingNode
    {
        public string Value
        {
            get => val.Text;
            set => val.Text = value;
        }
        private static readonly Color outlinePenColor = Color.FromArgb(34, 218, 218);

        public PathNode(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
        }

        public override Color GetDefaultShapeColor() => outlinePenColor;

        public override PPath GetDefaultShape() => PPath.CreateEllipse(0, 0, 50, 50);
    }

    public class BioPathPoint : PathNode
    {
        public BioPathPoint(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
            shape.Brush = dynamicPathfindingNodeBrush;
        }
    }

    public class PathNode_Dynamic : PathNode
    {
        public PathNode_Dynamic(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
            shape.Brush = dynamicPathnodefindingNodeBrush;
        }
    }

    public class SFXNav_WayPoint : PathfindingNode
    {
        private static readonly Color outlinePenColor = Color.FromArgb(255, 255, 50, 125);
        private static readonly PointF[] outlineShape = { new PointF(0, 20), 
            new PointF(25, 20), new PointF(25, 0),  //top left right angle
            new PointF(50, 25), new PointF(25, 50), // right point
            new PointF(25, 30), new PointF(0, 30) }; // End

        public SFXNav_WayPoint(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
        }

        public override Color GetDefaultShapeColor() => outlinePenColor;

        public override PPath GetDefaultShape() => PPath.CreatePolygon(outlineShape);
    }

    public class CoverLink : PathfindingNode
    {
        private static readonly Color outlinePenColor = Color.FromArgb(63, 102, 207);
        private static readonly PointF[] outlineShape = { new PointF(0, 50), new PointF(0, 35), new PointF(15, 35), new PointF(15, 0), new PointF(35, 0), new PointF(35, 35), new PointF(50, 35), new PointF(50, 50) };

        public CoverLink(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
        }

        public override Color GetDefaultShapeColor() => outlinePenColor;

        public override PPath GetDefaultShape() => PPath.CreatePolygon(outlineShape);
    }

    public class SFXDynamicCoverLink : CoverLink
    {
        public SFXDynamicCoverLink(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
            shape.Brush = dynamicPathfindingNodeBrush;
        }
    }

    public class SFXNav_BoostNode : PathfindingNode
    {
        private static readonly Color outlinePenColor = Color.FromArgb(17, 189, 146);
        private static readonly PointF[] boostdownshape = { new PointF(0, 0), new PointF(50, 0), new PointF(50, 50), new PointF(40, 40), new PointF(30, 50), new PointF(20, 40), new PointF(10, 50), new PointF(0, 40) };
        private static readonly PointF[] boostbottomshape = { new PointF(0, 50), new PointF(50, 50), new PointF(50, 0), new PointF(40, 10), new PointF(30, 0), new PointF(20, 10), new PointF(10, 0), new PointF(0, 10) };

        public SFXNav_BoostNode(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
        }

        public override Color GetDefaultShapeColor() => outlinePenColor;

        public override PPath GetDefaultShape()
        {
            BoolProperty bTopNode = export.GetProperty<BoolProperty>("bTopNode");
            PointF[] shapetouse = boostbottomshape;
            if (bTopNode?.Value == true)
            {
                shapetouse = boostdownshape;
            }
            return PPath.CreatePolygon(shapetouse);
        }
    }

    public class SFXNav_JumpDownNode : PathfindingNode
    {
        private static readonly Color outlinePenColor = Color.FromArgb(191, 82, 93);
        private static readonly PointF[] jumptopshape = { new PointF(0, 0), new PointF(40, 0), new PointF(40, 35), new PointF(50, 35), new PointF(35, 50), new PointF(20, 35), new PointF(30, 35), new PointF(30, 10), new PointF(0, 10) };
        private static readonly PointF[] jumplandingshape = { new PointF(15, 0), new PointF(35, 0), new PointF(35, 20), new PointF(50, 20), new PointF(25, 40), new PointF(50, 40), new PointF(50, 50), new PointF(0, 50), new PointF(0, 40), new PointF(25, 40), new PointF(0, 20), new PointF(15, 20) };

        public SFXNav_JumpDownNode(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
        }

        public override Color GetDefaultShapeColor() => outlinePenColor;

        public override PPath GetDefaultShape()
        {
            BoolProperty bTopNode = export.GetProperty<BoolProperty>("bTopNode");
            PointF[] shapetouse = jumplandingshape;
            if (bTopNode?.Value == true)
            {
                shapetouse = jumptopshape;
            }
            return PPath.CreatePolygon(shapetouse);
        }
    }

    public class SFXNav_LadderNode : PathfindingNode
    {
        private static readonly Color outlinePenColor = Color.FromArgb(127, 76, 186);
        private static readonly PointF[] laddertopshape = { new PointF(0, 0), new PointF(50, 0), new PointF(50, 10), new PointF(30, 10), new PointF(30, 20), new PointF(50, 20), new PointF(50, 30), new PointF(30, 30), new PointF(30, 40),
            new PointF(35, 40), new PointF(25, 50), new PointF(15, 40),
            new PointF(20, 40), new PointF(20, 30), new PointF(0, 30),new PointF(0, 20), new PointF(20, 20),new PointF(20, 10), new PointF(0, 10) };

        private static readonly PointF[] ladderbottomshape = { new PointF(15, 10), new PointF(25, 0), new PointF(35, 10), new PointF(30, 10), new PointF(30, 10), new PointF(30, 20), new PointF(50, 20), new PointF(50, 30), new PointF(30, 30), new PointF(30, 40), new PointF(50, 40), new PointF(50, 50),
            new PointF(0, 50), new PointF(0, 40), new PointF(20, 40), new PointF(20, 30), new PointF(0, 30),new PointF(0, 20), new PointF(20, 20),new PointF(20, 10) };

        public SFXNav_LadderNode(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
        }

        public override Color GetDefaultShapeColor() => outlinePenColor;

        public override PPath GetDefaultShape()
        {
            BoolProperty bTopNode = export.GetProperty<BoolProperty>("bTopNode");
            PointF[] shapetouse = ladderbottomshape;
            if (bTopNode?.Value == true)
            {
                shapetouse = laddertopshape;
            }
            return PPath.CreatePolygon(shapetouse);
        }
    }

    public class SFXNav_LargeBoostNode : PathfindingNode
    {
        private static readonly Color outlinePenColor = Color.FromArgb(219, 112, 147);
        private static readonly PointF[] outlineShape = { new PointF(0, 10), new PointF(10, 0), new PointF(20, 10), new PointF(30, 0), new PointF(40, 10), new PointF(50, 0), new PointF(50, 50), new PointF(40, 40), new PointF(30, 50), new PointF(20, 40), new PointF(10, 50), new PointF(0, 40) };

        public SFXNav_LargeBoostNode(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
        }

        public override Color GetDefaultShapeColor() => outlinePenColor;

        public override PPath GetDefaultShape() => PPath.CreatePolygon(outlineShape);
    }
    public class SFXDoorMarker : PathfindingNode
    {
        private static readonly Color outlinePenColor = Color.FromArgb(0, 100, 0);
        private static readonly PointF[] outlineShape = { new PointF(0, 25), new PointF(10, 0), new PointF(10, 13), new PointF(40, 13), new PointF(40, 0), new PointF(50, 25), new PointF(40, 50), new PointF(40, 37), new PointF(10, 37), new PointF(10, 50) };

        public SFXDoorMarker(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
        }

        public override Color GetDefaultShapeColor() => outlinePenColor;

        public override PPath GetDefaultShape() => PPath.CreatePolygon(outlineShape);
    }

    public class SFXNav_LeapNodeHumanoid : PathfindingNode
    {
        private static readonly Color outlinePenColor = Color.FromArgb(0, 100, 0);
        private static readonly PointF[] outlineShape = {
            new PointF(20, 0), new PointF(30, 0), new PointF(30, 5), new PointF(27, 5), new PointF(27, 10),  //inner elbow of right arm
            new PointF(45, 10), new PointF(45, 3), new PointF(50, 3), new PointF(50, 14), new PointF(27, 14), //upper thigh of right leg
            new PointF(27, 25), new PointF(50, 25), new PointF(40, 45), new PointF(35, 45), new PointF(44, 30), //behind right leg kneecap 
            new PointF(26, 30), new PointF(7, 50), new PointF(0, 50), new PointF(23, 30), new PointF(23, 14), //left armpit
            new PointF(0, 22), new PointF(0, 18), new PointF(23, 10), new PointF(23, 5), new PointF(20, 5) //bottom left of head
        };

        public SFXNav_LeapNodeHumanoid(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
        }

        public override Color GetDefaultShapeColor() => outlinePenColor;

        public override PPath GetDefaultShape() => PPath.CreatePolygon(outlineShape);
    }

    public class PendingNode : PathfindingNode
    {
        private static readonly Color outlinePenColor = Color.FromArgb(0, 0, 255);

        public PendingNode(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
        }

        public override Color GetDefaultShapeColor() => outlinePenColor;

        public override PPath GetDefaultShape() => shape = PPath.CreateRectangle(0, 0, 50, 50);
    }

    public class CoverSlotMarker : PathfindingNode
    {
        private static readonly Color outlinePenColor = Color.FromArgb(153, 153, 0);
        private static readonly PointF[] outlineShape = { new PointF(0, 15), new PointF(25, 0), new PointF(50, 15), new PointF(25, 50) };

        public CoverSlotMarker(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
        }

        public override Color GetDefaultShapeColor() => outlinePenColor;

        public override PPath GetDefaultShape() => PPath.CreatePolygon(outlineShape);
    }

    public class SFXDynamicCoverSlotMarker : CoverSlotMarker
    {
        public SFXDynamicCoverSlotMarker(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
            shape.Brush = dynamicPathfindingNodeBrush;
        }
    }

    public class MantleMarker : PathfindingNode
    {
        private static readonly Color outlinePenColor = Color.FromArgb(85, 59, 255);
        private static readonly PointF[] outlineShape = { new PointF(0, 50), new PointF(0, 10), new PointF(35, 10), new PointF(35, 0), new PointF(50, 20), new PointF(35, 35), new PointF(35, 25), new PointF(20, 25), new PointF(20, 50), new PointF(0, 50) };

        public MantleMarker(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
        }

        public override Color GetDefaultShapeColor() => outlinePenColor;

        public override PPath GetDefaultShape() => PPath.CreatePolygon(outlineShape);
    }

    public class SFXNav_HarvesterMoveNode : PathfindingNode
    {
        //Not sure if this is actually implemented or not.
        private static readonly Color outlinePenColor = Color.FromArgb(165, 70, 70);
        //Shape may be same as mantle marker
        private static readonly PointF[] outlineShape = {
            new PointF(0, 0), new PointF(17, 0), new PointF(17, 20), new PointF(33, 20), new PointF(33, 0), new PointF(50, 0), //Top part of H
            new PointF(50, 50), new PointF(33, 50), new PointF(33, 30), new PointF(17, 30), new PointF(17, 50), new PointF(0, 50) }; //Bottom part of H

        public SFXNav_HarvesterMoveNode(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
        }

        public override Color GetDefaultShapeColor() => outlinePenColor;

        public override PPath GetDefaultShape() => PPath.CreatePolygon(outlineShape);
    }

    public class SFXNav_LargeMantleNode : PathfindingNode
    {
        private static readonly Color outlinePenColor = Color.FromArgb(255, 119, 95);
        private static readonly PointF[] outlineShape = { new PointF(0, 50), new PointF(0, 10), new PointF(35, 10), new PointF(35, 0), new PointF(50, 20), new PointF(35, 35), new PointF(35, 25), new PointF(20, 25), new PointF(20, 50), new PointF(0, 50) };

        public SFXNav_LargeMantleNode(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
        }

        public override Color GetDefaultShapeColor() => outlinePenColor;

        public override PPath GetDefaultShape() => PPath.CreatePolygon(outlineShape);
    }

    public class SFXEnemySpawnPoint : PathfindingNode
    {
        private static readonly Color outlinePenColor = Color.FromArgb(255, 0, 0);
        private static readonly PointF[] outlineShape = { new PointF(0, 0), new PointF(25, 12), new PointF(50, 0), new PointF(37, 25), new PointF(50, 50), new PointF(25, 37), new PointF(0, 50), new PointF(12, 25) };

        public SFXEnemySpawnPoint(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
        }

        public override Color GetDefaultShapeColor() => outlinePenColor;

        public override PPath GetDefaultShape() => PPath.CreatePolygon(outlineShape);
    }

    public class SFXNav_JumpNode : PathfindingNode
    {
        private static readonly Color outlinePenColor = Color.FromArgb(148, 0, 211);
        private static readonly PointF[] outlineShape = { new PointF(25, 0), new PointF(50, 50), new PointF(25, 37), new PointF(0, 50) };

        public SFXNav_JumpNode(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
        }

        public override Color GetDefaultShapeColor() => outlinePenColor;

        public override PPath GetDefaultShape() => PPath.CreatePolygon(outlineShape);
    }

    public class SFXNav_TurretPoint : PathfindingNode
    {
        private static readonly Color outlinePenColor = Color.FromArgb(139, 69, 19);
        private static readonly PointF[] outlineShape = { new PointF(25, 0), new PointF(50, 25), new PointF(25, 50), new PointF(0, 25) };

        public SFXNav_TurretPoint(int idx, float x, float y, IMEPackage p, PathingGraphEditor grapheditor)
            : base(idx, x, y, p, grapheditor)
        {
        }
        public override Color GetDefaultShapeColor() => outlinePenColor;

        public override PPath GetDefaultShape() => PPath.CreatePolygon(outlineShape);
    }
}