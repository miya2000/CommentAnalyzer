using Microsoft.CodeAnalysis;
using System;
using System.Linq;

namespace TestHelper
{
    /// <summary>
    /// Location where the diagnostic appears, as determined by path, line number, and column number.
    /// </summary>
    public struct DiagnosticResultLocation
    {
        public DiagnosticResultLocation(string path, int line, int column)
        {
            if (line < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(line), "line must be >= -1");
            }

            if (column < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(line), "column must be >= -1");
            }

            this.Path = path;
            this.Line = line;
            this.Column = column;
        }

        public string Path { get; }
        public int Line { get; }
        public int Column { get; }

        public override string ToString()
        {
            return "new DiagnosticResultLocation(@\"" + Path.Replace("\"", "\"\"") + "\", " + Line + ", " + Column + ")";
        }
    }

    /// <summary>
    /// Struct that stores information about a Diagnostic appearing in a source
    /// </summary>
    public struct DiagnosticResult
    {
        private DiagnosticResultLocation[] locations;

        public DiagnosticResultLocation[] Locations
        {
            get
            {
                if (this.locations == null)
                {
                    this.locations = new DiagnosticResultLocation[] { };
                }
                return this.locations;
            }

            set
            {
                this.locations = value;
            }
        }

        public DiagnosticSeverity Severity { get; set; }

        public string Id { get; set; }

        public string Message { get; set; }

        public string Path
        {
            get
            {
                return this.Locations.Length > 0 ? this.Locations[0].Path : "";
            }
        }

        public int Line
        {
            get
            {
                return this.Locations.Length > 0 ? this.Locations[0].Line : -1;
            }
        }

        public int Column
        {
            get
            {
                return this.Locations.Length > 0 ? this.Locations[0].Column : -1;
            }
        }

        public override string ToString()
        {
            return @"new DiagnosticResult
{
    Id = """ + Id + @""",
    Message = @""" + Message.Replace("\"", "\"\"") + @""",
    Severity = DiagnosticSeverity." + Severity.ToString() + @",
" + (Locations.Length == 0 ? "" : (
@"    Locations = new[] {
" + string.Join(",\r\n", Locations.Select(l => "        " + l.ToString())) + @"
    }
")) + "};";
        }
    }
}