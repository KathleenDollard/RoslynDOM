using RoslynDom.Common;

namespace RoslynDom
{
    public interface IRDomStemContainer
    {
        void AddOrMoveMember(IStemMember member);
        void RemoveMember(IStemMember member);
    }

    public interface IRDomTypeContainer
    {
        void AddOrMoveMember(ITypeMember member);
        void RemoveMember(ITypeMember member);
        void AddTypeParameter(ITypeParameter typeParameter);
        void RemoveTypeParameter(ITypeParameter typeParameter);
    }

    public interface IRDomCodeContainer
    {
        void AddOrMoveMember(ICodeMember member);
        void RemoveMember(ICodeMember member);
    }
}
