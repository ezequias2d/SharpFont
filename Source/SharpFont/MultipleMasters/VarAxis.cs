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
using SharpFont.Internal;
using SharpFont.MultipleMasters.Internal;

namespace SharpFont.MultipleMasters
{
	/// <summary>
	/// A simple structure used to model a given axis in design space for Multiple Masters and GX var fonts.
	/// </summary>
	public class VarAxis : NativeObject
	{
		#region Constructors

		internal VarAxis(IntPtr reference) : base(reference)
		{
		}

		#endregion

		#region Properties

		private VarAxisRec Rec => PInvokeHelper.PtrToRefStructure<VarAxisRec>(Reference);

		/// <summary>
		/// Gets the axis's name. Not always meaningful for GX.
		/// </summary>
		public string Name => Rec.Name;

		/// <summary>
		/// Gets the axis's minimum design coordinate.
		/// </summary>
		public int Minimum => (int)Rec.minimum;

		/// <summary>
		/// Gets the axis's default design coordinate. FreeType computes meaningful default values for MM; it is then
		/// an integer value, not in 16.16 format.
		/// </summary>
		public int Default => (int)Rec.def;

		/// <summary>
		/// Gets the axis's maximum design coordinate.
		/// </summary>
		public int Maximum => (int)Rec.maximum;

		/// <summary>
		/// Gets the axis's tag (the GX equivalent to ‘name’). FreeType provides default values for MM if possible.
		/// </summary>
		public uint Tag => (uint)Rec.tag;

		/// <summary>
		/// Gets the entry in ‘name’ table (another GX version of ‘name’). Not meaningful for MM.
		/// </summary>
		public uint StrId => Rec.strid;

		#endregion
	}
}
