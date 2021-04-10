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
	/// A structure used to represent CID Face information.
	/// </summary>
	public class FaceInfo : NativeObject
	{

		#region Constructors

		internal FaceInfo(IntPtr reference) : base(reference)
		{			
		}

		#endregion

		#region Properties

		private ref FaceInfoRec Rec => ref PInvokeHelper.PtrToRefStructure<FaceInfoRec>(Reference);

		/// <summary>
		/// The name of the font, usually condensed from FullName.
		/// </summary>
		public string CidFontName => Rec.CidFontName;

		/// <summary>
		/// The version number of the font.
		/// </summary>
		public int CidVersion => (int)Rec.cid_version;

		/// <summary>
		/// Gets the string identifying the font's manufacturer.
		/// </summary>
		public string Registry => Rec.Registry;

		/// <summary>
		/// Gets the unique identifier for the character collection.
		/// </summary>
		public string Ordering => Rec.Ordering;

		/// <summary>
		/// Gets the identifier (supplement number) of the character collection.
		/// </summary>
		public int Supplement => Rec.supplement;

		/// <summary>
		/// Gets the dictionary of font info that is not used by the PostScript interpreter.
		/// </summary>
		public FontInfo FontInfo => Rec.font_info;

		/// <summary>
		/// Gets the coordinates of the corners of the bounding box.
		/// </summary>
		public BBox FontBBox => Rec.font_bbox;

		/// <summary>
		/// Gets the value to form UniqueID entries for base fonts within a composite font.
		/// </summary>
		public uint UidBase => (uint)Rec.uid_base;

		/// <summary>
		/// Gets the number of entries in the XUID array.
		/// </summary>
		public int XuidCount => Rec.num_xuid;

		/// <summary>
		/// Gets the extended unique IDS that identify the form, which allows
		/// the PostScript interpreter to cache the output for reuse.
		/// </summary>
		public ReadOnlySpan<uint> Xuid
		{
			get
			{
				unsafe
				{
					var ptr = (FaceInfoRec*)Reference;

					return new ReadOnlySpan<uint>(&ptr->xuid1, 16);
				}
			}
		}

		/// <summary>
		/// Gets the offset in bytes to the charstring offset table.
		/// </summary>
		public uint CidMapOffset => (uint)Rec.cidmap_offset;

		/// <summary>
		/// Gets the length in bytes of the FDArray index.
		/// A value of 0 indicates that the charstring offset table doesn't contain
		/// any FDArray indexes.
		/// </summary>
		public int FDBytes => Rec.fd_bytes;

		/// <summary>
		/// Gets the length of the offset to the charstring for each CID in the CID font.
		/// </summary>
		public int GDBytes => Rec.gd_bytes;

		/// <summary>
		/// Gets the number of valid CIDs in the CIDFont.
		/// </summary>
		public uint CidCount => (uint)Rec.cid_count;

		/// <summary>
		/// Gets the number of entries in the FontDicts array.
		/// </summary>
		public int DictsCount => Rec.num_dicts;

		/// <summary>
		/// Gets the set of font dictionaries for this font.
		/// </summary>
		public FaceDict FontDicts
		{
			get
			{
				unsafe
				{
					var ptr = (FaceInfoRec*)Reference;
					return new FaceDict(new IntPtr(&ptr->font_dicts));
				}
			}
		}

		/// <summary>
		/// The offset of the data.
		/// </summary>
		public uint DataOffset => (uint)Rec.data_offset;

		#endregion
	}
}
