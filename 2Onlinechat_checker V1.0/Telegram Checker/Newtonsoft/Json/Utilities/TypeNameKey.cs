using System;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000F2 RID: 242
	internal struct TypeNameKey : IEquatable<TypeNameKey>
	{
		// Token: 0x06000D02 RID: 3330 RVA: 0x00051FF8 File Offset: 0x000501F8
		public TypeNameKey(string assemblyName, string typeName)
		{
			this.AssemblyName = assemblyName;
			this.TypeName = typeName;
		}

		// Token: 0x06000D03 RID: 3331 RVA: 0x00052008 File Offset: 0x00050208
		public override int GetHashCode()
		{
			string assemblyName = this.AssemblyName;
			int num = ((assemblyName != null) ? assemblyName.GetHashCode() : 0);
			string typeName = this.TypeName;
			return num ^ ((typeName != null) ? typeName.GetHashCode() : 0);
		}

		// Token: 0x06000D04 RID: 3332 RVA: 0x0005203C File Offset: 0x0005023C
		public override bool Equals(object obj)
		{
			return obj is TypeNameKey && this.Equals((TypeNameKey)obj);
		}

		// Token: 0x06000D05 RID: 3333 RVA: 0x00052058 File Offset: 0x00050258
		public bool Equals(TypeNameKey other)
		{
			return this.AssemblyName == other.AssemblyName && this.TypeName == other.TypeName;
		}

		// Token: 0x0400052C RID: 1324
		internal readonly string AssemblyName;

		// Token: 0x0400052D RID: 1325
		internal readonly string TypeName;
	}
}
