﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = System.Random;

namespace Sodalite.Utilities
{
	/// <summary>
	/// Collection of static utilities and extension methods used for Sodalite
	/// </summary>
	public static class SodaliteUtils
	{
		private static Random? _random;

		/// <summary>
		///	Helper method for loading a texture from a path
		/// </summary>
		/// <param name="file">Path to the file on disk</param>
		/// <returns>The loaded texture</returns>
		public static Texture2D LoadTextureFromFile(string file)
		{
			return LoadTextureFromBytes(File.ReadAllBytes(file));
		}

		/// <summary>
		///	Helper method for loading a texture from some bytes
		/// </summary>
		/// <param name="bytes">The bytes of the texture</param>
		/// <returns>The loaded texture</returns>
		public static Texture2D LoadTextureFromBytes(byte[] bytes)
		{
			Texture2D tex = new(0, 0);
			tex.LoadImage(bytes);
			return tex;
		}

		/// <summary>
		/// Extension method for simplifying loading files from an assembly's embedded
		/// resources.
		/// </summary>
		/// <param name="asm">The assembly to load a file from</param>
		/// <param name="file">The file to load</param>
		/// <returns>The bytes of the embedded file</returns>
		/// <exception cref="FileNotFoundException">File was not found inside the assembly</exception>
		public static byte[] GetResource(this Assembly asm, string file)
		{
			// Get the resource's actual name
			string? resource = asm.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(file));
			if (resource is null) throw new FileNotFoundException($"A resource with the name '{file}' was not found.", file);

			// Read the stream into a byte array
			using Stream stream = asm.GetManifestResourceStream(resource) ?? throw new FileNotFoundException("Somehow the file was found but the stream couldn't be gotten.");
			byte[] buffer = new byte[stream.Length];
			stream.Read(buffer, 0, buffer.Length);

			// Return the array
			return buffer;
		}

		/// <summary>
		///	Extension method for catching TypeReflectionLoadExceptions when using Assembly.GetTypes()
		/// </summary>
		/// <param name="asm">The assembly to get the types from</param>
		/// <returns>The types from the assembly that were able to load properly</returns>
		public static IEnumerable<Type> GetTypesSafe(this Assembly asm)
		{
			try
			{
				return asm.GetTypes();
			}
			catch (ReflectionTypeLoadException e)
			{
				return e.Types.Where(t => t is not null);
			}
		}

		/// <summary>
		///	Extension method to check if the value has a given flag
		/// </summary>
		/// <param name="value">The value to check on</param>
		/// <param name="flag">The flag to check for</param>
		/// <typeparam name="TEnum">The type of the enum</typeparam>
		/// <returns>True if the value has a given flag set</returns>
		public static bool HasFlag<TEnum>(this TEnum value, TEnum flag) where TEnum : Enum
		{
			return (Convert.ToInt32(value) & Convert.ToInt32(flag)) != 0;
		}

		/// <summary>
		/// Returns a random item from this list
		/// </summary>
		public static T GetRandom<T>(this IList<T> list)
		{
			// Make sure there is at least one item in the list
			if (list.Count < 1)
				throw new InvalidOperationException("Cannot get random item from empty list!");

			// Make sure our random is set and return a random item
			_random ??= new Random();
			return list[_random.Next(list.Count)];
		}

		/// <summary>
		/// Enumerator wrapper to handle a specific exception that happens while it is being enumerated.
		/// This method is a typed version of <see cref="TryCatch"/> that lets you handle specific exceptions.
		/// </summary>
		/// <param name="this">The enumerator to wrap</param>
		/// <param name="handler">The exception handler</param>
		/// <typeparam name="T">The type of the exception to handle</typeparam>
		/// <returns>The wrapped enumerator</returns>
		public static IEnumerator TryCatch<T>(this IEnumerator @this, Action<T> handler) where T : Exception
		{
			bool MoveNext()
			{
				try
				{
					return @this.MoveNext();
				}
				catch (T e)
				{
					handler(e);
					return false;
				}
			}

			while (MoveNext())
				yield return @this.Current;
		}

		/// <summary>
		/// Enumerator wrapper to handle exceptions that happen while it is being enumerated.
		/// This method should only be used when you don't know what kind of exception your coroutine will throw.
		/// If you do know, you should use <see cref="TryCatch{T}"/> instead.
		/// </summary>
		/// <param name="this">The enumerator to wrap</param>
		/// <param name="handler">The exception handler</param>
		/// <returns>The wrapped enumerator</returns>
		public static IEnumerator TryCatch(this IEnumerator @this, Action<Exception> handler) => @this.TryCatch<Exception>(handler);
	}
}
