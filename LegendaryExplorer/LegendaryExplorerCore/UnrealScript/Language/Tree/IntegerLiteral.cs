using LegendaryExplorerCore.UnrealScript.Analysis.Symbols;
using LegendaryExplorerCore.UnrealScript.Analysis.Visitors;
using static LegendaryExplorerCore.UnrealScript.Utilities.Keywords;

namespace LegendaryExplorerCore.UnrealScript.Language.Tree
{
    public class IntegerLiteral : Expression
    {
        //needs to be a long because this is assigned before it is combined with a preceding minus sign
        //and the minimum value of an int is of one greater magnitude than the max value
        public long Value;

        public string NumType = INT;

        public IntegerLiteral(long val, int start = -1, int end = -1) 
            : base(ASTNodeType.IntegerLiteral, start, end)
        {
            Value = val;
        }

        public override bool AcceptVisitor(IASTVisitor visitor)
        {
            return visitor.VisitNode(this);
        }

        public override VariableType ResolveType() =>
            NumType switch
            {
                BYTE => SymbolTable.ByteType,
                BIOMASK4 => SymbolTable.BioMask4Type,
                _ => SymbolTable.IntType
            };
    }
}
