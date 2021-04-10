﻿#region MIT License
/*Copyright (c) 2012-2013, 2015 Robert Rouhani <robert.rouhani@gmail.com>

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

namespace SharpFont
{
	/// <summary>
	/// A structure used for bitmap glyph images. This really is a ‘sub-class’ of <see cref="Glyph"/>.
	/// </summary>
	/// <remarks><para>
	/// You can typecast an <see cref="Glyph"/> to <see cref="BitmapGlyph"/> if you have ‘<see cref="Glyph.Format"/> ==
	/// <see cref="GlyphFormat.Bitmap"/>’. This lets you access the bitmap's contents easily.
	/// </para><para>
	/// The corresponding pixel buffer is always owned by <see cref="BitmapGlyph"/> and is thus created and destroyed
	/// with it.
	/// </para></remarks>
	public sealed class BitmapGlyph
	{
		#region Constructors

		internal BitmapGlyph(Glyph original)
		{
			Root = original;
		}

		#endregion

		#region Properties

		private ref BitmapGlyphRec Rec => ref PInvokeHelper.PtrToRefStructure<BitmapGlyphRec>(Root.Reference);

		/// <summary>
		/// Gets the root <see cref="Glyph"/> fields.
		/// </summary>
		public Glyph Root { get; }

		/// <summary>
		/// Gets the left-side bearing, i.e., the horizontal distance from the current pen position to the left border
		/// of the glyph bitmap.
		/// </summary>
		public int Left => Rec.left;

		/// <summary>
		/// Gets the top-side bearing, i.e., the vertical distance from the current pen position to the top border of
		/// the glyph bitmap. This distance is positive for upwards y!
		/// </summary>
		public int Top => Rec.top;

		/// <summary>
		/// Gets a descriptor for the bitmap.
		/// </summary>
		public FTBitmap Bitmap
		{
			get
			{
				unsafe
				{
					var rec = (BitmapGlyphRec*)Root.Reference;
					return new FTBitmap(new IntPtr(&rec->bitmap), null);
				}
			}
		}

		#endregion

		#region Operators

		/// <summary>
		/// Casts a <see cref="BitmapGlyph"/> back up to a <see cref="Glyph"/>. The eqivalent of
		/// <see cref="Root"/>.
		/// </summary>
		/// <param name="g">A <see cref="BitmapGlyph"/>.</param>
		/// <returns>A <see cref="Glyph"/>.</returns>
		public static implicit operator Glyph(BitmapGlyph g) =>
			g.Root;

		#endregion
	}
}
