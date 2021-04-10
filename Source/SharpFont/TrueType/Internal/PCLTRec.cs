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
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
	internal unsafe struct PCLTRec
	{
		internal IntPtr Version;
		internal UIntPtr FontNumber;
		internal ushort Pitch;
		internal ushort xHeight;
		internal ushort Style;
		internal ushort TypeFamily;
		internal ushort CapHeight;
		internal ushort SymbolSet;

		internal fixed byte TypeFace[16];

		private fixed byte characterComplement[8];
		internal ReadOnlySpan<byte> CharacterComplement
		{
			get
			{
				unsafe
				{
					fixed (byte* ptr = characterComplement)
						return new ReadOnlySpan<byte>(ptr, 8).ToArray();
				}
			}
		}

		private fixed byte fileName[6];
		internal ReadOnlySpan<byte> FileName
		{
			get
			{
				unsafe
				{
					fixed (byte* ptr = fileName)
						return new ReadOnlySpan<byte>(ptr, 6).ToArray();
				}
			}
		}

		internal byte StrokeWeight;
		internal byte WidthType;
		internal byte SerifStyle;
		internal byte Reserved;

		public string TypeFaceString
		{
			get
			{
				unsafe
				{
					fixed(byte* ptr = TypeFace)
						return Marshal.PtrToStringAnsi(new IntPtr(ptr), 16);
				}
			}
		} 
	}
}
