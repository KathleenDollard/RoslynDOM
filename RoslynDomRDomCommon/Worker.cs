namespace RoslynDom.Common
{
   public class Worker
   {
      internal Worker(ICreateFromWorker createFromWorker, IBuildSyntaxWorker buildSyntaxWorker)
      {
         CreateFromWorker = createFromWorker;
         BuildSyntaxWorker = buildSyntaxWorker;
      }

      public ICreateFromWorker CreateFromWorker { get; private set; }

      public IBuildSyntaxWorker BuildSyntaxWorker { get; private set; }
   }
}