﻿using System.Collections.Generic;
using LegendaryExplorerCore.UnrealScript.Analysis.Visitors;
using LegendaryExplorerCore.UnrealScript.Language.Util;
using LegendaryExplorerCore.UnrealScript.Utilities;
using static LegendaryExplorerCore.Unreal.UnrealFlags;

namespace LegendaryExplorerCore.UnrealScript.Language.Tree
{
    public class State : ASTNode, IContainsByteCode, IHasFileReference
    {
        public EStateFlags Flags;
        public string Name { get; }
        public CodeBody Body { get; set; }
        public State Parent;
        public List<Function> Functions;
        public List<Function> Ignores;
        public List<Label> Labels;

        public State(string name, CodeBody body, EStateFlags flags,
            State parent, List<Function> funcs, List<Function> ignores,
            List<Label> labels, SourcePosition start, SourcePosition end)
            : base(ASTNodeType.State, start, end)
        {
            Flags = flags;
            Name = name;
            Body = body;
            Parent = parent;
            Functions = funcs;
            Ignores = ignores;
            Labels = labels;
            if (Body != null) Body.Outer = this;
        }

        public override bool AcceptVisitor(IASTVisitor visitor)
        {
            return visitor.VisitNode(this);
        }
        public override IEnumerable<ASTNode> ChildNodes
        {
            get
            {
                yield return Parent;
                if (Functions != null) foreach (Function function in Functions) yield return function;
                yield return Body;
                if (Ignores != null) foreach (Function function in Ignores) yield return function;
            }
        }

        public string FilePath { get; init; }
        public int UIndex { get; init; }
    }
}