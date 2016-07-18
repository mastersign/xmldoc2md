using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Mastersign.XmlDoc
{
    // https://msdn.microsoft.com/en-us/library/fsbx0t7x.aspx
    public class CRefParsing
    {
        private static readonly Regex CRefPattern
            = new Regex(@"^(?<kind>\w)\:(?<def>.*)$");

        private static readonly Regex NamespacePattern
            = new Regex(@"^(?<ns>[^\.]+(?:\.[^\.]+)*?)$");

        private static readonly Regex TypePattern
            = new Regex(@"^(?:(?<ns>[^\.]+(?:\.[^\.]+)*?)\.)?(?<type>[^\.]+?)$");

        private static readonly Regex MethodPattern
            = new Regex(@"^(?:(?<ns>[^\.]+(?:\.[^\.]+)*?)\.)?(?<type>[^\.]+?)\.(?<name>[^\.\(]+)\((?<args>.*)\)(?:~(?<ret>.*))?$");

        private static readonly Regex FieldPattern
            = new Regex(@"^(?:(?<ns>[^\.]+(?:\.[^\.]+)*?)\.)?(?<type>[^\.]+?)\.(?<name>[^\.\(]+)$");

        private static readonly Regex PropertyPattern
            = new Regex(@"^(?:(?<ns>[^\.]+(?:\.[^\.]+)*?)\.)?(?<type>[^\.]+?)\.(?<name>[^\.\(]+)(?:\((?<args>.*)\))?$");

        private static readonly Regex EventPattern
            = new Regex(@"^(?:(?<ns>[^\.]+(?:\.[^\.]+)*?)\.)?(?<type>[^\.]+?)\.(?<name>[^\.\(]+)$");

        public string MemberKind(string cref)
        {
            if (cref == null) throw new ArgumentNullException(nameof(cref));
            if (cref.StartsWith("!")) { return "error"; }
            int p = cref.IndexOf(':');
            if (p < 0) { return "invalid"; }
            switch (cref.Substring(0, p))
            {
                case "N": return "namespace";
                case "T": return "type";
                case "F": return "field";
                case "P": return "property";
                case "M": return "method";
                case "E": return "event";
                default: return "unknown";
            }
        }

        public string ErrorMessage(string cref)
        {
            if (cref == null) throw new ArgumentNullException(nameof(cref));
            if (MemberKind(cref) != "error") return null;
            return cref.Substring(1).TrimStart();
        }

        public string Namespace(string cref)
        {
            if (cref == null) throw new ArgumentNullException(nameof(cref));
            var kind = MemberKind(cref);
            if (kind == "unknown" ||
                kind == "error")
            {
                return null;
            }
            var crefM = CRefPattern.Match(cref);
            if (!crefM.Success)
            {
                throw new ArgumentException("The given cref has no valid syntax.", nameof(cref));
            }
            var def = crefM.Groups["def"].Value;
            Match m = default(Match);
            switch (kind)
            {
                case "namespace":
                    m = NamespacePattern.Match(def);
                    break;
                case "type":
                    m = TypePattern.Match(def);
                    break;
                case "method":
                    m = MethodPattern.Match(def);
                    break;
                case "field":
                    m = FieldPattern.Match(def);
                    break;
                case "property":
                    m = PropertyPattern.Match(def);
                    break;
                case "event":
                    m = EventPattern.Match(def);
                    break;
            }
            Debug.Assert(m != null);
            if (!m.Success)
            {
                throw new ArgumentException("The given cref has no namespace.", nameof(cref));
            }
            var ns = m.Groups["ns"].Value;
            return ns != string.Empty ? ns : null;
        }

        public string TypeName(string cref)
        {
            if (cref == null) throw new ArgumentNullException(nameof(cref));
            var kind = MemberKind(cref);
            if (kind == "namespace" ||
                kind == "unknown" ||
                kind == "error")
            {
                return null;
            }
            var crefM = CRefPattern.Match(cref);
            if (!crefM.Success)
            {
                throw new ArgumentException("The given cref has no valid syntax.", nameof(cref));
            }
            var def = crefM.Groups["def"].Value;
            Match m = default(Match);
            switch (kind)
            {
                case "type":
                    m = TypePattern.Match(def);
                    break;
                case "method":
                    m = MethodPattern.Match(def);
                    break;
                case "field":
                    m = FieldPattern.Match(def);
                    break;
                case "property":
                    m = PropertyPattern.Match(def);
                    break;
                case "event":
                    m = EventPattern.Match(def);
                    break;
            }
            Debug.Assert(m != null);
            if (!m.Success)
            {
                throw new ArgumentException("The given cref has no type name.", nameof(cref));
            }
            return m.Groups["type"].Value;
        }

        public string MemberName(string cref)
        {
            if (cref == null) throw new ArgumentNullException(nameof(cref));
            var kind = MemberKind(cref);
            if (kind == "namespace" ||
                kind == "type" ||
                kind == "unknown" ||
                kind == "error")
            {
                return null;
            }
            var crefM = CRefPattern.Match(cref);
            if (!crefM.Success)
            {
                throw new ArgumentException("The given cref has no valid syntax.", nameof(cref));
            }
            var def = crefM.Groups["def"].Value;
            Match m = default(Match);
            switch (kind)
            {
                case "method":
                    m = MethodPattern.Match(def);
                    break;
                case "field":
                    m = FieldPattern.Match(def);
                    break;
                case "property":
                    m = PropertyPattern.Match(def);
                    break;
                case "event":
                    m = EventPattern.Match(def);
                    break;
            }
            Debug.Assert(m != null);
            if (!m.Success)
            {
                throw new ArgumentException("The given cref has no member name.", nameof(cref));
            }
            return m.Groups["name"].Value;
        }
    }

    public class CRefFormatting
    {
        private static readonly CRefParsing parsing = new CRefParsing();

        public string FormatLabel(string cref)
        {
            switch (parsing.MemberKind(cref))
            {
                case "namespace": return parsing.Namespace(cref);
                case "type": return parsing.TypeName(cref);
                case "field": return parsing.MemberName(cref);
                case "method": return parsing.MemberName(cref);
                case "property": return parsing.MemberName(cref);
                case "event": return parsing.MemberName(cref);
                default: return "UNKNOWN_KIND_OF_MEMBER";
            }
        }
    }
}
