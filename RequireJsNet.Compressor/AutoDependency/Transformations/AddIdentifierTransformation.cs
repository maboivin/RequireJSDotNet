namespace RequireJsNet.Compressor.Transformations
{
    using System;
    using System.Dynamic;
    using System.Linq;

    using Jint.Parser.Ast;

    using RequireJsNet.Compressor.Parsing;

    internal class AddIdentifierTransformation : IRequireTransformation
    {
        public RequireCall RequireCall { get; set; }

        protected string IdentifierName { get; set; }

        public static AddIdentifierTransformation Create(RequireCall call, string identifier)
        {
            return new AddIdentifierTransformation
            {
                RequireCall = call,
                IdentifierName = identifier
            };
        }
        
        public void Execute(ref string script)
        {
            var call = RequireCall.ParentNode.Node.As<CallExpression>();
            var firstArg = call.Arguments.First();
            
            // since ther's no range for the argument list itself and we asume that the call has at least one argument already,
            // we'll split the script right before the first argument of the current call
            var beforeInsertPoint = script.Substring(0, firstArg.Range[0]);
            var afterInsertPoint = script.Substring(firstArg.Range[0], script.Length - firstArg.Range[0]);
            script = beforeInsertPoint + string.Format("'{0}', ", IdentifierName) + afterInsertPoint;
            RequireCall.Id = IdentifierName;
        }

        public int[] GetAffectedRange()
        {
            var call = RequireCall.ParentNode.Node.As<CallExpression>();

            // since there's no range for the argument list itself and we might not have an identifier at all,
            // just return something that positions it where it should be in the execution pipeline
            var calleeEnd = call.Callee.Range[1];
            return new int[] { calleeEnd, calleeEnd + 1 };
        }
    }
}