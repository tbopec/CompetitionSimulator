using System;
using System.Linq.Expressions;

namespace Eurosim.Core
{
	[Serializable]
	public class Model
	{
		public bool Equals(Model other)
		{
			if(ReferenceEquals(null, other)) return false;
			if(ReferenceEquals(this, other)) return true;
			return Equals(other.FilePath, FilePath) &&
			       Equals(other.ResourceName, ResourceName) &&
			       Equals(other._content, _content);
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != typeof(Model)) return false;
			return Equals((Model)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = (FilePath != null ? FilePath.GetHashCode() : 0);
				result = (result * 397) ^ (ResourceName != null ? ResourceName.GetHashCode() : 0);
				result = (result * 397) ^ (_content != null ? _content.GetHashCode() : 0);
				return result;
			}
		}

		public override string ToString()
		{
			return string.Format("FilePath: {0}, ResourceName: {1}", FilePath, ResourceName);
		}

		/// <summary>
		/// ���������������� ������ �� ��������
		/// </summary>
		/// <param name="func">
		/// ������, ����� ���� ������� ����� ����������� � �������� �������� �������.
		/// ��������, ()=>Resources.testtexture �������� testtexture.
		/// </param>
		public static Model FromResource(Expression<Func<byte[]>> func)
		{
			return new Model
			       	{
			       		ResourceName = CommonUtils.GetPropertyName(func)
			       	};
		}

		public string FilePath { get; set; }

		public string ResourceName { get; private set; }
		
		/// <summary>
		/// ��� ������ ������ ���������� ����� �������������. ������������ � �������������
		/// ����� ������������ <see cref="FromResource"/> 
		/// </summary>
		[Obsolete("��� ������ ������ ���������� ����� �������������. ������������ � �������������")]
		public Byte[] Content { get { return _content; } set { _content = value; } }

		private byte[] _content;
	}
}