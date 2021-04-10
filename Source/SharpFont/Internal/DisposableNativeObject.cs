using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SharpFont.Internal
{
	public abstract class DisposableNativeObject : INativeObject, IDisposable
	{
		private readonly IntPtr _reference;
		private bool _inDispose;
		public DisposableNativeObject(IntPtr reference)
		{
			IsDisposed = false;
			_reference = reference;
		}

		~DisposableNativeObject()
		{
			if (!IsDisposed && !_inDispose)
			{
				_inDispose = true;
				DisposeEvent?.Invoke(false);
				Dispose(false);
				IsDisposed = true;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the object has been disposed.
		/// </summary>
		public bool IsDisposed { get; private set; }

		public event Action<bool> DisposeEvent;

		public IntPtr Reference
		{
			get
			{
				if (IsDisposed)
					throw new ObjectDisposedException("Reference", "Cannot access a disposed object.");
				return _reference;
			}
		}

		protected abstract void Dispose(bool disposing);

		public void Dispose()
		{
			if (!IsDisposed && !_inDispose)
			{
				_inDispose = true;
				DisposeEvent?.Invoke(true);
				Dispose(true);
				IsDisposed = true;
			}
		}

		protected void ForceDisposeState()
		{
			_inDispose = true;
			IsDisposed = true;
		}
	}
}
