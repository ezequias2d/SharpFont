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

namespace SharpFont.Fnt.Internal
{
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct HeaderRec
	{
		internal const int CopyrightSize = 60;

		internal ushort version;
		internal UIntPtr file_size;

		internal fixed byte copyright[CopyrightSize];
		internal ushort file_type;
		internal ushort nominal_point_size;
		internal ushort vertical_resolution;
		internal ushort horizontal_resolution;
		internal ushort ascent;
		internal ushort internal_leading;
		internal ushort external_leading;
		internal byte italic;
		internal byte underline;
		internal byte strike_out;
		internal ushort weight;
		internal byte charset;
		internal ushort pixel_width;
		internal ushort pixel_height;
		internal byte pitch_and_family;
		internal ushort avg_width;
		internal ushort max_width;
		internal byte first_char;
		internal byte last_char;
		internal byte default_char;
		internal byte break_char;
		internal ushort bytes_per_row;
		internal UIntPtr device_offset;
		internal UIntPtr face_name_offset;
		internal UIntPtr bits_pointer;
		internal UIntPtr bits_offset;
		internal byte reserved;
		internal UIntPtr flags;
		internal ushort A_space;
		internal ushort B_space;
		internal ushort C_space;
		internal ushort color_table_offset;

		internal UIntPtr reserved1;
		internal UIntPtr reserved2;
		internal UIntPtr reserved3;
		internal UIntPtr reserved4;
	}
}
