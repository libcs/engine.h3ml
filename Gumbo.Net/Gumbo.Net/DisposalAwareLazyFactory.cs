using System;

namespace Gumbo
{
    internal class DisposalAwareLazyFactory
    {
        readonly Func<bool> _IsDisposed;
        readonly string _ObjectName;

        public DisposalAwareLazyFactory(Func<bool> isDisposed, string objectName)
        {
            _IsDisposed = isDisposed ?? throw new ArgumentNullException(nameof(isDisposed));
            _ObjectName = objectName ?? throw new ArgumentNullException(nameof(objectName));
        }

        public Lazy<T> Create<T>(Func<T> factoryMethod) => new Lazy<T>(() =>
        {
            if (_IsDisposed())
                throw new ObjectDisposedException(_ObjectName);
            return factoryMethod();
        });
    }
}
