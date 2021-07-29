﻿using System.Collections.Generic;
using ME3ExplorerCore.UnrealScript.Analysis.Visitors;
using ME3ExplorerCore.UnrealScript.Parsing;
using ME3ExplorerCore.UnrealScript.Utilities;

namespace ME3ExplorerCore.UnrealScript.Language.Tree
{
    public class ArraySymbolRef : SymbolReference
    {
        public Expression Index;
        public Expression Array;

        public ArraySymbolRef(Expression array, Expression index, SourcePosition start, SourcePosition end) 
            : base(array, start: start, end: end)
        {
            Index = index;
            Type = ASTNodeType.ArrayReference;
            Array = array;
        }

        public override VariableType ResolveType()
        {
            return Array.ResolveType() switch
            {
                 DynamicArrayType dynArrType => dynArrType.ElementType,
                 StaticArrayType staticArrayType => staticArrayType.ElementType,
                _ => throw new ParseException("Expected an array!")
            };
        }

        public override bool AcceptVisitor(IASTVisitor visitor)
        {
            return visitor.VisitNode(this);
        }
        public override IEnumerable<ASTNode> ChildNodes
        {
            get
            {
                yield return Array;
                yield return Index;
            }
        }
    }
}
