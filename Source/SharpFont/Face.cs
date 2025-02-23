﻿#region MIT License
/*Copyright (c) 2012-2016 Robert Rouhani <robert.rouhani@gmail.com>

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
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

using SharpFont.Bdf;
using SharpFont.Internal;
using SharpFont.MultipleMasters;
using SharpFont.PostScript;
using SharpFont.TrueType;

namespace SharpFont
{
	/// <summary>
	/// FreeType root face class structure. A face object models a typeface in a font file.
	/// </summary>
	/// <remarks>
	/// Fields may be changed after a call to <see cref="AttachFile"/> or <see cref="AttachStream"/>.
	/// </remarks>
	public sealed class Face : DisposableNativeObject
	{
		#region Mac Structures
		/// <summary>
		/// A FOND resource reference.
		/// </summary>
		public struct Fond
		{
			/// <summary>
			/// Reference of a FOND resource.
			/// </summary>
			public IntPtr Ptr;
		}
		/// <summary>
		/// FSSpec reference to the file.
		/// </summary>
		public struct FSSpec
		{
			/// <summary>
			/// Reference of a FSSpec.
			/// </summary>
			public IntPtr Ptr;
		}
		/// <summary>
		/// FSRef reference to the font file
		/// </summary>
		public struct FSRef
		{
			/// <summary>
			/// Reference of a FSRef.
			/// </summary>
			public IntPtr Ptr;
		}
		#endregion

		#region static
		private static IntPtr NewFace(Library library, string path, int faceIndex)
		{
			Error err = FT.FT_New_Face(library.Reference, path, faceIndex, out IntPtr reference);

			if (err != Error.Ok)
				throw new FreeTypeException(err);
			return reference;
		}
		private static IntPtr NewFace(Library library, Span<byte> file, int faceIndex, out MemoryHandle handle)
		{
			Memory<byte> memory = file.ToArray();
			handle = memory.Pin();

			IntPtr reference;
			Error err;
			unsafe
			{
				err = FT.FT_New_Memory_Face(
					library.Reference,
					new IntPtr(handle.Pointer),
					file.Length, faceIndex,
					out reference);
			}

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return reference;
		}
		private static IntPtr NewFace(Library library, IntPtr bufferPtr, int length, int faceIndex)
		{
			Error err = FT.FT_New_Memory_Face(library.Reference, bufferPtr, length, faceIndex, out IntPtr reference);

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return reference;
		}
		private static IntPtr NewFace(Library library, Fond fond, int faceIndex)
		{
			if (!FT.IsMacOS)
				throw new InvalidOperationException($"This {nameof(NewFace)} function can only be called on macOS.");

			Error err = FT.FT_New_Face_From_FOND(library.Reference, fond.Ptr, faceIndex, out var faceRef);
			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return faceRef;
		}
		private static IntPtr NewFace(Library library, FSSpec spec, int faceIndex)
		{
			if (!FT.IsMacOS)
				throw new InvalidOperationException(
					$"This {nameof(NewFace)} function can only be called on macOS.");

			Error err = FT.FT_New_Face_From_FSSpec(library.Reference, spec.Ptr, faceIndex, out IntPtr faceRef);

			if (err != Error.Ok)
				throw new FreeTypeException(err);
			return faceRef;
		}
		private static IntPtr NewFace(Library library, FSRef @ref, int faceIndex)
		{
			if (!FT.IsMacOS)
				throw new InvalidOperationException(
					$"This {nameof(NewFace)} can only be called on macOS.");

			Error err = FT.FT_New_Face_From_FSRef(library.Reference, @ref.Ptr, faceIndex, out IntPtr faceRef);

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return faceRef;
		}
		private static IntPtr NewFace(Library library, OpenArgs args, int faceIndex)
		{
			Error err = FT.FT_Open_Face(library.Reference, args.Reference, faceIndex, out IntPtr faceRef);

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return faceRef;
		}
		#endregion

		#region Fields

		private readonly bool _hasMemoryFaceHandle;
		private MemoryHandle _memoryFaceHandle;

		private readonly Library parentLibrary;
		private readonly List<FTSize> childSizes;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Face"/> class with a default faceIndex of 0.
		/// </summary>
		/// <param name="library">The parent <see cref="Library"/>.</param>
		/// <param name="path">The path of the font file.</param>
		public Face(Library library, string path)
			: this(library, path, 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Face"/> class.
		/// </summary>
		/// <param name="library">The parent <see cref="Library"/>.</param>
		/// <param name="path">The path of the font file.</param>
		/// <param name="faceIndex">The index of the face to take from the file.</param>
		public Face(Library library, string path, int faceIndex)
			: this(library, NewFace(library, path, faceIndex))
		{
		}

		//TODO make an overload with a FileStream instead of a byte[]

		/// <summary>
		/// Initializes a new instance of the <see cref="Face"/> class from a file that's already loaded into memory.
		/// </summary>
		/// <param name="library">The parent <see cref="Library"/>.</param>
		/// <param name="file">The loaded file.</param>
		/// <param name="faceIndex">The index of the face to take from the file.</param>
		public Face(Library library, Span<byte> file, int faceIndex)
			: this(library, NewFace(library, file, faceIndex, out var handle))
		{
			_hasMemoryFaceHandle = true;
			_memoryFaceHandle = handle;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Face"/> class from a file that's already loaded into memory.
		/// </summary>
		/// <param name="library">The parent <see cref="Library"/>.</param>
		/// <param name="bufferPtr">A pointer to a buffer of a loaded file. Must not be freed before <see cref="Dispose"/>.</param>
		/// <param name="length">The length of bufferPtr.</param>
		/// <param name="faceIndex">The index of the face to take from the file.</param>
		public Face(Library library, IntPtr bufferPtr, int length, int faceIndex)
			: this(library, NewFace(library, bufferPtr, length, faceIndex))
		{
		}

		/// <summary>
		/// Only For MacOS.
		/// Initializes a new instance of the <see cref="Face"/> class from a FOND resource.
		/// </summary>
		/// <remarks>
		/// Create <see cref="Face"/> object from fonts that are installed in the system
		/// as follows.
		/// <code>
		/// fond = GetResource( 'FOND', fontName );
		/// error = FT_New_Face_From_FOND( library, fond, 0, &amp;face );
		/// </code>
		/// </remarks>
		/// <param name="library">A handle to the library resource.</param>
		/// <param name="fond">A FOND resource.</param>
		/// <param name="faceIndex">Only supported for the -1 ‘sanity check’ special case.</param>
		public Face(Library library, Fond fond, int faceIndex) : this(library, NewFace(library, fond, faceIndex))
		{

		}

		/// <summary>
		/// Only for MacOS.
		/// Initializes a new instance of the <see cref="Face"/> class from a given resource and typeface index using an FSSpec to the font file.
		/// </summary>
		/// <remarks>
		/// <see cref="Face(Library, FSSpec, int)"/> is identical to <see cref="Face(Library, string, int)"/> except it accepts an FSSpec instead
		/// of a path.
		/// </remarks>
		/// <param name="library">The parent <see cref="Library"/>.</param>
		/// <param name="spec">FSSpec to the font file.</param>
		/// <param name="faceIndex">The index of the face within the resource. The first face has index 0.</param>
		/// <returns>A handle to a new face object.</returns>
		public Face(Library library, FSSpec spec, int faceIndex) : this(library, NewFace(library, spec, faceIndex))
		{

		}

		/// <summary>
		/// Only for MacOS.
		/// Initializes a new instance of the <see cref="Face"/> class from a given resource and typeface index using an FSRef to the font file.
		/// </summary>
		/// <remarks>
		/// <see cref="Face(Library, FSRef, int)"/> is identical to <see cref="Face(Library, string, int)"/> except it accepts an FSRef instead of
		/// a path.
		/// </remarks>
		/// <param name="library">The parent <see cref="Library"/>.</param>
		/// <param name="ref">FSRef to the font file.</param>
		/// <param name="faceIndex">The index of the face within the resource. The first face has index 0.</param>
		/// <returns>A handle to a new face object.</returns>
		public Face(Library library, FSRef @ref, int faceIndex) : this(library, NewFace(library, @ref, faceIndex))
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Face"/> class from a given resource described by <see cref="OpenArgs"/>.
		/// </summary>
		/// <remarks><para>
		/// Unlike FreeType 1.x, this function automatically creates a glyph slot for the face object which can be
		/// accessed directly through <see cref="Glyph"/>.
		/// </para><para>
		/// OpenFace can be used to quickly check whether the font format of a given font resource is supported by
		/// FreeType. If the ‘faceIndex’ field is negative, the function's return value is 0 if the font format is
		/// recognized, or non-zero otherwise; the function returns a more or less empty face handle in ‘*aface’ (if
		/// ‘aface’ isn't NULL). The only useful field in this special case is <see cref="FaceCount"/> which gives
		/// the number of faces within the font file. After examination, the returned <see cref="Face"/> structure
		/// should be deallocated with a call to <see cref="Dispose"/>.
		/// </para><para>
		/// Each new face object created with this function also owns a default <see cref="FTSize"/> object, accessible
		/// as <see cref="Size"/>.
		/// </para><para>
		/// See the discussion of reference counters in the description of FT_Reference_Face.
		/// </para></remarks>
		/// <param name="library">The parent <see cref="Library"/>.</param>
		/// <param name="args">A pointer to an <see cref="OpenArgs"/> structure which must be filled by the caller.</param>
		/// <param name="faceIndex">The index of the face within the font. The first face has index 0.</param>
		/// <returns>A handle to a new face object. If ‘faceIndex’ is greater than or equal to zero, it must be non-NULL.</returns>
		public Face(Library library, OpenArgs args, int faceIndex) : this(library, NewFace(library, args, faceIndex))
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Face"/> class.
		/// </summary>
		/// <param name="reference">A pointer to the unmanaged memory containing the Face.</param>
		/// <param name="parent">The parent <see cref="Library"/>.</param>
		internal Face(Library parent, IntPtr reference): base(reference)
		{
			_hasMemoryFaceHandle = false;
			childSizes = new List<FTSize>();

			if (parent != null)
			{
				parentLibrary = parent;
				parentLibrary.AddChildFace(this);
			}
			else
				// increment the internal count of face.
				FT.FT_Reference_Face(Reference);
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the face is disposed.
		/// </summary>
		public event EventHandler Disposed;

		#endregion

		#region Properties

		private ref FaceRec Rec => ref PInvokeHelper.PtrToRefStructure<FaceRec>(Reference);

		/// <summary>
		/// Gets the number of faces in the font file. Some font formats can have multiple faces in a font file.
		/// </summary>
		public long FaceCount => (long)Rec.num_faces;

		/// <summary>
		/// Gets the index of the face in the font file. It is set to 0 if there is only one face in the font file.
		/// </summary>
		public long FaceIndex => (long)Rec.face_index;

		/// <summary>
		/// Gets a set of bit flags that give important information about the face.
		/// </summary>
		/// <see cref="FaceFlags"/>
		public FaceFlags FaceFlags => (FaceFlags)Rec.face_flags;

		/// <summary>
		/// Gets a set of bit flags indicating the style of the face.
		/// </summary>
		/// <see cref="StyleFlags"/>
		public StyleFlags StyleFlags => (StyleFlags)Rec.style_flags;

		/// <summary><para>
		/// Gets the number of glyphs in the face. If the face is scalable and has sbits (see ‘num_fixed_sizes’), it is
		/// set to the number of outline glyphs.
		/// </para><para>
		/// For CID-keyed fonts, this value gives the highest CID used in the font.
		/// </para></summary>
		public long GlyphCount => (long)Rec.num_glyphs;

		/// <summary>
		/// Gets the face's family name. This is an ASCII string, usually in English, which describes the typeface's
		/// family (like ‘Times New Roman’, ‘Bodoni’, ‘Garamond’, etc). This is a least common denominator used to list
		/// fonts. Some formats (TrueType &amp; OpenType) provide localized and Unicode versions of this string.
		/// Applications should use the format specific interface to access them. Can be NULL (e.g., in fonts embedded
		/// in a PDF file).
		/// </summary>
		public string FamilyName => Rec.FamilyName;

		/// <summary>
		/// Gets the face's style name. This is an ASCII string, usually in English, which describes the typeface's
		/// style (like ‘Italic’, ‘Bold’, ‘Condensed’, etc). Not all font formats provide a style name, so this field
		/// is optional, and can be set to NULL. As for ‘family_name’, some formats provide localized and Unicode
		/// versions of this string. Applications should use the format specific interface to access them.
		/// </summary>
		public string StyleName => Rec.StyleName;

		/// <summary>
		/// Gets the number of bitmap strikes in the face. Even if the face is scalable, there might still be bitmap
		/// strikes, which are called ‘sbits’ in that case.
		/// </summary>
		public int FixedSizesCount => Rec.num_fixed_sizes;

		/// <summary>
		/// Gets an array of FT_Bitmap_Size for all bitmap strikes in the face. It is set to NULL if there is no bitmap
		/// strike.
		/// </summary>
		public ReadOnlySpan<BitmapSize> AvailableSizes
		{
			get
			{
				int count = FixedSizesCount;

				if (count == 0)
					return ReadOnlySpan<BitmapSize>.Empty;

				unsafe
				{
					var rec = (FaceRec*)Reference;
					return new ReadOnlySpan<BitmapSize>((BitmapSize*)rec->available_sizes, count);
				}
			}
		}

		/// <summary>
		/// Gets the number of charmaps in the face.
		/// </summary>
		public int CharmapsCount => Rec.num_charmaps;

		/// <summary>
		/// Gets an array of the charmaps of the face.
		/// </summary>
		public ReadOnlySpan<CharMap> CharMaps
		{
			get
			{
				int count = CharmapsCount;

				if (count == 0)
					return ReadOnlySpan<CharMap>.Empty;

				CharMap[] charmaps = new CharMap[count];

				unsafe
				{
					IntPtr* array = (IntPtr*)Rec.charmaps;
					for (int i = 0; i < count; i++)
						charmaps[i] = new CharMap(*array++, this);
				}

				return charmaps;
			}
		}

		/// <summary>
		/// Gets or sets a field reserved for client uses.
		/// </summary>
		/// <see cref="Generic"/>
		[Obsolete("Use the Tag property and Disposed event.")]
		public Generic Generic
		{
			get => Rec.generic;
			set => Rec.generic = value;
		}

		/// <summary><para>
		/// Gets the font bounding box. Coordinates are expressed in font units (see ‘units_per_EM’). The box is large
		/// enough to contain any glyph from the font. Thus, ‘bbox.yMax’ can be seen as the ‘maximal ascender’, and
		/// ‘bbox.yMin’ as the ‘minimal descender’. Only relevant for scalable formats.
		/// </para><para>
		/// Note that the bounding box might be off by (at least) one pixel for hinted fonts. See FT_Size_Metrics for
		/// further discussion.
		/// </para></summary>
		public BBox BBox => Rec.bbox;

		/// <summary>
		/// Gets the number of font units per EM square for this face. This is typically 2048 for TrueType fonts, and
		/// 1000 for Type 1 fonts. Only relevant for scalable formats.
		/// </summary>
		public ushort UnitsPerEM => Rec.units_per_EM;

		/// <summary>
		/// Gets the typographic ascender of the face, expressed in font units. For font formats not having this
		/// information, it is set to ‘bbox.yMax’. Only relevant for scalable formats.
		/// </summary>
		public short Ascender => Rec.ascender;

		/// <summary>
		/// Gets the typographic descender of the face, expressed in font units. For font formats not having this
		/// information, it is set to ‘bbox.yMin’.Note that this field is usually negative. Only relevant for scalable
		/// formats.
		/// </summary>
		public short Descender => Rec.descender;

		/// <summary>
		/// Gets the height is the vertical distance between two consecutive baselines, expressed in font units. It is
		/// always positive. Only relevant for scalable formats.
		/// </summary>
		public short Height => Rec.height;

		/// <summary>
		/// Gets the maximal advance width, in font units, for all glyphs in this face. This can be used to make word
		/// wrapping computations faster. Only relevant for scalable formats.
		/// </summary>
		public short MaxAdvanceWidth => Rec.max_advance_width;

		/// <summary>
		/// Gets the maximal advance height, in font units, for all glyphs in this face. This is only relevant for
		/// vertical layouts, and is set to ‘height’ for fonts that do not provide vertical metrics. Only relevant for
		/// scalable formats.
		/// </summary>
		public short MaxAdvanceHeight => Rec.max_advance_height;

		/// <summary>
		/// Gets the position, in font units, of the underline line for this face. It is the center of the underlining
		/// stem. Only relevant for scalable formats.
		/// </summary>
		public short UnderlinePosition => Rec.underline_position;

		/// <summary>
		/// Gets the thickness, in font units, of the underline for this face. Only relevant for scalable formats.
		/// </summary>
		public short UnderlineThickness => Rec.underline_thickness;

		/// <summary>
		/// Gets the face's associated glyph slot(s).
		/// </summary>
		public GlyphSlot Glyph => new GlyphSlot(Rec.glyph, this, parentLibrary);

		/// <summary>
		/// Gets the current active size for this face.
		/// </summary>
		public FTSize Size => new FTSize(Rec.size, false, this);

		/// <summary>
		/// Gets the current active charmap for this face.
		/// </summary>
		public CharMap CharMap => Rec.charmap != null ? new CharMap(Rec.charmap, this) : null;

		/// <summary>
		/// Gets or sets an object used to identify this instance of <see cref="Face"/>. This object will not be
		/// modified or accessed internally.
		/// </summary>
		/// <remarks>
		/// This is a replacement for FT_Generic in FreeType. If you are retrieving the same object multiple times
		/// from functions, this object will not appear in new copies.
		/// </remarks>
		public object Tag { get; set; }

		#endregion

		#region Methods

		#region FreeType Version

		/// <summary><para>
		/// Parse all bytecode instructions of a TrueType font file to check whether any of the patented opcodes are
		/// used. This is only useful if you want to be able to use the unpatented hinter with fonts that do not use
		/// these opcodes.
		/// </para><para>
		/// Note that this function parses all glyph instructions in the font file, which may be slow.
		/// </para></summary>
		/// <remarks>
		/// Since May 2010, TrueType hinting is no longer patented.
		/// </remarks>
		/// <returns>True if this is a TrueType font that uses one of the patented opcodes, false otherwise.</returns>
		public bool CheckTrueTypePatents() => FT.FT_Face_CheckTrueTypePatents(Reference);

		/// <summary>
		/// Enable or disable the unpatented hinter for a given <see cref="Face"/>. Only enable it if you have
		/// determined that the face doesn't use any patented opcodes.
		/// </summary>
		/// <remarks>
		/// Since May 2010, TrueType hinting is no longer patented.
		/// </remarks>
		/// <param name="value">New boolean setting.</param>
		/// <returns>
		/// The old setting value. This will always be false if this is not an SFNT font, or if the unpatented hinter
		/// is not compiled in this instance of the library.
		/// </returns>
		/// <see cref="CheckTrueTypePatents"/>
		public bool SetUnpatentedHinting(bool value) => FT.FT_Face_SetUnpatentedHinting(Reference, value);

		#endregion

		#region Base Interface

		/// <summary>
		/// This function calls <see cref="AttachStream"/> to attach a file.
		/// </summary>
		/// <param name="path">The pathname.</param>
		public void AttachFile(string path)
		{
			Error err = FT.FT_Attach_File(Reference, path);

			if (err != Error.Ok)
				throw new FreeTypeException(err);
		}

		/// <summary>
		/// ‘Attach’ data to a face object. Normally, this is used to read additional information for the face object.
		/// For example, you can attach an AFM file that comes with a Type 1 font to get the kerning values and other
		/// metrics.
		/// </summary>
		/// <remarks><para>
		/// The meaning of the ‘attach’ (i.e., what really happens when the new file is read) is not fixed by FreeType
		/// itself. It really depends on the font format (and thus the font driver).
		/// </para><para>
		/// Client applications are expected to know what they are doing when invoking this function. Most drivers
		/// simply do not implement file attachments.
		/// </para></remarks>
		/// <param name="parameters">A pointer to <see cref="OpenArgs"/> which must be filled by the caller.</param>
		public void AttachStream(OpenArgs parameters)
		{
			Error err = FT.FT_Attach_Stream(Reference, parameters.Reference);

			if (err != Error.Ok)
				throw new FreeTypeException(err);
		}

		/// <summary>
		/// Select a bitmap strike.
		/// </summary>
		/// <param name="strikeIndex">
		/// The index of the bitmap strike in the <see cref="Face.AvailableSizes"/> field of <see cref="Face"/>
		/// structure.
		/// </param>
		public void SelectSize(int strikeIndex)
		{
			Error err = FT.FT_Select_Size(Reference, strikeIndex);

			if (err != Error.Ok)
				throw new FreeTypeException(err);
		}

		/// <summary>
		/// Resize the scale of the active <see cref="FTSize"/> object in a face.
		/// </summary>
		/// <param name="request">A pointer to a <see cref="SizeRequest"/>.</param>
		public unsafe void RequestSize(SizeRequest request)
		{
			Error err = FT.FT_Request_Size(Reference, (IntPtr)(&request));

			if (err != Error.Ok)
				throw new FreeTypeException(err);
		}

		/// <summary>
		/// This function calls <see cref="RequestSize"/> to request the nominal size (in points).
		/// </summary>
		/// <remarks><para>
		/// If either the character width or height is zero, it is set equal to the other value.
		/// </para><para>
		/// If either the horizontal or vertical resolution is zero, it is set equal to the other value.
		/// </para><para>
		/// A character width or height smaller than 1pt is set to 1pt; if both resolution values are zero, they are
		/// set to 72dpi.
		/// </para></remarks>
		/// <param name="width">The nominal width, in 26.6 fractional points.</param>
		/// <param name="height">The nominal height, in 26.6 fractional points.</param>
		/// <param name="horizontalResolution">The horizontal resolution in dpi.</param>
		/// <param name="verticalResolution">The vertical resolution in dpi.</param>
		public void SetCharSize(Fixed26Dot6 width, Fixed26Dot6 height, uint horizontalResolution, uint verticalResolution)
		{
			Error err = FT.FT_Set_Char_Size(Reference, (IntPtr)width.Value, (IntPtr)height.Value, horizontalResolution, verticalResolution);

			if (err != Error.Ok)
				throw new FreeTypeException(err);
		}

		/// <summary>
		/// This function calls <see cref="RequestSize"/> to request the nominal size (in pixels).
		/// </summary>
		/// <param name="width">The nominal width, in pixels.</param>
		/// <param name="height">The nominal height, in pixels</param>
		public void SetPixelSizes(uint width, uint height)
		{
			Error err = FT.FT_Set_Pixel_Sizes(Reference, width, height);

			if (err != Error.Ok)
				throw new FreeTypeException(err);
		}

		/// <summary>
		/// A function used to load a single glyph into the glyph slot of a face object.
		/// </summary>
		/// <remarks><para>
		/// The loaded glyph may be transformed. See <see cref="SetTransform()"/> for the details.
		/// </para><para>
		/// For subsetted CID-keyed fonts, <see cref="Error.InvalidArgument"/> is returned for invalid CID values (this
		/// is, for CID values which don't have a corresponding glyph in the font). See the discussion of the
		/// <see cref="SharpFont.FaceFlags.CidKeyed"/> flag for more details.
		/// </para></remarks>
		/// <param name="glyphIndex">
		/// The index of the glyph in the font file. For CID-keyed fonts (either in PS or in CFF format) this argument
		/// specifies the CID value.
		/// </param>
		/// <param name="flags">
		/// A flag indicating what to load for this glyph. The <see cref="LoadFlags"/> constants can be used to control
		/// the glyph loading process (e.g., whether the outline should be scaled, whether to load bitmaps or not,
		/// whether to hint the outline, etc).
		/// </param>
		/// <param name="target">The target to OR with the flags.</param>
		public void LoadGlyph(uint glyphIndex, LoadFlags flags, LoadTarget target)
		{
			Error err = FT.FT_Load_Glyph(Reference, glyphIndex, (int)flags | (int)target);

			if (err != Error.Ok)
				throw new FreeTypeException(err);
		}

		/// <summary>
		/// A function used to load a single glyph into the glyph slot of a face object, according to its character
		/// code.
		/// </summary>
		/// <remarks>
		/// This function simply calls <see cref="GetCharIndex"/> and <see cref="LoadGlyph"/>
		/// </remarks>
		/// <param name="charCode">
		/// The glyph's character code, according to the current charmap used in the face.
		/// </param>
		/// <param name="flags">
		/// A flag indicating what to load for this glyph. The <see cref="LoadFlags"/> constants can be used to control
		/// the glyph loading process (e.g., whether the outline should be scaled, whether to load bitmaps or not,
		/// whether to hint the outline, etc).
		/// </param>
		/// <param name="target">The target to OR with the flags.</param>
		public void LoadChar(uint charCode, LoadFlags flags, LoadTarget target)
		{
			Error err = FT.FT_Load_Char(Reference, charCode, (int)flags | (int)target);

			if (err != Error.Ok)
				throw new FreeTypeException(err);
		}

		/// <summary>
		/// A function used to set the transformation that is applied to glyph images when they are loaded into a glyph
		/// slot through <see cref="LoadGlyph"/>.
		/// </summary>
		/// <remarks><para>
		/// The transformation is only applied to scalable image formats after the glyph has been loaded. It means that
		/// hinting is unaltered by the transformation and is performed on the character size given in the last call to
		/// <see cref="SetCharSize"/> or <see cref="SetPixelSizes"/>.
		/// </para><para>
		/// Note that this also transforms the ‘face.glyph.advance’ field, but not the values in ‘face.glyph.metrics’.
		/// </para></remarks>
		/// <param name="matrix">
		/// A pointer to the transformation's 2x2 matrix. Use the method overloads for the identity matrix.
		/// </param>
		/// <param name="delta">
		/// A pointer to the translation vector. Use the method overloads for the null vector.
		/// </param>
		public unsafe void SetTransform(FTMatrix matrix, FTVector delta) =>
			FT.FT_Set_Transform(Reference, (IntPtr)(&matrix), (IntPtr)(&delta));

		/// <summary>
		/// A function used to set the transformation that is applied to glyph images when they are loaded into a glyph
		/// slot through <see cref="LoadGlyph"/> with the identity matrix.
		/// </summary>
		/// <remarks><para>
		/// The transformation is only applied to scalable image formats after the glyph has been loaded. It means that
		/// hinting is unaltered by the transformation and is performed on the character size given in the last call to
		/// <see cref="SetCharSize"/> or <see cref="SetPixelSizes"/>.
		/// </para><para>
		/// Note that this also transforms the ‘face.glyph.advance’ field, but not the values in ‘face.glyph.metrics’.
		/// </para></remarks>
		/// <param name="delta">
		/// A pointer to the translation vector. Use the method overloads for the null vector.
		/// </param>
		public unsafe void SetTransform(FTVector delta) =>
			FT.FT_Set_Transform(Reference, IntPtr.Zero, (IntPtr)(&delta));

		/// <summary>
		/// A function used to set the transformation that is applied to glyph images when they are loaded into a glyph
		/// slot through <see cref="LoadGlyph"/> with the null vector.
		/// </summary>
		/// <remarks><para>
		/// The transformation is only applied to scalable image formats after the glyph has been loaded. It means that
		/// hinting is unaltered by the transformation and is performed on the character size given in the last call to
		/// <see cref="SetCharSize"/> or <see cref="SetPixelSizes"/>.
		/// </para><para>
		/// Note that this also transforms the ‘face.glyph.advance’ field, but not the values in ‘face.glyph.metrics’.
		/// </para></remarks>
		/// <param name="matrix">
		/// A pointer to the transformation's 2x2 matrix. Use the method overloads for the identity matrix.
		/// </param>
		public unsafe void SetTransform(FTMatrix matrix) =>
			FT.FT_Set_Transform(Reference, (IntPtr)(&matrix), IntPtr.Zero);

		/// <summary>
		/// A function used to set the transformation that is applied to glyph images when they are loaded into a glyph
		/// slot through <see cref="LoadGlyph"/> with the null vector and the identity matrix.
		/// </summary>
		/// <remarks><para>
		/// The transformation is only applied to scalable image formats after the glyph has been loaded. It means that
		/// hinting is unaltered by the transformation and is performed on the character size given in the last call to
		/// <see cref="SetCharSize"/> or <see cref="SetPixelSizes"/>.
		/// </para><para>
		/// Note that this also transforms the ‘face.glyph.advance’ field, but not the values in ‘face.glyph.metrics’.
		/// </para></remarks>
		public unsafe void SetTransform() =>
			FT.FT_Set_Transform(Reference, IntPtr.Zero, IntPtr.Zero);

		/// <summary>
		/// Return the kerning vector between two glyphs of a same face.
		/// </summary>
		/// <remarks>
		/// Only horizontal layouts (left-to-right &amp; right-to-left) are supported by this method. Other layouts, or
		/// more sophisticated kernings, are out of the scope of this API function -- they can be implemented through
		/// format-specific interfaces.
		/// </remarks>
		/// <param name="leftGlyph">The index of the left glyph in the kern pair.</param>
		/// <param name="rightGlyph">The index of the right glyph in the kern pair.</param>
		/// <param name="mode">Determines the scale and dimension of the returned kerning vector.</param>
		/// <returns>
		/// The kerning vector. This is either in font units or in pixels (26.6 format) for scalable formats, and in
		/// pixels for fixed-sizes formats.
		/// </returns>
		public FTVector26Dot6 GetKerning(uint leftGlyph, uint rightGlyph, KerningMode mode)
		{
			Error err = FT.FT_Get_Kerning(Reference, leftGlyph, rightGlyph, (uint)mode, out var kern);

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return kern;
		}

		/// <summary>
		/// Return the track kerning for a given face object at a given size.
		/// </summary>
		/// <param name="pointSize">The point size in 16.16 fractional points.</param>
		/// <param name="degree">The degree of tightness.</param>
		/// <returns>The kerning in 16.16 fractional points.</returns>
		public Fixed16Dot16 GetTrackKerning(Fixed16Dot16 pointSize, int degree)
		{
			Error err = FT.FT_Get_Track_Kerning(Reference, (IntPtr)pointSize.Value, degree, out var kerning);

			if (err != Error.Ok)
				throw new FreeTypeException(err);
			
			return Fixed16Dot16.FromRawValue((int)kerning);
		}

		/// <summary>
		/// Retrieve the ASCII name of a given glyph in a face. This only works for faces that have the
		/// <see cref="FaceFlags.GlyphNames"/> flag.
		/// </summary>
		/// <remarks><para>
		/// An error is returned if the face doesn't provide glyph names or if the glyph index is invalid. In all cases
		/// of failure, the first byte of ‘buffer’ is set to 0 to indicate an empty name.
		/// </para><para>
		/// The glyph name is truncated to fit within the buffer if it is too long. The returned string is always
		/// zero-terminated.
		/// </para><para>
		/// Be aware that FreeType reorders glyph indices internally so that glyph index 0 always corresponds to the
		/// ‘missing glyph’ (called ‘.notdef’).
		/// </para><para>
		/// This function is not compiled within the library if the config macro ‘FT_CONFIG_OPTION_NO_GLYPH_NAMES’ is
		/// defined in ‘include/freetype/config/ftoptions.h’.
		/// </para></remarks>
		/// <param name="glyphIndex">The glyph index.</param>
		/// <param name="bufferSize">The maximal number of bytes available in the buffer.</param>
		/// <returns>The ASCII name of a given glyph in a face.</returns>
		public string GetGlyphName(uint glyphIndex, int bufferSize) =>
			GetGlyphName(glyphIndex, new byte[bufferSize]);

		/// <summary>
		/// Retrieve the ASCII name of a given glyph in a face. This only works for faces that have the
		/// <see cref="FaceFlags.GlyphNames"/> flag.
		/// </summary>
		/// <remarks><para>
		/// An error is returned if the face doesn't provide glyph names or if the glyph index is invalid. In all cases
		/// of failure, the first byte of ‘buffer’ is set to 0 to indicate an empty name.
		/// </para><para>
		/// The glyph name is truncated to fit within the buffer if it is too long. The returned string is always
		/// zero-terminated.
		/// </para><para>
		/// Be aware that FreeType reorders glyph indices internally so that glyph index 0 always corresponds to the
		/// ‘missing glyph’ (called ‘.notdef’).
		/// </para><para>
		/// This function is not compiled within the library if the config macro ‘FT_CONFIG_OPTION_NO_GLYPH_NAMES’ is
		/// defined in ‘include/freetype/config/ftoptions.h’.
		/// </para></remarks>
		/// <param name="glyphIndex">The glyph index.</param>
		/// <param name="buffer">The target buffer where the name is copied to.</param>
		/// <returns>The ASCII name of a given glyph in a face.</returns>
		public unsafe string GetGlyphName(uint glyphIndex, byte[] buffer)
		{
			fixed (byte* ptr = buffer)
			{
				IntPtr intptr = new IntPtr(ptr);
				Error err = FT.FT_Get_Glyph_Name(Reference, glyphIndex, intptr, (uint)buffer.Length);

				if (err != Error.Ok)
					throw new FreeTypeException(err);

				return Marshal.PtrToStringAnsi(intptr);
			}
		}

		/// <summary>
		/// Retrieve the ASCII Postscript name of a given face, if available. This only works with Postscript and
		/// TrueType fonts.
		/// </summary>
		/// <remarks>
		/// The returned pointer is owned by the face and is destroyed with it.
		/// </remarks>
		/// <returns>A pointer to the face's Postscript name. NULL if unavailable.</returns>
		public string GetPostscriptName() =>
			Marshal.PtrToStringAnsi(FT.FT_Get_Postscript_Name(Reference));

		/// <summary>
		/// Select a given charmap by its encoding tag (as listed in ‘freetype.h’).
		/// </summary>
		/// <remarks><para>
		/// This function returns an error if no charmap in the face corresponds to the encoding queried here.
		/// </para><para>
		/// Because many fonts contain more than a single cmap for Unicode encoding, this function has some special
		/// code to select the one which covers Unicode best. It is thus preferable to <see cref="SetCharmap"/> in
		/// this case.
		/// </para></remarks>
		/// <param name="encoding">A handle to the selected encoding.</param>
		public void SelectCharmap(Encoding encoding)
		{
			Error err = FT.FT_Select_Charmap(Reference, encoding);

			if (err != Error.Ok)
				throw new FreeTypeException(err);
		}

		/// <summary>
		/// Select a given charmap for character code to glyph index mapping.
		/// </summary>
		/// <remarks>
		/// This function returns an error if the charmap is not part of the face (i.e., if it is not listed in the
		/// <see cref="Face.CharMaps"/>’ table).
		/// </remarks>
		/// <param name="charmap">A handle to the selected charmap.</param>
		public void SetCharmap(CharMap charmap)
		{
			Error err = FT.FT_Set_Charmap(Reference, charmap.Reference);

			if (err != Error.Ok)
				throw new FreeTypeException(err);
		}

		/// <summary>
		/// Return the glyph index of a given character code. This function uses a charmap object to do the mapping.
		/// </summary>
		/// <remarks>
		/// If you use FreeType to manipulate the contents of font files directly, be aware that the glyph index
		/// returned by this function doesn't always correspond to the internal indices used within the file. This is
		/// done to ensure that value 0 always corresponds to the ‘missing glyph’.
		/// </remarks>
		/// <param name="charCode">The character code.</param>
		/// <returns>The glyph index. 0 means ‘undefined character code’.</returns>
		public uint GetCharIndex(uint charCode) => FT.FT_Get_Char_Index(Reference, charCode);

		/// <summary>
		/// This function is used to return the first character code in the current charmap of a given face. It also
		/// returns the corresponding glyph index.
		/// </summary>
		/// <remarks><para>
		/// You should use this function with <see cref="GetNextChar"/> to be able to parse all character codes
		/// available in a given charmap.
		/// </para><para>
		/// Note that ‘agindex’ is set to 0 if the charmap is empty. The result itself can be 0 in two cases: if the
		/// charmap is empty or when the value 0 is the first valid character code.
		/// </para></remarks>
		/// <param name="glyphIndex">Glyph index of first character code. 0 if charmap is empty.</param>
		/// <returns>The charmap's first character code.</returns>
		public uint GetFirstChar(out uint glyphIndex) =>
			FT.FT_Get_First_Char(Reference, out glyphIndex);

		/// <summary>
		/// This function is used to return the next character code in the current charmap of a given face following
		/// the value ‘charCode’, as well as the corresponding glyph index.
		/// </summary>
		/// <remarks><para>
		/// You should use this function with <see cref="GetFirstChar"/> to walk over all character codes available
		/// in a given charmap. See the note for this function for a simple code example.
		/// </para><para>
		/// Note that ‘*agindex’ is set to 0 when there are no more codes in the charmap.
		/// </para></remarks>
		/// <param name="charCode">The starting character code.</param>
		/// <param name="glyphIndex">Glyph index of first character code. 0 if charmap is empty.</param>
		/// <returns>The charmap's next character code.</returns>
		public uint GetNextChar(uint charCode, out uint glyphIndex) =>
			FT.FT_Get_Next_Char(Reference, charCode, out glyphIndex);

		/// <summary>
		/// Return the glyph index of a given glyph name. This function uses driver specific objects to do the
		/// translation.
		/// </summary>
		/// <param name="name">The glyph name.</param>
		/// <returns>The glyph index. 0 means ‘undefined character code’.</returns>
		public uint GetNameIndex(string name) =>
			FT.FT_Get_Name_Index(Reference, Marshal.StringToHGlobalAnsi(name));

		/// <summary>
		/// Return the <see cref="EmbeddingTypes"/> flags for a font.
		/// </summary>
		/// <remarks>
		/// Use this function rather than directly reading the ‘fs_type’ field in the <see cref="PostScript.FontInfo"/>
		/// structure which is only guaranteed to return the correct results for Type 1 fonts.
		/// </remarks>
		/// <returns>The fsType flags, <see cref="EmbeddingTypes"/>.</returns>
		public EmbeddingTypes GetFSTypeFlags() =>
			FT.FT_Get_FSType_Flags(Reference);

		#endregion

		#region Glyph Variants

		/// <summary>
		/// Return the glyph index of a given character code as modified by the variation selector.
		/// </summary>
		/// <remarks><para>
		/// If you use FreeType to manipulate the contents of font files directly, be aware that the glyph index
		/// returned by this function doesn't always correspond to the internal indices used within the file. This is
		/// done to ensure that value 0 always corresponds to the ‘missing glyph’.
		/// </para><para>
		/// This function is only meaningful if a) the font has a variation selector cmap sub table, and b) the current
		/// charmap has a Unicode encoding.
		/// </para></remarks>
		/// <param name="charCode">The character code point in Unicode.</param>
		/// <param name="variantSelector">The Unicode code point of the variation selector.</param>
		/// <returns>
		/// The glyph index. 0 means either ‘undefined character code’, or ‘undefined selector code’, or ‘no variation
		/// selector cmap subtable’, or ‘current CharMap is not Unicode’.
		/// </returns>
		public uint GetCharVariantIndex(uint charCode, uint variantSelector) =>
			FT.FT_Face_GetCharVariantIndex(Reference, charCode, variantSelector);

		/// <summary>
		/// Check whether this variant of this Unicode character is the one to be found in the ‘cmap’.
		/// </summary>
		/// <remarks>
		/// This function is only meaningful if the font has a variation selector cmap subtable.
		/// </remarks>
		/// <param name="charCode">The character codepoint in Unicode.</param>
		/// <param name="variantSelector">The Unicode codepoint of the variation selector.</param>
		/// <returns>
		/// 1 if found in the standard (Unicode) cmap, 0 if found in the variation selector cmap, or -1 if it is not a
		/// variant.
		/// </returns>
		public int GetCharVariantIsDefault(uint charCode, uint variantSelector) =>
			FT.FT_Face_GetCharVariantIsDefault(Reference, charCode, variantSelector);

		/// <summary>
		/// Return a zero-terminated list of Unicode variant selectors found in the font.
		/// </summary>
		/// <returns>
		/// A pointer to an array of selector code points, or NULL if there is no valid variant selector cmap subtable.
		/// </returns>
		public uint[] GetVariantSelectors()
		{
			List<uint> list = new List<uint>();

			unsafe
			{
				var ptr = (uint*)FT.FT_Face_GetVariantSelectors(Reference);
				do
					list.Add(*ptr++);
				while (*ptr != 0);
			}

			return list.ToArray();
		}

		/// <summary>
		/// Return a zero-terminated list of Unicode variant selectors found in the font.
		/// </summary>
		/// <remarks>
		/// The last item in the array is 0; the array is owned by the <see cref="Face"/> object but can be overwritten
		/// or released on the next call to a FreeType function.
		/// </remarks>
		/// <param name="charCode">The character codepoint in Unicode.</param>
		/// <returns>
		/// A pointer to an array of variant selector code points which are active for the given character, or NULL if
		/// the corresponding list is empty.
		/// </returns>
		public uint[] GetVariantsOfChar(uint charCode)
		{
			List<uint> list = new List<uint>();

			unsafe
			{
				var ptr = (uint*)FT.FT_Face_GetVariantsOfChar(Reference, charCode);
				do
					list.Add(*ptr++);
				while (*ptr != 0);
			}

			return list.ToArray();
		}

		/// <summary>
		/// Return a zero-terminated list of Unicode character codes found for the specified variant selector.
		/// </summary>
		/// <remarks>
		/// The last item in the array is 0; the array is owned by the <see cref="Face"/> object but can be overwritten
		/// or released on the next call to a FreeType function.
		/// </remarks>
		/// <param name="variantSelector">The variant selector code point in Unicode.</param>
		/// <returns>
		/// A list of all the code points which are specified by this selector (both default and non-default codes are
		/// returned) or NULL if there is no valid cmap or the variant selector is invalid.
		/// </returns>
		public uint[] GetCharsOfVariant(uint variantSelector)
		{
			List<uint> list = new List<uint>();

			unsafe
			{
				var ptr = (uint*)FT.FT_Face_GetCharsOfVariant(Reference, variantSelector);
				do
					list.Add(*ptr++);
				while (*ptr != 0);
			}

			return list.ToArray();
		}

		#endregion

		#region Size Management

		/// <summary>
		/// Create a new size object from a given face object.
		/// </summary>
		/// <remarks>
		/// You need to call <see cref="FTSize.Activate"/> in order to select the new size for upcoming calls to
		/// <see cref="SetPixelSizes"/>, <see cref="SetCharSize"/>, <see cref="LoadGlyph"/>, <see cref="LoadChar"/>,
		/// etc.
		/// </remarks>
		/// <returns>A handle to a new size object.</returns>
		public FTSize NewSize()
		{
			return new FTSize(this);
		}

		#endregion

		#region Multiple Masters

		/// <summary><para>
		/// Retrieve the Multiple Master descriptor of a given font.
		/// </para><para>
		/// This function can't be used with GX fonts.
		/// </para></summary>
		/// <returns>The Multiple Masters descriptor.</returns>
		public MultiMaster GetMultiMaster()
		{
			Error err = FT.FT_Get_Multi_Master(Reference, out var masterRef);

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return new MultiMaster(masterRef);
		}

		/// <summary>
		/// Retrieve the Multiple Master/GX var descriptor of a given font.
		/// </summary>
		/// <returns>
		/// The Multiple Masters/GX var descriptor. Allocates a data structure, which the user must free (a single call
		/// to FT_FREE will do it).
		/// </returns>
		public MMVar GetMMVar()
		{
			Error err = FT.FT_Get_MM_Var(Reference, out var varRef);

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return new MMVar(varRef);
		}

		/// <summary><para>
		/// For Multiple Masters fonts, choose an interpolated font design through design coordinates.
		/// </para><para>
		/// This function can't be used with GX fonts.
		/// </para></summary>
		/// <param name="coords">An array of design coordinates.</param>
		public unsafe void SetMMDesignCoordinates(long[] coords)
		{
			fixed (void* ptr = coords)
			{
				IntPtr coordsPtr = (IntPtr)ptr;
				Error err = FT.FT_Set_MM_Design_Coordinates(Reference, (uint)coords.Length, coordsPtr);

				if (err != Error.Ok)
					throw new FreeTypeException(err);
			}
		}

		/// <summary>
		/// For Multiple Master or GX Var fonts, choose an interpolated font design through design coordinates.
		/// </summary>
		/// <param name="coords">An array of design coordinates.</param>
		public unsafe void SetVarDesignCoordinates(long[] coords)
		{
			fixed (void* ptr = coords)
			{
				IntPtr coordsPtr = (IntPtr)ptr;
				Error err = FT.FT_Set_Var_Design_Coordinates(Reference, (uint)coords.Length, coordsPtr);

				if (err != Error.Ok)
					throw new FreeTypeException(err);
			}
		}

		/// <summary>
		/// For Multiple Masters and GX var fonts, choose an interpolated font design through normalized blend
		/// coordinates.
		/// </summary>
		/// <param name="coords">The design coordinates array (each element must be between 0 and 1.0).</param>
		public unsafe void SetMMBlendCoordinates(long[] coords)
		{
			fixed (void* ptr = coords)
			{
				IntPtr coordsPtr = (IntPtr)ptr;
				Error err = FT.FT_Set_MM_Blend_Coordinates(Reference, (uint)coords.Length, coordsPtr);

				if (err != Error.Ok)
					throw new FreeTypeException(err);
			}
		}

		/// <summary>
		/// This is another name of <see cref="SetMMBlendCoordinates"/>.
		/// </summary>
		/// <param name="coords">The design coordinates array (each element must be between 0 and 1.0).</param>
		public unsafe void SetVarBlendCoordinates(long[] coords)
		{
			fixed (void* ptr = coords)
			{
				IntPtr coordsPtr = (IntPtr)ptr;
				Error err = FT.FT_Set_Var_Blend_Coordinates(Reference, (uint)coords.Length, coordsPtr);

				if (err != Error.Ok)
					throw new FreeTypeException(err);
			}
		}

		#endregion

		#region TrueType Tables

		/// <summary>
		/// Return a pointer to a given SFNT table within a face.
		/// </summary>
		/// <remarks><para>
		/// The table is owned by the face object and disappears with it.
		/// </para><para>
		/// This function is only useful to access SFNT tables that are loaded by the sfnt, truetype, and opentype
		/// drivers. See <see cref="SfntTag"/> for a list.
		/// </para></remarks>
		/// <param name="tag">The index of the SFNT table.</param>
		/// <returns><para>
		/// A type-less pointer to the table. This will be 0 in case of error, or if the corresponding table was not
		/// found OR loaded from the file.
		/// </para><para>
		/// Use a typecast according to ‘tag’ to access the structure elements.
		/// </para></returns>
		public object GetSfntTable(SfntTag tag)
		{
			IntPtr tableRef = FT.FT_Get_Sfnt_Table(Reference, tag);

			if (tableRef == IntPtr.Zero)
				return null;

			switch (tag)
			{
				case SfntTag.Header:
					return new Header(tableRef);
				case SfntTag.HorizontalHeader:
					return new HoriHeader(tableRef);
				case SfntTag.MaxProfile:
					return new MaxProfile(tableRef);
				case SfntTag.OS2:
					return new OS2(tableRef);
				case SfntTag.Pclt:
					return new Pclt(tableRef);
				case SfntTag.Postscript:
					return new Postscript(tableRef);
				case SfntTag.VertHeader:
					return new VertHeader(tableRef);
				default:
					return null;
			}
		}

		/// <summary>
		/// Load any font table into client memory.
		/// </summary>
		/// <remarks>
		/// If you need to determine the table's length you should first call this function with ‘*length’ set to 0, as
		/// in the following example:
		/// <code>
		/// UIntPtr  length = 0;
		///
		///
		/// error = FT_Load_Sfnt_Table( face, tag, 0, NULL, &amp;length );
		/// if ( error ) { ... table does not exist ... }
		///
		/// buffer = malloc( length );
		/// if ( buffer == NULL ) { ... not enough memory ... }
		///
		/// error = FT_Load_Sfnt_Table( face, tag, 0, buffer, &amp;length );
		/// if ( error ) { ... could not load table ... }
		/// </code>
		/// </remarks>
		/// <param name="tag">
		/// The four-byte tag of the table to load. Use the value 0 if you want to access the whole font file.
		/// Otherwise, you can use one of the definitions found in the FT_TRUETYPE_TAGS_H file, or forge a new one with
		/// FT_MAKE_TAG.
		/// </param>
		/// <param name="offset">The starting offset in the table (or file if tag == 0).</param>
		/// <param name="buffer">
		/// The target buffer address. The client must ensure that the memory array is big enough to hold the data.
		/// </param>
		/// <param name="length"><para>
		/// If the ‘length’ parameter is NULL, then try to load the whole table. Return an error code if it fails.
		/// </para><para>
		/// Else, if ‘*length’ is 0, exit immediately while returning the table's (or file) full size in it.
		/// </para><para>
		/// Else the number of bytes to read from the table or file, from the starting offset.
		/// </para></param>
		public void LoadSfntTable(uint tag, int offset, IntPtr buffer, ref uint length)
		{
			Error err = FT.FT_Load_Sfnt_Table(Reference, tag, offset, buffer, ref length);

			if (err != Error.Ok)
				throw new FreeTypeException(err);
		}

		/// <summary>
		/// Return information on an SFNT table.
		/// </summary>
		/// <param name="tableIndex">
		/// The index of an SFNT table. The function returns <see cref="Error.TableMissing"/> for an invalid value.
		/// </param>
		/// <param name="tag">
		/// The name tag of the SFNT table. If the value is NULL, ‘table_index’ is ignored, and ‘length’ returns the
		/// number of SFNT tables in the font.
		/// </param>
		/// <returns>The length of the SFNT table (or the number of SFNT tables, depending on ‘tag’).</returns>
		public unsafe uint SfntTableInfo(uint tableIndex, SfntTag tag)
		{
			Error err = FT.FT_Sfnt_Table_Info(Reference, tableIndex, &tag, out var length);

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return length;
		}

		/// <summary>
		/// Only gets the number of SFNT tables.
		/// </summary>
		/// <returns>The number of SFNT tables.</returns>
		public unsafe uint SfntTableInfo()
		{
			Error err = FT.FT_Sfnt_Table_Info(Reference, 0, null, out var length);

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return length;
		}

		#endregion

		#region Type 1 Tables
		/// <see cref="FaceFlags.GlyphNames"/> flag.
		/// <summary><para>
		/// Return true if a given face provides reliable PostScript glyph names. This is similar to check the
		/// <see cref="FaceFlags.GlyphNames"/> flag, except that certain fonts (mostly TrueType) contain incorrect
		/// glyph name tables.
		/// </para><para>
		/// When this function returns true, the caller is sure that the glyph names returned by
		/// <see cref="GetGlyphName(uint, int)"/> are reliable.
		/// </para></summary>
		/// <returns>Boolean. True if glyph names are reliable.</returns>
		public bool HasPSGlyphNames() => FT.FT_Has_PS_Glyph_Names(Reference);

		/// <summary>
		/// Retrieve the <see cref="PostScript.FontInfo"/> structure corresponding to a given PostScript font.
		/// </summary>
		/// <remarks><para>
		/// The string pointers within the font info structure are owned by the face and don't need to be freed by the
		/// caller.
		/// </para><para>
		/// If the font's format is not PostScript-based, this function will return the
		/// <see cref="Error.InvalidArgument"/> error code.
		/// </para></remarks>
		/// <returns>Output font info structure pointer.</returns>
		public FontInfo GetPSFontInfo()
		{
			var err = FT.FT_Get_PS_Font_Info(Reference, out var fontInfoRec);
			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return fontInfoRec;
		}

		/// <summary>
		/// Retrieve the <see cref="PostScript.Private"/> structure corresponding to a given PostScript font.
		/// </summary>
		/// <remarks><para>
		/// The string pointers within the <see cref="PostScript.Private"/> structure are owned by the face and don't
		/// need to be freed by the caller.
		/// </para><para>
		/// If the font's format is not PostScript-based, this function returns the <see cref="Error.InvalidArgument"/>
		/// error code.
		/// </para></remarks>
		/// <returns>Output private dictionary structure pointer.</returns>
		public Private GetPSFontPrivate()
		{
			Error err = FT.FT_Get_PS_Font_Private(Reference, out var privateRec);

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return privateRec;
		}

		/// <summary>
		/// Retrieve the value for the supplied key from a PostScript font.
		/// </summary>
		/// <remarks><para>
		/// The values returned are not pointers into the internal structures of the face, but are ‘fresh’ copies, so
		/// that the memory containing them belongs to the calling application. This also enforces the ‘read-only’
		/// nature of these values, i.e., this function cannot be used to manipulate the face.
		/// </para><para>
		/// ‘value’ is a void pointer because the values returned can be of various types.
		/// </para><para>
		/// If either ‘value’ is NULL or ‘value_len’ is too small, just the required memory size for the requested
		/// entry is returned.
		/// </para><para>
		/// The ‘idx’ parameter is used, not only to retrieve elements of, for example, the FontMatrix or FontBBox, but
		/// also to retrieve name keys from the CharStrings dictionary, and the charstrings themselves. It is ignored
		/// for atomic values.
		/// </para><para>
		/// <see cref="PostScript.DictionaryKeys.BlueScale"/> returns a value that is scaled up by 1000. To get the
		/// value as in the font stream, you need to divide by 65536000.0 (to remove the FT_Fixed scale, and the x1000
		/// scale).
		/// </para><para>
		/// IMPORTANT: Only key/value pairs read by the FreeType interpreter can be retrieved. So, for example,
		/// PostScript procedures such as NP, ND, and RD are not available. Arbitrary keys are, obviously, not be
		/// available either.
		/// </para><para>
		/// If the font's format is not PostScript-based, this function returns the <see cref="Error.InvalidArgument"/>
		/// error code.
		/// </para></remarks>
		/// <param name="key">An enumeration value representing the dictionary key to retrieve.</param>
		/// <param name="idx">For array values, this specifies the index to be returned.</param>
		/// <param name="value">A pointer to memory into which to write the value.</param>
		/// <param name="valueLength">The size, in bytes, of the memory supplied for the value.</param>
		/// <returns>
		/// The amount of memory (in bytes) required to hold the requested value (if it exists, -1 otherwise).
		/// </returns>
		public int GetPSFontValue(DictionaryKeys key, uint idx, ref IntPtr value, int valueLength)
		{
			return FT.FT_Get_PS_Font_Value(Reference, key, idx, ref value, valueLength);
		}

		#endregion

		#region SFNT Names

		/// <summary>
		/// Retrieve the number of name strings in the SFNT ‘name’ table.
		/// </summary>
		/// <returns>The number of strings in the ‘name’ table.</returns>
		public uint GetSfntNameCount()
		{
			return FT.FT_Get_Sfnt_Name_Count(Reference);
		}

		/// <summary>
		/// Retrieve a string of the SFNT ‘name’ table for a given index.
		/// </summary>
		/// <remarks><para>
		/// The ‘string’ array returned in the ‘aname’ structure is not null-terminated. The application should
		/// deallocate it if it is no longer in use.
		/// </para><para>
		/// Use <see cref="GetSfntNameCount"/> to get the total number of available ‘name’ table entries, then do a
		/// loop until you get the right platform, encoding, and name ID.
		/// </para></remarks>
		/// <param name="idx">The index of the ‘name’ string.</param>
		/// <returns>The indexed <see cref="SfntName"/> structure.</returns>
		public SfntName GetSfntName(uint idx)
		{
			Error err = FT.FT_Get_Sfnt_Name(Reference, idx, out var nameRec);

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return nameRec;
		}

		#endregion

		#region BDF and PCF Files

		/// <summary>
		/// Retrieve a BDF font character set identity, according to the BDF specification.
		/// </summary>
		/// <remarks>
		/// This function only works with BDF faces, returning an error otherwise.
		/// </remarks>
		/// <param name="encoding">Charset encoding, as a C string, owned by the face.</param>
		/// <param name="registry">Charset registry, as a C string, owned by the face.</param>
		public void GetBdfCharsetId(out string encoding, out string registry)
		{
			Error err = FT.FT_Get_BDF_Charset_ID(Reference, out encoding, out registry);

			if (err != Error.Ok)
				throw new FreeTypeException(err);
		}

		/// <summary>
		/// Retrieve a BDF property from a BDF or PCF font file.
		/// </summary>
		/// <remarks><para>
		/// This function works with BDF and PCF fonts. It returns an error otherwise. It also returns an error if the
		/// property is not in the font.
		/// </para><para>
		/// A ‘property’ is a either key-value pair within the STARTPROPERTIES ... ENDPROPERTIES block of a BDF font or
		/// a key-value pair from the ‘info->props’ array within a ‘FontRec’ structure of a PCF font.
		/// </para><para>
		/// Integer properties are always stored as ‘signed’ within PCF fonts; consequently,
		/// <see cref="PropertyType.Cardinal"/> is a possible return value for BDF fonts only.
		/// </para><para>
		/// In case of error, ‘aproperty->type’ is always set to <see cref="PropertyType.None"/>.
		/// </para></remarks>
		/// <param name="propertyName">The property name.</param>
		/// <returns>The property.</returns>
		public Property GetBdfProperty(string propertyName)
		{

			Error err = FT.FT_Get_BDF_Property(Reference, propertyName, out var propertyRef);

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return new Property(propertyRef);
		}

		#endregion

		#region CID Fonts

		/// <summary>
		/// Retrieve the Registry/Ordering/Supplement triple (also known as the "R/O/S") from a CID-keyed font.
		/// </summary>
		/// <remarks>
		/// This function only works with CID faces, returning an error otherwise.
		/// </remarks>
		/// <param name="registry">The registry, as a C string, owned by the face.</param>
		/// <param name="ordering">The ordering, as a C string, owned by the face.</param>
		/// <param name="supplement">The supplement.</param>
		public void GetCidRegistryOrderingSupplement(out string registry, out string ordering, out int supplement)
		{
			Error err = FT.FT_Get_CID_Registry_Ordering_Supplement(Reference, out registry, out ordering, out supplement);

			if (err != Error.Ok)
				throw new FreeTypeException(err);
		}

		/// <summary>
		/// Retrieve the type of the input face, CID keyed or not. In constrast to the check
		/// <see cref="FaceFlags.CidKeyed"/> flag this function returns successfully also for
		/// CID-keyed fonts in an SNFT wrapper.
		/// </summary>
		/// <remarks>
		/// This function only works with CID faces and OpenType fonts, returning an error otherwise.
		/// </remarks>
		/// <returns>The type of the face as an FT_Bool.</returns>
		public bool GetCidIsInternallyCidKeyed()
		{
			Error err = FT.FT_Get_CID_Is_Internally_CID_Keyed(Reference, out var is_cid);

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return is_cid == 1;
		}

		/// <summary>
		/// Retrieve the CID of the input glyph index.
		/// </summary>
		/// <remarks>
		/// This function only works with CID faces and OpenType fonts, returning an error otherwise.
		/// </remarks>
		/// <param name="glyphIndex">The input glyph index.</param>
		/// <returns>The CID as an uint.</returns>
		public uint GetCidFromGlyphIndex(uint glyphIndex)
		{
			Error err = FT.FT_Get_CID_From_Glyph_Index(Reference, glyphIndex, out var cid);

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return cid;
		}

		#endregion

		#region PFR Fonts

		/// <summary>
		/// Return the outline and metrics resolutions of a given PFR face.
		/// </summary>
		/// <remarks>
		/// If the input face is not a PFR, this function will return an error. However, in all cases, it will return
		/// valid values.
		/// </remarks>
		/// <param name="outlineResolution">
		/// Outline resolution. This is equivalent to ‘face->units_per_EM’ for non-PFR fonts. Optional (parameter can
		/// be NULL).
		/// </param>
		/// <param name="metricsResolution">
		/// Metrics resolution. This is equivalent to ‘outline_resolution’ for non-PFR fonts. Optional (parameter can
		/// be NULL).
		/// </param>
		/// <param name="metricsXScale">
		/// A 16.16 fixed-point number used to scale distance expressed in metrics units to device sub-pixels. This is
		/// equivalent to ‘face->size->x_scale’, but for metrics only. Optional (parameter can be NULL).
		/// </param>
		/// <param name="metricsYScale">
		/// Same as ‘ametrics_x_scale’ but for the vertical direction. optional (parameter can be NULL).
		/// </param>
		public void GetPfrMetrics(out uint outlineResolution, out uint metricsResolution, out Fixed16Dot16 metricsXScale, out Fixed16Dot16 metricsYScale)
		{
			Error err = FT.FT_Get_PFR_Metrics(Reference, out outlineResolution, out metricsResolution, out var tmpXScale, out var tmpYScale);

			metricsXScale = Fixed16Dot16.FromRawValue((int)tmpXScale);
			metricsYScale = Fixed16Dot16.FromRawValue((int)tmpYScale);

			if (err != Error.Ok)
				throw new FreeTypeException(err);
		}

		/// <summary>
		/// Return the kerning pair corresponding to two glyphs in a PFR face. The distance is expressed in metrics
		/// units, unlike the result of <see cref="GetKerning"/>.
		/// </summary>
		/// <remarks><para>
		/// This function always return distances in original PFR metrics units. This is unlike
		/// <see cref="GetKerning"/> with the <see cref="KerningMode.Unscaled"/> mode, which always returns
		/// distances converted to outline units.
		/// </para><para>
		/// You can use the value of the ‘x_scale’ and ‘y_scale’ parameters returned by <see cref="GetPfrMetrics"/> to
		/// scale these to device sub-pixels.
		/// </para></remarks>
		/// <param name="left">Index of the left glyph.</param>
		/// <param name="right">Index of the right glyph.</param>
		/// <returns>A kerning vector.</returns>
		public FTVector GetPfrKerning(uint left, uint right)
		{
			Error err = FT.FT_Get_PFR_Kerning(Reference, left, right, out var vector);

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return vector;
		}

		/// <summary>
		/// Return a given glyph advance, expressed in original metrics units, from a PFR font.
		/// </summary>
		/// <remarks>
		/// You can use the ‘x_scale’ or ‘y_scale’ results of <see cref="GetPfrMetrics"/> to convert the advance to
		/// device sub-pixels (i.e., 1/64th of pixels).
		/// </remarks>
		/// <param name="glyphIndex">The glyph index.</param>
		/// <returns>The glyph advance in metrics units.</returns>
		public int GetPfrAdvance(uint glyphIndex)
		{
			Error err = FT.FT_Get_PFR_Advance(Reference, glyphIndex, out var advance);

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return advance;
		}

		#endregion

		#region Windows FNT Files

		/// <summary>
		/// Retrieve a Windows FNT font info header.
		/// </summary>
		/// <remarks>
		/// This function only works with Windows FNT faces, returning an error otherwise.
		/// </remarks>
		/// <returns>The WinFNT header.</returns>
		public Fnt.Header GetWinFntHeader()
		{
			Error err = FT.FT_Get_WinFNT_Header(Reference, out var headerRef);

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return new Fnt.Header(headerRef);
		}

		#endregion

		#region Font Formats

		/// <summary>
		/// Return a string describing the format of a given face, using values which can be used as an X11
		/// FONT_PROPERTY. Possible values are ‘TrueType’, ‘Type 1’, ‘BDF’, ‘PCF’, ‘Type 42’, ‘CID Type 1’, ‘CFF’,
		/// ‘PFR’, and ‘Windows FNT’.
		/// </summary>
		/// <returns>Font format string. NULL in case of error.</returns>
		public string GetX11FontFormat()
		{
			return Marshal.PtrToStringAnsi(FT.FT_Get_X11_Font_Format(Reference));
		}

		#endregion

		#region Gasp Table

		/// <summary>
		/// Read the ‘gasp’ table from a TrueType or OpenType font file and return the entry corresponding to a given
		/// character pixel size.
		/// </summary>
		/// <param name="ppem">The vertical character pixel size.</param>
		/// <returns>
		/// Bit flags (see <see cref="Gasp"/>), or <see cref="Gasp.NoTable"/> if there is no ‘gasp’ table in the face.
		/// </returns>
		public Gasp GetGasp(uint ppem) =>
			FT.FT_Get_Gasp(Reference, ppem);

		#endregion

		#region Quick retrieval of advance values

		/// <summary>
		/// Retrieve the advance value of a given glyph outline in a <see cref="Face"/>. By default, the unhinted
		/// advance is returned in font units.
		/// </summary>
		/// <remarks><para>
		/// This function may fail if you use <see cref="LoadFlags.AdvanceFlagFastOnly"/> and if the corresponding font
		/// backend doesn't have a quick way to retrieve the advances.
		/// </para><para>
		/// A scaled advance is returned in 16.16 format but isn't transformed by the affine transformation specified
		/// by <see cref="SetTransform()"/>.
		/// </para></remarks>
		/// <param name="glyphIndex">The glyph index.</param>
		/// <param name="flags">
		/// A set of bit flags similar to those used when calling <see cref="LoadGlyph"/>, used to determine what kind
		/// of advances you need.
		/// </param>
		/// <returns><para>
		/// The advance value, in either font units or 16.16 format.
		/// </para><para>
		/// If <see cref="LoadFlags.VerticalLayout"/> is set, this is the vertical advance corresponding to a vertical
		/// layout. Otherwise, it is the horizontal advance in a horizontal layout.
		/// </para></returns>
		public Fixed16Dot16 GetAdvance(uint glyphIndex, LoadFlags flags)
		{
			Error err = FT.FT_Get_Advance(Reference, glyphIndex, flags, out var padvance);

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			return Fixed16Dot16.FromRawValue((int)padvance);
		}

		/// <summary>
		/// Retrieve the advance values of several glyph outlines in an
		/// <see cref="Face"/>. By default, the unhinted advances are returned
		/// in font units.
		/// </summary>
		/// <remarks><para>
		/// This function may fail if you use
		/// <see cref="LoadFlags.AdvanceFlagFastOnly"/> and if the
		/// corresponding font backend doesn't have a quick way to retrieve the
		/// advances.
		/// </para><para>
		/// Scaled advances are returned in 16.16 format but aren't transformed
		/// by the affine transformation specified by
		/// <see cref="SetTransform()"/>.
		/// </para></remarks>
		/// <param name="start">The first glyph index.</param>
		/// <param name="count">The number of advance values you want to retrieve.</param>
		/// <param name="flags">A set of bit flags similar to those used when calling <see cref="LoadGlyph"/>.</param>
		/// <returns><para>The advances, in either font units or 16.16 format. This array must contain at least ‘count’ elements.
		/// </para><para>
		/// If <see cref="LoadFlags.VerticalLayout"/> is set, these are the vertical advances corresponding to a vertical layout. Otherwise, they are the horizontal advances in a horizontal layout.</para></returns>
		public unsafe Fixed16Dot16[] GetAdvances(uint start, uint count, LoadFlags flags)
		{
			Error err = FT.FT_Get_Advances(Reference, start, count, flags, out var advPtr);

			if (err != Error.Ok)
				throw new FreeTypeException(err);

			//create a new array and copy the data from the pointer over
			Fixed16Dot16[] advances = new Fixed16Dot16[count];
			IntPtr* ptr = (IntPtr*)advPtr;

			for (int i = 0; i < count; i++)
				advances[i] = Fixed16Dot16.FromRawValue((int)ptr[i]);

			return advances;
		}

		#endregion

		#region OpenType Validation

		/// <summary>
		/// Validate various OpenType tables to assure that all offsets and indices are valid. The idea is that a
		/// higher-level library which actually does the text layout can access those tables without error checking
		/// (which can be quite time consuming).
		/// </summary>
		/// <remarks><para>
		/// This function only works with OpenType fonts, returning an error otherwise.
		/// </para><para>
		/// After use, the application should deallocate the five tables with <see cref="OpenTypeFree"/>. A NULL value
		/// indicates that the table either doesn't exist in the font, or the application hasn't asked for validation.
		/// </para></remarks>
		/// <param name="flags">A bit field which specifies the tables to be validated.</param>
		/// <param name="baseTable">A pointer to the BASE table.</param>
		/// <param name="gdefTable">A pointer to the GDEF table.</param>
		/// <param name="gposTable">A pointer to the GPOS table.</param>
		/// <param name="gsubTable">A pointer to the GSUB table.</param>
		/// <param name="jstfTable">A pointer to the JSTF table.</param>
		public void OpenTypeValidate(OpenTypeValidationFlags flags, out IntPtr baseTable, out IntPtr gdefTable, out IntPtr gposTable, out IntPtr gsubTable, out IntPtr jstfTable)
		{
			Error err = FT.FT_OpenType_Validate(Reference, flags, out baseTable, out gdefTable, out gposTable, out gsubTable, out jstfTable);

			if (err != Error.Ok)
				throw new FreeTypeException(err);
		}

		/// <summary>
		/// Free the buffer allocated by OpenType validator.
		/// </summary>
		/// <remarks>
		/// This function must be used to free the buffer allocated by <see cref="OpenTypeValidate"/> only.
		/// </remarks>
		/// <param name="table">The pointer to the buffer that is allocated by <see cref="OpenTypeValidate"/>.</param>
		public void OpenTypeFree(IntPtr table)
		{
			FT.FT_OpenType_Free(Reference, table);
		}

		#endregion

		#region TrueTypeGX/AAT Validation

		/// <summary>
		/// Validate various TrueTypeGX tables to assure that all offsets and indices are valid. The idea is that a
		/// higher-level library which actually does the text layout can access those tables without error checking
		/// (which can be quite time consuming).
		/// </summary>
		/// <remarks><para>
		/// This function only works with TrueTypeGX fonts, returning an error otherwise.
		/// </para><para>
		/// After use, the application should deallocate the buffers pointed to by each ‘tables’ element, by calling
		/// <see cref="TrueTypeGXFree"/>. A NULL value indicates that the table either doesn't exist in the font, the
		/// application hasn't asked for validation, or the validator doesn't have the ability to validate the sfnt
		/// table.
		/// </para></remarks>
		/// <param name="flags">A bit field which specifies the tables to be validated.</param>
		/// <param name="tables">
		/// The array where all validated sfnt tables are stored. The array itself must be allocated by a client.
		/// </param>
		/// <param name="tableLength">
		/// The size of the ‘tables’ array. Normally, FT_VALIDATE_GX_LENGTH should be passed.
		/// </param>
		public void TrueTypeGXValidate(TrueTypeValidationFlags flags, byte[][] tables, uint tableLength) =>
			FT.FT_TrueTypeGX_Validate(Reference, flags, tables, tableLength);

		/// <summary>
		/// Free the buffer allocated by TrueTypeGX validator.
		/// </summary>
		/// <remarks>
		/// This function must be used to free the buffer allocated by <see cref="TrueTypeGXValidate"/> only.
		/// </remarks>
		/// <param name="table">The pointer to the buffer allocated by <see cref="TrueTypeGXValidate"/>.</param>
		public void TrueTypeGXFree(IntPtr table) =>
			FT.FT_TrueTypeGX_Free(Reference, table);

		/// <summary><para>
		/// Validate classic (16-bit format) kern table to assure that the offsets and indices are valid. The idea is
		/// that a higher-level library which actually does the text layout can access those tables without error
		/// checking (which can be quite time consuming).
		/// </para><para>
		/// The ‘kern’ table validator in <see cref="TrueTypeGXValidate"/> deals with both the new 32-bit format and
		/// the classic 16-bit format, while <see cref="ClassicKernValidate"/> only supports the classic 16-bit format.
		/// </para></summary>
		/// <remarks>
		/// After use, the application should deallocate the buffers pointed to by ‘ckern_table’, by calling
		/// <see cref="ClassicKernFree"/>. A NULL value indicates that the table doesn't exist in the font.
		/// </remarks>
		/// <param name="flags">A bit field which specifies the dialect to be validated.</param>
		/// <returns>A pointer to the kern table.</returns>
		public IntPtr ClassicKernValidate(ClassicKernValidationFlags flags)
		{
			FT.FT_ClassicKern_Validate(Reference, flags, out var ckernRef);
			return ckernRef;
		}

		/// <summary>
		/// Free the buffer allocated by classic Kern validator.
		/// </summary>
		/// <remarks>
		/// This function must be used to free the buffer allocated by <see cref="ClassicKernValidate"/> only.
		/// </remarks>
		/// <param name="table">
		/// The pointer to the buffer that is allocated by <see cref="ClassicKernValidate"/>.
		/// </param>
		public void ClassicKernFree(IntPtr table)
		{
			FT.FT_ClassicKern_Free(Reference, table);
		}

		#endregion

		internal void AddChildSize(FTSize child)
		{
			childSizes.Add(child);
		}

		internal void RemoveChildSize(FTSize child)
		{
			childSizes.Remove(child);
		}

		/// <summary>
		/// Dispose this face.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			foreach (FTSize s in childSizes)
				s.Dispose();

			childSizes.Clear();

			FT.FT_Done_Face(Reference);

			// removes itself from the parent Library, with a check to prevent this from happening when Library is
			// being disposed (Library disposes all it's children with a foreach loop, this causes an
			// InvalidOperationException for modifying a collection during enumeration)
			if (!parentLibrary.IsDisposed)
				parentLibrary.RemoveChildFace(this);

			if (_hasMemoryFaceHandle)
				_memoryFaceHandle.Dispose();

			Disposed?.Invoke(this, EventArgs.Empty);
		}

		#endregion
	}
}
