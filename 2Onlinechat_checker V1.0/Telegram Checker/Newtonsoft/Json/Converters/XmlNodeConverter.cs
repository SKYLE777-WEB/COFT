using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Converters
{
	// Token: 0x02000188 RID: 392
	public class XmlNodeConverter : JsonConverter
	{
		// Token: 0x170003BE RID: 958
		// (get) Token: 0x06001447 RID: 5191 RVA: 0x0006F184 File Offset: 0x0006D384
		// (set) Token: 0x06001448 RID: 5192 RVA: 0x0006F18C File Offset: 0x0006D38C
		public string DeserializeRootElementName { get; set; }

		// Token: 0x170003BF RID: 959
		// (get) Token: 0x06001449 RID: 5193 RVA: 0x0006F198 File Offset: 0x0006D398
		// (set) Token: 0x0600144A RID: 5194 RVA: 0x0006F1A0 File Offset: 0x0006D3A0
		public bool WriteArrayAttribute { get; set; }

		// Token: 0x170003C0 RID: 960
		// (get) Token: 0x0600144B RID: 5195 RVA: 0x0006F1AC File Offset: 0x0006D3AC
		// (set) Token: 0x0600144C RID: 5196 RVA: 0x0006F1B4 File Offset: 0x0006D3B4
		public bool OmitRootObject { get; set; }

		// Token: 0x0600144D RID: 5197 RVA: 0x0006F1C0 File Offset: 0x0006D3C0
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			IXmlNode xmlNode = this.WrapXml(value);
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
			this.PushParentNamespaces(xmlNode, xmlNamespaceManager);
			if (!this.OmitRootObject)
			{
				writer.WriteStartObject();
			}
			this.SerializeNode(writer, xmlNode, xmlNamespaceManager, !this.OmitRootObject);
			if (!this.OmitRootObject)
			{
				writer.WriteEndObject();
			}
		}

		// Token: 0x0600144E RID: 5198 RVA: 0x0006F220 File Offset: 0x0006D420
		private IXmlNode WrapXml(object value)
		{
			XObject xobject = value as XObject;
			if (xobject != null)
			{
				return XContainerWrapper.WrapNode(xobject);
			}
			XmlNode xmlNode = value as XmlNode;
			if (xmlNode != null)
			{
				return XmlNodeWrapper.WrapNode(xmlNode);
			}
			throw new ArgumentException("Value must be an XML object.", "value");
		}

		// Token: 0x0600144F RID: 5199 RVA: 0x0006F268 File Offset: 0x0006D468
		private void PushParentNamespaces(IXmlNode node, XmlNamespaceManager manager)
		{
			List<IXmlNode> list = null;
			IXmlNode xmlNode = node;
			while ((xmlNode = xmlNode.ParentNode) != null)
			{
				if (xmlNode.NodeType == XmlNodeType.Element)
				{
					if (list == null)
					{
						list = new List<IXmlNode>();
					}
					list.Add(xmlNode);
				}
			}
			if (list != null)
			{
				list.Reverse();
				foreach (IXmlNode xmlNode2 in list)
				{
					manager.PushScope();
					foreach (IXmlNode xmlNode3 in xmlNode2.Attributes)
					{
						if (xmlNode3.NamespaceUri == "http://www.w3.org/2000/xmlns/" && xmlNode3.LocalName != "xmlns")
						{
							manager.AddNamespace(xmlNode3.LocalName, xmlNode3.Value);
						}
					}
				}
			}
		}

		// Token: 0x06001450 RID: 5200 RVA: 0x0006F37C File Offset: 0x0006D57C
		private string ResolveFullName(IXmlNode node, XmlNamespaceManager manager)
		{
			string text = ((node.NamespaceUri == null || (node.LocalName == "xmlns" && node.NamespaceUri == "http://www.w3.org/2000/xmlns/")) ? null : manager.LookupPrefix(node.NamespaceUri));
			if (!string.IsNullOrEmpty(text))
			{
				return text + ":" + XmlConvert.DecodeName(node.LocalName);
			}
			return XmlConvert.DecodeName(node.LocalName);
		}

		// Token: 0x06001451 RID: 5201 RVA: 0x0006F404 File Offset: 0x0006D604
		private string GetPropertyName(IXmlNode node, XmlNamespaceManager manager)
		{
			switch (node.NodeType)
			{
			case XmlNodeType.Element:
				if (node.NamespaceUri == "http://james.newtonking.com/projects/json")
				{
					return "$" + node.LocalName;
				}
				return this.ResolveFullName(node, manager);
			case XmlNodeType.Attribute:
				if (node.NamespaceUri == "http://james.newtonking.com/projects/json")
				{
					return "$" + node.LocalName;
				}
				return "@" + this.ResolveFullName(node, manager);
			case XmlNodeType.Text:
				return "#text";
			case XmlNodeType.CDATA:
				return "#cdata-section";
			case XmlNodeType.ProcessingInstruction:
				return "?" + this.ResolveFullName(node, manager);
			case XmlNodeType.Comment:
				return "#comment";
			case XmlNodeType.DocumentType:
				return "!" + this.ResolveFullName(node, manager);
			case XmlNodeType.Whitespace:
				return "#whitespace";
			case XmlNodeType.SignificantWhitespace:
				return "#significant-whitespace";
			case XmlNodeType.XmlDeclaration:
				return "?xml";
			}
			throw new JsonSerializationException("Unexpected XmlNodeType when getting node name: " + node.NodeType);
		}

		// Token: 0x06001452 RID: 5202 RVA: 0x0006F53C File Offset: 0x0006D73C
		private bool IsArray(IXmlNode node)
		{
			foreach (IXmlNode xmlNode in node.Attributes)
			{
				if (xmlNode.LocalName == "Array" && xmlNode.NamespaceUri == "http://james.newtonking.com/projects/json")
				{
					return XmlConvert.ToBoolean(xmlNode.Value);
				}
			}
			return false;
		}

		// Token: 0x06001453 RID: 5203 RVA: 0x0006F5CC File Offset: 0x0006D7CC
		private void SerializeGroupedNodes(JsonWriter writer, IXmlNode node, XmlNamespaceManager manager, bool writePropertyName)
		{
			int count = node.ChildNodes.Count;
			if (count != 0)
			{
				if (count == 1)
				{
					string propertyName = this.GetPropertyName(node.ChildNodes[0], manager);
					this.WriteGroupedNodes(writer, manager, writePropertyName, node.ChildNodes, propertyName);
					return;
				}
				Dictionary<string, object> dictionary = null;
				string text = null;
				for (int i = 0; i < node.ChildNodes.Count; i++)
				{
					IXmlNode xmlNode = node.ChildNodes[i];
					string propertyName2 = this.GetPropertyName(xmlNode, manager);
					object obj;
					if (dictionary == null)
					{
						if (text == null)
						{
							text = propertyName2;
						}
						else if (!(propertyName2 == text))
						{
							dictionary = new Dictionary<string, object>();
							if (i > 1)
							{
								List<IXmlNode> list = new List<IXmlNode>(i);
								for (int j = 0; j < i; j++)
								{
									list.Add(node.ChildNodes[j]);
								}
								dictionary.Add(text, list);
							}
							else
							{
								dictionary.Add(text, node.ChildNodes[0]);
							}
							dictionary.Add(propertyName2, xmlNode);
						}
					}
					else if (!dictionary.TryGetValue(propertyName2, out obj))
					{
						dictionary.Add(propertyName2, xmlNode);
					}
					else
					{
						List<IXmlNode> list2 = obj as List<IXmlNode>;
						if (list2 == null)
						{
							list2 = new List<IXmlNode> { (IXmlNode)obj };
							dictionary[propertyName2] = list2;
						}
						list2.Add(xmlNode);
					}
				}
				if (dictionary == null)
				{
					this.WriteGroupedNodes(writer, manager, writePropertyName, node.ChildNodes, text);
					return;
				}
				foreach (KeyValuePair<string, object> keyValuePair in dictionary)
				{
					List<IXmlNode> list3 = keyValuePair.Value as List<IXmlNode>;
					if (list3 != null)
					{
						this.WriteGroupedNodes(writer, manager, writePropertyName, list3, keyValuePair.Key);
					}
					else
					{
						this.WriteGroupedNodes(writer, manager, writePropertyName, (IXmlNode)keyValuePair.Value, keyValuePair.Key);
					}
				}
			}
		}

		// Token: 0x06001454 RID: 5204 RVA: 0x0006F7E0 File Offset: 0x0006D9E0
		private void WriteGroupedNodes(JsonWriter writer, XmlNamespaceManager manager, bool writePropertyName, List<IXmlNode> groupedNodes, string elementNames)
		{
			if (groupedNodes.Count == 1 && !this.IsArray(groupedNodes[0]))
			{
				this.SerializeNode(writer, groupedNodes[0], manager, writePropertyName);
				return;
			}
			if (writePropertyName)
			{
				writer.WritePropertyName(elementNames);
			}
			writer.WriteStartArray();
			for (int i = 0; i < groupedNodes.Count; i++)
			{
				this.SerializeNode(writer, groupedNodes[i], manager, false);
			}
			writer.WriteEndArray();
		}

		// Token: 0x06001455 RID: 5205 RVA: 0x0006F86C File Offset: 0x0006DA6C
		private void WriteGroupedNodes(JsonWriter writer, XmlNamespaceManager manager, bool writePropertyName, IXmlNode node, string elementNames)
		{
			if (!this.IsArray(node))
			{
				this.SerializeNode(writer, node, manager, writePropertyName);
				return;
			}
			if (writePropertyName)
			{
				writer.WritePropertyName(elementNames);
			}
			writer.WriteStartArray();
			this.SerializeNode(writer, node, manager, false);
			writer.WriteEndArray();
		}

		// Token: 0x06001456 RID: 5206 RVA: 0x0006F8BC File Offset: 0x0006DABC
		private void SerializeNode(JsonWriter writer, IXmlNode node, XmlNamespaceManager manager, bool writePropertyName)
		{
			switch (node.NodeType)
			{
			case XmlNodeType.Element:
				if (this.IsArray(node) && XmlNodeConverter.AllSameName(node) && node.ChildNodes.Count > 0)
				{
					this.SerializeGroupedNodes(writer, node, manager, false);
					return;
				}
				manager.PushScope();
				foreach (IXmlNode xmlNode in node.Attributes)
				{
					if (xmlNode.NamespaceUri == "http://www.w3.org/2000/xmlns/")
					{
						string text = ((xmlNode.LocalName != "xmlns") ? XmlConvert.DecodeName(xmlNode.LocalName) : string.Empty);
						string value = xmlNode.Value;
						manager.AddNamespace(text, value);
					}
				}
				if (writePropertyName)
				{
					writer.WritePropertyName(this.GetPropertyName(node, manager));
				}
				if (!this.ValueAttributes(node.Attributes) && node.ChildNodes.Count == 1 && node.ChildNodes[0].NodeType == XmlNodeType.Text)
				{
					writer.WriteValue(node.ChildNodes[0].Value);
				}
				else if (node.ChildNodes.Count == 0 && node.Attributes.Count == 0)
				{
					if (((IXmlElement)node).IsEmpty)
					{
						writer.WriteNull();
					}
					else
					{
						writer.WriteValue(string.Empty);
					}
				}
				else
				{
					writer.WriteStartObject();
					for (int i = 0; i < node.Attributes.Count; i++)
					{
						this.SerializeNode(writer, node.Attributes[i], manager, true);
					}
					this.SerializeGroupedNodes(writer, node, manager, true);
					writer.WriteEndObject();
				}
				manager.PopScope();
				return;
			case XmlNodeType.Attribute:
			case XmlNodeType.Text:
			case XmlNodeType.CDATA:
			case XmlNodeType.ProcessingInstruction:
			case XmlNodeType.Whitespace:
			case XmlNodeType.SignificantWhitespace:
				if (node.NamespaceUri == "http://www.w3.org/2000/xmlns/" && node.Value == "http://james.newtonking.com/projects/json")
				{
					return;
				}
				if (node.NamespaceUri == "http://james.newtonking.com/projects/json" && node.LocalName == "Array")
				{
					return;
				}
				if (writePropertyName)
				{
					writer.WritePropertyName(this.GetPropertyName(node, manager));
				}
				writer.WriteValue(node.Value);
				return;
			case XmlNodeType.Comment:
				if (writePropertyName)
				{
					writer.WriteComment(node.Value);
					return;
				}
				return;
			case XmlNodeType.Document:
			case XmlNodeType.DocumentFragment:
				this.SerializeGroupedNodes(writer, node, manager, writePropertyName);
				return;
			case XmlNodeType.DocumentType:
			{
				IXmlDocumentType xmlDocumentType = (IXmlDocumentType)node;
				writer.WritePropertyName(this.GetPropertyName(node, manager));
				writer.WriteStartObject();
				if (!string.IsNullOrEmpty(xmlDocumentType.Name))
				{
					writer.WritePropertyName("@name");
					writer.WriteValue(xmlDocumentType.Name);
				}
				if (!string.IsNullOrEmpty(xmlDocumentType.Public))
				{
					writer.WritePropertyName("@public");
					writer.WriteValue(xmlDocumentType.Public);
				}
				if (!string.IsNullOrEmpty(xmlDocumentType.System))
				{
					writer.WritePropertyName("@system");
					writer.WriteValue(xmlDocumentType.System);
				}
				if (!string.IsNullOrEmpty(xmlDocumentType.InternalSubset))
				{
					writer.WritePropertyName("@internalSubset");
					writer.WriteValue(xmlDocumentType.InternalSubset);
				}
				writer.WriteEndObject();
				return;
			}
			case XmlNodeType.XmlDeclaration:
			{
				IXmlDeclaration xmlDeclaration = (IXmlDeclaration)node;
				writer.WritePropertyName(this.GetPropertyName(node, manager));
				writer.WriteStartObject();
				if (!string.IsNullOrEmpty(xmlDeclaration.Version))
				{
					writer.WritePropertyName("@version");
					writer.WriteValue(xmlDeclaration.Version);
				}
				if (!string.IsNullOrEmpty(xmlDeclaration.Encoding))
				{
					writer.WritePropertyName("@encoding");
					writer.WriteValue(xmlDeclaration.Encoding);
				}
				if (!string.IsNullOrEmpty(xmlDeclaration.Standalone))
				{
					writer.WritePropertyName("@standalone");
					writer.WriteValue(xmlDeclaration.Standalone);
				}
				writer.WriteEndObject();
				return;
			}
			}
			throw new JsonSerializationException("Unexpected XmlNodeType when serializing nodes: " + node.NodeType);
		}

		// Token: 0x06001457 RID: 5207 RVA: 0x0006FD10 File Offset: 0x0006DF10
		private static bool AllSameName(IXmlNode node)
		{
			using (List<IXmlNode>.Enumerator enumerator = node.ChildNodes.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.LocalName != node.LocalName)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06001458 RID: 5208 RVA: 0x0006FD80 File Offset: 0x0006DF80
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JsonToken tokenType = reader.TokenType;
			if (tokenType != JsonToken.StartObject)
			{
				if (tokenType == JsonToken.Null)
				{
					return null;
				}
				throw JsonSerializationException.Create(reader, "XmlNodeConverter can only convert JSON that begins with an object.");
			}
			else
			{
				XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(new NameTable());
				IXmlDocument xmlDocument = null;
				IXmlNode xmlNode = null;
				if (typeof(XObject).IsAssignableFrom(objectType))
				{
					if (objectType != typeof(XContainer) && objectType != typeof(XDocument) && objectType != typeof(XElement) && objectType != typeof(XNode) && objectType != typeof(XObject))
					{
						throw JsonSerializationException.Create(reader, "XmlNodeConverter only supports deserializing XDocument, XElement, XContainer, XNode or XObject.");
					}
					xmlDocument = new XDocumentWrapper(new XDocument());
					xmlNode = xmlDocument;
				}
				if (typeof(XmlNode).IsAssignableFrom(objectType))
				{
					if (objectType != typeof(XmlDocument) && objectType != typeof(XmlElement) && objectType != typeof(XmlNode))
					{
						throw JsonSerializationException.Create(reader, "XmlNodeConverter only supports deserializing XmlDocument, XmlElement or XmlNode.");
					}
					xmlDocument = new XmlDocumentWrapper(new XmlDocument
					{
						XmlResolver = null
					});
					xmlNode = xmlDocument;
				}
				if (xmlDocument == null || xmlNode == null)
				{
					throw JsonSerializationException.Create(reader, "Unexpected type when converting XML: " + objectType);
				}
				if (!string.IsNullOrEmpty(this.DeserializeRootElementName))
				{
					this.ReadElement(reader, xmlDocument, xmlNode, this.DeserializeRootElementName, xmlNamespaceManager);
				}
				else
				{
					reader.ReadAndAssert();
					this.DeserializeNode(reader, xmlDocument, xmlNamespaceManager, xmlNode);
				}
				if (objectType == typeof(XElement))
				{
					XElement xelement = (XElement)xmlDocument.DocumentElement.WrappedNode;
					xelement.Remove();
					return xelement;
				}
				if (objectType == typeof(XmlElement))
				{
					return xmlDocument.DocumentElement.WrappedNode;
				}
				return xmlDocument.WrappedNode;
			}
		}

		// Token: 0x06001459 RID: 5209 RVA: 0x0006FF78 File Offset: 0x0006E178
		private void DeserializeValue(JsonReader reader, IXmlDocument document, XmlNamespaceManager manager, string propertyName, IXmlNode currentNode)
		{
			if (propertyName == "#text")
			{
				currentNode.AppendChild(document.CreateTextNode(XmlNodeConverter.ConvertTokenToXmlValue(reader)));
				return;
			}
			if (propertyName == "#cdata-section")
			{
				currentNode.AppendChild(document.CreateCDataSection(XmlNodeConverter.ConvertTokenToXmlValue(reader)));
				return;
			}
			if (propertyName == "#whitespace")
			{
				currentNode.AppendChild(document.CreateWhitespace(XmlNodeConverter.ConvertTokenToXmlValue(reader)));
				return;
			}
			if (propertyName == "#significant-whitespace")
			{
				currentNode.AppendChild(document.CreateSignificantWhitespace(XmlNodeConverter.ConvertTokenToXmlValue(reader)));
				return;
			}
			if (!string.IsNullOrEmpty(propertyName) && propertyName[0] == '?')
			{
				this.CreateInstruction(reader, document, currentNode, propertyName);
				return;
			}
			if (string.Equals(propertyName, "!DOCTYPE", StringComparison.OrdinalIgnoreCase))
			{
				this.CreateDocumentType(reader, document, currentNode);
				return;
			}
			if (reader.TokenType == JsonToken.StartArray)
			{
				this.ReadArrayElements(reader, document, propertyName, currentNode, manager);
				return;
			}
			this.ReadElement(reader, document, currentNode, propertyName, manager);
		}

		// Token: 0x0600145A RID: 5210 RVA: 0x00070094 File Offset: 0x0006E294
		private void ReadElement(JsonReader reader, IXmlDocument document, IXmlNode currentNode, string propertyName, XmlNamespaceManager manager)
		{
			if (string.IsNullOrEmpty(propertyName))
			{
				throw JsonSerializationException.Create(reader, "XmlNodeConverter cannot convert JSON with an empty property name to XML.");
			}
			Dictionary<string, string> dictionary = this.ReadAttributeElements(reader, manager);
			string text = MiscellaneousUtils.GetPrefix(propertyName);
			if (propertyName.StartsWith('@'))
			{
				string text2 = propertyName.Substring(1);
				string prefix = MiscellaneousUtils.GetPrefix(text2);
				XmlNodeConverter.AddAttribute(reader, document, currentNode, propertyName, text2, manager, prefix);
				return;
			}
			if (propertyName.StartsWith('$'))
			{
				if (propertyName == "$values")
				{
					propertyName = propertyName.Substring(1);
					text = manager.LookupPrefix("http://james.newtonking.com/projects/json");
					this.CreateElement(reader, document, currentNode, propertyName, manager, text, dictionary);
					return;
				}
				if (propertyName == "$id" || propertyName == "$ref" || propertyName == "$type" || propertyName == "$value")
				{
					string text3 = propertyName.Substring(1);
					string text4 = manager.LookupPrefix("http://james.newtonking.com/projects/json");
					XmlNodeConverter.AddAttribute(reader, document, currentNode, propertyName, text3, manager, text4);
					return;
				}
			}
			this.CreateElement(reader, document, currentNode, propertyName, manager, text, dictionary);
		}

		// Token: 0x0600145B RID: 5211 RVA: 0x000701C4 File Offset: 0x0006E3C4
		private void CreateElement(JsonReader reader, IXmlDocument document, IXmlNode currentNode, string elementName, XmlNamespaceManager manager, string elementPrefix, Dictionary<string, string> attributeNameValues)
		{
			IXmlElement xmlElement = this.CreateElement(elementName, document, elementPrefix, manager);
			currentNode.AppendChild(xmlElement);
			if (attributeNameValues != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in attributeNameValues)
				{
					string text = XmlConvert.EncodeName(keyValuePair.Key);
					string prefix = MiscellaneousUtils.GetPrefix(keyValuePair.Key);
					IXmlNode xmlNode = ((!string.IsNullOrEmpty(prefix)) ? document.CreateAttribute(text, manager.LookupNamespace(prefix) ?? string.Empty, keyValuePair.Value) : document.CreateAttribute(text, keyValuePair.Value));
					xmlElement.SetAttributeNode(xmlNode);
				}
			}
			switch (reader.TokenType)
			{
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Date:
			case JsonToken.Bytes:
			{
				string text2 = XmlNodeConverter.ConvertTokenToXmlValue(reader);
				if (text2 != null)
				{
					xmlElement.AppendChild(document.CreateTextNode(text2));
					return;
				}
				return;
			}
			case JsonToken.Null:
				return;
			case JsonToken.EndObject:
				manager.RemoveNamespace(string.Empty, manager.DefaultNamespace);
				return;
			}
			manager.PushScope();
			this.DeserializeNode(reader, document, manager, xmlElement);
			manager.PopScope();
			manager.RemoveNamespace(string.Empty, manager.DefaultNamespace);
		}

		// Token: 0x0600145C RID: 5212 RVA: 0x00070338 File Offset: 0x0006E538
		private static void AddAttribute(JsonReader reader, IXmlDocument document, IXmlNode currentNode, string propertyName, string attributeName, XmlNamespaceManager manager, string attributePrefix)
		{
			if (currentNode.NodeType == XmlNodeType.Document)
			{
				throw JsonSerializationException.Create(reader, "JSON root object has property '{0}' that will be converted to an attribute. A root object cannot have any attribute properties. Consider specifying a DeserializeRootElementName.".FormatWith(CultureInfo.InvariantCulture, propertyName));
			}
			string text = XmlConvert.EncodeName(attributeName);
			string text2 = XmlNodeConverter.ConvertTokenToXmlValue(reader);
			IXmlNode xmlNode = ((!string.IsNullOrEmpty(attributePrefix)) ? document.CreateAttribute(text, manager.LookupNamespace(attributePrefix), text2) : document.CreateAttribute(text, text2));
			((IXmlElement)currentNode).SetAttributeNode(xmlNode);
		}

		// Token: 0x0600145D RID: 5213 RVA: 0x000703B4 File Offset: 0x0006E5B4
		private static string ConvertTokenToXmlValue(JsonReader reader)
		{
			switch (reader.TokenType)
			{
			case JsonToken.Integer:
				if (reader.Value is BigInteger)
				{
					return ((BigInteger)reader.Value).ToString(CultureInfo.InvariantCulture);
				}
				return XmlConvert.ToString(Convert.ToInt64(reader.Value, CultureInfo.InvariantCulture));
			case JsonToken.Float:
				if (reader.Value is decimal)
				{
					return XmlConvert.ToString((decimal)reader.Value);
				}
				if (reader.Value is float)
				{
					return XmlConvert.ToString((float)reader.Value);
				}
				return XmlConvert.ToString(Convert.ToDouble(reader.Value, CultureInfo.InvariantCulture));
			case JsonToken.String:
			{
				object value = reader.Value;
				if (value == null)
				{
					return null;
				}
				return value.ToString();
			}
			case JsonToken.Boolean:
				return XmlConvert.ToString(Convert.ToBoolean(reader.Value, CultureInfo.InvariantCulture));
			case JsonToken.Null:
				return null;
			case JsonToken.Date:
			{
				if (reader.Value is DateTimeOffset)
				{
					return XmlConvert.ToString((DateTimeOffset)reader.Value);
				}
				DateTime dateTime = Convert.ToDateTime(reader.Value, CultureInfo.InvariantCulture);
				return XmlConvert.ToString(dateTime, DateTimeUtils.ToSerializationMode(dateTime.Kind));
			}
			case JsonToken.Bytes:
				return Convert.ToBase64String((byte[])reader.Value);
			}
			throw JsonSerializationException.Create(reader, "Cannot get an XML string value from token type '{0}'.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
		}

		// Token: 0x0600145E RID: 5214 RVA: 0x00070540 File Offset: 0x0006E740
		private void ReadArrayElements(JsonReader reader, IXmlDocument document, string propertyName, IXmlNode currentNode, XmlNamespaceManager manager)
		{
			string prefix = MiscellaneousUtils.GetPrefix(propertyName);
			IXmlElement xmlElement = this.CreateElement(propertyName, document, prefix, manager);
			currentNode.AppendChild(xmlElement);
			int num = 0;
			while (reader.Read() && reader.TokenType != JsonToken.EndArray)
			{
				this.DeserializeValue(reader, document, manager, propertyName, xmlElement);
				num++;
			}
			if (this.WriteArrayAttribute)
			{
				this.AddJsonArrayAttribute(xmlElement, document);
			}
			if (num == 1 && this.WriteArrayAttribute)
			{
				foreach (IXmlNode xmlNode in xmlElement.ChildNodes)
				{
					IXmlElement xmlElement2 = xmlNode as IXmlElement;
					if (xmlElement2 != null && xmlElement2.LocalName == propertyName)
					{
						this.AddJsonArrayAttribute(xmlElement2, document);
						break;
					}
				}
			}
		}

		// Token: 0x0600145F RID: 5215 RVA: 0x0007062C File Offset: 0x0006E82C
		private void AddJsonArrayAttribute(IXmlElement element, IXmlDocument document)
		{
			element.SetAttributeNode(document.CreateAttribute("json:Array", "http://james.newtonking.com/projects/json", "true"));
			if (element is XElementWrapper && element.GetPrefixOfNamespace("http://james.newtonking.com/projects/json") == null)
			{
				element.SetAttributeNode(document.CreateAttribute("xmlns:json", "http://www.w3.org/2000/xmlns/", "http://james.newtonking.com/projects/json"));
			}
		}

		// Token: 0x06001460 RID: 5216 RVA: 0x00070690 File Offset: 0x0006E890
		private Dictionary<string, string> ReadAttributeElements(JsonReader reader, XmlNamespaceManager manager)
		{
			switch (reader.TokenType)
			{
			case JsonToken.StartConstructor:
			case JsonToken.Integer:
			case JsonToken.Float:
			case JsonToken.String:
			case JsonToken.Boolean:
			case JsonToken.Null:
			case JsonToken.Date:
			case JsonToken.Bytes:
				return null;
			}
			Dictionary<string, string> dictionary = null;
			bool flag = false;
			while (!flag && reader.Read())
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType != JsonToken.PropertyName)
				{
					if (tokenType != JsonToken.Comment && tokenType != JsonToken.EndObject)
					{
						throw JsonSerializationException.Create(reader, "Unexpected JsonToken: " + reader.TokenType);
					}
					flag = true;
				}
				else
				{
					string text = reader.Value.ToString();
					if (!string.IsNullOrEmpty(text))
					{
						char c = text[0];
						if (c != '$')
						{
							if (c == '@')
							{
								if (dictionary == null)
								{
									dictionary = new Dictionary<string, string>();
								}
								text = text.Substring(1);
								reader.ReadAndAssert();
								string text2 = XmlNodeConverter.ConvertTokenToXmlValue(reader);
								dictionary.Add(text, text2);
								string text3;
								if (this.IsNamespaceAttribute(text, out text3))
								{
									manager.AddNamespace(text3, text2);
								}
							}
							else
							{
								flag = true;
							}
						}
						else if (text == "$values" || text == "$id" || text == "$ref" || text == "$type" || text == "$value")
						{
							string text4 = manager.LookupPrefix("http://james.newtonking.com/projects/json");
							if (text4 == null)
							{
								if (dictionary == null)
								{
									dictionary = new Dictionary<string, string>();
								}
								int? num = null;
								while (manager.LookupNamespace("json" + num) != null)
								{
									num = new int?(num.GetValueOrDefault() + 1);
								}
								text4 = "json" + num;
								dictionary.Add("xmlns:" + text4, "http://james.newtonking.com/projects/json");
								manager.AddNamespace(text4, "http://james.newtonking.com/projects/json");
							}
							if (text == "$values")
							{
								flag = true;
							}
							else
							{
								text = text.Substring(1);
								reader.ReadAndAssert();
								if (!JsonTokenUtils.IsPrimitiveToken(reader.TokenType))
								{
									throw JsonSerializationException.Create(reader, "Unexpected JsonToken: " + reader.TokenType);
								}
								if (dictionary == null)
								{
									dictionary = new Dictionary<string, string>();
								}
								object value = reader.Value;
								string text2 = ((value != null) ? value.ToString() : null);
								dictionary.Add(text4 + ":" + text, text2);
							}
						}
						else
						{
							flag = true;
						}
					}
					else
					{
						flag = true;
					}
				}
			}
			return dictionary;
		}

		// Token: 0x06001461 RID: 5217 RVA: 0x00070950 File Offset: 0x0006EB50
		private void CreateInstruction(JsonReader reader, IXmlDocument document, IXmlNode currentNode, string propertyName)
		{
			if (propertyName == "?xml")
			{
				string text = null;
				string text2 = null;
				string text3 = null;
				while (reader.Read() && reader.TokenType != JsonToken.EndObject)
				{
					string text4 = reader.Value.ToString();
					if (!(text4 == "@version"))
					{
						if (!(text4 == "@encoding"))
						{
							if (!(text4 == "@standalone"))
							{
								throw JsonSerializationException.Create(reader, "Unexpected property name encountered while deserializing XmlDeclaration: " + reader.Value);
							}
							reader.ReadAndAssert();
							text3 = XmlNodeConverter.ConvertTokenToXmlValue(reader);
						}
						else
						{
							reader.ReadAndAssert();
							text2 = XmlNodeConverter.ConvertTokenToXmlValue(reader);
						}
					}
					else
					{
						reader.ReadAndAssert();
						text = XmlNodeConverter.ConvertTokenToXmlValue(reader);
					}
				}
				IXmlNode xmlNode = document.CreateXmlDeclaration(text, text2, text3);
				currentNode.AppendChild(xmlNode);
				return;
			}
			IXmlNode xmlNode2 = document.CreateProcessingInstruction(propertyName.Substring(1), XmlNodeConverter.ConvertTokenToXmlValue(reader));
			currentNode.AppendChild(xmlNode2);
		}

		// Token: 0x06001462 RID: 5218 RVA: 0x00070A58 File Offset: 0x0006EC58
		private void CreateDocumentType(JsonReader reader, IXmlDocument document, IXmlNode currentNode)
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			string text4 = null;
			while (reader.Read() && reader.TokenType != JsonToken.EndObject)
			{
				string text5 = reader.Value.ToString();
				if (!(text5 == "@name"))
				{
					if (!(text5 == "@public"))
					{
						if (!(text5 == "@system"))
						{
							if (!(text5 == "@internalSubset"))
							{
								throw JsonSerializationException.Create(reader, "Unexpected property name encountered while deserializing XmlDeclaration: " + reader.Value);
							}
							reader.ReadAndAssert();
							text4 = XmlNodeConverter.ConvertTokenToXmlValue(reader);
						}
						else
						{
							reader.ReadAndAssert();
							text3 = XmlNodeConverter.ConvertTokenToXmlValue(reader);
						}
					}
					else
					{
						reader.ReadAndAssert();
						text2 = XmlNodeConverter.ConvertTokenToXmlValue(reader);
					}
				}
				else
				{
					reader.ReadAndAssert();
					text = XmlNodeConverter.ConvertTokenToXmlValue(reader);
				}
			}
			IXmlNode xmlNode = document.CreateXmlDocumentType(text, text2, text3, text4);
			currentNode.AppendChild(xmlNode);
		}

		// Token: 0x06001463 RID: 5219 RVA: 0x00070B58 File Offset: 0x0006ED58
		private IXmlElement CreateElement(string elementName, IXmlDocument document, string elementPrefix, XmlNamespaceManager manager)
		{
			string text = XmlConvert.EncodeName(elementName);
			string text2 = (string.IsNullOrEmpty(elementPrefix) ? manager.DefaultNamespace : manager.LookupNamespace(elementPrefix));
			if (string.IsNullOrEmpty(text2))
			{
				return document.CreateElement(text);
			}
			return document.CreateElement(text, text2);
		}

		// Token: 0x06001464 RID: 5220 RVA: 0x00070BAC File Offset: 0x0006EDAC
		private void DeserializeNode(JsonReader reader, IXmlDocument document, XmlNamespaceManager manager, IXmlNode currentNode)
		{
			JsonToken tokenType;
			for (;;)
			{
				tokenType = reader.TokenType;
				switch (tokenType)
				{
				case JsonToken.StartConstructor:
				{
					string text = reader.Value.ToString();
					while (reader.Read())
					{
						if (reader.TokenType == JsonToken.EndConstructor)
						{
							break;
						}
						this.DeserializeValue(reader, document, manager, text, currentNode);
					}
					goto IL_0193;
				}
				case JsonToken.PropertyName:
				{
					if (currentNode.NodeType == XmlNodeType.Document && document.DocumentElement != null)
					{
						goto Block_3;
					}
					string text2 = reader.Value.ToString();
					reader.ReadAndAssert();
					if (reader.TokenType == JsonToken.StartArray)
					{
						int num = 0;
						while (reader.Read() && reader.TokenType != JsonToken.EndArray)
						{
							this.DeserializeValue(reader, document, manager, text2, currentNode);
							num++;
						}
						if (num != 1 || !this.WriteArrayAttribute)
						{
							goto IL_0193;
						}
						using (List<IXmlNode>.Enumerator enumerator = currentNode.ChildNodes.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								IXmlNode xmlNode = enumerator.Current;
								IXmlElement xmlElement = xmlNode as IXmlElement;
								if (xmlElement != null && xmlElement.LocalName == text2)
								{
									this.AddJsonArrayAttribute(xmlElement, document);
									break;
								}
							}
							goto IL_0193;
						}
					}
					this.DeserializeValue(reader, document, manager, text2, currentNode);
					goto IL_0193;
				}
				case JsonToken.Comment:
					currentNode.AppendChild(document.CreateComment((string)reader.Value));
					goto IL_0193;
				}
				break;
				IL_0193:
				if (!reader.Read())
				{
					return;
				}
			}
			if (tokenType - JsonToken.EndObject > 1)
			{
				throw JsonSerializationException.Create(reader, "Unexpected JsonToken when deserializing node: " + reader.TokenType);
			}
			return;
			Block_3:
			throw JsonSerializationException.Create(reader, "JSON root object has multiple properties. The root object must have a single property in order to create a valid XML document. Consider specifying a DeserializeRootElementName.");
		}

		// Token: 0x06001465 RID: 5221 RVA: 0x00070D68 File Offset: 0x0006EF68
		private bool IsNamespaceAttribute(string attributeName, out string prefix)
		{
			if (attributeName.StartsWith("xmlns", StringComparison.Ordinal))
			{
				if (attributeName.Length == 5)
				{
					prefix = string.Empty;
					return true;
				}
				if (attributeName[5] == ':')
				{
					prefix = attributeName.Substring(6, attributeName.Length - 6);
					return true;
				}
			}
			prefix = null;
			return false;
		}

		// Token: 0x06001466 RID: 5222 RVA: 0x00070DC4 File Offset: 0x0006EFC4
		private bool ValueAttributes(List<IXmlNode> c)
		{
			foreach (IXmlNode xmlNode in c)
			{
				if (!(xmlNode.NamespaceUri == "http://james.newtonking.com/projects/json") && (!(xmlNode.NamespaceUri == "http://www.w3.org/2000/xmlns/") || !(xmlNode.Value == "http://james.newtonking.com/projects/json")))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06001467 RID: 5223 RVA: 0x00070E5C File Offset: 0x0006F05C
		public override bool CanConvert(Type valueType)
		{
			if (valueType.AssignableToTypeName("System.Xml.Linq.XObject", false))
			{
				return this.IsXObject(valueType);
			}
			return valueType.AssignableToTypeName("System.Xml.XmlNode", false) && this.IsXmlNode(valueType);
		}

		// Token: 0x06001468 RID: 5224 RVA: 0x00070E94 File Offset: 0x0006F094
		[MethodImpl(MethodImplOptions.NoInlining)]
		private bool IsXObject(Type valueType)
		{
			return typeof(XObject).IsAssignableFrom(valueType);
		}

		// Token: 0x06001469 RID: 5225 RVA: 0x00070EA8 File Offset: 0x0006F0A8
		[MethodImpl(MethodImplOptions.NoInlining)]
		private bool IsXmlNode(Type valueType)
		{
			return typeof(XmlNode).IsAssignableFrom(valueType);
		}

		// Token: 0x04000711 RID: 1809
		internal static readonly List<IXmlNode> EmptyChildNodes = new List<IXmlNode>();

		// Token: 0x04000712 RID: 1810
		private const string TextName = "#text";

		// Token: 0x04000713 RID: 1811
		private const string CommentName = "#comment";

		// Token: 0x04000714 RID: 1812
		private const string CDataName = "#cdata-section";

		// Token: 0x04000715 RID: 1813
		private const string WhitespaceName = "#whitespace";

		// Token: 0x04000716 RID: 1814
		private const string SignificantWhitespaceName = "#significant-whitespace";

		// Token: 0x04000717 RID: 1815
		private const string DeclarationName = "?xml";

		// Token: 0x04000718 RID: 1816
		private const string JsonNamespaceUri = "http://james.newtonking.com/projects/json";
	}
}
