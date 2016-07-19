using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

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
        public string TypeName { get; private set; }

        public string FullTypeName
        {
            get
            {
                return string.IsNullOrEmpty(Namespace)
                    ? TypeName
                    : Namespace + "." + TypeName;
            }
        }

        protected CRefType(CRefKind kind, string ns, string type)
            : base(kind, ns)
        {
            TypeName = type;
        }

        public CRefType(string ns, string type)
            : this(CRefKind.Type, ns, type)
        {
        }
    }

    public abstract class CRefMember : CRefType
    {
        public string MemberName { get; private set; }

        protected CRefMember(CRefKind kind, string ns, string type, string name)
            : base(kind, ns, type)
        {
            MemberName = name;
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

        public CRefArgumentType ReturnType { get; private set; } // only in use with casting operators

        public CRefMethod(string ns, string type, string name, CRefArgumentType[] arguments, CRefArgumentType returnType)
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
            = new Regex(@"^(?:(?<ns>[^\.]+(?:\.[^\.]+)*?)\.)?(?<type>[^\.\(]+?)$");

        private static readonly Regex MethodPattern
            = new Regex(@"^(?:(?<ns>[^\.\(]+(?:\.[^\.]+)*?)\.)?(?<type>[^\.\(]+?)\.(?<name>[^\.\(]+)(?:\((?<args>.+?)\)(?:~(?<ret>.+))?)?$");

        private static readonly Regex FieldPattern
            = new Regex(@"^(?:(?<ns>[^\.]+(?:\.[^\.]+)*?)\.)?(?<type>[^\.\(]+?)\.(?<name>[^\.\(]+)$");

        private static readonly Regex PropertyPattern
            = new Regex(@"^(?:(?<ns>[^\.\(]+(?:\.[^\.]+)*?)\.)?(?<type>[^\.\(]+?)\.(?<name>[^\.\(]+)(?:\((?<args>.+?)\))?$");

        private static readonly Regex EventPattern
            = new Regex(@"^(?:(?<ns>[^\.]+(?:\.[^\.]+)*?)\.)?(?<type>[^\.\(]+?)\.(?<name>[^\.\(]+)$");

        private static readonly Regex ArgumentTypePattern
            = new Regex(@"^(?:(?<ns>[^\.]+(?:\.[^\.]+)*?)\.)?(?<type>[^\.\(]+?)(?<mod>(?:\*|@|\^|\[[\d,\:\?]*\])*)$");

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

        private static CRefArgumentType ParseArgumentType(string type)
        {
            if (type == null) return null;
            var m = ArgumentTypePattern.Match(type);
            if (!m.Success) return null;
            return new CRefArgumentType(
                EmptyToNull(m.Groups["ns"].Value),
                m.Groups["type"].Value,
                EmptyToNull(m.Groups["mod"].Value));
        }

        private static CRefArgumentType[] ParseArgumentList(string args)
        {
            var parts = args.Split(',');
            var result = new List<CRefArgumentType>();
            foreach (var part in parts)
            {
                result.Add(ParseArgumentType(part));
            }
            return result.ToArray();
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
                        ParseArgumentType(m.Groups["ret"].Value));
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
            return error != null ? error.Message ?? string.Empty : string.Empty;
        }

        public string MemberKind(string cref)
        {
            var result = Parse(cref);
            return result.Kind.ToString();
        }

        public string Namespace(string cref)
        {
            var ns = Parse(cref) as CRefNamespace;
            return ns != null ? ns.Namespace ?? string.Empty : string.Empty;
        }

        public string TypeName(string cref)
        {
            var type = Parse(cref) as CRefType;
            return type != null ? type.TypeName ?? string.Empty : string.Empty;
        }

        public string MemberName(string cref)
        {
            var member = Parse(cref) as CRefMember;
            return member != null ? member.MemberName ?? string.Empty : string.Empty;
        }

        public CRefArgumentType[] Arguments(string cref)
        {
            var result = Parse(cref);
            var method = result as CRefMethod;
            if (method != null) return method.Arguments;
            var property = result as CRefProperty;
            return property != null ? property.Arguments : null;
        }

        public CRefArgumentType ReturnType(string cref)
        {
            var method = Parse(cref) as CRefMethod;
            return method != null ? method.ReturnType : null;
        }
    }

    public class CRefFormatting
    {
        private static Regex GenericTypePattern
            = new Regex(@"`(\d+)");

        private static Regex GenericMethodPattern
            = new Regex(@"``(\d+)$");

        public CRefFormatting()
        {
            FileNameExtension = ".md";
            UrlBase = "";
            UrlFileNameExtension = ".html";
        }

        public string FileNameExtension { get; set; }

        public string UrlBase { get; set; }

        public string UrlFileNameExtension { get; set; }

        public XmlDocument[] XmlDocs { get; set; }

        private delegate bool ElementCriteria(XmlElement node);

        private XmlElement FindFirstElement(string xpath, ElementCriteria criteria)
        {
            foreach (var xmlDoc in XmlDocs)
            {
                var nodeSet = xmlDoc.SelectNodes(xpath);
                foreach (XmlElement el in nodeSet)
                {
                    if (criteria(el)) return el;
                }
            }
            return null;
        }

        private string[] GetTypeArgumentNames(string typeName, int n)
        {
            var memberEl = FindFirstElement("/doc/members/member",
                el => el.HasAttribute("name")
                   && el.GetAttribute("name") == "T:" + typeName);
            var result = new string[n];
            var i = 0;
            if (memberEl != null)
            {
                var typeParamEls = memberEl.SelectNodes("typeparam");
                if (typeParamEls != null)
                {
                    foreach(XmlElement el in typeParamEls)
                    {
                        result[i] = el.GetAttribute("name");
                        i++;
                        if (i >= n) break;
                    }
                }
            }
            for (; i < n; i++)
            {
                result[i] = "T" + (i + 1);
            }
            return result;
        }

        private string FormatGenerics(CRefType cref)
        {
            return GenericTypePattern.Replace(cref.FullTypeName, m =>
            {
                var n = int.Parse(m.Groups[1].Value);
                var argumentNames = GetTypeArgumentNames(
                    cref.FullTypeName.Substring(0, m.Index + m.Length), n);
                if (argumentNames == null)
                {
                    argumentNames = new string[n];
                    for (var i = 0; i < n; i++) argumentNames[i] = "T" + (i + 1);
                }
                return "<" + string.Join(", ", argumentNames) + ">";
            });
        }

        private string ShortName(string name)
        {
            return name.Contains(".")
                ? name.Substring(name.LastIndexOf('.') + 1)
                : name;
        }

        private string FormatArguments(CRefArgumentType[] arguments)
        {
            throw new NotImplementedException();
        }

        public string EscapeMarkdown(string text)
        {
            return text
                .Replace("<", "&lt;")
                .Replace(">", "&gt;");
        }

        public string Label(string cref)
        {
            var result = CRefParsing.Parse(cref);

            switch (result.Kind)
            {
                case CRefKind.Namespace: return ((CRefNamespace)result).Namespace;
                case CRefKind.Type: return ShortName(FormatGenerics((CRefType)result));
                case CRefKind.Field: return ((CRefMember)result).MemberName;
                case CRefKind.Method: return ((CRefMember)result).MemberName;
                case CRefKind.Property: return ((CRefMember)result).MemberName;
                case CRefKind.Event: return ((CRefMember)result).MemberName;
                default: return "UNKNOWN_KIND_OF_MEMBER";
            }
        }

        public string FullLabel(string cref)
        {
            var result = CRefParsing.Parse(cref);

            switch (result.Kind)
            {
                case CRefKind.Namespace: return ((CRefNamespace)result).Namespace;
                case CRefKind.Type: return FormatGenerics((CRefType)result);
                case CRefKind.Field: return ((CRefMember)result).MemberName;
                case CRefKind.Method: return ((CRefMember)result).MemberName;
                case CRefKind.Property: return ((CRefMember)result).MemberName;
                case CRefKind.Event: return ((CRefMember)result).MemberName;
                default: return "UNKNOWN_KIND_OF_MEMBER";
            }
        }

        public string CRef(Type t)
        {
            return t.FullName.Replace('+', '.');
        }

        public string FileName(Type t)
        {
            return CRef(t) + FileNameExtension;
        }

        public string FileName(string cref)
        {
            var result = CRefParsing.Parse(cref);

            if (result.Kind == CRefKind.Namespace)
            {
                var ns = (CRefNamespace)result;
                return ns.Namespace + FileNameExtension;
            }
            var type = result as CRefType;
            if (type != null)
            {
                return type.Namespace != null
                    ? type.Namespace + "." + type.TypeName + FileNameExtension
                    : type.TypeName + FileNameExtension;
            }
            return null;
        }
    }
}
