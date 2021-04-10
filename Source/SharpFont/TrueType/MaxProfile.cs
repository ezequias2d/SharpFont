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
using SharpFont.TrueType.Internal;

namespace SharpFont.TrueType
{
	/// <summary>
	/// The maximum profile is a table containing many max values which can be used to pre-allocate arrays. This
	/// ensures that no memory allocation occurs during a glyph load.
	/// </summary>
	/// <remarks>
	/// This structure is only used during font loading.
	/// </remarks>
	public class MaxProfile : NativeObject
	{

		#region Constructors

		internal MaxProfile(IntPtr reference) : base(reference)
		{
		}

		#endregion

		#region Properties
		private ref MaxProfileRec Rec => ref PInvokeHelper.PtrToRefStructure<MaxProfileRec>(Reference);

		/// <summary>
		/// Gets the version number.
		/// </summary>
		public int Version => (int)Rec.version;
		
		/// <summary>
		/// Gets the number of glyphs in this TrueType font.
		/// </summary>
		public ushort GlyphCount => Rec.numGlyphs;

		/// <summary>
		/// Gets the maximum number of points in a non-composite TrueType glyph. See also the structure element
		/// ‘maxCompositePoints’.
		/// </summary>
		public ushort MaxPoints => Rec.maxPoints;

		/// <summary>
		/// Gets the maximum number of contours in a non-composite TrueType glyph. See also the structure element
		/// ‘maxCompositeContours’.
		/// </summary>
		public ushort MaxContours => Rec.maxContours;

		/// <summary>
		/// Gets the maximum number of points in a composite TrueType glyph. See also the structure element
		/// ‘maxPoints’.
		/// </summary>
		public ushort MaxCompositePoints => Rec.maxCompositePoints;

		/// <summary>
		/// Gets the maximum number of contours in a composite TrueType glyph. See also the structure element
		/// ‘maxContours’.
		/// </summary>
		public ushort MaxCompositeContours => Rec.maxCompositeContours;

		/// <summary>
		/// Gets the maximum number of zones used for glyph hinting.
		/// </summary>
		public ushort MaxZones => Rec.maxZones;

		/// <summary>
		/// Gets the maximum number of points in the twilight zone used for glyph hinting.
		/// </summary>
		public ushort MaxTwilightPoints => Rec.maxTwilightPoints;

		/// <summary>
		/// Gets the maximum number of elements in the storage area used for glyph hinting.
		/// </summary>
		public ushort MaxStorage => Rec.maxStorage;

		/// <summary>
		/// Gets the maximum number of function definitions in the TrueType bytecode for this font.
		/// </summary>
		public ushort MaxFunctionDefs => Rec.maxFunctionDefs;

		/// <summary>
		/// Gets the maximum number of instruction definitions in the TrueType bytecode for this font.
		/// </summary>
		public ushort MaxInstructionDefs => Rec.maxInstructionDefs;

		/// <summary>
		/// Gets the maximum number of stack elements used during bytecode interpretation.
		/// </summary>
		public ushort MaxStackElements => Rec.maxStackElements;

		/// <summary>
		/// Gets the maximum number of TrueType opcodes used for glyph hinting.
		/// </summary>
		public ushort MaxSizeOfInstructions => Rec.maxSizeOfInstructions;

		/// <summary>
		/// Gets the maximum number of simple (i.e., non- composite) glyphs in a composite glyph.
		/// </summary>
		public ushort MaxComponentElements => Rec.maxComponentElements;

		/// <summary>
		/// Gets the maximum nesting depth of composite glyphs.
		/// </summary>
		public ushort MaxComponentDepth => Rec.maxComponentDepth;

		#endregion
	}
}
