using System;
using System.Linq.Expressions;
using System.Reflection;

namespace OhioBox.Storage.MySql.Moranbernate
{
	//this class was copied from somewhere else, we will generate another. But for the now it will do.

	internal class ExpressionProcessor
	{
		public static string FindMemberExpression(Expression expression)
		{
			if (expression is MemberExpression)
			{
				MemberExpression memberExpression = (MemberExpression)expression;
				if (memberExpression.Expression.NodeType != ExpressionType.MemberAccess && memberExpression.Expression.NodeType != ExpressionType.Call)
					return memberExpression.Member.Name;
				if (IsNullableOfT(memberExpression.Member.DeclaringType) && memberExpression.Member.Name == "Value")
					return FindMemberExpression(memberExpression.Expression);
				return FindMemberExpression(memberExpression.Expression) + "." + memberExpression.Member.Name;
			}
			if (expression is UnaryExpression)
			{
				UnaryExpression unaryExpression = (UnaryExpression)expression;
				if (unaryExpression.NodeType != ExpressionType.Convert)
					throw new Exception("Cannot interpret member from " + expression);
				return FindMemberExpression(unaryExpression.Operand);
			}
			if (!(expression is MethodCallExpression))
				throw new Exception("Could not determine member from " + expression);
			MethodCallExpression methodCallExpression = (MethodCallExpression)expression;
			if (methodCallExpression.Method.Name == "GetType")
				return ClassMember(methodCallExpression.Object);
			if (methodCallExpression.Method.Name == "get_Item")
				return FindMemberExpression(methodCallExpression.Object);
			if (methodCallExpression.Method.Name == "First")
				return FindMemberExpression(methodCallExpression.Arguments[0]);
			throw new Exception("Unrecognised method call in epression " + expression);
		}

		private static Type FindMemberType(Expression expression)
		{
			if (expression is MemberExpression)
				return expression.Type;
			if (expression is UnaryExpression)
			{
				UnaryExpression unaryExpression = (UnaryExpression)expression;
				if (unaryExpression.NodeType != ExpressionType.Convert)
					throw new Exception("Cannot interpret member from " + expression);
				return FindMemberType(unaryExpression.Operand);
			}
			if (expression is MethodCallExpression)
				return typeof(Type);
			throw new Exception("Could not determine member type from " + expression);
		}

		private static bool IsMemberExpression(Expression expression)
		{
			if (expression is MemberExpression)
			{
				MemberExpression memberExpression = (MemberExpression)expression;
				if (memberExpression.Expression == null)
					return false;
				if (memberExpression.Expression.NodeType == ExpressionType.Parameter || memberExpression.Expression.NodeType == ExpressionType.MemberAccess && EvaluatesToNull(memberExpression.Expression))
					return true;
			}
			if (!(expression is UnaryExpression))
				return false;
			UnaryExpression unaryExpression = (UnaryExpression)expression;
			if (unaryExpression.NodeType != ExpressionType.Convert)
				throw new Exception("Cannot interpret member from " + expression);
			return IsMemberExpression(unaryExpression.Operand);
		}

		private static bool EvaluatesToNull(Expression expression)
		{
			return Expression.Lambda(expression).Compile().DynamicInvoke() == null;
		}

		public static object ConvertType(object value, Type type)
		{
			if (value == null)
				return null;
			if (type.IsAssignableFrom(value.GetType()))
				return value;
			if (IsNullableOfT(type))
				type = Nullable.GetUnderlyingType(type);
			if (type.GetTypeInfo().IsEnum)
				return Enum.ToObject(type, value);
			if (type.GetTypeInfo().IsPrimitive)
				return Convert.ChangeType(value, type);
			throw new Exception("Cannot convert '" + value + "' to " + type);
		}

		private static bool IsNullableOfT(Type type)
		{
			if (type.GetTypeInfo().IsGenericType)
				return type.GetGenericTypeDefinition() == typeof(Nullable<>);
			return false;
		}

		private static string ClassMember(Expression expression)
		{
			if (expression.NodeType == ExpressionType.MemberAccess)
				return FindMemberExpression(expression) + ".class";
			return "class";
		}
	}
}
