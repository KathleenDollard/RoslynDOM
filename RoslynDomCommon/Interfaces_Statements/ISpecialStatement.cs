using RoslynDom.Common;

namespace RoslynDom.Common
{
    public interface ISpecialStatement : IStatement
    {
        string SpecialStatementKind { get; } // language or platform specific. String so they can be understood across platforms

        // At least
        //Checked, 
        //Lock,
        //Unsafe,
        //Yield,
        //Fixed,
        //Goto,
        //Labeled,
    }
}
