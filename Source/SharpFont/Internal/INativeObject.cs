using System;
using System.Collections.Generic;
using System.Text;

namespace SharpFont.Internal
{
	/// <summary>
	/// An interface that describes that the implementation has a reference to an area of unmanaged memory as an object of a native library.
	/// </summary>
	public interface INativeObject
	{
		/// <summary>
		/// A reference to the native object.
		/// </summary>
		IntPtr Reference { get; }
	}
}
