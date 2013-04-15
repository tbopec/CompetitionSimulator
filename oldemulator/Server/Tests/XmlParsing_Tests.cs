using System.Collections.Generic;
using AIRLab.Mathematics;
using AIRLab.Thornado;
using Eurosim.ClientLib;
using Eurosim.Core;
using NUnit.Framework;

namespace EurosimNetworkServer.Tests
{
	public class XmlParsing_Tests
	{
		[Test]
		public void ArbitraryNodeSelection()
		{
			const string navigatorDataString = @"<a><b>ccc</b></a>";
			string s = string.Format(@"<ACMSensorInfo>
				<NavigatorData>{0}</NavigatorData>
				<MagicEye><Objects><item0>s</item0>
				<item1>s</item1></Objects>
				</MagicEye></ACMSensorInfo>", navigatorDataString);
			string result = SimpleXmlParser.GetTagContent(s, "a");
			Assert.AreEqual("<b>ccc</b>", result);
			result = SimpleXmlParser.GetTagWithContent(s, "a");
			Assert.AreEqual(navigatorDataString, result);
			result = SimpleXmlParser.GetTagContent(s, "NavigatorData");
			Assert.AreEqual(navigatorDataString, result);
			result = SimpleXmlParser.GetTagWithContent(s, "NavigatorData");
			Assert.AreEqual("<NavigatorData>"+navigatorDataString+"</NavigatorData>",result);
		}

		private static string GetSensorDataForNavigator(double navX, double navY, double navAngle)
		{
			var acmInfo=new ACMSensorInfo{NavigatorInfo = new List<NavigatorData>{new NavigatorData() {Location = new Frame2D(navX, navY, Angle.FromGrad(navAngle))}}};
			return IO.XML.WriteToString(acmInfo);
		}
	}
}