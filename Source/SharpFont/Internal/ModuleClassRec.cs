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

namespace SharpFont.Internal
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct ModuleClassRec
	{
		internal uint module_flags;
		internal IntPtr module_size;

		internal IntPtr module_name;
		internal IntPtr module_version;
		internal IntPtr module_requires;

		internal IntPtr module_interface;

		internal IntPtr module_init;
		internal IntPtr module_done;
		internal IntPtr get_interface;

		public string ModuleName => Marshal.PtrToStringAnsi(module_name);
		public Fixed16Dot16 Version => Fixed16Dot16.FromRawValue((int)module_version);
		public Fixed16Dot16 Requires => Fixed16Dot16.FromRawValue((int)module_requires);

		public ModuleConstructor ModuleInit => Marshal.GetDelegateForFunctionPointer<ModuleConstructor>(module_init);
		public ModuleDestructor ModuleDone => Marshal.GetDelegateForFunctionPointer<ModuleDestructor>(module_done);
		public ModuleRequester GetInterface => Marshal.GetDelegateForFunctionPointer<ModuleRequester>(get_interface);
	}
}
