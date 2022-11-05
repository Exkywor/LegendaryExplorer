using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LegendaryExplorerCore.Packages;

namespace LegendaryExplorerCore.Unreal.BinaryConverters
{
    public class StateFrame : ObjectBinary
    {
        public int Node; //UStruct*
        public int StateNode; //UState*, not in UDK
        public ulong ProbeMask; //uint in UDK
        public uint LatentAction; //ushort for every game other than ME1/ME2
        public int Offset;


        protected override void Serialize(SerializingContainer2 sc)
        {
            sc.Serialize(ref Node);
            sc.Serialize(ref StateNode);
            if (sc.Game is MEGame.UDK)
            {
                if (sc.IsLoading)
                {
                    uint tmp = sc.ms.ReadUInt32();
                    ProbeMask = 0xFFFFFFFF00000000UL | tmp;
                }
                else
                {
                    uint tmp = (uint)ProbeMask;
                    sc.ms.Writer.WriteUInt32(tmp);
                }
            }
            else
            {
                sc.Serialize(ref ProbeMask);
            }
            if (sc.Game is MEGame.ME1 or MEGame.ME2 && sc.Pcc.Platform is not MEPackage.GamePlatform.PS3)
            {
                sc.Serialize(ref LatentAction);
            }
            else
            {
                ushort tmp = (ushort)LatentAction;
                sc.Serialize(ref tmp);
                if (sc.IsLoading)
                {
                    LatentAction = tmp;
                }
            }
            if (sc.IsLoading)
            {
                int stateStackCount = sc.ms.ReadInt32();
                if (stateStackCount is not 0)
                {
                    throw new Exception("Number of state stack frames is greater than Zero! This should never be the case in a serialized object, and is likely due to data corruption.");
                }
            }
            else
            {
                sc.ms.Writer.WriteInt32(0);
            }
            sc.Serialize(ref Offset);
        }

        public override void ForEachUIndex<TAction>(MEGame game, in TAction action)
        {
            Unsafe.AsRef(action).Invoke(ref Node, nameof(Node));
            Unsafe.AsRef(action).Invoke(ref StateNode, nameof(StateNode));
        }

        public static StateFrame Create(int classUIndex)
        {
            return new StateFrame
            {
                Node = classUIndex,
                StateNode = classUIndex,
                ProbeMask = ulong.MaxValue,
                Offset = -1
            };
        }

        public static StateFrame FromExport(ExportEntry export)
        {
            if (!export.HasStack)
            {
                throw new Exception($"Cannot deserialize a {nameof(StateFrame)} for an export that does not have the {nameof(UnrealFlags.EObjectFlags.HasStack)} flag set.");
            }
            var sc = new SerializingContainer2(new MemoryStream(export.GetPrePropBinary()), export.FileRef, true);
            var sf = new StateFrame();
            sf.Serialize(sc);
            return sf;
        }
    }
}
