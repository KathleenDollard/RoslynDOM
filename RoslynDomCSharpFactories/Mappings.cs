using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using RoslynDom.Common;
using Microsoft.CodeAnalysis;

namespace RoslynDom.CSharp
{
    internal static class Mappings
    {
        private static List<Tuple<SyntaxKind, SyntaxKind, LiteralKind>> LiteralKindMap = new List<Tuple<SyntaxKind, SyntaxKind, LiteralKind>>()
        {
            Tuple.Create(SyntaxKind.StringLiteralToken,    SyntaxKind.StringLiteralExpression,    LiteralKind.String),
            Tuple.Create( SyntaxKind.NumericLiteralToken,   SyntaxKind.NumericLiteralExpression,   LiteralKind.Numeric),
            Tuple.Create( SyntaxKind.TrueKeyword,           SyntaxKind.TrueKeyword,                LiteralKind.Boolean),
            Tuple.Create( SyntaxKind.FalseKeyword,          SyntaxKind.FalseKeyword,               LiteralKind.Boolean),
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

        private static List<Tuple<SyntaxKind, AssignmentOperator>> assignmentOpMap = new List<Tuple<SyntaxKind, AssignmentOperator>>()
        {
             Tuple.Create( SyntaxKind.SimpleAssignmentExpression, AssignmentOperator.Equals ),
             Tuple.Create( SyntaxKind.AddAssignmentExpression, AssignmentOperator.AddAssignment),
             Tuple.Create( SyntaxKind.SubtractAssignmentExpression, AssignmentOperator.SubtractAssignment),
             Tuple.Create( SyntaxKind.MultiplyAssignmentExpression, AssignmentOperator.MultiplyAssignment),
             Tuple.Create( SyntaxKind.DivideAssignmentExpression, AssignmentOperator.DivideAssignment),
             Tuple.Create( SyntaxKind.ModuloAssignmentExpression, AssignmentOperator.ModuloAssignment),
             Tuple.Create( SyntaxKind.AndAssignmentExpression, AssignmentOperator.AndAssignment),
             Tuple.Create( SyntaxKind.ExclusiveOrAssignmentExpression, AssignmentOperator.ExclusiveOrAssignment),
             Tuple.Create( SyntaxKind.OrAssignmentExpression, AssignmentOperator.OrAssignment),
             Tuple.Create( SyntaxKind.LeftShiftAssignmentExpression, AssignmentOperator.LeftShiftAssignment),
             Tuple.Create( SyntaxKind.RightShiftAssignmentExpression, AssignmentOperator.RightShiftAssignment)
        };

        public static AssignmentOperator AssignmentOperatorFromCSharpKind(SyntaxKind kind)
        {
            foreach (var tuple in assignmentOpMap)
            {
                if (tuple.Item1 == kind) { return tuple.Item2; }
            }
            throw new InvalidOperationException();
        }

        public static SyntaxKind SyntaxKindFromAssignmentOperator(AssignmentOperator op)
        {
            foreach (var tuple in assignmentOpMap)
            {
                if (tuple.Item2 == op) { return tuple.Item1; }
            }
            throw new InvalidOperationException();
        }


        private static List<Tuple<SyntaxKind, Operator>> operatorMap = new List<Tuple<SyntaxKind, Operator>>()
        {
             Tuple.Create( SyntaxKind.PlusToken                      , Operator.Plus),                   
             Tuple.Create( SyntaxKind.MinusToken                     , Operator.Minus),                  
             Tuple.Create( SyntaxKind.AsteriskToken                  , Operator.Asterisk),               
             Tuple.Create( SyntaxKind.SlashToken                     , Operator.Slash),                  
             Tuple.Create( SyntaxKind.PercentToken                   , Operator.Percent),                
             Tuple.Create( SyntaxKind.AmpersandToken                 , Operator.Ampersand),              
             Tuple.Create( SyntaxKind.BarToken                       , Operator.Bar),                    
             Tuple.Create( SyntaxKind.CaretToken                     , Operator.Caret),                  
             Tuple.Create( SyntaxKind.LessThanLessThanToken          , Operator.LessThanLessThan),       
             Tuple.Create( SyntaxKind.GreaterThanGreaterThanToken    , Operator.GreaterThanGreaterThan), 
             Tuple.Create( SyntaxKind.EqualsEqualsToken              , Operator.EqualsEquals),           
             Tuple.Create( SyntaxKind.ExclamationEqualsToken         , Operator.ExclamationEquals),      
             Tuple.Create( SyntaxKind.GreaterThanToken               , Operator.GreaterThan),            
             Tuple.Create( SyntaxKind.LessThanToken                  , Operator.LessThan),               
             Tuple.Create( SyntaxKind.GreaterThanEqualsToken         , Operator.GreaterThanEquals),      
             Tuple.Create( SyntaxKind.LessThanEqualsToken            , Operator.LessThanEquals)
        };




        public static Operator OperatorFromCSharpKind(SyntaxKind kind)
        {
            foreach (var tuple in operatorMap)
            {
                if (tuple.Item1 == kind) { return tuple.Item2; }
            }
            throw new InvalidOperationException();
        }

        public static SyntaxKind SyntaxKindFromOperator(Operator op)
        {
            foreach (var tuple in operatorMap)
            {
                if (tuple.Item2 == op) { return tuple.Item1; }
            }
            throw new InvalidOperationException();
        }


        private static List<Tuple<Accessibility, AccessModifier>> accessModifierMap = new List<Tuple<Accessibility, AccessModifier>>()
        {
             Tuple.Create(Accessibility. Private, AccessModifier. Private),
             Tuple.Create(Accessibility. ProtectedAndInternal, AccessModifier. ProtectedAndInternal),
             Tuple.Create(Accessibility. ProtectedAndFriend , AccessModifier. ProtectedAndFriend ),
             Tuple.Create(Accessibility. Protected , AccessModifier. Protected ),
             Tuple.Create(Accessibility. Internal , AccessModifier. Internal ),
             Tuple.Create(Accessibility. Friend , AccessModifier. Friend ),
             Tuple.Create(Accessibility. NotApplicable, AccessModifier. None),
             Tuple.Create(Accessibility. ProtectedOrInternal , AccessModifier. ProtectedOrInternal ),
             Tuple.Create(Accessibility. ProtectedOrFriend , AccessModifier. ProtectedOrFriend),
             Tuple.Create(Accessibility. Public , AccessModifier. Public )
        };
        public static Accessibility AccessibilityFromAccessModifier(AccessModifier accessModifier)
        {
            foreach (var tuple in accessModifierMap)
            {
                if (tuple.Item2 == accessModifier) { return tuple.Item1; }
            }
            throw new InvalidOperationException();
        }

        public static AccessModifier AccessModifierFromAccessibility(Accessibility accessibility)
        {
            foreach (var tuple in accessModifierMap)
            {
                if (tuple.Item1 == accessibility) { return tuple.Item2; }
            }
            throw new InvalidOperationException();
        }

        private static List<Tuple<string, string>> typeAliasMap = new List<Tuple<string, string>>()
        {
             Tuple.Create("sbyte"  ,"SByte"),
             Tuple.Create("short"  ,"Int16"),
             Tuple.Create("int"    ,"Int32"),
             Tuple.Create("long"   ,"Int64"),
             Tuple.Create("byte"   ,"Byte"),
             Tuple.Create("ushort" ,"UInt16"),
             Tuple.Create("uint"   ,"UInt32"),
             Tuple.Create("ulong"  ,"UInt64"),
             Tuple.Create("decimal","Decimal"),
             Tuple.Create("float"  ,"Single"  ),
             Tuple.Create("double" ,"Double" ),
             Tuple.Create("bool"   ,"Boolean"   ),
             Tuple.Create("string" ,"String" ),
             Tuple.Create("char"   ,"Char" )
        };

        public static string SystemTypeFromAlias(string alias)
        {
            foreach (var tuple in typeAliasMap)
            {
                if (tuple.Item1 == alias) { return tuple.Item2; }
            }
            throw new InvalidOperationException();
        }

        public static string AliasFromSystemType(string systemTypeName)
        {
            foreach (var tuple in typeAliasMap)
            {
                if (tuple.Item2 == systemTypeName) { return tuple.Item1; }
            }
            throw new InvalidOperationException();
        }


        private static List<Tuple<SyntaxKind, Variance>> GenericVarianceMap = new List<Tuple<SyntaxKind, Variance>>()
        {
            Tuple.Create( SyntaxKind.InKeyword,     Variance.In),
            Tuple.Create( SyntaxKind.OutKeyword,             Variance.Out)
        };

        public static Variance VarianceFromVarianceKind(SyntaxKind kind)
        {
            foreach (var tuple in GenericVarianceMap)
            {
                if (tuple.Item1 == kind) { return tuple.Item2; }
            }
            return Variance.None;
        }

        public static SyntaxKind VarianceKindFromVariance(Variance variance)
        {
           
            foreach (var tuple in GenericVarianceMap)
            {
                if (tuple.Item2 == variance)
                { return tuple.Item1; }
            }
            return SyntaxKind.None ;
        }
    }
}
