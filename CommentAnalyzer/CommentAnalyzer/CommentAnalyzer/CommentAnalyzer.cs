using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Text.RegularExpressions;

namespace CommentAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class CommentAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "CommentAnalyzer";

        internal static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        internal static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        internal static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        internal const string Category = "Comment";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeBlock, SyntaxKind.Block);
        }

        private void AnalyzeBlock(SyntaxNodeAnalysisContext context)
        {
            var startRegex = new Regex(@"\bstart\b", RegexOptions.IgnoreCase);
            var dateRegex = new Regex(@"\b(?:\d{2,4}/\d{1,2}/\d{2,4}|\d{2,4}-\d{1,2}-\d{2,4})\b");

            var block = context.Node as BlockSyntax;

            var tokens = block.ChildNodesAndTokens();

            foreach (var line in tokens.Where(t => t.HasLeadingTrivia).SelectMany(t => t.GetLeadingTrivia().SplitLines()))
            {
                // find line contains SingleLineComment only.
                foreach (var firstTrivia in line.SkipWhile(n => n.Kind() == SyntaxKind.WhitespaceTrivia).Take(1))
                {
                    if (firstTrivia.Kind() == SyntaxKind.SingleLineCommentTrivia)
                    {
                        var commentText = firstTrivia.ToFullString();

                        if (startRegex.IsMatch(commentText) && dateRegex.IsMatch(commentText))
                        {
                            var diagnostic = Diagnostic.Create(Rule, firstTrivia.GetLocation(), "START");
                            context.ReportDiagnostic(diagnostic);
                        }
                    }
                }
            }
        }
    }
}
