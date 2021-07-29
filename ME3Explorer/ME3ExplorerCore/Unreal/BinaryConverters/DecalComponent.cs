﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ME3ExplorerCore.Packages;
using ME3ExplorerCore.SharpDX;

namespace ME3ExplorerCore.Unreal.BinaryConverters
{
    public class DecalComponent : ObjectBinary
    {
        public StaticReceiverData[] StaticRecievers;

        protected override void Serialize(SerializingContainer2 sc)
        {
            sc.Serialize(ref StaticRecievers, SCExt.Serialize);
        }

        public override List<(UIndex, string)> GetUIndexes(MEGame game)
        {
            var uIndexes = new List<(UIndex, string)>();

            for (int i = 0; i < StaticRecievers.Length; i++)
            {
                StaticReceiverData data = StaticRecievers[i];
                uIndexes.Add((data.PrimitiveComponent, $"StaticReciever[{i}].PrimitiveComponent"));
                if (game == MEGame.ME3)
                {
                    uIndexes.AddRange(data.ShadowMap1D.Select((u, j) => (u, $"StaticReciever[{i}].ShadowMap1D[{j}]")));
                }
            }

            return uIndexes;
        }
    }

    public class StaticReceiverData
    {
        public UIndex PrimitiveComponent;
        public DecalVertex[] Vertices;
        public ushort[] Indices;
        public uint NumTriangles;
        public LightMap LightMap;
        public UIndex[] ShadowMap1D;//ME3
        public int Data;//ME3
        public int InstanceIndex;//ME3
    }

    public class DecalVertex
    {
        public Vector3 Position;
        public PackedNormal TangentX;
        public PackedNormal TangentZ;
        public Vector2 ProjectedUVs;//not ME3
        public Vector2 LightMapCoordinate;
        public Vector2 NormalTransform1;//not ME3
        public Vector2 NormalTransform2;//not ME3
    }

    public static partial class SCExt
    {
        public static void  Serialize(this SerializingContainer2 sc, ref DecalVertex vert)
        {
            if (sc.IsLoading)
            {
                vert = new DecalVertex();
            }
            sc.Serialize(ref vert.Position);
            sc.Serialize(ref vert.TangentX);
            sc.Serialize(ref vert.TangentZ);
            if (sc.Game < MEGame.ME3)
            {
                sc.Serialize(ref vert.ProjectedUVs);
            }
            sc.Serialize(ref vert.LightMapCoordinate);
            if (sc.Game < MEGame.ME3)
            {
                sc.Serialize(ref vert.NormalTransform1);
                sc.Serialize(ref vert.NormalTransform2);
            }
        }
        public static void Serialize(this SerializingContainer2 sc, ref StaticReceiverData dat)
        {
            if (sc.IsLoading)
            {
                dat = new StaticReceiverData();
            }
            sc.Serialize(ref dat.PrimitiveComponent);
            sc.BulkSerialize(ref dat.Vertices, Serialize, sc.Game == MEGame.ME3 ? 28 : 52);
            sc.BulkSerialize(ref dat.Indices, SCExt.Serialize, 2);
            sc.Serialize(ref dat.NumTriangles);
            sc.Serialize(ref dat.LightMap);
            if (sc.Game == MEGame.ME3)
            {
                sc.Serialize(ref dat.ShadowMap1D, Serialize);
                sc.Serialize(ref dat.Data);
                sc.Serialize(ref dat.InstanceIndex);
            }
        }
    }
}