﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace QuickFont
{
	class Helper
	{
		public static T[] ToArray<T>(ICollection<T> collection)
		{
			T[] output = new T[collection.Count];
			collection.CopyTo(output, 0);
			return output;
		}

		/// <summary>
		/// Ensures GL.End() is called
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="code"></param>
		public static void SafeGLBegin(BeginMode mode, Action code)
		{
			GL.Begin(mode);

			code();

			GL.End();
		}

		/// <summary>
		/// Ensures that state is disabled
		/// </summary>
		/// <param name="cap"></param>
		/// <param name="code"></param>
		public static void SafeGLEnable(EnableCap cap, Action code)
		{
			var en = GL.IsEnabled(cap);
			if (!en)
			GL.Enable(cap);

			code();
			if (!en)
			GL.Disable(cap);
		}

		/// <summary>
		/// Ensures that multiple states are disabled
		/// </summary>
		/// <param name="cap"></param>
		/// <param name="code"></param>
		public static void SafeGLEnable(EnableCap[] caps, Action code)
		{
			bool[] shouldrestore = new bool[caps.Length];
			for (int i = 0; i < caps.Length; i++)
			{
				if (GL.IsEnabled(caps[i]))
					shouldrestore[i] = false;
				else
				{
					shouldrestore[i] = true;
					GL.Enable(caps[i]);
				}
			}
			code();

			for (int i = 0; i < caps.Length; i++)
			{
				if (shouldrestore[i])
					GL.Disable(caps[i]);
			}
		}

		public static void SafeGLEnableClientStates(ArrayCap[] caps, Action code)
		{
			GL.PushClientAttrib(ClientAttribMask.ClientAllAttribBits);
			foreach (var cap in caps)
				GL.EnableClientState(cap);

			code();

			foreach (var cap in caps)
				GL.DisableClientState(cap);
			GL.PopClientAttrib();
		}

		public static int ToRgba(Color color)
		{
			return color.A << 24 | color.B << 16 | color.G << 8 | color.R;
		}
	}
}
