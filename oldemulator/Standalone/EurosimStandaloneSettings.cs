using System;
using AIRLab.Thornado;

namespace EurosimStandalone
{
	[Thornado]
	[Serializable]
	internal class EurosimStandaloneSettings
	{
		[Thornado] public readonly bool UseKeyboard=false;
	}
}