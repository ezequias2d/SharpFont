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
	/// A structure used to model a TrueType PCLT table. All fields comply to the TrueType specification.
	/// </summary>
	public class Pclt : NativeObject
	{

		#region Constructors

		internal Pclt(IntPtr reference) : base(reference)
		{
		}

		#endregion

		#region Properties

		private ref PCLTRec Rec => ref PInvokeHelper.PtrToRefStructure<PCLTRec>(Reference);

		/// <summary>
		/// The version number of this table. Version 1.0 is represented as 0x00010000.
		/// </summary>
		public int Version => (int)Rec.Version;
		
		/// <summary>
		/// A unique identifier for the font. Refer to the specification for the meaning of various bits.
		/// </summary>
		public uint FontNumber => (uint)Rec.FontNumber;

		/// <summary>
		/// The width of the space character, in FUnits (see UnitsPerEm in the head table).
		/// </summary>
		public ushort Pitch => Rec.Pitch;

		/// <summary>
		/// The height of the optical height of the lowercase 'x' in FUnits.
		/// This is a separate value from its height measurement.
		/// </summary>
		public ushort Height => Rec.xHeight;

		/// <summary>
		/// Describes structural appearance and effects of letterforms.
		/// </summary>
		public ushort Style => Rec.Style;

		/// <summary>
		/// Encodes the font vendor code and font family code into 16 bits.
		/// Refer to the spec for details.
		/// </summary>
		public ushort TypeFamily => Rec.TypeFamily;

		/// <summary>
		/// The height of the optical height of the uppercase 'H' in FUnits.
		/// This is a separate value from its height measurement.
		/// </summary>
		public ushort CapHeight => Rec.CapHeight;

		/// <summary>
		/// Encodes the symbol set's number field and ID field.
		/// Refer to the spec for details.
		/// </summary>
		public ushort SymbolSet => Rec.SymbolSet;

		/// <summary>
		/// The name and style of the font. The names of fonts within a family should be identical and the
		/// style identifiers should be standardized: e.g., Bd, It, BdIt. Length is 16 bytes.
		/// </summary>
		public string Typeface => Rec.TypeFaceString;

		/// <summary>
		/// Identifies the symbol collections provided by the font. Length is 8 bytes.
		/// Refer to the spec for details.
		/// </summary>
		public ReadOnlySpan<byte> CharacterComplement
			=> Rec.CharacterComplement;

		/// <summary>
		/// A standardized filename of the font. Length is 6 bytes.
		/// Refer to the spec for details.
		/// </summary>
		public ReadOnlySpan<byte> FileName => Rec.FileName;

		/// <summary>
		/// Indicates the stroke weight. Valid values are in the range -7 to 7. Length is 1 byte.
		/// </summary>
		public byte StrokeWeight => Rec.StrokeWeight;
		
		/// <summary>
		/// Indicates the stroke weight. Valid values are in the range -5 to 5. Length is 1 byte.
		/// </summary>
		public byte WidthType => Rec.WidthType;
		
		/// <summary>
		/// Encodes the serif style. The top two bits indicate sans serif/monoline or serif/contrasting.
		/// Valid values for the lower 6 bits are in the range 0 to 12. Length is 1 byte.
		/// </summary>
		public byte SerifStyle => Rec.SerifStyle;
		
		/// <summary>
		/// Reserved. Set to 0.
		/// </summary>
		public byte Reserved => Rec.Reserved;

		#endregion
	}
}
