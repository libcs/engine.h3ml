using System;
using System.Linq;
using System.Xml.XPath;

namespace Gumbo.Xml
{
    internal class GumboNavigator : XPathNavigator
    {
        class NavigatorState : IEquatable<NavigatorState>
        {
            public NodeWrapper Node { get; private set; }
            public AttributeWrapper Attribute { get; private set; }
            public NavigatorState(AttributeWrapper attribute) => Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
            public NavigatorState(NodeWrapper node) => Node = node ?? throw new ArgumentNullException(nameof(node));

            public void SetCurrent(AttributeWrapper attribute)
            {
                Node = null;
                Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
            }

            public void SetCurrent(NodeWrapper node)
            {
                Node = node ?? throw new ArgumentNullException(nameof(node));
                Attribute = null;
            }

            public bool Equals(NavigatorState other) => Node == other.Node && Attribute == other.Attribute;
        }

        readonly NavigatorState _State;
        readonly GumboWrapper _Gumbo;

        public override string BaseURI => throw new NotImplementedException();

        public GumboNavigator(GumboWrapper gumbo, NodeWrapper node)
        {
            _Gumbo = gumbo;
            _State = new NavigatorState(node);
        }
        public GumboNavigator(GumboWrapper gumbo, AttributeWrapper attribute)
        {
            _Gumbo = gumbo;
            _State = new NavigatorState(attribute);
        }

        public override bool IsEmptyElement => _State.Node != null
            && _State.Node.Type == GumboNodeType.GUMBO_NODE_ELEMENT
            && !_State.Node.Children.Any();

        public override bool IsSamePosition(XPathNavigator other) => !(other is GumboNavigator otherGumboNav) ? false : _State.Equals(otherGumboNav._State);

        public override string LocalName => _State.Attribute != null
            ? _State.Attribute.Name
            : _State.Node is ElementWrapper element
            ? !string.IsNullOrEmpty(element.OriginalTagName) ? element.OriginalTagName.Split(':').Last() : element.NormalizedTagName
            : string.Empty;

        public override bool MoveTo(XPathNavigator other) => !(other is GumboNavigator otherGumboNav) ? false : _State == otherGumboNav._State;

        public override bool MoveToFirstAttribute()
        {
            if (!(_State.Node is ElementWrapper element))
                return false;
            var firstAttr = element.Attributes.FirstOrDefault();
            if (firstAttr == null)
                return false;
            _State.SetCurrent(firstAttr);
            return true;
        }

        public override bool MoveToFirstChild()
        {
            if (_State.Node == null)
                return false;
            var child = _State.Node.Children.FirstOrDefault();
            if (child == null)
                return false;
            _State.SetCurrent(child);
            return true;
        }

        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope) => throw new NotImplementedException();

        public override bool MoveToId(string id)
        {
            _Gumbo.MarshalAll();
            var element = _Gumbo.GetElementById(id);
            if (element == null)
                return false;
            _State.SetCurrent(element);
            return true;
        }

        public override bool MoveToNext()
        {
            if (_State.Node == null || _State.Node.Parent == null)
                return false;
            var parent = _State.Node.Parent;
            var nextIndex = parent.Children.IndexOf(_State.Node) + 1;
            if (nextIndex >= parent.Children.Length)
                return false;
            _State.SetCurrent(parent.Children[nextIndex]);
            return true;
        }

        public override bool MoveToNextAttribute()
        {
            if (_State.Attribute == null)
                return false;
            var parent = _State.Attribute.Parent;
            var nextIndex = parent.Attributes.IndexOf(_State.Attribute) + 1;
            if (nextIndex >= parent.Attributes.Length)
                return false;
            _State.SetCurrent(parent.Attributes[nextIndex]);
            return true;
        }

        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope) => throw new NotImplementedException();

        public override bool MoveToParent()
        {
            if (_State.Node == null || _State.Node.Parent == null)
                return false;
            _State.SetCurrent(_State.Node.Parent);
            return true;
        }

        public override bool MoveToPrevious()
        {
            if (_State.Node == null || _State.Node.Parent == null)
                return false;
            var parent = _State.Node.Parent;
            var nextIndex = parent.Children.IndexOf(_State.Node) - 1;
            if (nextIndex < 0)
                return false;
            _State.SetCurrent(parent.Children[nextIndex]);
            return true;
        }

        public override string Name => _State.Attribute != null
            ? _State.Attribute.OriginalName
            : _State.Node is ElementWrapper element ? element.OriginalTagName : string.Empty;

        public override System.Xml.XmlNameTable NameTable => throw new NotImplementedException();

        public override string NamespaceURI => string.Empty;

        public override XPathNodeType NodeType
        {
            get
            {
                if (_State.Attribute != null)
                    return XPathNodeType.Attribute;
                System.Diagnostics.Debug.Assert(_State.Node != null);
                switch (_State.Node.Type)
                {
                    case GumboNodeType.GUMBO_NODE_DOCUMENT: return XPathNodeType.Root;
                    case GumboNodeType.GUMBO_NODE_ELEMENT:
                    case GumboNodeType.GUMBO_NODE_TEMPLATE: return XPathNodeType.Element;
                    case GumboNodeType.GUMBO_NODE_TEXT:
                    case GumboNodeType.GUMBO_NODE_CDATA: return XPathNodeType.Text;
                    case GumboNodeType.GUMBO_NODE_COMMENT: return XPathNodeType.Comment;
                    case GumboNodeType.GUMBO_NODE_WHITESPACE: return XPathNodeType.Whitespace;
                    default: throw new NotImplementedException();
                }
            }
        }

        public override string Prefix
        {
            get
            {
                if (_State.Attribute != null)
                    switch (_State.Attribute.Namespace)
                    {
                        case GumboAttributeNamespaceEnum.GUMBO_ATTR_NAMESPACE_NONE: return string.Empty;
                        case GumboAttributeNamespaceEnum.GUMBO_ATTR_NAMESPACE_XLINK: return "xlink";
                        case GumboAttributeNamespaceEnum.GUMBO_ATTR_NAMESPACE_XML: return "xml";
                        case GumboAttributeNamespaceEnum.GUMBO_ATTR_NAMESPACE_XMLNS: return "xmlns";
                        default: throw new NotSupportedException($"Namespace '{_State.Attribute.Namespace}' is not supported");
                    }
                return string.Empty; // namespaces are implicit in html/svg/mathml
            }
        }

        public override string Value => _State.Attribute != null
            ? _State.Attribute.Value
            : _State.Node is ElementWrapper element ? element.Value : _State.Node is TextWrapper text ? text.Value : string.Empty;

        public override XPathNavigator Clone()
        {
            if (_State.Node != null)
                return new GumboNavigator(_Gumbo, _State.Node);
            if (_State.Attribute != null)
                return new GumboNavigator(_Gumbo, _State.Attribute);
            throw new InvalidOperationException();
        }
    }
}
