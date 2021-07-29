﻿using System.Collections.Generic;
using ME3ExplorerCore.UnrealScript.Analysis.Visitors;
using ME3ExplorerCore.UnrealScript.Utilities;

namespace ME3ExplorerCore.UnrealScript.Language.Tree
{
    public class CaseStatement : Statement
    {
        public Expression Value;

        public CaseStatement(Expression expr, SourcePosition start, SourcePosition end) 
            : base(ASTNodeType.CaseStatement, start, end) 
        {
            Value = expr;
        }

        public override bool AcceptVisitor(IASTVisitor visitor)
        {
            return visitor.VisitNode(this);
        }
        public override IEnumerable<ASTNode> ChildNodes
        {
            get
            {
                yield return Value;
            }
        }
    }
}
