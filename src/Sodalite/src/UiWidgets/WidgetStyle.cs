﻿using System;
using UnityEngine;

namespace Sodalite.UiWidgets
{
	/// <summary>
	///	Base class for widget styles. Any kind of commonly configurable element
	/// properties can be set in this style class.
	/// </summary>
	public class WidgetStyle : MonoBehaviour
	{
		// Internal references to the defaults
		internal static Font? DefaultTextFont;
		internal static Sprite? DefaultButtonSprite;

		/// <summary>
		/// The text font
		/// </summary>
		public Font TextFont { get; set; } = DefaultTextFont is not null && DefaultTextFont
			? DefaultTextFont
			: throw new InvalidOperationException("Default text font was null!");

		/// <summary>
		/// The text color
		/// </summary>
		public Color TextColor { get; set; } = Color.white;

		/// <summary>
		/// Sprite used for buttons
		/// </summary>
		public Sprite ButtonSprite { get; set; } = DefaultButtonSprite is not null && DefaultButtonSprite
			? DefaultButtonSprite
			: throw new InvalidOperationException("Default text font was null!");

		/// <summary>
		/// The button sprite tint while it is unselected
		/// </summary>
		public Color ButtonColorUnselected { get; set; } = new(27 / 255f, 73 / 255f, 155 / 255f, 160 / 255f);

		/// <summary>
		/// The button sprite tint when selected (hovered)
		/// </summary>
		public Color ButtonColorSelected { get; set; } = new(192 / 255f, 202 / 255f, 222 / 255f, 216 / 255f);
	}
}
