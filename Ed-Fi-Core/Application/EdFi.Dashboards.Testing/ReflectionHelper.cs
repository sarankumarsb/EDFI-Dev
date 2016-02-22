// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace EdFi.Dashboards.Testing
{
	public static class ReflectionHelper
	{
		public static bool MeetsSpecialGenericConstraints(Type genericArgType, Type proposedSpecificType)
		{
			GenericParameterAttributes gpa = genericArgType.GenericParameterAttributes;
			GenericParameterAttributes constraints = gpa & GenericParameterAttributes.SpecialConstraintMask;

			// No constraints, away we go!
			if (constraints == GenericParameterAttributes.None)
				return true;

			// "class" constraint and this is a value type
			if ((constraints & GenericParameterAttributes.ReferenceTypeConstraint) != 0
				&& proposedSpecificType.IsValueType)
			{
				return false;
			}

			// "struct" constraint and this is a value type
			if ((constraints & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0
				&& !proposedSpecificType.IsValueType)
			{
				return false;
			}

			// "new()" constraint and this type has no default constructor
			if ((constraints & GenericParameterAttributes.DefaultConstructorConstraint) != 0
				&& proposedSpecificType.GetConstructor(Type.EmptyTypes) == null)
			{
				return false;
			}

			return true;
		}

		public static PropertyInfo GetProperty<MODEL>(Expression<Func<MODEL, object>> expression)
		{
			MemberExpression memberExpression = GetMemberExpression(expression);
			return (PropertyInfo)memberExpression.Member;
		}

		public static PropertyInfo GetProperty<MODEL, T>(Expression<Func<MODEL, T>> expression)
		{
			MemberExpression memberExpression = GetMemberExpression(expression);
			return (PropertyInfo)memberExpression.Member;
		}

		private static MemberExpression GetMemberExpression<MODEL, T>(Expression<Func<MODEL, T>> expression)
		{
			MemberExpression memberExpression = null;
			if (expression.Body.NodeType == ExpressionType.Convert)
			{
				var body = (UnaryExpression)expression.Body;
				memberExpression = body.Operand as MemberExpression;
			}
			else if (expression.Body.NodeType == ExpressionType.MemberAccess)
			{
				memberExpression = expression.Body as MemberExpression;
			}


			if (memberExpression == null) throw new ArgumentException("Not a member access", "member");
			return memberExpression;
		}

		//public static Accessor GetAccessor<MODEL>(Expression<Func<MODEL, object>> expression)
		//{
		//    MemberExpression memberExpression = getMemberExpression(expression);

		//    return GetAccessor(memberExpression);
		//}

		//public static Accessor GetAccessor(MemberExpression memberExpression)
		//{
		//    var list = new List<PropertyInfo>();

		//    while (memberExpression != null)
		//    {
		//        list.Add((PropertyInfo)memberExpression.Member);
		//        memberExpression = memberExpression.Expression as MemberExpression;
		//    }

		//    if (list.Count == 1)
		//    {
		//        return new SingleProperty(list[0]);
		//    }

		//    list.Reverse();
		//    return new PropertyChain(list.ToArray());
		//}

		//public static Accessor GetAccessor<MODEL, T>(Expression<Func<MODEL, T>> expression)
		//{
		//    MemberExpression memberExpression = getMemberExpression(expression);

		//    return GetAccessor(memberExpression);
		//}

		public static MethodInfo GetMethod<T>(Expression<Func<T, object>> expression)
		{
			return new FindMethodVisitor(expression).Method;
		}

		public static MethodInfo GetMethod<DELEGATE>(Expression<DELEGATE> expression)
		{
			return new FindMethodVisitor(expression).Method;
		}

        public static MethodInfo GetMethod<T, U>(Expression<Func<T, U>> expression)
		{
			return new FindMethodVisitor(expression).Method;
		}

        public static MethodInfo GetMethod<T, U, V>(Expression<Func<T, U, V>> expression)
		{
			return new FindMethodVisitor(expression).Method;
		}

		public static MethodInfo GetMethod<T>(Expression<Action<T>> expression)
		{
			return new FindMethodVisitor(expression).Method;
		}


		public static T GetAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
		{
			object[] atts = provider.GetCustomAttributes(typeof(T), true);
			return atts.Length > 0 ? atts[0] as T : null;
		}

		public static bool HasAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
		{
			object[] atts = provider.GetCustomAttributes(typeof(T), true);
			return atts.Length > 0;
		}

		public static void ForAttribute<T>(this ICustomAttributeProvider provider, Action<T> action) where T : Attribute
		{
			foreach (T attribute in provider.GetCustomAttributes(typeof(T), true))
			{
				action(attribute);
			}
		}

		//public static void ForAttribute<T>(this Accessor accessor, Action<T> action) where T : Attribute
		//{
		//    foreach (T attribute in accessor.InnerProperty.GetCustomAttributes(typeof(T), true))
		//    {
		//        action(attribute);
		//    }
		//}

		//public static T GetAttribute<T>(this Accessor provider) where T : Attribute
		//{
		//    return provider.InnerProperty.GetAttribute<T>();
		//}

		//public static bool HasAttribute<T>(this Accessor provider) where T : Attribute
		//{
		//    return provider.InnerProperty.HasAttribute<T>();
		//}


	}

	public class FindMethodVisitor : ExpressionVisitorBase
	{
		private MethodInfo _method;

		public FindMethodVisitor(Expression expression)
		{
			Visit(expression);
		}

		public MethodInfo Method { get { return _method; } }

		protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
		{
			_method = methodCallExpression.Method;
			return methodCallExpression;
		}
	}

	/// <summary>
	/// Provides virtual methods that can be used by subclasses to parse an expression tree.
	/// </summary>
	/// <remarks>
	/// This class actually already exists in the System.Core assembly...as an internal class.
	/// I can only speculate as to why it is internal, but it is obviously much too dangerous
	/// for anyone outside of Microsoft to be using...
	/// </remarks>
	[DebuggerStepThrough, DebuggerNonUserCode]
	public abstract class ExpressionVisitorBase
	{
		public virtual Expression Visit(Expression expression)
		{
			if (expression == null) return expression;

			switch (expression.NodeType)
			{
				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
				case ExpressionType.Not:
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
				case ExpressionType.ArrayLength:
				case ExpressionType.Quote:
				case ExpressionType.TypeAs:
					return VisitUnary((UnaryExpression)expression);
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
				case ExpressionType.Divide:
				case ExpressionType.Modulo:
				case ExpressionType.And:
				case ExpressionType.AndAlso:
				case ExpressionType.Or:
				case ExpressionType.OrElse:
				case ExpressionType.LessThan:
				case ExpressionType.LessThanOrEqual:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual:
				case ExpressionType.Equal:
				case ExpressionType.NotEqual:
				case ExpressionType.Coalesce:
				case ExpressionType.ArrayIndex:
				case ExpressionType.RightShift:
				case ExpressionType.LeftShift:
				case ExpressionType.ExclusiveOr:
					return VisitBinary((BinaryExpression)expression);
				case ExpressionType.TypeIs:
					return VisitTypeIs((TypeBinaryExpression)expression);
				case ExpressionType.Conditional:
					return VisitConditional((ConditionalExpression)expression);
				case ExpressionType.Constant:
					return VisitConstant((ConstantExpression)expression);
				case ExpressionType.Parameter:
					return VisitParameter((ParameterExpression)expression);
				case ExpressionType.MemberAccess:
					return VisitMemberAccess((MemberExpression)expression);
				case ExpressionType.Call:
					return VisitMethodCall((MethodCallExpression)expression);
				case ExpressionType.Lambda:
					return VisitLambda((LambdaExpression)expression);
				case ExpressionType.New:
					return VisitNew((NewExpression)expression);
				case ExpressionType.NewArrayInit:
				case ExpressionType.NewArrayBounds:
					return VisitNewArray((NewArrayExpression)expression);
				case ExpressionType.Invoke:
					return VisitInvocation((InvocationExpression)expression);
				case ExpressionType.MemberInit:
					return VisitMemberInit((MemberInitExpression)expression);
				case ExpressionType.ListInit:
					return VisitListInit((ListInitExpression)expression);
				default:
					throw new NotSupportedException(String.Format("Unhandled expression type: '{0}'", expression.NodeType));
			}
		}

		protected virtual MemberBinding VisitBinding(MemberBinding binding)
		{
			switch (binding.BindingType)
			{
				case MemberBindingType.Assignment:
					return VisitMemberAssignment((MemberAssignment)binding);
				case MemberBindingType.MemberBinding:
					return VisitMemberMemberBinding((MemberMemberBinding)binding);
				case MemberBindingType.ListBinding:
					return VisitMemberListBinding((MemberListBinding)binding);
				default:
					throw new NotSupportedException(string.Format("Unhandled binding type '{0}'", binding.BindingType));
			}
		}

		protected virtual ElementInit VisitElementInitializer(ElementInit initializer)
		{
			ReadOnlyCollection<Expression> arguments = VisitList(initializer.Arguments);
			if (arguments != initializer.Arguments)
			{
				return Expression.ElementInit(initializer.AddMethod, arguments);
			}
			return initializer;
		}

		protected virtual Expression VisitUnary(UnaryExpression unaryExpression)
		{
			Expression operand = Visit(unaryExpression.Operand);
			if (operand != unaryExpression.Operand)
			{
				return Expression.MakeUnary(unaryExpression.NodeType, operand, unaryExpression.Type, unaryExpression.Method);
			}
			return unaryExpression;
		}

		protected virtual Expression VisitBinary(BinaryExpression binaryExpression)
		{
			Expression left = Visit(binaryExpression.Left);
			Expression right = Visit(binaryExpression.Right);
			Expression conversion = Visit(binaryExpression.Conversion);

			if (left != binaryExpression.Left || right != binaryExpression.Right || conversion != binaryExpression.Conversion)
			{
				if (binaryExpression.NodeType == ExpressionType.Coalesce && binaryExpression.Conversion != null)
					return Expression.Coalesce(left, right, conversion as LambdaExpression);
				else
					return Expression.MakeBinary(binaryExpression.NodeType, left, right, binaryExpression.IsLiftedToNull, binaryExpression.Method);
			}
			return binaryExpression;
		}

		protected virtual Expression VisitTypeIs(TypeBinaryExpression typeBinaryExpression)
		{
			Expression expr = Visit(typeBinaryExpression.Expression);
			if (expr != typeBinaryExpression.Expression)
			{
				return Expression.TypeIs(expr, typeBinaryExpression.TypeOperand);
			}
			return typeBinaryExpression;
		}

		protected virtual Expression VisitConstant(ConstantExpression constantExpression)
		{
			return constantExpression;
		}

		protected virtual Expression VisitConditional(ConditionalExpression conditionalExpression)
		{
			Expression test = Visit(conditionalExpression.Test);
			Expression ifTrue = Visit(conditionalExpression.IfTrue);
			Expression ifFalse = Visit(conditionalExpression.IfFalse);

			if (test != conditionalExpression.Test || ifTrue != conditionalExpression.IfTrue || ifFalse != conditionalExpression.IfFalse)
			{
				return Expression.Condition(test, ifTrue, ifFalse);
			}

			return conditionalExpression;
		}

		protected virtual Expression VisitParameter(ParameterExpression parameterExpression)
		{
			return parameterExpression;
		}

		protected virtual Expression VisitMemberAccess(MemberExpression memberExpression)
		{
			Expression exp = Visit(memberExpression.Expression);
			if (exp != memberExpression.Expression)
			{
				return Expression.MakeMemberAccess(exp, memberExpression.Member);
			}
			return memberExpression;
		}

		protected virtual Expression VisitMethodCall(MethodCallExpression methodCallExpression)
		{
			Expression obj = Visit(methodCallExpression.Object);
			IEnumerable<Expression> args = VisitList(methodCallExpression.Arguments);

			if (obj != methodCallExpression.Object || args != methodCallExpression.Arguments)
			{
				return Expression.Call(obj, methodCallExpression.Method, args);
			}

			return methodCallExpression;
		}

		protected virtual ReadOnlyCollection<Expression> VisitList(ReadOnlyCollection<Expression> original)
		{
			List<Expression> list = null;
			for (int i = 0, n = original.Count; i < n; i++)
			{
				Expression p = Visit(original[i]);
				if (list != null)
				{
					list.Add(p);
				}
				else if (p != original[i])
				{
					list = new List<Expression>(n);
					for (int j = 0; j < i; j++)
					{
						list.Add(original[j]);
					}
					list.Add(p);
				}
			}

			if (list != null)
				return list.AsReadOnly();

			return original;
		}

		protected virtual MemberAssignment VisitMemberAssignment(MemberAssignment assignment)
		{
			Expression e = Visit(assignment.Expression);

			if (e != assignment.Expression)
			{
				return Expression.Bind(assignment.Member, e);
			}

			return assignment;
		}

		protected virtual MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding binding)
		{
			IEnumerable<MemberBinding> bindings = VisitBindingList(binding.Bindings);

			if (bindings != binding.Bindings)
			{
				return Expression.MemberBind(binding.Member, bindings);
			}

			return binding;
		}

		protected virtual MemberListBinding VisitMemberListBinding(MemberListBinding binding)
		{
			IEnumerable<ElementInit> initializers = VisitElementInitializerList(binding.Initializers);

			if (initializers != binding.Initializers)
			{
				return Expression.ListBind(binding.Member, initializers);
			}
			return binding;
		}

		protected virtual IEnumerable<MemberBinding> VisitBindingList(ReadOnlyCollection<MemberBinding> original)
		{
			List<MemberBinding> list = null;
			for (int i = 0, n = original.Count; i < n; i++)
			{
				MemberBinding b = VisitBinding(original[i]);
				if (list != null)
				{
					list.Add(b);
				}
				else if (b != original[i])
				{
					list = new List<MemberBinding>(n);
					for (int j = 0; j < i; j++)
					{
						list.Add(original[j]);
					}
					list.Add(b);
				}
			}

			if (list != null)
				return list;

			return original;
		}

		protected virtual IEnumerable<ElementInit> VisitElementInitializerList(ReadOnlyCollection<ElementInit> original)
		{
			List<ElementInit> list = null;
			for (int i = 0, n = original.Count; i < n; i++)
			{
				ElementInit init = VisitElementInitializer(original[i]);
				if (list != null)
				{
					list.Add(init);
				}
				else if (init != original[i])
				{
					list = new List<ElementInit>(n);
					for (int j = 0; j < i; j++)
					{
						list.Add(original[j]);
					}
					list.Add(init);
				}
			}

			if (list != null)
				return list;

			return original;
		}

		protected virtual Expression VisitLambda(LambdaExpression lambda)
		{
			Expression body = Visit(lambda.Body);
			if (body != lambda.Body)
			{
				return Expression.Lambda(lambda.Type, body, lambda.Parameters);
			}
			return lambda;
		}

		protected virtual NewExpression VisitNew(NewExpression newExpression)
		{
			IEnumerable<Expression> args = VisitList(newExpression.Arguments);
			if (args != newExpression.Arguments)
			{
				if (newExpression.Members != null)
					return Expression.New(newExpression.Constructor, args, newExpression.Members);
				else
					return Expression.New(newExpression.Constructor, args);
			}

			return newExpression;
		}

		protected virtual Expression VisitMemberInit(MemberInitExpression memberInitExpression)
		{
			NewExpression n = VisitNew(memberInitExpression.NewExpression);
			IEnumerable<MemberBinding> bindings = VisitBindingList(memberInitExpression.Bindings);

			if (n != memberInitExpression.NewExpression || bindings != memberInitExpression.Bindings)
			{
				return Expression.MemberInit(n, bindings);
			}

			return memberInitExpression;
		}

		protected virtual Expression VisitListInit(ListInitExpression listInitExpression)
		{
			NewExpression n = VisitNew(listInitExpression.NewExpression);
			IEnumerable<ElementInit> initializers = VisitElementInitializerList(listInitExpression.Initializers);

			if (n != listInitExpression.NewExpression || initializers != listInitExpression.Initializers)
			{
				return Expression.ListInit(n, initializers);
			}

			return listInitExpression;
		}

		protected virtual Expression VisitNewArray(NewArrayExpression newArrayExpression)
		{
			IEnumerable<Expression> exprs = VisitList(newArrayExpression.Expressions);
			if (exprs != newArrayExpression.Expressions)
			{
				if (newArrayExpression.NodeType == ExpressionType.NewArrayInit)
				{
					return Expression.NewArrayInit(newArrayExpression.Type.GetElementType(), exprs);
				}
				else
				{
					return Expression.NewArrayBounds(newArrayExpression.Type.GetElementType(), exprs);
				}
			}

			return newArrayExpression;
		}

		protected virtual Expression VisitInvocation(InvocationExpression invocationExpression)
		{
			IEnumerable<Expression> args = VisitList(invocationExpression.Arguments);
			Expression expr = Visit(invocationExpression.Expression);

			if (args != invocationExpression.Arguments || expr != invocationExpression.Expression)
			{
				return Expression.Invoke(expr, args);
			}

			return invocationExpression;
		}
	}
}