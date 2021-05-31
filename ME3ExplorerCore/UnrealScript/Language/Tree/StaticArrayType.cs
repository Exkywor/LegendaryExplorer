﻿using System;
using System.Collections.Generic;
using ME3ExplorerCore.UnrealScript.Analysis.Visitors;
using ME3ExplorerCore.UnrealScript.Utilities;

namespace ME3ExplorerCore.UnrealScript.Language.Tree
{
    public class StaticArrayType : VariableType, IEquatable<StaticArrayType>
    {
        public VariableType ElementType;
        public int Length;

        public StaticArrayType(VariableType elementType, int length, SourcePosition start = null, SourcePosition end = null) : base(elementType.Name, start, end)
        {
            ElementType = elementType;
            Length = length;
        }

        public override bool AcceptVisitor(IASTVisitor visitor)
        {
            return visitor.VisitNode(this);
        }

        public override IEnumerable<ASTNode> ChildNodes
        {
            get
            {
                if (Declaration != null) yield return Declaration;
                yield return ElementType;
            }
        }

        public override int Size => (ElementType?.Size ?? 0) * Length;

        public bool Equals(StaticArrayType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(ElementType, other.ElementType) && Length == other.Length;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StaticArrayType)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ElementType != null ? ElementType.GetHashCode() : 0) * 397) ^ Length;
            }
        }

        public static bool operator ==(StaticArrayType left, StaticArrayType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StaticArrayType left, StaticArrayType right)
        {
            return !Equals(left, right);
        }
    }
}
