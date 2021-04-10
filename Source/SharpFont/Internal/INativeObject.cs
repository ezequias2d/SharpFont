using System;
using System.Collections.Generic;
using System.Text;

namespace SharpFont.Internal
{
	public interface INativeObject
	{
		IntPtr Reference { get; }
	}
}
