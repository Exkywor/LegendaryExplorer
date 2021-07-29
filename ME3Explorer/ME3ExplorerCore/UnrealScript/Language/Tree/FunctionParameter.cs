﻿using ME3ExplorerCore.Helpers;
using ME3ExplorerCore.UnrealScript.Analysis.Visitors;
using ME3ExplorerCore.UnrealScript.Utilities;
using static ME3ExplorerCore.Unreal.UnrealFlags;

namespace ME3ExplorerCore.UnrealScript.Language.Tree
{
    public class FunctionParameter : VariableDeclaration
    {
        public bool IsOptional
        {
            get => Flags.Has(EPropertyFlags.OptionalParm);
            set => Flags = value ? Flags | EPropertyFlags.OptionalParm : Flags & ~EPropertyFlags.OptionalParm;
        }
        public bool IsOut
        {
            get => Flags.Has(EPropertyFlags.OutParm);
            set => Flags = value ? Flags | EPropertyFlags.OutParm : Flags & ~EPropertyFlags.OutParm;
        }
        public Expression DefaultParameter;
        public CodeBody UnparsedDefaultParam;

        public FunctionParameter(VariableType type, EPropertyFlags flags, string Name, int arrayLength = 1, SourcePosition start = null, SourcePosition end = null)
            : base(type, flags, Name, arrayLength, "None", start, end)
        {
            Type = ASTNodeType.FunctionParameter;
        }

        public override bool AcceptVisitor(IASTVisitor visitor)
        {
            return visitor.VisitNode(this);
        }
    }
}
