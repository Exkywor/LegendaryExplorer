﻿using System;
using System.Collections.Generic;
using System.Linq;
using ME3Explorer.SharedUI;
using ME3ExplorerCore.Helpers;
using ME3ExplorerCore.Misc;
using ME3ExplorerCore.Packages;
using ME3ExplorerCore.Unreal;
using SharpDX;

namespace ME3Explorer.CurveEd
{
    public enum CurveType : byte
    {
        InterpCurveQuat,
        InterpCurveFloat,
        InterpCurveVector,
        InterpCurveVector2D,
        InterpCurveTwoVectors,
        InterpCurveLinearColor,
    }

    public class InterpCurve : NotifyPropertyChangedBase
    {

        private readonly IMEPackage pcc;
        private readonly CurveType curveType;

        public string Name { get; set; }
        public ObservableCollectionExtended<Curve> Curves { get; set; }
        public InterpCurve(IMEPackage _pcc, StructProperty prop)
        {
            pcc = _pcc;

            Curves = new ObservableCollectionExtended<Curve>();
            Name = prop.Name;
            curveType = Enums.Parse<CurveType>(prop.StructType);

            float InVal = 0f;
            CurveMode InterpMode = CurveMode.CIM_Linear;
            var points = prop.Properties.GetProp<ArrayProperty<StructProperty>>("Points");
            switch (curveType)
            {
                case CurveType.InterpCurveQuat:
                    throw new NotImplementedException($"InterpCurveQuat has not been implemented yet.");
                case CurveType.InterpCurveFloat:
                    float OutVal = 0f;
                    float ArriveTangent = 0f;
                    float LeaveTangent = 0f;
                    var vals = new LinkedList<CurvePoint>();
                    foreach (var point in points)
                    {
                        foreach (var p in point.Properties)
                        {
                            switch (p)
                            {
                                case FloatProperty floatProp when floatProp.Name == "InVal":
                                    InVal = floatProp.Value;
                                    break;
                                case FloatProperty floatProp when floatProp.Name == "OutVal":
                                    OutVal = floatProp.Value;
                                    break;
                                case FloatProperty floatProp when floatProp.Name == "ArriveTangent":
                                    ArriveTangent = floatProp.Value;
                                    break;
                                case FloatProperty floatProp when floatProp.Name == "LeaveTangent":
                                    LeaveTangent = floatProp.Value;
                                    break;
                                case EnumProperty enumProp when enumProp.Name == "InterpMode" && Enum.TryParse(enumProp.Value, out CurveMode enumVal):
                                    InterpMode = enumVal;
                                    break;
                            }
                        }
                        vals.AddLast(new CurvePoint(InVal, OutVal, ArriveTangent, LeaveTangent, InterpMode));
                    }
                    Curves.Add(new Curve("X", vals));
                    break;
                case CurveType.InterpCurveVector:
                    Vector3 OutValVec = new Vector3(0, 0, 0);
                    Vector3 ArriveTangentVec = new Vector3(0, 0, 0);
                    Vector3 LeaveTangentVec = new Vector3(0, 0, 0);
                    var x = new LinkedList<CurvePoint>();
                    var y = new LinkedList<CurvePoint>();
                    var z = new LinkedList<CurvePoint>();
                    foreach (var point in points)
                    {
                        foreach (var p in point.Properties)
                        {
                            switch (p)
                            {
                                case FloatProperty floatProp when floatProp.Name == "InVal":
                                    InVal = floatProp.Value;
                                    break;
                                case StructProperty structProp when structProp.Name == "OutVal":
                                    OutValVec = CommonStructs.GetVector3(structProp);
                                    break;
                                case StructProperty structProp when structProp.Name == "ArriveTangent":
                                    ArriveTangentVec = CommonStructs.GetVector3(structProp);
                                    break;
                                case StructProperty structProp when structProp.Name == "LeaveTangent":
                                    LeaveTangentVec = CommonStructs.GetVector3(structProp);
                                    break;
                                case EnumProperty enumProp when enumProp.Name == "InterpMode" && Enum.TryParse(enumProp.Value, out CurveMode enumVal):
                                    InterpMode = enumVal;
                                    break;
                            }
                        }
                        x.AddLast(new CurvePoint(InVal, OutValVec.X, ArriveTangentVec.X, LeaveTangentVec.X, InterpMode));
                        y.AddLast(new CurvePoint(InVal, OutValVec.Y, ArriveTangentVec.Y, LeaveTangentVec.Y, InterpMode));
                        z.AddLast(new CurvePoint(InVal, OutValVec.Z, ArriveTangentVec.Z, LeaveTangentVec.Z, InterpMode));
                    }
                    if (Name == "EulerTrack")
                    {
                        Curves.Add(new Curve("Roll", x));
                        Curves.Add(new Curve("Pitch", y));
                        Curves.Add(new Curve("Yaw", z));
                    }
                    else
                    {
                        Curves.Add(new Curve("X", x));
                        Curves.Add(new Curve("Y", y));
                        Curves.Add(new Curve("Z", z));
                    }
                    break;
                case CurveType.InterpCurveVector2D:
                    throw new NotImplementedException($"InterpCurveVector2D has not been implemented yet.");
                case CurveType.InterpCurveTwoVectors:
                    throw new NotImplementedException($"InterpCurveTwoVectors has not been implemented yet.");
                case CurveType.InterpCurveLinearColor:
                    throw new NotImplementedException($"InterpCurveLinearColor has not been implemented yet.");
            }
            foreach (var curve in Curves)
            {
                curve.SharedValueChanged += Curve_SharedValueChanged;
                curve.ListModified += Curve_ListModified;
            }
        }

