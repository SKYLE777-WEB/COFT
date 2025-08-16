using System;

namespace Newtonsoft.Json
{
	// Token: 0x020000C7 RID: 199
	public enum WriteState
	{
		// Token: 0x0400046F RID: 1135
		Error,
		// Token: 0x04000470 RID: 1136
		Closed,
		// Token: 0x04000471 RID: 1137
		Object,
		// Token: 0x04000472 RID: 1138
		Array,
		// Token: 0x04000473 RID: 1139
		Constructor,
		// Token: 0x04000474 RID: 1140
		Property,
		// Token: 0x04000475 RID: 1141
		Start
	}
}
