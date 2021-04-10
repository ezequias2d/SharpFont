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

using SharpFont.PostScript.Internal;

namespace SharpFont.PostScript
{
	/// <summary>
	/// A structure used to model a Type 1 or Type 2 private dictionary. Note that for Multiple Master fonts, each
	/// instance has its own Private dictionary.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct Private
	{
		private const int BlueValuesFixedCount = 14;
		private const int OtherBluesFixedCount = 10;
		private const int FamilyBluesFixedCount = 14;
		private const int FamilyOtherBluesFixedCount = 10;

		private const int SnapWidthsFixedCount = 13;
		private const int SnapHeightsFixedCount = 13;

		private const int MinFeatureFixedCount = 2;

		#region Fields
		internal int unique_id;
		internal int lenIV;

		internal byte num_blue_values;
		internal byte num_other_blues;
		internal byte num_family_blues;
		internal byte num_family_other_blues;

		internal fixed short blue_values[BlueValuesFixedCount];
		internal fixed short other_blues[OtherBluesFixedCount];
		internal fixed short family_blues[FamilyBluesFixedCount];
		internal fixed short family_other_blues[FamilyOtherBluesFixedCount];

		internal IntPtr blue_scale;
		internal int blue_shift;
		internal int blue_fuzz;

		internal ushort standard_width;
		internal ushort standard_height;

		internal byte num_snap_widths;
		internal byte num_snap_heights;
		internal byte force_bold;
		internal byte round_stem_up;

		internal fixed short snap_widths[SnapWidthsFixedCount];
		internal fixed short snap_heights[SnapHeightsFixedCount];

		internal IntPtr expansion_factor;

		internal IntPtr language_group;
		internal IntPtr password;

		internal fixed short min_feature[MinFeatureFixedCount];
		#endregion
		#region Properties

		/// <summary>
		/// Gets the ID unique to the Type 1 font.
		/// </summary>
		public int UniqueId => unique_id;

		/// <summary>
		/// Gets the number of random bytes at the beginning of charstrings (for encryption).
		/// </summary>
		public int LenIV => lenIV;

		/// <summary>
		/// Gets the number of values (pairs) in the Blues array.
		/// </summary>
		public byte BlueValuesCount => num_blue_values;

		/// <summary>
		/// Gets the number of values (pairs) in the OtherBlues array.
		/// </summary>
		public byte OtherBluesCount => num_other_blues;

		/// <summary>
		/// Gets the number of values (pairs) in the FamilyBlues array.
		/// </summary>
		public byte FamilyBluesCount => num_family_blues;

		/// <summary>
		/// Gets the number of values (pairs) in the FamilyOtherBlues array.
		/// </summary>
		public byte FamilyOtherBluesCount => num_family_other_blues;

		/// <summary>
		/// Gets the pairs of blue values.
		/// </summary>
		public ReadOnlySpan<short> BlueValues
		{
			get
			{
				fixed(void* ptr = blue_values)
					return new ReadOnlySpan<short>(ptr, BlueValuesFixedCount).ToArray();
			}
		}

		/// <summary>
		/// Gets the pairs of blue values.
		/// </summary>
		public short[] OtherBlues
		{
			get
			{
				fixed (void* ptr = other_blues)
					return new ReadOnlySpan<short>(ptr, OtherBluesFixedCount).ToArray();
			}
		}

		/// <summary>
		/// Gets the pairs of blue values.
		/// </summary>
		public short[] FamilyBlues
		{
			get
			{
				fixed (void* ptr = family_blues)
					return new ReadOnlySpan<short>(ptr, FamilyBluesFixedCount).ToArray();
			}
		}

		/// <summary>
		/// Gets the pairs of blue values.
		/// </summary>
		public short[] FamilyOtherBlues
		{
			get
			{
				fixed (void* ptr = family_other_blues)
					return new ReadOnlySpan<short>(ptr, FamilyOtherBluesFixedCount).ToArray();
			}
		}

		/// <summary>
		/// Gets the point size at which overshoot suppression ceases.
		/// </summary>
		public int BlueScale => (int)blue_scale;

		/// <summary>
		/// Gets whether characters smaller than the size given by BlueScale
		/// should have overshoots suppressed.
		/// </summary>
		public int BlueShift => blue_shift;

		/// <summary>
		/// Gets the number of character space units to extend the effect of an
		/// alignment zone on a horizontal stem. Setting this to 0 is recommended
		/// because it is unreliable.
		/// </summary>
		public int BlueFuzz => blue_fuzz;

		/// <summary>
		/// Indicates the standard stroke width of vertical stems.
		/// </summary>
		public ushort StandardWidth => standard_width;

		/// <summary>
		/// Indicates the standard stroke width of horizontal stems.
		/// </summary>
		public ushort StandardHeight => standard_height;

		/// <summary>
		/// Indicates the number of values in the SnapWidths array.
		/// </summary>
		public byte SnapWidthsCount => num_snap_widths;

		/// <summary>
		/// Indicates the number of values in the SnapHeights array.
		/// </summary>
		public byte SnapHeightsCount => num_snap_heights;

		/// <summary>
		/// Gets whether bold characters should appear thicker than non-bold characters
		/// at very small point sizes, where otherwise bold characters might appear the
		/// same as non-bold characters.
		/// </summary>
		public bool ForceBold => force_bold == 1;

		/// <summary>
		/// Superseded by the LanguageGroup entry.
		/// </summary>
		public bool RoundStemUp => round_stem_up == 1;

		/// <summary>
		/// StemSnapH is an array of up to 12 values of the most common stroke widths for horizontal stems
		/// (measured vertically).
		/// </summary>
		public short[] SnapWidths
		{
			get
			{
				fixed (void* ptr = snap_widths)
					return new ReadOnlySpan<short>(ptr, SnapWidthsFixedCount).ToArray();
			}
		}

		/// <summary>
		/// StemSnapV is an array of up to 12 values of the most common stroke widths for vertical stems
		/// (measured horizontally).
		/// </summary>
		public short[] SnapHeights
		{
			get
			{
				fixed (void* ptr = snap_heights)
					return new ReadOnlySpan<short>(ptr, SnapHeightsFixedCount).ToArray();
			}
		}

		/// <summary>
		/// The Expansion Factor provides a limit for changing character bounding boxes during
		/// processing that adjusts the size of fonts of Language Group 1.
		/// </summary>
		public int ExpansionFactor => (int)expansion_factor;

		/// <summary>
		/// Indicates the aesthetic characteristics of the font. Currently, only LanguageGroup 0
		/// (e.g. Latin, Greek, Cyrillic, etc.) and LanguageGroup 1 (e.g. Chinese ideographs, Japanese
		/// Kanji, etc) are recognized.
		/// </summary>
		public int LanguageGroup => (int)language_group;

		/// <summary>
		/// The Password value is required for the Type 1 BuildChar to operate.
		/// It must be set to 5839.
		/// </summary>
		public int Password => (int)password;

		/// <summary>
		/// The MinFeature value is required for the Type 1 BuildChar to operate, but is obsolete.
		/// It must be set to {16,16}.
		/// </summary>
		public ReadOnlySpan<short> MinFeature
		{
			get
			{
				fixed (void* ptr = min_feature)
					return new ReadOnlySpan<short>(ptr, MinFeatureFixedCount).ToArray();
			}
		}

		#endregion
	}
}
