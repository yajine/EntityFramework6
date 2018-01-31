using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Data.Entity.Infrastructure.Interception
{
    /// <summary>
    /// property memberbinding interceptor
    /// </summary>
    public interface IPropertyBindingInterceptor : IDbInterceptor
    {
        /// <summary>
        /// change current memberbinding
        /// </summary>
        /// <param name="memberBinding">memberBinding</param>
        /// <returns>memberBinding</returns>
        MemberBinding ChangePropertyBinding(MemberBinding memberBinding);

        /// <summary>
        /// create new property memberbindings
        /// </summary>
        /// <param name="entityType">entity type</param>
        /// <returns>list of memberbinding</returns>
        List<MemberBinding> CreatePropertyBindings(Type entityType);
    }
}
