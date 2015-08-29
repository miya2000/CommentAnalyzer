using System;
using System.Globalization;
using System.Linq;
using System.Threading;
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
            //set default culture.
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
        }
        [TestCleanup]
        public void Cleanup()
        {
            //reset default culture.
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
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

        #region TypeDeclaration_01_Class
        /// <summary>
        /// TypeDeclaration - Class
        /// </summary>
        [TestMethod]
        public void TypeDeclaration_01_Class()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            //2015-01-01 EDIT START
            //void Hoge()
            //{
            //   var a = 100;
            //}
            void Fuga()
            {
                //var a = 100;
                var a = 120;
            }
            //2015-01-01 EDIT END
        }
    }";

            var expectedDiagnostics = new DiagnosticResult
            {
                Id = "CommentAnalyzer",
                Message = "Detect 'START' comment.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 8, 13)
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
            void Fuga()
            {
                //var a = 100;
                var a = 120;
            }
        }
    }";

            var actual = Verifier.GetFixResult(test, CommentAnalyzerCodeFixProvider.ActionKeyRemoveComments);
            actual.Is(expected);
        }
        #endregion
        #region TypeDeclaration_02_Struct
        /// <summary>
        /// TypeDeclaration - Struct
        /// </summary>
        [TestMethod]
        public void TypeDeclaration_02_Struct()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        struct TypeName
        {
            //2015-01-01 EDIT START
            //string Hoge() { get; set; }
            string Fuga() { get; set; }
            //2015-01-01 EDIT END
        }
    }";

            var expectedDiagnostics = new DiagnosticResult
            {
                Id = "CommentAnalyzer",
                Message = "Detect 'START' comment.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 8, 13)
                }
            };

            var actualDiagnostics = Verifier.GetSortedDiagnostics(test);
            Verifier.VerifyDiagnosticResults(actualDiagnostics, expectedDiagnostics);

            var expected = @"
    using System;

    namespace ConsoleApplication1
    {
        struct TypeName
        {
            string Fuga() { get; set; }
        }
    }";

            var actual = Verifier.GetFixResult(test, CommentAnalyzerCodeFixProvider.ActionKeyRemoveComments);
            actual.Is(expected);
        }
        #endregion
        #region TypeDeclaration_03_Interface
        /// <summary>
        /// TypeDeclaration - Interface
        /// </summary>
        [TestMethod]
        public void TypeDeclaration_03_Interface()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        interface TypeName
        {
            //2015-01-01 EDIT START
            //void Hoge();
            void Fuga();
            //2015-01-01 EDIT END
        }
    }";

            var expectedDiagnostics = new DiagnosticResult
            {
                Id = "CommentAnalyzer",
                Message = "Detect 'START' comment.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 8, 13)
                }
            };

            var actualDiagnostics = Verifier.GetSortedDiagnostics(test);
            Verifier.VerifyDiagnosticResults(actualDiagnostics, expectedDiagnostics);

            var expected = @"
    using System;

    namespace ConsoleApplication1
    {
        interface TypeName
        {
            void Fuga();
        }
    }";

            var actual = Verifier.GetFixResult(test, CommentAnalyzerCodeFixProvider.ActionKeyRemoveComments);
            actual.Is(expected);
        }
        #endregion
        #region TypeDeclaration_04_Enum
        /// <summary>
        /// TypeDeclaration - Enum
        /// </summary>
        [TestMethod]
        public void TypeDeclaration_04_Enum()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        enum TypeName
        {
            //2015-01-01 EDIT START
            //Hoge;
            Fuga;
            //2015-01-01 EDIT END
        }
    }";

            var expectedDiagnostics = new DiagnosticResult
            {
                Id = "CommentAnalyzer",
                Message = "Detect 'START' comment.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 8, 13)
                }
            };

            var actualDiagnostics = Verifier.GetSortedDiagnostics(test);
            Verifier.VerifyDiagnosticResults(actualDiagnostics, expectedDiagnostics);

            var expected = @"
    using System;

    namespace ConsoleApplication1
    {
        enum TypeName
        {
            Fuga;
        }
    }";

            var actual = Verifier.GetFixResult(test, CommentAnalyzerCodeFixProvider.ActionKeyRemoveComments);
            actual.Is(expected);
        }
        #endregion
        #region TypeDeclaration_05_NestedClass
        /// <summary>
        /// TypeDeclaration - Nested Class
        /// </summary>
        [TestMethod]
        public void TypeDeclaration_05_NestedClass()
        {
            var test = @"
    using System;

    namespace ConsoleApplication1
    {
        class TypeName
        {
            class NestedTypeName
            {
                //2015-01-01 EDIT START
                //void Hoge()
                //{
                //   var a = 100;
                //}
                void Fuga()
                {
                    //var a = 100;
                    var a = 120;
                }
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
                    new DiagnosticResultLocation("Test0.cs", 10, 17)
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
            class NestedTypeName
            {
                void Fuga()
                {
                    //var a = 100;
                    var a = 120;
                }
            }
        }
    }";

            var actual = Verifier.GetFixResult(test, CommentAnalyzerCodeFixProvider.ActionKeyRemoveComments);
            actual.Is(expected);
        }
        #endregion

        #region Localization_01_default
        /// <summary>
        /// Localization - default
        /// </summary>
        [TestMethod]
        public void Localization_01_default()
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

            var actualDiagnostics = Verifier.GetSortedDiagnostics(test);
            actualDiagnostics[0].Id.Is("CommentAnalyzer");
            actualDiagnostics[0].Descriptor.Title.ToString().Is("Comment Analyzer");
            actualDiagnostics[0].Descriptor.Description.ToString().Is("Remove senseless comments.");
            actualDiagnostics[0].GetMessage().Is("Detect 'START' comment.");

            var actualActions = Verifier.GetFixActions(test);
            actualActions.Single(n => n.EquivalenceKey == CommentAnalyzerCodeFixProvider.ActionKeyRemoveComment).Title.Is("Remove Comment");
            actualActions.Single(n => n.EquivalenceKey == CommentAnalyzerCodeFixProvider.ActionKeyRemoveComments).Title.Is("Remove Comments");
        }
        #endregion
        #region Localization_02_jaJP
        /// <summary>
        /// Localization - ja-JP
        /// </summary>
        [TestMethod]
        public void Localization_02_jaJP()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("ja-JP");

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

            var actualDiagnostics = Verifier.GetSortedDiagnostics(test);
            actualDiagnostics[0].Id.Is("CommentAnalyzer");
            actualDiagnostics[0].Descriptor.Title.ToString().Is("コメントアナライザー");
            actualDiagnostics[0].Descriptor.Description.ToString().Is("意味のないコメントを削除します。");
            actualDiagnostics[0].GetMessage().Is("'START' コメントを検出しました.");

            var actualActions = Verifier.GetFixActions(test);
            actualActions.Single(n => n.EquivalenceKey == CommentAnalyzerCodeFixProvider.ActionKeyRemoveComment).Title.Is("コメントを削除（1行）");
            actualActions.Single(n => n.EquivalenceKey == CommentAnalyzerCodeFixProvider.ActionKeyRemoveComments).Title.Is("コメントを削除（まとめて）");
        }
        #endregion

    }
}