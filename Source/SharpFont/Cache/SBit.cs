﻿#region MIT License
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

using SharpFont.Cache.Internal;
using SharpFont.Internal;

namespace SharpFont.Cache
{
	/// <summary>
	/// A handle to a small bitmap cache. These are special cache objects used to store small glyph bitmaps (and
	/// anti-aliased pixmaps) in a much more efficient way than the traditional glyph image cache implemented by
	/// <see cref="ImageCache"/>.
	/// </summary>
	public class SBit : NativeObject
	{

		#region Constructors

		internal SBit(IntPtr reference) : base(reference)
		{
		}

		#endregion

		#region Properties
		private ref SBitRec Rec => ref PInvokeHelper.PtrToRefStructure<SBitRec>(Reference);

		/// <summary>
		/// Gets the bitmap width in pixels.
		/// </summary>
		public byte Width => Rec.width;

		/// <summary>
		/// Gets the bitmap height in pixels.
		/// </summary>
		public byte Height => Rec.height;

		/// <summary>
		/// Gets the horizontal distance from the pen position to the left bitmap border (a.k.a. ‘left side bearing’,
		/// or ‘lsb’).
		/// </summary>
		public byte Left => Rec.left;

		/// <summary>
		/// Gets the vertical distance from the pen position (on the baseline) to the upper bitmap border (a.k.a. ‘top
		/// side bearing’). The distance is positive for upwards y coordinates.
		/// </summary>
		public byte Top => Rec.top;

		/// <summary>
		/// Gets the format of the glyph bitmap (monochrome or gray).
		/// </summary>
		public byte Format => Rec.format;

		/// <summary>
		/// Gets the maximum gray level value (in the range 1 to 255).
		/// </summary>
		public byte MaxGrays => Rec.max_grays;

		/// <summary>
		/// Gets the number of bytes per bitmap line. May be positive or negative.
		/// </summary>
		public short Pitch => Rec.pitch;

		/// <summary>
		/// Gets the horizontal advance width in pixels.
		/// </summary>
		public byte AdvanceX => Rec.xadvance;

		/// <summary>
		/// Gets the vertical advance height in pixels.
		/// </summary>
		public byte AdvanceY => Rec.yadvance;

		/// <summary>
		/// Gets a pointer to the bitmap pixels.
		/// </summary>
		public IntPtr Buffer => Rec.buffer;

		#endregion
	}
}
