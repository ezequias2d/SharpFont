#region MIT License
/*Copyright (c) 2012-2014 Robert Rouhani <robert.rouhani@gmail.com>

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
	/// <summary><para>
	/// A structure used to model a TrueType OS/2 table. This is the long table version. All fields comply to the
	/// TrueType specification.
	/// </para><para>
	/// Note that we now support old Mac fonts which do not include an OS/2 table. In this case, the ‘version’ field is
	/// always set to 0xFFFF.
	/// </para></summary>
	public class OS2 : NativeObject
	{

		#region Constructors

		internal OS2(IntPtr reference) :base(reference)
		{
		}

		#endregion

		#region Properties

		private ref OS2Rec Rec => ref PInvokeHelper.PtrToRefStructure<OS2Rec>(Reference);

		/// <summary>
		/// The version of this table.
		/// </summary>
		public ushort Version => Rec.version;

		/// <summary>
		/// The average glyph width, computed by averaging ALL non-zero width glyphs in the font, in pels/em.
		/// </summary>
		public short AverageCharWidth => Rec.xAvgCharWidth;

		/// <summary>
		/// The visual weight of the font.
		/// </summary>
		public ushort WeightClass => Rec.usWeightClass;

		/// <summary>
		/// The relative change in width from the normal aspect ratio.
		/// </summary>
		public ushort WidthClass => Rec.usWidthClass;

		/// <summary>
		/// Font embedding and subsetting licensing rights as determined by the font author.
		/// </summary>
		public EmbeddingTypes EmbeddingType => Rec.fsType;

		/// <summary>
		/// The font author's recommendation for sizing glyphs (em square) to create subscripts when a glyph doesn't exist for a subscript.
		/// </summary>
		public short SubscriptSizeX => Rec.ySubscriptXSize;

		/// <summary>
		/// The font author's recommendation for sizing glyphs (em height) to create subscripts when a glyph doesn't exist for a subscript.
		/// </summary>
		public short SubscriptSizeY => Rec.ySubscriptYSize;

		/// <summary>
		/// The font author's recommendation for vertically positioning subscripts that are created when a glyph doesn't exist for a subscript.
		/// </summary>
		public short SubscriptOffsetX => Rec.ySubscriptXOffset;

		/// <summary>
		/// The font author's recommendation for horizontally positioning subscripts that are created when a glyph doesn't exist for a subscript.
		/// </summary>
		public short SubscriptOffsetY => Rec.ySubscriptYOffset;

		/// <summary>
		/// The font author's recommendation for sizing glyphs (em square) to create superscripts when a glyph doesn't exist for a subscript.
		/// </summary>
		public short SuperscriptSizeX => Rec.ySuperscriptXSize;

		/// <summary>
		/// The font author's recommendation for sizing glyphs (em height) to create superscripts when a glyph doesn't exist for a subscript.
		/// </summary>
		public short SuperscriptSizeY => Rec.ySuperscriptYSize;

		/// <summary>
		/// The font author's recommendation for vertically positioning superscripts that are created when a glyph doesn't exist for a subscript.
		/// </summary>
		public short SuperscriptOffsetX => Rec.ySuperscriptXOffset;

		/// <summary>
		/// The font author's recommendation for horizontally positioning superscripts that are created when a glyph doesn't exist for a subscript.
		/// </summary>
		public short SuperscriptOffsetY => Rec.ySuperscriptYOffset;

		/// <summary>
		/// The thickness of the strikeout stroke.
		/// </summary>
		public short StrikeoutSize => Rec.yStrikeoutSize;

		/// <summary>
		/// The position of the top of the strikeout line relative to the baseline.
		/// </summary>
		public short StrikeoutPosition => Rec.yStrikeoutPosition;

		/// <summary>
		/// The IBM font family class and subclass, useful for choosing visually similar fonts.
		/// </summary>
		/// <remarks>Refer to https://www.microsoft.com/typography/otspec160/ibmfc.htm. </remarks>
		public short FamilyClass => Rec.sFamilyClass;

		//TODO write a PANOSE class from TrueType spec?
		/// <summary>
		/// The Panose values describe visual characteristics of the font.
		/// Similar fonts can then be selected based on their Panose values.
		/// </summary>
		public ReadOnlySpan<byte> Panose => Rec.Panose;

		/// <summary>
		/// Unicode character range, bits 0-31.
		/// </summary>
		public uint UnicodeRange1 => (uint)Rec.ulUnicodeRange1;

		/// <summary>
		/// Unicode character range, bits 32-63.
		/// </summary>
		public uint UnicodeRange2 => (uint)Rec.ulUnicodeRange2;

		/// <summary>
		/// Unicode character range, bits 64-95.
		/// </summary>
		public uint UnicodeRange3 => (uint)Rec.ulUnicodeRange3;

		/// <summary>
		/// Unicode character range, bits 96-127.
		/// </summary>
		public uint UnicodeRange4 => (uint)Rec.ulUnicodeRange4;

		/// <summary>
		/// The vendor's identifier.
		/// </summary>
		public ReadOnlySpan<byte> VendorId => Rec.AchVendID;

		/// <summary>
		/// Describes variations in the font.
		/// </summary>
		public ushort SelectionFlags => Rec.fsSelection;

		/// <summary>
		/// The minimum Unicode index (character code) in this font.
		/// Since this value is limited to 0xFFFF, applications should not use this field.
		/// </summary>
		public ushort FirstCharIndex => Rec.usFirstCharIndex;

		/// <summary>
		/// The maximum Unicode index (character code) in this font.
		/// Since this value is limited to 0xFFFF, applications should not use this field.
		/// </summary>
		public ushort LastCharIndex => Rec.usLastCharIndex;
		
		/// <summary>
		/// The ascender value, useful for computing a default line spacing in conjunction with unitsPerEm.
		/// </summary>
		public short TypographicAscender => Rec.sTypoAscender;
		
		/// <summary>
		/// The descender value, useful for computing a default line spacing in conjunction with unitsPerEm.
		/// </summary>
		public short TypographicDescender => Rec.sTypoDescender;
		
		/// <summary>
		/// The line gap value, useful for computing a default line spacing in conjunction with unitsPerEm.
		/// </summary>
		public short TypographicLineGap => Rec.sTypoLineGap;

		/// <summary>
		/// The ascender metric for Windows, usually set to yMax. Windows will clip glyphs that go above this value.
		/// </summary>
		public ushort WindowsAscent => Rec.usWinAscent;

		/// <summary>
		/// The descender metric for Windows, usually set to yMin. Windows will clip glyphs that go below this value.
		/// </summary>
		public ushort WindowsDescent => Rec.usWinDescent;

		/// <summary>
		/// Specifies the code pages encompassed by this font.
		/// </summary>
		public uint CodePageRange1 => (uint)Rec.ulCodePageRange1;

		/// <summary>
		/// Specifies the code pages encompassed by this font.
		/// </summary>
		public uint CodePageRange2 => (uint)Rec.ulUnicodeRange1;

		/// <summary>
		/// The approximate height of non-ascending lowercase letters relative to the baseline.
		/// </summary>
		public short Height => Rec.sxHeight;
		
		/// <summary>
		/// The approximate height of uppercase letters relative to the baseline.
		/// </summary>
		public short CapHeight => Rec.sCapHeight;

		/// <summary>
		/// The Unicode index (character code)  of the glyph to use when a glyph doesn't exist for the requested character.
		/// Since this value is limited to 0xFFFF, applications should not use this field.
		/// </summary>
		public ushort DefaultChar => Rec.usDefaultChar;

		/// <summary>
		/// The Unicode index (character code)  of the glyph to use as the break character.
		/// The 'space' character is normally the break character.
		/// Since this value is limited to 0xFFFF, applications should not use this field.
		/// </summary>
		public ushort BreakChar => Rec.usBreakChar;

		/// <summary>
		/// The maximum number of characters needed to determine glyph context when applying features such as ligatures.
		/// </summary>
		public ushort MaxContext => Rec.usMaxContext;

		/// <summary>
		/// The lowest point size at which the font starts to be used, in twips.
		/// </summary>
		public ushort LowerOpticalPointSize => Rec.usLowerOpticalPointSize;

		/// <summary>
		/// The highest point size at which the font is no longer used, in twips.
		/// </summary>
		public ushort UpperOpticalPointSize => Rec.usUpperOpticalPointSize;

		#endregion
	}
}
