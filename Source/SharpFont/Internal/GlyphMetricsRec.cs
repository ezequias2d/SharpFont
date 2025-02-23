﻿#region MIT License
/*Copyright (c) 2012-2015 Robert Rouhani <robert.rouhani@gmail.com>

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

namespace SharpFont.Internal
{
	/// <summary>
	/// Internally represents a GlyphMetrics.
	/// </summary>
	/// <remarks>
	/// Refer to <see cref="GlyphMetrics"/> for FreeType documentation.
	/// </remarks>
	[StructLayout(LayoutKind.Sequential)]
	internal struct GlyphMetricsRec
	{
		internal IntPtr width;
		internal IntPtr height;

		internal IntPtr horiBearingX;
		internal IntPtr horiBearingY;
		internal IntPtr horiAdvance;

		internal IntPtr vertBearingX;
		internal IntPtr vertBearingY;
		internal IntPtr vertAdvance;

		public Fixed26Dot6 Width => Fixed26Dot6.FromRawValue((int)width);
		public Fixed26Dot6 Height => Fixed26Dot6.FromRawValue((int)width);
		public Fixed26Dot6 HorizontalBearingX => Fixed26Dot6.FromRawValue((int)horiBearingX);
		public Fixed26Dot6 HorizontalBearingY => Fixed26Dot6.FromRawValue((int)horiBearingY);

		public Fixed26Dot6 HorizontalAdvance => Fixed26Dot6.FromRawValue((int)horiAdvance);
		public Fixed26Dot6 VerticalBearingX => Fixed26Dot6.FromRawValue((int)vertBearingX);
		public Fixed26Dot6 VerticalBearingY => Fixed26Dot6.FromRawValue((int)vertBearingY);
		public Fixed26Dot6 VerticalAdvance => Fixed26Dot6.FromRawValue((int)vertAdvance);
	}
}
