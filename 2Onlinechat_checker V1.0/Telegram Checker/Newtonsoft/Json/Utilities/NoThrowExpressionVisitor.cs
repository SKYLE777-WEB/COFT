using System;
using System.Linq.Expressions;

namespace Newtonsoft.Json.Utilities
{
	// Token: 0x020000DD RID: 221
	internal class NoThrowExpressionVisitor : ExpressionVisitor
	{
		// Token: 0x06000C41 RID: 3137 RVA: 0x0004DFD8 File Offset: 0x0004C1D8
		protected override Expression VisitConditional(ConditionalExpression node)
		{
			if (node.IfFalse.NodeType == ExpressionType.Throw)
			{
				return Expression.Condition(node.Test, node.IfTrue, Expression.Constant(NoThrowExpressionVisitor.ErrorResult));
			}
			return base.VisitConditional(node);
		}

		// Token: 0x040004ED RID: 1261
		internal static readonly object ErrorResult = new object();
	}
}
