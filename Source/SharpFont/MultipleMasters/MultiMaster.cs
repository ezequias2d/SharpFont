#region MIT License
/*Copyright (c) 2012-2013 Robert Rouhani <robert.rouhani@gmail.com>

SharpFont based on Tao.FreeType, Copyright (c) 2003-2007 Tao Framework Team

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/
#endregion

using System;
using System.Runtime.InteropServices;
using SharpFont.Internal;
using SharpFont.MultipleMasters.Internal;

namespace SharpFont.MultipleMasters
{
	/// <summary><para>
	/// A structure used to model the axes and space of a Multiple Masters font.
	/// </para><para>
	/// This structure can't be used for GX var fonts.
	/// </para></summary>
	public class MultiMaster : NativeObject
	{
		#region Constructors

		internal MultiMaster(IntPtr reference) : base(reference)
		{
		}

		#endregion

		#region Properties

		private ref MultiMasterRec Rec => ref PInvokeHelper.PtrToRefStructure<MultiMasterRec>(Reference);

		/// <summary>
		/// Gets the number of axes. Cannot exceed 4.
		/// </summary>
		public uint AxisCount => Rec.num_axis;

		/// <summary>
		/// Gets the number of designs; should be normally 2^num_axis even though the Type 1 specification strangely
		/// allows for intermediate designs to be present. This number cannot exceed 16.
		/// </summary>
		public uint DesignsCount => Rec.num_designs;

		/// <summary>
		/// Gets a table of axis descriptors.
		/// </summary>
		public ReadOnlySpan<MMAxis> Axis
		{
			get
			{
				unsafe
				{
					var ptr = (MultiMasterRec*)Reference;
					return new ReadOnlySpan<MMAxis>(&ptr->axis1, 4);
				}
			}
		}

		#endregion
	}
}
