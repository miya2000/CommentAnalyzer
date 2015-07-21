using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TestHelper
{
    public class CodeAnalysisVerifier
    {
        public CodeAnalysisVerifier(DiagnosticAnalyzer analyzer, CodeFixProvider codefix)
        {
            this.Analyzer = analyzer;
            this.CodeFix = codefix;
        }

        public string Language { get; set; } = LanguageNames.CSharp;
        public DiagnosticAnalyzer Analyzer { get; }
        public CodeFixProvider CodeFix { get; }

        public Diagnostic[] GetSortedDiagnostics(params string[] sources)
        {
            return CodeAnalysisHelper.GetSortedDiagnostics(sources, this.Language, this.Analyzer);
        }

        public void VerifyDiagnosticResults(IEnumerable<Diagnostic> actualResults, params DiagnosticResult[] expectedResults)
        {
            CodeAnalysisHelper.VerifyDiagnosticResults(actualResults, this.Analyzer, expectedResults);
        }

        public string GetFixResult(string oldSource, string equivalenceKey = null, bool allowNewCompilerDiagnostics = false)
        {
            return CodeAnalysisHelper.GetFixResult(this.Language, this.Analyzer, this.CodeFix, oldSource, equivalenceKey, allowNewCompilerDiagnostics);
        }
    }
}
