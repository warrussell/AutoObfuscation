using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.MSBuild;

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

            var oldSolution = workspace.OpenSolutionAsync(path).Result;
            Solution newSolution = oldSolution;
            foreach (var projectId in newSolution.ProjectIds)
            {
                var project = newSolution.GetProject(projectId);
                foreach (var documentId in project.DocumentIds)
                {
                    var document = project.GetDocument(documentId);
                    
                    var newDocument = Obfuscate(document);

                    newSolution = newDocument.Project.Solution;
                }
            }

            workspace.TryApplyChanges(newSolution);
        }

        private static Document Obfuscate(Document document)
        {
            Document newDocument = document;
            Console.WriteLine("Found document: " + document.Name + " [PRESS ENTER]");
            Console.ReadLine();
            return newDocument;
        }
    }
}
