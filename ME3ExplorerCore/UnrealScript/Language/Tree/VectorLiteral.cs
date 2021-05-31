﻿using ME3ExplorerCore.UnrealScript.Analysis.Visitors;
using ME3ExplorerCore.UnrealScript.Utilities;

namespace ME3ExplorerCore.UnrealScript.Language.Tree
{
    public class VectorLiteral : Expression
    {
        public float X;
        public float Y;
        public float Z;

        public VectorLiteral(float x, float y, float z, SourcePosition start = null, SourcePosition end = null) : base(ASTNodeType.VectorLiteral, start, end)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override VariableType ResolveType()
        {
            return new VariableType(Keywords.VECTOR)
            {
                PropertyType = EPropertyType.Vector
            };
        }

        public override bool AcceptVisitor(IASTVisitor visitor)
        {
            return visitor.VisitNode(this);
        }
    }
}
