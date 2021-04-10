#region MIT License
/*Copyright (c) 2012-2013, 2016 Robert Rouhani <robert.rouhani@gmail.com>

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
using SharpFont.PostScript.Internal;

namespace SharpFont.PostScript
{
	/// <summary>
	/// A structure used to represent data in a CID top-level dictionary.
	/// </summary>
	public class FaceDict : NativeObject
	{
		#region Constructors

		internal FaceDict(IntPtr reference) : base(reference)
		{
		}

		#endregion

		#region Properties

		private ref FaceDictRec Rec => ref PInvokeHelper.PtrToRefStructure<FaceDictRec>(Reference);

		/// <summary>
		/// Gets the Private structure containing more information.
		/// </summary>
		public Private PrivateDictionary => Rec.private_dict;

		/// <summary>
		/// Gets the length of the BuildChar entry.
		/// </summary>
		public uint BuildCharLength => Rec.len_buildchar;

		/// <summary>
		/// Gets whether to force bold characters when a regular character has
		/// strokes drawn 1-pixel wide.
		/// </summary>
		public int ForceBoldThreshold => (int)Rec.forcebold_threshold;

		/// <summary>
		/// Gets the width of stroke.
		/// </summary>
		public int StrokeWidth => (int)Rec.stroke_width;

		/// <summary>
		/// Gets hinting useful for rendering glyphs such as barcodes and logos that
		/// have many counters.
		/// </summary>
		public int ExpansionFactor => (int)Rec.expansion_factor;

		/// <summary>
		/// Gets the method for painting strokes (fill or outline).
		/// </summary>
		public byte PaintType => Rec.paint_type;

		/// <summary>
		/// Gets the type of font. Must be set to 1 for all Type 1 fonts.
		/// </summary>
		public byte FontType => Rec.font_type;

		/// <summary>
		/// Gets the matrix that indicates scaling of space units.
		/// </summary>
		public FTMatrix FontMatrix => Rec.font_matrix;

		/// <summary>
		/// Gets the offset of the font.
		/// </summary>
		public FTVector FontOffset => Rec.font_offset;

		/// <summary>
		/// Gets the number of subroutines.
		/// </summary>
		public uint SubrsCount => Rec.num_subrs;

		/// <summary>
		/// Gets the offset in bytes, from the start of the
		/// data section of the CIDFont to the beginning of the SubrMap.
		/// </summary>
		public uint SubrmapOffset => (uint)Rec.subrmap_offset;

		/// <summary>
		/// Gets the number of bytes needed to store the SD value.
		/// </summary>
		public int SDBytes => Rec.sd_bytes;

		#endregion
	}
}
