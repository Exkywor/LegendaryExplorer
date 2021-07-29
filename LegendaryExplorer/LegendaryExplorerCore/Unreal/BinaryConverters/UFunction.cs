﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegendaryExplorerCore.Helpers;
using LegendaryExplorerCore.Packages;
using static LegendaryExplorerCore.Unreal.UnrealFlags;

namespace LegendaryExplorerCore.Unreal.BinaryConverters
{
    public class UFunction : UStruct
    {
        public ushort NativeIndex;
        public byte OperatorPrecedence; //ME1/2
        public EFunctionFlags FunctionFlags;
        public ushort ReplicationOffset; //ME1/2
        public NameReference FriendlyName; //ME1/2
        protected override void Serialize(SerializingContainer2 sc)
        {
            base.Serialize(sc);
            sc.Serialize(ref NativeIndex);
            if (sc.Game.IsGame1() || sc.Game.IsGame2()) //This is present on PS3 ME1/ME2 but not ME3 for some reason.
            {
                sc.Serialize(ref OperatorPrecedence);
            }
            sc.Serialize(ref FunctionFlags);
            if (sc.Game is MEGame.ME1 or MEGame.ME2 && sc.Pcc.Platform != MEPackage.GamePlatform.PS3 && FunctionFlags.Has(EFunctionFlags.Net))
            {
                sc.Serialize(ref ReplicationOffset);
            }
            if ((sc.Game.IsGame1() || sc.Game.IsGame2()) && sc.Pcc.Platform != MEPackage.GamePlatform.PS3)
            {
                sc.Serialize(ref FriendlyName);
            }
        }

        public override List<(NameReference, string)> GetNames(MEGame game)
        {
            var names = base.GetNames(game);

            names.Add((FriendlyName, nameof(FriendlyName)));

            return names;
        }
    }

    public static partial class SCExt
    {
        public static void Serialize(this SerializingContainer2 sc, ref EFunctionFlags flags)
        {
            if (sc.IsLoading)
            {
                flags = (EFunctionFlags)sc.ms.ReadUInt32();
            }
            else
            {
                sc.ms.Writer.WriteUInt32((uint)flags);
            }
        }
    }
}