        private bool updatingCurves = false;
        private void Curve_ListModified(object sender, (bool added, int index) e)
        {
            if (updatingCurves)
            {
                return;
            }
            updatingCurves = true;
            int index = e.index;
            Curve c = sender as Curve;
            //added
            if (e.added)
            {
                foreach (var curve in Curves)
                {
                    if (curve != c)
                    {
                        CurvePoint p = c.CurvePoints.ElementAt(index);
                        if (index == 0)
                        {
                            curve.CurvePoints.AddFirst(new CurvePoint(p.InVal, Enumerable.First(curve.CurvePoints).OutVal, 0, 0, p.InterpMode));
                        }
                        else
                        {
                            LinkedListNode<CurvePoint> prevNode = curve.CurvePoints.NodeAt(index - 1);
                            float outVal = prevNode.Value.OutVal;
                            if (prevNode.Next != null)
                            {
                                outVal += (prevNode.Next.Value.OutVal - outVal) / 2;
                            }
                            curve.CurvePoints.AddAfter(prevNode, new CurvePoint(p.InVal, outVal, 0, 0, p.InterpMode));
                        }
                    }
                }
            }
            //removed
            else
            {

                foreach (var curve in Curves)
                {
                    if (curve != c)
                    {
                        curve.CurvePoints.RemoveAt(index);
                    }
                }
            }
            updatingCurves = false;
        }

        private void Curve_SharedValueChanged(object sender, EventArgs e)
        {
            if (updatingCurves)
            {
                return;
            }
            updatingCurves = true;
            Curve c = sender as Curve;
            foreach (var curve in Curves)
            {
                if (curve != c)
                {
                    for (int i = 0; i < curve.CurvePoints.Count; i++)
                    {
                        curve.CurvePoints.ElementAt(i).InterpMode = c.CurvePoints.ElementAt(i).InterpMode;
                        curve.CurvePoints.ElementAt(i).InVal = c.CurvePoints.ElementAt(i).InVal;
                    }
                }
            }
            updatingCurves = false;
        }

        public StructProperty WriteProperties()
        {
            switch (curveType)
            {
                case CurveType.InterpCurveQuat:
                    break;
                case CurveType.InterpCurveFloat:
                    return new StructProperty("InterpCurveFloat", new PropertyCollection
                    {
                        new ArrayProperty<StructProperty>(Curves[0].CurvePoints.Select(point =>
                            new StructProperty("InterpCurvePointFloat", new PropertyCollection
                            {
                                new FloatProperty(point.InVal, "InVal"),
                                new FloatProperty(point.OutVal, "OutVal"),
                                new FloatProperty(point.ArriveTangent, "ArriveTangent"),
                                new FloatProperty(point.LeaveTangent, "LeaveTangent"),
                                new EnumProperty(point.InterpMode.ToString(), "EInterpCurveMode", pcc.Game, "InterpMode")
                            })
                        ), "Points")
                    }, Name);
                case CurveType.InterpCurveVector:
                    var points = new List<StructProperty>();
                    LinkedListNode<CurvePoint> xNode = Curves[0].CurvePoints.First;
                    LinkedListNode<CurvePoint> yNode = Curves[1].CurvePoints.First;
                    LinkedListNode<CurvePoint> zNode = Curves[2].CurvePoints.First;
                    while (xNode != null && yNode != null && zNode != null)
                    {
                        points.Add(new StructProperty("InterpCurvePointVector", new PropertyCollection
                        {
                            new FloatProperty(xNode.Value.InVal, "InVal"),
                            new StructProperty("Vector", new PropertyCollection
                            {
                                new FloatProperty(xNode.Value.OutVal),
                                new FloatProperty(yNode.Value.OutVal),
                                new FloatProperty(zNode.Value.OutVal)
                            }, "OutVal", true),
                            new StructProperty("Vector", new PropertyCollection
                            {
                                new FloatProperty(xNode.Value.ArriveTangent),
                                new FloatProperty(yNode.Value.ArriveTangent),
                                new FloatProperty(zNode.Value.ArriveTangent)
                            }, "ArriveTangent", true),
                            new StructProperty("Vector", new PropertyCollection
                            {
                                new FloatProperty(xNode.Value.LeaveTangent),
                                new FloatProperty(yNode.Value.LeaveTangent),
                                new FloatProperty(zNode.Value.LeaveTangent)
                            }, "LeaveTangent", true),
                            new EnumProperty(xNode.Value.InterpMode.ToString(), "EInterpCurveMode", pcc.Game, "InterpMode")
                        }));
                        xNode = xNode.Next;
                        yNode = yNode.Next;
                        zNode = zNode.Next;
                    }
                    return new StructProperty("InterpCurveVector", new PropertyCollection
                    {
                        new ArrayProperty<StructProperty>(points, "Points")
                    }, Name);
                case CurveType.InterpCurveVector2D:
                    break;
                case CurveType.InterpCurveTwoVectors:
                    break;
                case CurveType.InterpCurveLinearColor:
                    break;
            }

            return null;
        }
    }
}
