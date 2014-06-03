using System;

namespace Web.AdminTool.Utilities.Enums
{
	/// <summary>
	/// Can be used to add another description to an enum.
	/// </summary>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public sealed class LongDescriptionAttribute : Attribute
	{
		public LongDescriptionAttribute(string longDescription)
		{
			LongDescription = longDescription;
		}

		public string LongDescription { get; private set; }
	}
}