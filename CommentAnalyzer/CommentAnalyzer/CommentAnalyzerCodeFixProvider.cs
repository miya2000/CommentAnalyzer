using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;
using System.Text.RegularExpressions;

namespace CommentAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CommentAnalyzerCodeFixProvider)), Shared]
    public class CommentAnalyzerCodeFixProvider : CodeFixProvider
    {
        public const string ActionKeyRemoveComment = CommentAnalyzer.DiagnosticId + ".RemoveComment";
        public const string ActionKeyRemoveComments = CommentAnalyzer.DiagnosticId + ".RemoveComments";

        public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(CommentAnalyzer.DiagnosticId);
        public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();

            var comment = root.FindTrivia(context.Span.Start);

            context.RegisterCodeFix(
                CodeAction.Create(Resources.RemoveCommentTitle, c => RemoveComment(context.Document, root, comment, c), ActionKeyRemoveComment),
                diagnostic);
            context.RegisterCodeFix(
                CodeAction.Create(Resources.RemoveCommentsTitle, c => RemoveComments(context.Document, root, comment, c), ActionKeyRemoveComments),
                diagnostic);
        }

        private Task<Document> RemoveComment(Document document, SyntaxNode root, SyntaxTrivia comment, CancellationToken cancellationToken)
        {
            var line = comment.Token.LeadingTrivia.SplitLines().First(n => n.Contains(comment));

            var newRoot = root.ReplaceTrivia(line, (_, __) => default(SyntaxTrivia));

            var newDocument = document.WithSyntaxRoot(newRoot);

            return Task.FromResult(newDocument);
        }

        private Task<Document> RemoveComments(Document document, SyntaxNode root, SyntaxTrivia comment, CancellationToken cancellationToken)
        {
            var startCommentText = comment.ToFullString().Substring(2).Trim(); // skip "//"
            var endCommentText = Regex.Replace(startCommentText, @"\bstart\b", "end", RegexOptions.IgnoreCase);
            var endCommentRegex = new Regex(string.Join(@"\s+", Regex.Split(endCommentText, @"\s+").Select(n => Regex.Escape(n))), RegexOptions.IgnoreCase);

            var lines = new List<SyntaxTrivia>();

            var block = comment.Token.Parent.FirstAncestorOrSelf<BlockSyntax>();

            var tokens = block.ChildNodesAndTokens();

            foreach (var line in tokens.Where(t => t.HasLeadingTrivia).SelectMany(t => t.GetLeadingTrivia().SplitLines()).SkipWhile(n => !n.Contains(comment)))
            {
                // find line contains SingleLineComment only.
                foreach (var firstTrivia in line.SkipWhile(n => n.Kind() == SyntaxKind.WhitespaceTrivia).Take(1))
                {
                    if (firstTrivia.Kind() == SyntaxKind.SingleLineCommentTrivia)
                    {
                        lines.AddRange(line);

                        var commentText = firstTrivia.ToFullString();

                        if (endCommentRegex.IsMatch(commentText))
                        {
                            goto EOL;
                        }
                    }
                }
            }

        EOL:

            var newRoot = root.ReplaceTrivia(lines, (_, __) => default(SyntaxTrivia));

            var newDocument = document.WithSyntaxRoot(newRoot);

            return Task.FromResult(newDocument);
        }
    }
}