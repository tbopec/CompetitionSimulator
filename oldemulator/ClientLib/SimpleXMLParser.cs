using System.Xml;

namespace Eurosim.ClientLib
{
	public static class SimpleXmlParser
	{
		public static string GetTagContent(string str, string tagName)
		{
			XmlNode xmlNode = GetNode(str, tagName);
			return xmlNode.InnerXml;
		}

		public static string GetTagWithContent(string str, string tagName)
		{
			return GetNode(str, tagName).OuterXml;
		}

		public static string GetRootTagName(string str)
		{
			var doc = new XmlDocument();
			doc.LoadXml(str);
			return doc.DocumentElement.Name;
		}

		private static XmlNode GetNode(string str, string tagName)
		{
			var doc = new XmlDocument();
			doc.LoadXml(str);
			XmlNode xmlNode = doc.GetElementsByTagName(tagName)[0];
			return xmlNode;
		}
	}
}