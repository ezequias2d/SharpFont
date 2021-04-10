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

namespace SharpFont
{
	/// <summary>
	/// The renderer module class descriptor.
	/// </summary>
	public class RendererClass : NativeObject
	{

		#region Constructors

		internal RendererClass(IntPtr reference) : base(reference)
		{
		}

		#endregion

		#region Properties

		private ref RendererClassRec Rec => ref PInvokeHelper.PtrToRefStructure<RendererClassRec>(Reference);

		/// <summary>
		/// Gets the root <see cref="ModuleClass"/> fields.
		/// </summary>
		public ModuleClass Root => new ModuleClass(Reference);

		/// <summary>
		/// Gets the glyph image format this renderer handles.
		/// </summary>
		public GlyphFormat Format => Rec.glyph_format;

		/// <summary>
		/// Gets a method used to render the image that is in a given glyph slot into a bitmap.
		/// </summary>
		public IntPtr RenderGlyph => Rec.render_glyph;

		/// <summary>
		/// Gets a method used to transform the image that is in a given glyph slot.
		/// </summary>
		public IntPtr TransformGlyph => Rec.transform_glyph;

		/// <summary>
		/// Gets a method used to access the glyph's cbox.
		/// </summary>
		public IntPtr GetGlyphCBox => Rec.get_glyph_cbox;

		/// <summary>
		/// Gets a method used to pass additional parameters.
		/// </summary>
		public IntPtr SetMode => Rec.set_mode;

		/// <summary>
		/// Gets a pointer to its raster's class.
		/// </summary>
		/// <remarks>For <see cref="GlyphFormat.Outline"/> renderers only.</remarks>
		public RasterFuncs RasterClass
		{
			get
			{
				unsafe
				{
					return *(RasterFuncs*)Rec.raster_class;
				}
			}
		}
		#endregion
	}
}
