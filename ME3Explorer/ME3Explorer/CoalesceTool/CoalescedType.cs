﻿using System.ComponentModel;

namespace MassEffect3.CoalesceTool
{
	public enum CoalescedType
	{
		//[Display(Name = "Binary")]
		[Description("Binary Coalesced file.")]
		Binary,

		//[Display(Name = "Xml")]
		[Description("Xml Coalesced file.")]
		Xml
	}
}
