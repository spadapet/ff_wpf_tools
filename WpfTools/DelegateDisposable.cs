using System;

namespace WpfTools
{
    public sealed class DelegateDisposable : IDisposable
    {
        private readonly Action disposeAction;

        public DelegateDisposable(Action disposeAction)
        {
            this.disposeAction = disposeAction;
        }

        void IDisposable.Dispose()
        {
            this.disposeAction?.Invoke();
        }
    }
}
