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
	[StructLayout(LayoutKind.Sequential)]
	internal struct StreamRec
	{
		internal IntPtr @base;
		internal UIntPtr size;
		internal UIntPtr pos;

		internal StreamDesc descriptor;
		internal StreamDesc pathname;
		internal IntPtr read;
		internal IntPtr close;

		internal IntPtr memory;
		internal IntPtr cursor;
		internal IntPtr limit;

		public StreamIOFunc Read => Marshal.GetDelegateForFunctionPointer<StreamIOFunc>(read);
		public StreamCloseFunc Close => Marshal.GetDelegateForFunctionPointer<StreamCloseFunc>(close);
	}
}
