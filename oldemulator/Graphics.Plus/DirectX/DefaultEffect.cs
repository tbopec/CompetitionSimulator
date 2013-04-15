using System.Collections.Generic;
using SlimDX;
using SlimDX.Direct3D9;

namespace Eurosim.Graphics.DirectX
{
	internal class DefaultEffect : Effect
	{
		public DefaultEffect(Device device, List<Light> lights)
		{
			_device = device;
			_lights = lights;
			Initialize();
		}

		public override sealed void Initialize()
		{
			for(int i = 0; i < _lights.Count; i++)
			{
				_device.SetLight(i, _lights[i]);
				_device.EnableLight(i, true);
			}
			_device.SetRenderState(RenderState.Lighting, true);
			if(SceneConfig.EnableSpecularHighlights)
				_device.SetRenderState(RenderState.SpecularEnable, true);
			_device.SetRenderState(RenderState.NormalizeNormals, true);
			_device.SetRenderState(RenderState.DiffuseMaterialSource, ColorSource.Material);
			_device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Add);
			foreach(Light light in _lights)
			{
				ShadowMatrices[light] = Matrix.Shadow(new Vector4(light.Direction, 1),
				                                      new Plane(new Vector3(0, 0, -1), 0.05f));
			}
		}

		public override void DrawScene(DirectXScene scene)
		{
			_device.BeginScene();
			NormalMode();
			Matrix oldWorld = WorldTransform;
			if(SceneConfig.EnableShadows)
			{
				_device.SetRenderState(RenderState.StencilEnable, true);
				_device.SetRenderState(RenderState.StencilFunc, Compare.Always);
				_device.SetRenderState(RenderState.StencilRef, 1); //unique value
				//Device.SetRenderState(RenderState.StencilMask, 0);
				_device.SetRenderState(RenderState.StencilPass, StencilOperation.Replace);
				scene.UpdateModels(scene.Floor);
				_device.SetRenderState(RenderState.StencilEnable, false);
				ShadowMode();
				//MultiplyWorldTransform(ShadowMatrices[_lights[0]]);
				scene.UpdateModels(scene.RootBody, scene.Floor);
				NormalMode();
			}
			WorldTransform = oldWorld;
			scene.UpdateModels(scene.RootBody, scene.Floor);
			_device.EndScene();
		}

		public override void DrawModel(DirectXModel model)
		{
			if(model.Mesh == null)
				return;
			Matrix x = WorldTransform;
			MultiplyWorldTransform(model.ModifierMatrix);
			ExtendedMaterial[] materials = model.Mesh.GetMaterials();
			for(int i = 0; i < materials.Length; i++)
			{
				BaseTexture oldTexture = _device.GetTexture(0);
				Texture texture;
				if(model.Textures.TryGetValue(i, out texture) && texture != null)
					_device.SetTexture(0, texture);
				_device.Material = materials[i].MaterialD3D;
				model.Mesh.DrawSubset(i);
				_device.SetTexture(0, oldTexture);
			}
			WorldTransform = x;
		}

		public override void MultiplyWorldTransform(Matrix matrix)
		{
			_device.MultiplyTransform(TransformState.World, matrix);
		}

		public override Matrix WorldTransform { get { return _device.GetTransform(TransformState.World); } set { _device.SetTransform(TransformState.World, value); } }

		public override Matrix ViewTransform { get { return _device.GetTransform(TransformState.View); } set { _device.SetTransform(TransformState.View, value); } }

		public override Matrix ProjectionTransform { get { return _device.GetTransform(TransformState.Projection); } set { _device.SetTransform(TransformState.Projection, value); } }

		private void ShadowMode()
		{
			for(int i = 0; i < _lights.Count; i++)
				_device.EnableLight(i, false);
			_device.SetRenderState(RenderState.ZEnable, false);
			_device.SetRenderState(RenderState.StencilEnable, true);
			_device.SetRenderState(RenderState.StencilFunc, Compare.Equal);
			_device.SetRenderState(RenderState.StencilRef, 1); //unique value
			_device.SetRenderState(RenderState.StencilPass, StencilOperation.Zero);
			_device.SetRenderState(RenderState.AlphaBlendEnable, true);
			_device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
			_device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
			_device.SetRenderState(RenderState.Lighting, true);
			_device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.SelectArg1);
			_device.SetTextureStageState(0, TextureStage.Constant, 0xFFFFFFFF);
			if(_deviceSupportsConstantTextureArg)
			{
				try
				{
					_device.SetTextureStageState(0, TextureStage.AlphaArg0, TextureArgument.Constant);
				}
				catch(Direct3D9Exception)
				{
					_deviceSupportsConstantTextureArg = false;
				}
			}
		}

		private void NormalMode()
		{
			for(int i = 0; i < _lights.Count; i++)
				_device.EnableLight(i, true);
			_device.SetRenderState(RenderState.AlphaBlendEnable, true);
			_device.SetRenderState(RenderState.StencilEnable, false);
			_device.SetRenderState(RenderState.ZEnable, true);
			_device.SetRenderState(RenderState.Lighting, true);
			_device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Diffuse);
			_device.SetRenderState(RenderState.DiffuseMaterialSource, ColorSource.Material);
		}

		private readonly Device _device;
		private readonly List<Light> _lights;
		private static readonly Dictionary<Light, Matrix> ShadowMatrices = new Dictionary<Light, Matrix>();
		private bool _deviceSupportsConstantTextureArg = true;
	}
}