using System.Windows.Forms;
using NUnit.Framework;
using SlimDX.Direct3D9;

namespace Eurosim.Graphics.DirectX
{
	internal partial class DeviceWorker
	{
		[TestFixture]
		public class DeviceCreationTests
		{
			[SetUp]
			public void SetUp()
			{
				_d3D = new Direct3D();
				_form = new Form();
				_deviceparams = new PresentParameters
				                {
				                	BackBufferWidth = 10,
				                	BackBufferHeight = 10,
				                	EnableAutoDepthStencil = false,
				                	DeviceWindowHandle = _form.Handle,
				                };
			}

			[TearDown]
			public void TearDown()
			{
				_d3D.Dispose();
				if (_device != null)
					_device.Dispose();
			}

			[Test]
			public void Software()
			{
				bool result = TryCreateSoftwareDevice(_d3D, _deviceparams, out _device);
				Assert.That(result);
			}

			[Test]
			public void Hardware()
			{
				bool result = TryCreateHardwareDevice(_d3D, _deviceparams, out _device);
				Assert.That(result);
			}

			private Direct3D _d3D;
			private PresentParameters _deviceparams;
			private Form _form;
			private Device _device;
		}
	}
}