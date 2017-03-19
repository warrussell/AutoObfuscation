using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.CSharp.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AutoObfuscation
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = args[0];
            Console.Write("Path received: " + path + " [PRESS ENTER]");
            Console.ReadLine();
            var workspace = MSBuildWorkspace.Create();

            bool secureFloatExists = false;
            var solution = workspace.OpenSolutionAsync(path).Result;
            foreach (var projectId in solution.ProjectIds)
            {
                Console.WriteLine("Found Project: " + projectId);
                var project = solution.GetProject(projectId);
                foreach (var documentId in project.DocumentIds)
                {
                    var document = project.GetDocument(documentId);

                    //TODO: check imports
                    if (document.Name == "SecureFloat")
                    {
                        secureFloatExists = true;
                    }

                    document = Obfuscate(document);
                    project = document.Project;
                }

                solution = project.Solution;
            }

            Console.WriteLine(workspace.TryApplyChanges(solution));

            if (!secureFloatExists)
            {
                //add SecureFloat.cs to project
                Console.WriteLine("SecureFloat.cs is not in the project [PRESS ENTER]");
            }
            Console.Read();

            /* Replace Method Return Type
            syncInterface
              .ReplaceNodes(
                  syncInterface.Members.OfType<MethodDeclarationSyntax>(),
                  (a, b) =>
                      b.WithReturnType(
                          SyntaxFactory.GenericName("Task").AddTypeArgumentListArguments(b.ReturnType)))
              .NormalizeWhitespace();
            */
        }

        private static Document Obfuscate(Document document)
        {
            Console.WriteLine("Found document: " + document.Name + "\n" + document.GetTextAsync().Result.ToString() + "\n[PRESS ENTER]");
            Console.ReadLine();

            var root = document.GetSyntaxRootAsync().Result;
            //MyRewriter rewriter = new MyRewriter();
            //rewriter.Visit(root);
            var firstVariable = root.DescendantNodes().OfType<VariableDeclarationSyntax>().First();
            var secureFloat = SyntaxFactory.VariableDeclaration(SyntaxFactory.IdentifierName("AutoObfuscation.SecureFloat "), firstVariable.Variables);
            var newRoot = root.ReplaceNode(firstVariable, secureFloat);
            document = document.WithText(newRoot.GetText());

            Console.WriteLine("New document:\n" + document.GetTextAsync().Result.ToString() + "\n[PRESS ENTER]");

            return document;
        }
    }

    public class MyRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            Console.WriteLine("Found declaration: " + node.GetText() + ". Type " + node.Type + " var " + node.Variables.Count);
            if (node.Variables.Count > 0)
            {
                if (node.Type.ToString() == SyntaxFactory.IdentifierName("float").ToString())
                {
                    var genericName = SyntaxFactory.GenericName(SyntaxFactory.Identifier("SecureFloat"));// = SecureFloat<>
                    var newIdentifier = SyntaxFactory.IdentifierName("SecureFloat");
                    var newType = node.Type.ReplaceNode(node.Type, newIdentifier);
                    var newNode = SyntaxFactory.VariableDeclaration(newType, node.Variables);
                    Console.WriteLine("t1: " + genericName.ToFullString() + " t2: " + newType.GetFirstToken() + " newNode: " + newNode.ToString() + " [PRESS ENTER]");
                    Console.ReadLine();
                    return newNode;
                }
            }
            Console.WriteLine("[PRESS ENTER]");
            Console.ReadLine();
            //TypeSyntax secureFloatSyntax = new TypeSyntax();
            return base.VisitVariableDeclaration(node);
        }

        /*
        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax m)
        {
            var newReturnType = SF.GenericName(SF.Identifier("Task"), SF.TypeArgumentList(new SeparatedSyntaxList<TypeSyntax>().Add(m.ReturnType.WithoutTrivia())));
            newReturnType = newReturnType.InsertTriviaBefore(newReturnType.GetLeadingTrivia().First(), m.ReturnType.GetLeadingTrivia().AsEnumerable());
            newReturnType = newReturnType.InsertTriviaAfter(newReturnType.GetTrailingTrivia().First(), m.ReturnType.GetTrailingTrivia().AsEnumerable());
            return m.ReplaceNode(m.ReturnType, newReturnType);
        }
        */
    }
}
