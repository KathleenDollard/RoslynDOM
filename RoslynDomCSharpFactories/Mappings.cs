using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using RoslynDom.Common;

namespace RoslynDom.CSharp
{
    internal static class Mappings
    {
        private static List<Tuple<SyntaxKind, SyntaxKind, LiteralKind>> LiteralKindMap = new List<Tuple<SyntaxKind, SyntaxKind, LiteralKind>>()
        {
            new Tuple<SyntaxKind, SyntaxKind, LiteralKind>(SyntaxKind.StringLiteralToken,    SyntaxKind.StringLiteralExpression,    LiteralKind.String),
            new Tuple<SyntaxKind, SyntaxKind,LiteralKind>( SyntaxKind.NumericLiteralToken,   SyntaxKind.NumericLiteralExpression,   LiteralKind.Numeric),
            new Tuple<SyntaxKind,SyntaxKind, LiteralKind>( SyntaxKind.TrueKeyword,           SyntaxKind.TrueKeyword,                LiteralKind.Boolean),
            new Tuple<SyntaxKind, SyntaxKind,LiteralKind>( SyntaxKind.FalseKeyword,          SyntaxKind.FalseKeyword,               LiteralKind.Boolean),
        };

        public static LiteralKind LiteralKindFromSyntaxKind(SyntaxKind kind)
        {
            foreach (var tuple in LiteralKindMap)
            {
                if (tuple.Item1 == kind) { return tuple.Item3; }
            }
            throw new InvalidOperationException();
        }

        public static SyntaxKind SyntaxKindFromLiteralKind(LiteralKind literalKind, object value)
        {
            if (literalKind == LiteralKind.Boolean)
            {
                if ((bool)value) { return SyntaxKind.TrueLiteralExpression; }
                return SyntaxKind.FalseLiteralExpression;
            }
            foreach (var tuple in LiteralKindMap)
            {
                if (tuple.Item3 == literalKind)
                { return tuple.Item2; }
            }
            throw new InvalidOperationException();
        }

        private static List<Tuple<SyntaxKind, Operator>> operatorMap = new List<Tuple<SyntaxKind, Operator>>()
        {
             new Tuple<SyntaxKind, Operator> ( SyntaxKind.SimpleAssignmentExpression, Operator.Equals ),
             new Tuple<SyntaxKind, Operator> ( SyntaxKind.AddAssignmentExpression, Operator.AddAssignment),
             new Tuple<SyntaxKind, Operator> ( SyntaxKind.SubtractAssignmentExpression, Operator.SubtractAssignment),
             new Tuple<SyntaxKind, Operator> ( SyntaxKind.MultiplyAssignmentExpression, Operator.MultiplyAssignment),
             new Tuple<SyntaxKind, Operator> ( SyntaxKind.DivideAssignmentExpression, Operator.DivideAssignment),
             new Tuple<SyntaxKind, Operator> ( SyntaxKind.ModuloAssignmentExpression, Operator.ModuloAssignment),
             new Tuple<SyntaxKind, Operator> ( SyntaxKind.AndAssignmentExpression, Operator.AndAssignment),
             new Tuple<SyntaxKind, Operator> ( SyntaxKind.ExclusiveOrAssignmentExpression, Operator.ExclusiveOrAssignment),
             new Tuple<SyntaxKind, Operator> ( SyntaxKind.OrAssignmentExpression, Operator.OrAssignment),
             new Tuple<SyntaxKind, Operator> ( SyntaxKind.LeftShiftAssignmentExpression, Operator.LeftShiftAssignment),
             new Tuple<SyntaxKind, Operator> ( SyntaxKind.RightShiftAssignmentExpression, Operator.RightShiftAssignment)
        };

        public static Operator GetOperatorFromCSharpKind(SyntaxKind kind)
        {
            foreach (var tuple in operatorMap)
            {
                if (tuple.Item1 == kind) { return tuple.Item2; }
            }
            throw new InvalidOperationException();
        }

        public static SyntaxKind GetSyntaxKindFromOperator(Operator op)
        {
            foreach (var tuple in operatorMap)
            {
                if (tuple.Item2 == op) { return tuple.Item1; }
            }
            throw new InvalidOperationException();
        }

    }
}
