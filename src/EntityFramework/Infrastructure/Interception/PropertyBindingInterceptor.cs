using System.Collections.Generic;
using System.Data.Entity.Core.Common.Internal.Materialization;
using System.Data.Entity.Core.Objects.ELinq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.Entity.Infrastructure.Interception
{
    /// <summary>
    /// property memberbinding interceptor
    /// </summary>
    public abstract class PropertyBindingInterceptor : IPropertyBindingInterceptor
    {
        /// <summary>
        /// change current memberbinding
        /// </summary>
        /// <param name="memberBinding">memberBinding</param>
        /// <returns>memberBinding</returns>
        public virtual MemberBinding ChangePropertyBinding(MemberBinding memberBinding)
        {
            return memberBinding;
        }

        /// <summary>
        /// create new property memberbindings
        /// </summary>
        /// <param name="entityType">entity type</param>
        /// <returns>list of memberbinding</returns>
        public virtual List<MemberBinding> CreatePropertyBindings(Type entityType)
        {
            List<MemberBinding> memberBindings = new List<MemberBinding>();
            return memberBindings;
        }

        /// <summary>
        /// create property read expression by property name and type
        /// </summary>
        /// <param name="propertyType">property type</param>
        /// <param name="propertyName">property name</param>
        /// <returns>expression</returns>
        protected Expression CreateReadExpression(Type propertyType, string propertyName)
        {
            bool needsNullableCheck;
            MethodInfo readerMethod = CodeGenEmitter.GetReaderMethod(propertyType, out needsNullableCheck);
            MethodCallExpression getOrdinalExpression = Expression.Call(CodeGenEmitter.Shaper_Reader, CodeGenEmitter.DbDataReader_GetOrdinal, Expression.Constant(propertyName));
            BinaryExpression validOrdinalExpression = Expression.Equal(getOrdinalExpression, Expression.Constant(-1));
            MethodCallExpression getValueExpression = Expression.Call(CodeGenEmitter.Shaper_Reader, readerMethod, getOrdinalExpression);
            MethodCallExpression isDbNullExpression = Expression.Call(CodeGenEmitter.Shaper_Reader, CodeGenEmitter.DbDataReader_IsDBNull, getOrdinalExpression);
            BinaryExpression orExpression = Expression.Or(validOrdinalExpression, isDbNullExpression);
            object defaultValue = TypeSystem.GetDefaultValue(propertyType);
            ConstantExpression constantExpression;
            if (propertyType.IsEnum) constantExpression = Expression.Constant((int)defaultValue);
            else constantExpression = Expression.Constant(defaultValue, propertyType);
            ConditionalExpression readExpression = Expression.Condition(orExpression, constantExpression, getValueExpression);
            return readExpression;
        }

        /// <summary>
        /// create property read expression by ordinal
        /// </summary>
        /// <param name="propertyType">property type</param>
        /// <param name="ordinal">ordinal</param>
        /// <returns>expression</returns>
        protected Expression CreateReadExpression(Type propertyType, int ordinal)
        {
            bool needsNullableCheck;
            MethodInfo readerMethod = CodeGenEmitter.GetReaderMethod(propertyType, out needsNullableCheck);
            Expression getValueExpression = CodeGenEmitter.Emit_Reader_GetValue(ordinal, propertyType);
            Expression readExpression = CodeGenEmitter.Emit_Conditional_NotDBNull(getValueExpression, ordinal, propertyType);
            return readExpression;
        }

        /// <summary>
        /// create memberbinding to dictionary
        /// </summary>
        /// <param name="dictionaryType">type of dictionary</param>
        /// <param name="dictionaryProperty">property name of dictionary</param>
        /// <param name="key">key name</param>
        /// <param name="readExpression">property read expression</param>
        /// <returns>memberbinding</returns>
        protected MemberBinding CreateDictionaryBinding(Type dictionaryType, PropertyInfo dictionaryProperty, string key, Expression readExpression)
        {
            var addMethod = dictionaryType.GetMethod("Add");
            var expression = Expression.ElementInit(
                        addMethod,
                        Expression.Constant(key),
                        Expression.Convert(readExpression, 
                        typeof(object)));
            return Expression.ListBind(dictionaryProperty, expression);
        }
    }
}
