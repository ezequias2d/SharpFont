﻿#region MIT License
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

using SharpFont.Fnt.Internal;
using SharpFont.Internal;

namespace SharpFont.Fnt
{
	/// <summary>
	/// Describes the general appearance of the font.
	/// </summary>
	public enum Family
	{
		/// <summary>
		/// Don't care or don't know which family.
		/// </summary>
		DontCare = 0,

		/// <summary>
		/// The font has a Roman appearance.
		/// </summary>
		Roman = 1,

		/// <summary>
		/// The font has a Swiss appearance.
		/// </summary>
		Swiss = 2,

		/// <summary>
		/// The font has a Modern appearance.
		/// </summary>
		Modern = 3,

		/// <summary>
		/// The font has a script-like appearance.
		/// </summary>
		Script = 4,

		/// <summary>
		/// The font is decorative.
		/// </summary>
		Decorative = 5
	}

	/// <summary>
	/// Provides flags for font proportions and color.
	/// </summary>
	public enum Flags : ushort
	{
		/// <summary>
		/// Font is fixed.
		/// </summary>
		Fixed = 1 << 0,

		/// <summary>
		/// Font is proportional.
		/// </summary>
		Proportional = 1 << 1,

		/// <summary>
		/// Font is ABC fixed.
		/// </summary>
		AbcFixed = 1 << 2,

		/// <summary>
		/// Font is ABC proportional.
		/// </summary>
		AbcProportional = 1 << 3,

		/// <summary>
		/// Font is 2-bit color.
		/// </summary>
		Color1 = 1 << 4,

		/// <summary>
		/// Font is 4-bit color.
		/// </summary>
		Color16 = 1 << 5,

		/// <summary>
		/// Font is 8-bit color.
		/// </summary>
		Color256 = 1 << 6,

		/// <summary>
		/// Font is RGB color.
		/// </summary>
		RgbColor = 1 << 7
	}

	/// <summary>
	/// Windows FNT Header info.
	/// </summary>
	public class Header : NativeObject
	{

		#region Constructors

		internal Header(IntPtr reference) : base(reference)
		{
		}

		#endregion

		#region Properties

		private ref HeaderRec Rec => ref PInvokeHelper.PtrToRefStructure<HeaderRec>(Reference);

		/// <summary>
		/// Gets the version format of the file (e.g. 0x0200).
		/// </summary>
		public ushort Version => Rec.version;

		/// <summary>
		/// Gets the size of the file in bytes.
		/// </summary>
		public uint FileSize => (uint)Rec.file_size;

		/// <summary>
		/// Gets the copyright text.
		/// Limited to 60 bytes.
		/// </summary>
		public ReadOnlySpan<byte> Copyright
		{
			get
			{
				unsafe
				{
					var ptr = (HeaderRec*)Reference;
					return new ReadOnlySpan<byte>(&ptr->copyright, HeaderRec.CopyrightSize);
				}
			}
		}

		/// <summary>
		/// Gets the filetype (vector or bitmap). This is exclusively for GDI use.
		/// </summary>
		public ushort FileType => Rec.file_type;

		/// <summary>
		/// Gets the nominal point size determined by the designer at which the font looks
		/// best.
		/// </summary>
		public ushort NominalPointSize => Rec.nominal_point_size;

		/// <summary>
		/// Gets the nominal vertical resolution in dots per inch.
		/// </summary>
		public ushort VerticalResolution => Rec.vertical_resolution;

		/// <summary>
		/// Gets the nominal horizontal resolution in dots per inch.
		/// </summary>
		public ushort HorizontalResolution => Rec.horizontal_resolution;

		/// <summary>
		/// Gets the height of the font's ascent from the baseline.
		/// </summary>
		public ushort Ascent => Rec.ascent;

		/// <summary>
		/// Gets the amount of leading inside the bounds of <see cref="PixelHeight"/>.
		/// </summary>
		public ushort InternalLeading => Rec.internal_leading;

		/// <summary>
		/// Gets the amount of leading the designer recommends to be added between
		/// rows.
		/// </summary>
		public ushort ExternalLeading => Rec.external_leading;

		/// <summary>
		/// Gets whether the font is italic.
		/// </summary>
		public bool Italic => (0x01 & Rec.italic) == 0x01;

		/// <summary>
		/// Ges whether the font includes underlining.
		/// </summary>
		public bool Underline => (0x01 & Rec.underline) == 0x01;

