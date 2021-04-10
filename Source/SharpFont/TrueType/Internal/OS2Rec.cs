#region MIT License
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

namespace SharpFont.TrueType.Internal
{
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct OS2Rec
	{
		internal ushort version;
		internal short xAvgCharWidth;
		internal ushort usWeightClass;
		internal ushort usWidthClass;
		internal EmbeddingTypes fsType;
		internal short ySubscriptXSize;
		internal short ySubscriptYSize;
		internal short ySubscriptXOffset;
		internal short ySubscriptYOffset;
		internal short ySuperscriptXSize;
		internal short ySuperscriptYSize;
		internal short ySuperscriptXOffset;
		internal short ySuperscriptYOffset;
		internal short yStrikeoutSize;
		internal short yStrikeoutPosition;
		internal short sFamilyClass;

		private fixed byte _panose[10];
		internal ReadOnlySpan<byte> Panose
		{
			get
			{
				unsafe
				{
					fixed (byte* ptr = _panose)
						return new ReadOnlySpan<byte>(ptr, 10).ToArray();
				}
			}
		}

		internal UIntPtr ulUnicodeRange1;
		internal UIntPtr ulUnicodeRange2;
		internal UIntPtr ulUnicodeRange3;
		internal UIntPtr ulUnicodeRange4;

		private fixed byte _achVendID[4];
		internal ReadOnlySpan<byte> AchVendID
		{
			get
			{
				unsafe
				{
					fixed (byte* ptr = _achVendID)
						return new ReadOnlySpan<byte>(ptr, 4).ToArray();
				}
			}
		}

		internal ushort fsSelection;
		internal ushort usFirstCharIndex;
		internal ushort usLastCharIndex;
		internal short sTypoAscender;
		internal short sTypoDescender;
		internal short sTypoLineGap;
		internal ushort usWinAscent;
		internal ushort usWinDescent;

		internal UIntPtr ulCodePageRange1;
		internal UIntPtr ulCodePageRange2;

		internal short sxHeight;
		internal short sCapHeight;
		internal ushort usDefaultChar;
		internal ushort usBreakChar;
		internal ushort usMaxContext;

		internal ushort usLowerOpticalPointSize;
		internal ushort usUpperOpticalPointSize;
	}
}
