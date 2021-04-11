using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SharpFont.Internal
{
	/// <summary>
	/// a class that represents a reference to an area of unmanaged memory,
	/// such as an object from a native library, that can and should be discarded
	/// when it is no longer used or no longer accessible by any part of the program
	/// (destructor called by the <see cref="GC"/>).
	/// </summary>
	public abstract class DisposableNativeObject : INativeObject, IDisposable
	{
		private readonly IntPtr _reference;
		private bool _inDispose;

		/// <summary>
		/// Initializes a new instance with an immutable reference to memory.
		/// </summary>
		/// <param name="reference"></param>
		public DisposableNativeObject(IntPtr reference)
		{
			IsDisposed = false;
			_reference = reference;
		}

		/// <summary>
		/// Destructor responsible for correctly disposing the object, if it
		/// has not yet been disposed, thus calling <see cref="DisposeEvent(bool)"/> DisposeEvent (false) and
		/// Dispose (false) respectively.
		/// </summary>
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

		/// <summary>
		/// Event invoked just before <see cref="Dispose(bool)"/>
		/// </summary>
		public event Action<bool> DisposeEvent;

		/// <summary>
		/// A reference to a native object.
		/// </summary>
		public IntPtr Reference
		{
			get
			{
				if (IsDisposed)
					throw new ObjectDisposedException("Reference", "Cannot access a disposed object.");
				return _reference;
			}
		}

		/// <summary>
		/// If disposing equals to true, must be managed and unmanaged resources dispose,
		/// otherwise, only unmanaged resource can be disposed and you should not reference
		/// any other objects.
		/// </summary>
		/// <param name="disposing">True, if it was called directly by the user code, otherwise, false.</param>
		protected abstract void Dispose(bool disposing);

		/// <summary>
		/// Implement <see cref="IDisposable.Dispose"/>
		/// </summary>
		public void Dispose()
		{
			if (!IsDisposed && !_inDispose)
			{
				_inDispose = true;
				GC.SuppressFinalize(this);
				DisposeEvent?.Invoke(true);
				Dispose(true);
				IsDisposed = true;
			}
		}

		/// <summary>
		/// Forces the IsDiposed state of the object.
		/// </summary>
		protected void ForceDisposeState()
		{
			_inDispose = true;
			IsDisposed = true;
		}
	}
}
