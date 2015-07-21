using System;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace CommentAnalyzer.Test
{
    [TestClass]
    public class CommentAnalyzerTest
    {
        CodeAnalysisVerifier Verifier { get; set; }
        CodeAnalysisVerifier NewVerifier () => new CodeAnalysisVerifier(new CommentAnalyzer(), new CommentAnalyzerCodeFixProvider());

        [TestInitialize]
        public void Initialize()
        {
            Verifier = NewVerifier();
        }

        #region SimpleStart_01_RemoveComment
        /// <summary>
        /// SimpleStart - RemoveComment
        /// </summary>
        [TestMethod]
        public void SimpleStart_01_RemoveComment()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            void Hoge()
            {
               //2015-01-01 EDIT START
               //var a = 100;
               var a = 120;
            }
        }
    }";

            var expectedDiagnostics = new DiagnosticResult
            {
                Id = "CommentAnalyzer",
                Message = "Detect 'START' comment.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 10, 16)
                }
            };

            var actualDiagnostics = Verifier.GetSortedDiagnostics(test);
            Verifier.VerifyDiagnosticResults(actualDiagnostics, expectedDiagnostics);

            var expected = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            void Hoge()
            {
               //var a = 100;
               var a = 120;
            }
        }
    }";

            var actual = Verifier.GetFixResult(test, CommentAnalyzerCodeFixProvider.ActionKeyRemoveComment);
            actual.Is(expected);
        }
        #endregion
        #region SimpleStart_02_RemoveComments
        /// <summary>
        /// SimpleStart - RemoveComments
        /// </summary>
        [TestMethod]
        public void SimpleStart_02_RemoveComments()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            void Hoge()
            {
               //2015-01-01 EDIT START
               //var a = 100;
               var a = 120;
            }
        }
    }";

            var expectedDiagnostics = new DiagnosticResult
            {
                Id = "CommentAnalyzer",
                Message = @"Detect 'START' comment.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] {
                    new DiagnosticResultLocation(@"Test0.cs", 10, 16)
                }
            };

            var actualDiagnostics = Verifier.GetSortedDiagnostics(test);
            Verifier.VerifyDiagnosticResults(actualDiagnostics, expectedDiagnostics);

            var expected = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            void Hoge()
            {
               var a = 120;
            }
        }
    }";

            var actual = Verifier.GetFixResult(test, CommentAnalyzerCodeFixProvider.ActionKeyRemoveComments);
            actual.Is(expected);
        }
        #endregion
        #region SimpleStartEnd_01_RemoveComment
        /// <summary>
        /// SimpleStart - RemoveComment
        /// </summary>
        [TestMethod]
        public void SimpleStartEnd_01_RemoveComment()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            void Hoge()
            {
               //2015-01-01 EDIT START
               //var a = 100;
               var a = 120;
               //2015-01-01 EDIT END
            }
        }
    }";

            var expectedDiagnostics = new DiagnosticResult
            {
                Id = "CommentAnalyzer",
                Message = "Detect 'START' comment.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 10, 16)
                }
            };

            var actualDiagnostics = Verifier.GetSortedDiagnostics(test);
            Verifier.VerifyDiagnosticResults(actualDiagnostics, expectedDiagnostics);

            var expected = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            void Hoge()
            {
               //var a = 100;
               var a = 120;
               //2015-01-01 EDIT END
            }
        }
    }";

            var actual = Verifier.GetFixResult(test, CommentAnalyzerCodeFixProvider.ActionKeyRemoveComment);
            actual.Is(expected);
        }
        #endregion
        #region SimpleStartEnd_02_RemoveComments
        /// <summary>
        /// SimpleStartEnd - RemoveComments
        /// </summary>
        [TestMethod]
        public void SimpleStartEnd_02_RemoveComments()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            void Hoge()
            {
               //2015-01-01 EDIT START
               //var a = 100;
               var a = 120;
               //2015-01-01 EDIT END
            }
        }
    }";

            var expectedDiagnostics = new DiagnosticResult
            {
                Id = "CommentAnalyzer",
                Message = @"Detect 'START' comment.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] {
                    new DiagnosticResultLocation(@"Test0.cs", 10, 16)
                }
            };

            var actualDiagnostics = Verifier.GetSortedDiagnostics(test);
            Verifier.VerifyDiagnosticResults(actualDiagnostics, expectedDiagnostics);

            var expected = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            void Hoge()
            {
               var a = 120;
            }
        }
    }";

            var actual = Verifier.GetFixResult(test, CommentAnalyzerCodeFixProvider.ActionKeyRemoveComments);
            actual.Is(expected);
        }
        #endregion

        #region RemoveComments_01_StartToBlockEnd
        /// <summary>
        /// RemoveComments - Remove commens from start comment to block end.
        /// </summary>
        [TestMethod]
        public void RemoveComments_01_StartToBlockEnd()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            void Hoge()
            {
               //2015-01-01 EDIT START
               //var a = 100;
               var a = 120;
               //var b = 120;
               var b = 100;
               //var c = a + b;
               var c = a * b;
               //aaa
               //bbb
            }
        }
    }";

            var expected = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            void Hoge()
            {
               var a = 120;
               var b = 100;
               var c = a * b;
            }
        }
    }";

            var actual = Verifier.GetFixResult(test, CommentAnalyzerCodeFixProvider.ActionKeyRemoveComments);
            actual.Is(expected);
        }
        #endregion
        #region RemoveComments_02_StartToEnd
        /// <summary>
        /// RemoveComments - Remove commens from start comment to end comment.
        /// </summary>
        [TestMethod]
        public void RemoveComments_02_StartToEnd()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            void Hoge()
            {
               //2015-01-01 EDIT START
               //var a = 100;
               var a = 120;
               //var b = 120;
               var b = 100;
               //var c = a + b;
               var c = a * b;
               //aaa
               //2015-01-01 EDIT END
               //bbb
            }
        }
    }";

            var expected = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            void Hoge()
            {
               var a = 120;
               var b = 100;
               var c = a * b;
               //bbb
            }
        }
    }";

            var actual = Verifier.GetFixResult(test, CommentAnalyzerCodeFixProvider.ActionKeyRemoveComments);
            actual.Is(expected);
        }
        #endregion
        #region RemoveComments_03_RemoveCommensInTheSameBlockOnly
        /// <summary>
        /// RemoveComments - Remove commens in the same block only.
        /// </summary>
        [TestMethod]
        public void RemoveComments_03_RemoveCommensInTheSameBlockOnly()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            // Hoge Start.
            void Hoge()
            {
               //2015-01-01 EDIT START
               //var a = 100;
               var a = 120;
               //if (a == 100)
               if (a == 120)
               {
                   //Write a value
                   Console.WriteLine($""{a}"");
               }
               //2015-01-01 EDIT END
            }
            // Hoge End.
        }
    }";

            var expected = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            // Hoge Start.
            void Hoge()
            {
               var a = 120;
               if (a == 120)
               {
                   //Write a value
                   Console.WriteLine($""{a}"");
               }
            }
            // Hoge End.
        }
    }";

            var actual = Verifier.GetFixResult(test, CommentAnalyzerCodeFixProvider.ActionKeyRemoveComments);
            actual.Is(expected);
        }
        #endregion

    }
}