		/// <summary>
		/// Ges whether the font includes strikeout.
		/// </summary>
		public bool Strikeout =>(0x01 & Rec.strike_out) == 0x01;

		/// <summary>
		/// Gets the weight of characters on a scale of 1 to 1000, with
		/// 400 being regular weight.
		/// </summary>
		public ushort Weight => Rec.weight;

		/// <summary>
		/// Gets the character set specified by the font.
		/// </summary>
		public byte Charset => Rec.charset;

		/// <summary>
		/// Gets the width of the vector grid (vector fonts). For raster fonts,
		/// a zero value indicates that characters have variables widths,
		/// otherwise, the value is the width of the bitmap for all characters.
		/// </summary>
		public ushort PixelWidth => Rec.pixel_width;

		/// <summary>
		/// Gets the height of the vector grid (vector fonts) or the height
		/// of the bitmap for all characters (raster fonts).
		/// </summary>
		public ushort PixelHeight => Rec.pixel_height;

		/// <summary>
		/// Gets whether the font is variable pitch.
		/// </summary>
		public byte PitchAndFamily => Rec.pitch_and_family;

		/// <summary>
		/// Gets the width of characters in the font, based on the width of 'X'.
		/// </summary>
		public ushort AverageWidth => Rec.avg_width;

		/// <summary>
		/// Gets the maximum width of all characters in the font.
		/// </summary>
		public ushort MaximumWidth => Rec.max_width;

		/// <summary>
		/// Gets the first character code specified in the font.
		/// </summary>
		public byte FirstChar => Rec.first_char;

		/// <summary>
		/// Gets the last character code specified in the font.
		/// </summary>
		public byte LastChar => Rec.last_char;

		/// <summary>
		/// Gets the character to substitute when a character is needed that
		/// isn't defined in the font.
		/// </summary>
		public byte DefaultChar => Rec.default_char;

		/// <summary>
		/// Gets the character that defines word breaks, for purposes of word
		/// wrapping and word spacing justification. This value is relative to
		/// the <see cref="FirstChar"/>, so the character code is this value
		/// minus <see cref="FirstChar"/>.
		/// </summary>
		public byte BreakChar => Rec.break_char;

		/// <summary>
		/// Gets the number of bytes in each row of the bitmap (raster fonts).
		/// </summary>
		public ushort BytesPerRow => Rec.bytes_per_row;

		/// <summary>
		/// Gets the offset in the file, in bytes, to the string that gives the device name.
		/// The value is 0 for generic fonts.
		/// </summary>
		public uint DeviceOffset => (uint)Rec.device_offset;

		/// <summary>
		/// Gets the offset in the file, in bytes, to the string that gives the face name
		/// (null-terminated).
		/// </summary>
		public uint FaceNameOffset => (uint)Rec.face_name_offset;

		/// <summary>
		/// Gets the absolute machine address of the bitmap,
		/// which is set by GDI at load time.
		/// </summary>
		public uint BitsPointer => (uint)Rec.bits_pointer;

		/// <summary>
		/// Gets the offset in the file, in bytes, to the beginning of the character data
		/// (raster or vector).
		/// </summary>
		public uint BitsOffset => (uint)Rec.bits_offset;

		/// <summary>
		/// Reservied.
		/// </summary>
		public byte Reserved => Rec.reserved;

		/// <summary>
		/// Gets <see cref="Flags"/> that describe font proportion and color.
		/// </summary>
		public Flags Flags => (Flags)Rec.flags;

		/// <summary>
		/// ASpace has not been used since before Windows 3.0.
		/// Set it to 0 for compatibility.
		/// </summary>
		public ushort ASpace => Rec.A_space;

		/// <summary>
		/// BSpace has not been used since before Windows 3.0.
		/// Set it to 0 for compatibility.
		/// </summary>
		public ushort BSpace => Rec.B_space;

		/// <summary>
		/// CSpace has not been used since before Windows 3.0.
		/// Set it to 0 for compatibility.
		/// </summary>
		public ushort CSpace => Rec.C_space;

		/// <summary>
		/// Gets the offset of the color table.
		/// </summary>
		public ushort ColorTableOffset => Rec.color_table_offset;

		/// <summary>
		/// This field is reserved.
		/// </summary>
		public ReadOnlySpan<IntPtr> Reserved1
		{
			get
			{
				unsafe
				{
					var ptr = (HeaderRec*)Reference;
					return new ReadOnlySpan<IntPtr>(&ptr->reserved1, 4);
				}
			}
		}

		#endregion
	}
}
