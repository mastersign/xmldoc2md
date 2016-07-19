using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Mastersign.XmlDoc
{
    // https://msdn.microsoft.com/en-us/library/fsbx0t7x.aspx

    public enum CRefKind
    {
        Unknown,
        Invalid,
        Error,
        Namespace,
        Type,
        Field,
        Method,
        Property,
        Event,
    }

    public class CRefParsingResult
    {
        public CRefKind Kind { get; private set; }

        public CRefParsingResult(CRefKind kind)
        {
            Kind = kind;
        }
    }

    public class CRefErrorMessage : CRefParsingResult
    {
        public string Message { get; private set; }

        public CRefErrorMessage(string message)
            : base(CRefKind.Error)
        {
            Message = message;
        }
    }

    public class CRefNamespace : CRefParsingResult
    {
        public string Namespace { get; private set; }

        protected CRefNamespace(CRefKind kind, string ns)
            : base(kind)
        {
            Namespace = ns;
        }

        public CRefNamespace(string ns)
            : this(CRefKind.Namespace, ns)
        {
        }
    }

    public class CRefType : CRefNamespace
    {
        public string Type { get; private set; }

        protected CRefType(CRefKind kind, string ns, string type)
            : base(kind, ns)
        {
            Type = type;
        }

        public CRefType(string ns, string type)
            : this(CRefKind.Type, ns, type)
        {
        }
    }

    public abstract class CRefMember : CRefType
    {
        public string Name { get; private set; }

        protected CRefMember(CRefKind kind, string ns, string type, string name)
            : base(kind, ns, type)
        {
            Name = name;
        }
    }

    public class CRefField : CRefMember
    {
        public CRefField(string ns, string type, string name)
            : base(CRefKind.Field, ns, type, name)
        {
        }
    }

    public class CRefArgumentType
    {
        public string Namespace { get; private set; }

        public string Type { get; private set; }

        public string Modifiers { get; private set; }

        public CRefArgumentType(string ns, string type, string mod)
        {
            Namespace = ns;
            Type = type;
            Modifiers = mod;
        }
    }

    public class CRefMethod : CRefMember
    {
        public CRefArgumentType[] Arguments { get; private set; }

        public string ReturnType { get; private set; } // only in use with casting operators

        public CRefMethod(string ns, string type, string name, CRefArgumentType[] arguments, string returnType)
            : base(CRefKind.Method, ns, type, name)
        {
            Arguments = arguments;
            ReturnType = returnType;
        }
    }

    public class CRefProperty : CRefMember
    {
        public CRefArgumentType[] Arguments { get; private set; }

        public CRefProperty(string ns, string type, string name, CRefArgumentType[] arguments)
            : base(CRefKind.Property, ns, type, name)
        {
            Arguments = arguments;
        }
    }

    public class CRefEvent : CRefMember
    {
        public CRefEvent(string ns, string type, string name)
            : base(CRefKind.Event, ns, type, name)
        {
        }
    }

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

        private static CRefKind ParseKind(string kind)
        {
            switch (kind)
            {
                case "N": return CRefKind.Namespace;
                case "T": return CRefKind.Type;
                case "F": return CRefKind.Field;
                case "P": return CRefKind.Property;
                case "M": return CRefKind.Method;
                case "E": return CRefKind.Event;
                default: return CRefKind.Unknown;
            }
        }

        private static string EmptyToNull(string value)
        {
            return string.IsNullOrEmpty(value) ? null : value;
        }

        private static CRefArgumentType[] ParseArgumentList(string args)
        {
            return null;
        }

        public static CRefParsingResult Parse(string cref)
        {
            if (cref == null) throw new ArgumentNullException("cref");
            if (cref.StartsWith("!"))
            {
                var message = cref.Substring(1).TrimStart();
                return new CRefErrorMessage(message);
            }
            var crefM = CRefPattern.Match(cref);
            if (!crefM.Success)
            {
                return new CRefParsingResult(CRefKind.Invalid);
            }
            var kind = ParseKind(crefM.Groups["kind"].Value);
            var def = crefM.Groups["def"].Value;
            Match m = default(Match);
            switch (kind)
            {
                case CRefKind.Namespace:
                    m = NamespacePattern.Match(def);
                    return new CRefNamespace(
                        EmptyToNull(m.Groups["ns"].Value));
                case CRefKind.Type:
                    m = TypePattern.Match(def);
                    return new CRefType(
                        EmptyToNull(m.Groups["ns"].Value),
                        m.Groups["type"].Value);
                case CRefKind.Field:
                    m = FieldPattern.Match(def);
                    return new CRefField(
                        EmptyToNull(m.Groups["ns"].Value),
                        m.Groups["type"].Value,
                        m.Groups["name"].Value);
                case CRefKind.Method:
                    m = MethodPattern.Match(def);
                    return new CRefMethod(
                        EmptyToNull(m.Groups["ns"].Value),
                        m.Groups["type"].Value,
                        m.Groups["name"].Value,
                        ParseArgumentList(m.Groups["args"].Value),
                        EmptyToNull(m.Groups["ret"].Value));
                case CRefKind.Property:
                    m = PropertyPattern.Match(def);
                    return new CRefProperty(
                        EmptyToNull(m.Groups["ns"].Value),
                        m.Groups["type"].Value,
                        m.Groups["name"].Value,
                        ParseArgumentList(m.Groups["args"].Value));
                case CRefKind.Event:
                    m = EventPattern.Match(def);
                    return new CRefEvent(
                        EmptyToNull(m.Groups["ns"].Value),
                        m.Groups["type"].Value,
                        m.Groups["name"].Value);
                case CRefKind.Invalid:
                    return new CRefParsingResult(kind);
                default:
                    return new CRefParsingResult(CRefKind.Unknown);
            }
        }

        public string ErrorMessage(string cref)
        {
            var error = Parse(cref) as CRefErrorMessage;
            return error != null ? error.Message : null;
        }

        public string Namespace(string cref)
        {
            var ns = Parse(cref) as CRefNamespace;
            return ns != null ? ns.Namespace : null;
        }

        public string TypeName(string cref)
        {
            var type = Parse(cref) as CRefType;
            return type != null ? type.Type : null;
        }

        public string MemberName(string cref)
        {
            var member = Parse(cref) as CRefMember;
            return member != null ? member.Name : null;
        }
    }

    public class CRefFormatting
    {
        public string FormatLabel(string cref)
        {
            var result = CRefParsing.Parse(cref);

            switch (result.Kind)
            {
                case CRefKind.Namespace: return ((CRefNamespace)result).Namespace;
                case CRefKind.Type: return ((CRefType)result).Type;
                case CRefKind.Field: return ((CRefMember)result).Name;
                case CRefKind.Method: return ((CRefMember)result).Name;
                case CRefKind.Property: return ((CRefMember)result).Name;
                case CRefKind.Event: return ((CRefMember)result).Name;
                default: return "UNKNOWN_KIND_OF_MEMBER";
            }
        }
    }
}
