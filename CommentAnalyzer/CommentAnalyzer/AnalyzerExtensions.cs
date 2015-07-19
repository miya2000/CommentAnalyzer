using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CommentAnalyzer
{
    public static class AnalyzerExtensions
    {
        public static IEnumerable<SyntaxTrivia[]> SplitLines(this SyntaxTriviaList list)
        {
            var line = new List<SyntaxTrivia>();
            foreach (var item in list)
            {
                line.Add(item);
                if (item.Kind() == SyntaxKind.EndOfLineTrivia)
                {
                    yield return line.ToArray();
                    line.Clear();
                }
            }
            if (line.Count > 0)
            {
                yield return line.ToArray();
            }
        }
    }
}
