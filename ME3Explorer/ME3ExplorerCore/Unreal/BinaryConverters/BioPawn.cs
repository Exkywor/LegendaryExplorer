﻿using System.Collections.Generic;
using System.Linq;
using ME3ExplorerCore.Misc;
using ME3ExplorerCore.Packages;

namespace ME3ExplorerCore.Unreal.BinaryConverters
{
    public class BioPawn : ObjectBinary
    {
        public OrderedMultiValueDictionary<NameReference, UIndex> AnimationMap;//? Speculative name
        protected override void Serialize(SerializingContainer2 sc)
        {
            sc.Serialize(ref AnimationMap, SCExt.Serialize, SCExt.Serialize);
        }

        public override List<(UIndex, string)> GetUIndexes(MEGame game)
        {
            return AnimationMap.Select((kvp, i) => (kvp.Value, $"AnimationMap[{i}]")).ToList();
        }

        public override List<(NameReference, string)> GetNames(MEGame game)
        {
            var names = new List<(NameReference, string)>();

            names.AddRange(AnimationMap.Select((kvp, i) => (kvp.Key, $"AnimationMap[{i}]")));

            return names;
        }
    }
}
