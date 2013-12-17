using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;

namespace WebmasterToolsApi.Extensions
{
    public static class XDocumentExtensions
    {
        /// <summary>
        /// Removes all namespaces from a XDocument
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        /// <remarks>Borrowed from http://stackoverflow.com/a/7238007/41596</remarks>
        public static XElement RemoveAllNamespaces(XElement e)
        {
            return new XElement(e.Name.LocalName,
                                (from n in e.Nodes()
                                 select ((n is XElement) ? RemoveAllNamespaces(n as XElement) : n)),
                                (e.HasAttributes) ? (from a in e.Attributes() select a) : null);
        }

        /// <summary>
        /// Converts a XDocument into a ExpandoObject
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static dynamic ToExpandoObject(this XDocument document)
        {
            return ParseNode(document.Root);
        }

        /// <summary>
        /// Formats a property name to a valid C# property name
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static string ToValidPropertyName(this string instance)
        {
            // Remove namespaces
            var stop = instance.IndexOf('}') + 1;
            instance = stop > 0 ? instance.Substring(stop) : instance;

            // Replace dashes with underscore
            instance = instance.Replace("-", "_");
            return instance;
        }

        /// <summary>
        /// Recursively walks through a XContainer adding properties to a ExpandoObject
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <remarks>Borrowed from http://famvdploeg.com/blog/2011/10/xml-to-expando/</remarks>
        private static dynamic ParseNode(XElement item)
        {
            dynamic result = new ExpandoObject();
            var properties = result as IDictionary<string, object>;

            // Properties
            var groupedProperties =
                item.Elements().Where(element => !element.HasElements).Select(RemoveAllNamespaces).GroupBy(
                    x => x.Name.ToString());

            foreach (var propertyGroup in groupedProperties)
            {
                var elementCount = propertyGroup.Count();
                if (elementCount == 1)
                {
                    // Single child
                    var first = propertyGroup.First();
                    var value = first.Value;

                    // If there is no inner text and only one attribute, use that value
                    if (string.IsNullOrWhiteSpace(value) && first.Attributes().Count() == 1)
                    {
                        value = first.FirstAttribute.Value;
                    }

                    properties[propertyGroup.Key.ToValidPropertyName()] = value;
                }
                else
                {
                    // List
                    var children = propertyGroup.Select(element => element.Value).ToList();
                    properties[propertyGroup.Key.ToValidPropertyName()] = children;
                }
            }

            // Children
            var groupedElements =
                item.Elements()
                    .Where(element => element.HasElements)
                    .Select(RemoveAllNamespaces)
                    .GroupBy(x => x.Name.ToString());

            foreach (var elementGroup in groupedElements)
            {
                var nrElements = elementGroup.Count();
                if (nrElements == 1)
                {
                    properties[elementGroup.Key.ToValidPropertyName()] = ParseNode(elementGroup.First());
                }
                else
                {
                    var multipleElements = elementGroup.Select(element => ParseNode(element)).ToList();
                    properties[elementGroup.Key.ToValidPropertyName()] = multipleElements;
                }
            }

            // Attributes
            if (item.HasAttributes)
            {
                foreach (var attribute in item.Attributes())
                {
                    properties[attribute.Name.ToString().ToValidPropertyName()] = attribute.Value;
                }
            }

            return result;
        }
    }
}