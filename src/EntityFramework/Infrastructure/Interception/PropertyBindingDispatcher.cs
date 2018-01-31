using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.Entity.Infrastructure.Interception
{
    internal class PropertyBindingDispatcher
    {
        private readonly InternalDispatcher<IPropertyBindingInterceptor> _internalDispatcher
            = new InternalDispatcher<IPropertyBindingInterceptor>();

        public InternalDispatcher<IPropertyBindingInterceptor> InternalDispatcher
        {
            get { return _internalDispatcher; }
        }

        public virtual void AddPropertyBindings(Type clrType, List<MemberBinding> bindings)
        {
            _internalDispatcher.Dispatch(
                (i) => {
                    bindings.AddRange(i.CreatePropertyBindings(clrType)); 
                });
        }

        public virtual MemberBinding ChangePropertyBinding(MemberBinding memberBinding)
        {
            return _internalDispatcher.Dispatch(
                memberBinding,
                (m, i) => i.ChangePropertyBinding(m));
        }
    }
}
