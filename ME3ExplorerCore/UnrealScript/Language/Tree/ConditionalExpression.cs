﻿using System.Collections.Generic;
using ME3ExplorerCore.UnrealScript.Analysis.Visitors;
using ME3ExplorerCore.UnrealScript.Utilities;

namespace ME3ExplorerCore.UnrealScript.Language.Tree
{
    public class ConditionalExpression : Expression
    {
        public Expression Condition;
        public Expression TrueExpression;
        public Expression FalseExpression;

        public VariableType ExpressionType;

        public ConditionalExpression(Expression cond, Expression first, Expression second, SourcePosition start, SourcePosition end)
            : base(ASTNodeType.ConditionalExpression, start, end)
        {
            Condition = cond;
            TrueExpression = first;
            FalseExpression = second;
        }

        public override bool AcceptVisitor(IASTVisitor visitor)
        {
            return visitor.VisitNode(this);
        }

        public override VariableType ResolveType()
        {
            return ExpressionType ?? TrueExpression.ResolveType() ?? FalseExpression.ResolveType();
        }
        public override IEnumerable<ASTNode> ChildNodes
        {
            get
            {
                yield return Condition;
                yield return TrueExpression;
                yield return FalseExpression;
            }
        }
    }
}
