using System;

namespace SharpFont.Internal
{
	/// <summary>
	/// Provide a consistent means for using pointers as references.
	/// </summary>
	public abstract class NativeObject : INativeObject
	{
		/// <summary>
		/// Construct a new NativeObject and assign the reference.
		/// </summary>
		/// <param name="reference"></param>
		protected NativeObject(IntPtr reference)
		{
			Reference = reference;
		}

		/// <summary>
		/// A reference to the native object.
		/// </summary>
		public IntPtr Reference { get; }
	}
}
