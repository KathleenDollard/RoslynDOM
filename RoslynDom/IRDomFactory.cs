using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using RoslynDom.Common;

namespace RoslynDom
{
    public interface IRDomFactory<TKind> :IRDomFactory 
    {
        bool CanCreateFrom(SyntaxNode syntaxNode);
        IEnumerable<TKind> CreateFrom(SyntaxNode syntaxNode);
        FactoryPriority Priority { get; }
    }

    public interface IRDomFactory
    {
 
    }

    public interface IPublicAnnotationFactory : IRDomFactory<PublicAnnotation >
    {    }
    //public class RDomFactory<TSyntax, TCreate, TFactory>
    //    (CandidatePriority candidatePriority, CandidateKind candidateKind)
    //    where TFactory : IRDomStatementFactory< TCreate >
    //    where TCreate : IDom
    //{
    //    // TCandidate should be IStatement, IExpression, IStemMember or ITypeMember, but this isn't currently enforced
    //    public CandidatePriority CandidatePriority { get; } = candidatePriority;
    //    public CandidateKind CandidateKind { get;  } = candidateKind;
    //}


    /// <summary>
    /// Priority for candidate selection. These are for clarity. Please add your
    /// own in the format "Normal + 1"
    /// </summary>
    public enum FactoryPriority
    {
        None = 0,
        Fallback =100,
        Normal = 200,
        Top = 300
    }

    //public enum CandidateKind
    //{
    //    Unknown = 0,
    //    StemMember,
    //    TypeMember,
    //    Statement,
    //    Expression
    //}

}
