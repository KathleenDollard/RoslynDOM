using System.Collections.Generic;
using RoslynDom.Common;

namespace RoslynDom
{
    public interface IRDomStemContainer :IStemContainer 
    {
        void AddOrMoveStemMember(IStemMember member);
        void RemoveStemMember(IStemMember member);
        void ClearStemMembers();
    }

    public interface IRDomTypeMemberContainer :ITypeMemberContainer 
    {
        void AddOrMoveMember(ITypeMember member);
        void RemoveMember(ITypeMember member);
        void ClearMembers();
        void AddOrMoveTypeParameter(ITypeParameter typeParameter);
        void RemoveTypeParameter(ITypeParameter typeParameter);
        void ClearTypeParameters();
    }

    public interface IRDomCodeContainer : ICodeContainer 
    {
        void AddOrMoveStatement(IStatement member);
        void RemoveStatement(IStatement member);
        void ClearStatements();
    }
}
