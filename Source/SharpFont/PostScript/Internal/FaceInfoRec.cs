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

namespace SharpFont.PostScript.Internal
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct FaceInfoRec
	{
		internal IntPtr cid_font_name;
		internal IntPtr cid_version;
		internal int cid_font_type;

		internal IntPtr registry;

		internal IntPtr ordering;
		internal int supplement;

		internal FontInfo font_info;
		internal BBox font_bbox;
		internal UIntPtr uid_base;

		internal int num_xuid;
		#region xuid
		internal UIntPtr xuid1;
		internal UIntPtr xuid2;
		internal UIntPtr xuid3;
		internal UIntPtr xuid4;
		internal UIntPtr xuid5;
		internal UIntPtr xuid6;
		internal UIntPtr xuid7;
		internal UIntPtr xuid8;
		internal UIntPtr xuid9;
		internal UIntPtr xuid10;
		internal UIntPtr xuid11;
		internal UIntPtr xuid12;
		internal UIntPtr xuid13;
		internal UIntPtr xuid14;
		internal UIntPtr xuid15;
		internal UIntPtr xuid16;
		#endregion

		internal UIntPtr cidmap_offset;
		internal int fd_bytes;
		internal int gd_bytes;
		internal UIntPtr cid_count;

		internal int num_dicts;
		internal IntPtr font_dicts;

		internal UIntPtr data_offset;

		public string CidFontName => Marshal.PtrToStringAnsi(cid_font_name);
		public string CidVersion => Marshal.PtrToStringAnsi(cid_version);
		public string Registry => Marshal.PtrToStringAnsi(registry);
		public string Ordering => Marshal.PtrToStringAnsi(ordering);
	}
}
