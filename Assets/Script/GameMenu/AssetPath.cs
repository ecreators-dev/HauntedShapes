using System;
using System.Collections.Generic;

using UnityEditor;

namespace Assets.Script.GameMenu
{
#if UNITY_EDITOR
		public struct AssetPath : IEquatable<AssetPath>, IComparable<AssetPath>
		{
				private AssetPath(string assetPath, string guid)
				{
						Path = assetPath;
						Guid = guid;
						Filename = System.IO.Path.GetFileNameWithoutExtension(Path.Replace("/", "\\"));
				}

				public string Path { get; }

				public string Guid { get; }
				public string Filename { get; }

				public static implicit operator AssetPath(AssetGuid guid) => new AssetPath(AssetDatabase.GUIDToAssetPath(guid), guid);
				public static implicit operator string(AssetPath assetPath) => assetPath.Path;

				public static AssetPath Build(string path, string guid)
				{
						return new AssetPath(path, guid);
				}

				public bool Equals(AssetPath other)
				{
						return other.Path.Equals(this.Path, StringComparison.OrdinalIgnoreCase)
								&& other.Guid.Equals(this.Guid, StringComparison.OrdinalIgnoreCase);
				}

				public override bool Equals(object obj)
				{
						return obj is AssetPath other && Equals(other);
				}

				public override int GetHashCode()
				{
						int hashCode = -928978190;
						hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Path);
						hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Guid);
						return hashCode;
				}

				public int CompareTo(AssetPath other)
				{
						return string.Compare(Path, other.Path, true);
				}
		}

		public struct AssetGuid : IEquatable<AssetGuid>, IEquatable<string>
		{
				private AssetGuid(string guid)
				{
						Guid = guid;
				}

				public string Guid { get; }

				public bool Equals(AssetGuid other)
				{
						return other.Guid.Equals(Guid, StringComparison.OrdinalIgnoreCase);
				}

				public bool Equals(string other)
				{
						return Guid.Equals(other, StringComparison.OrdinalIgnoreCase);
				}

				public override bool Equals(object obj)
				{
						return obj is AssetGuid other && Equals(other);
				}

				public override int GetHashCode()
				{
						return Guid.GetHashCode();
				}

				public static implicit operator AssetGuid(string guid) => new AssetGuid(guid);
				public static implicit operator string(AssetGuid guid) => guid.Guid;
		}
#endif
}