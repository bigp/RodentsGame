/** Title:  
  *----------------------------------------------------------
  * File:   ExtensionMethods.cs
  * Author: Brent Kilbasco
  *
  * Copyright (c) Egg Roll Digital 2015
  *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

using Points = System.Collections.Generic.List<UnityEngine.Vector2>;

namespace ExtensionMethods {
		
		
	/**~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
	/// 
	///     Class ExtensionMethods
	///------------------------------------------
	///<summary>
	/// Collection of class extension methods. 
	///</summary>
	/**~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/ 
	public static class ExtensionMethods{
	
		/**~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	      *  ToCurrencyString
	      * -----------------------------------------   
	      * ~ Accepts an int value and returns a string
	      *     money value formatted with commas.
	      *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/ 
		public static string ToCurrencyString(this int pCurrencyValue){
			return pCurrencyValue.ToString("n0");
		}//END ToCurrencyString

		/**~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	      *  ToCurrencyString
	      * -----------------------------------------   
	      * ~ Accepts an float value and returns a string
	      *     money value formatted with commas.
	      *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/ 
		public static string ToCurrencyString(this float pCurrencyValue){

			string retString = pCurrencyValue.ToString();
			string[] retStringAsFloat = retString.Split('.');
			
			if( retStringAsFloat[0].Length > 3 )
				for( int i = retStringAsFloat[0].Length-4; i >= 0; i -= 3 )
					retStringAsFloat[0] = retStringAsFloat[0].Insert( i+1, "," );

			if(retStringAsFloat.Length > 1)
				retString = retStringAsFloat[0] + "." + retStringAsFloat[1];
			
			return retString;
			
		}//END ToCurrencyString


		/**~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
	      *  ToTwoDecimals
	      * -----------------------------------------   
	      * ~ Accept
	      *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/ 
		public static float ToTwoDecimals(this float pValue){

			return (float)Math.Round( pValue, 2 );

		}//END ToTwoDecimals


		/// <summary>
		/// DOText function for TextMeshProUGUI components
		/// </summary>
		public static Tweener DOText(this TextMeshProUGUI tmproText, string endValue, float duration, bool richTextEnabled = true, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null) {
			
			return DOTween.To(() => tmproText.text, x => tmproText.text = x, endValue, duration).SetOptions(richTextEnabled, scrambleMode, scrambleChars);
			
		} 
		
		
		/// <summary>
		/// Align the left side of the given RectTransform to the left side of this ScrollRect
		/// </summary>
		public static void LeftAlign(this ScrollRect scrollRect, RectTransform targetRectTransform) {
			scrollRect.HorizontallyAlign(targetRectTransform, 0f);
		}
		
		/// <summary>
		/// Align the right side of the given RectTransform to the right side of this ScrollRect
		/// </summary>
		public static void RightAlign(this ScrollRect scrollRect, RectTransform targetRectTransform) {
			scrollRect.HorizontallyAlign(targetRectTransform, 1f);
		}
		
		/// <summary>
		/// Horizontally align the center of the given RectTransform to the center side of this ScrollRect
		/// </summary>
		public static void HorizontallyCenter(this ScrollRect scrollRect, RectTransform targetRectTransform) {
			scrollRect.HorizontallyAlign(targetRectTransform, 0.5f);
		}
		
		/// <summary>
		/// Horizontally align the ScrollRect to a RectTransform without scrolling past the edge of the content
		/// <param name="alignment"> 0 = left aligned, 1 = right aligned</param>
		/// </summary>
		public static void HorizontallyAlign(this ScrollRect scrollRect, RectTransform targetRectTransform, float alignment) {
			
			scrollRect.horizontalNormalizedPosition = scrollRect.CalculateHorizontalNormalizedPosition(targetRectTransform, alignment);
		}
		
		/// <summary>
		/// Get the normalized position of horizontally aligning the ScrollRect to a RectTransform without scrolling past the edge of the content
		/// <param name="alignment"> 0 = left aligned, 1 = right aligned</param>
		/// </summary>
		public static float CalculateHorizontalNormalizedPosition(this ScrollRect scrollRect, RectTransform targetRectTransform, float alignment) {
			
			// Get world position of the point of the traget transform
			// Alignment of 0 is left side of transform, 1 is right side
			Vector2 targetCenterLocalPosition = targetRectTransform.rect.center;
			float targetCenterOffset = targetRectTransform.rect.width * (alignment - 0.5f);
			Vector3 targetWorldPosition = targetRectTransform.TransformPoint(targetCenterLocalPosition + Vector2.right * targetCenterOffset);
			
			// Convert world position to position relative to scrollRect
			float targetXPosition = scrollRect.content.InverseTransformPoint(targetWorldPosition).x;
			
			// Calculate the new normalized position for the scroll rect by accounting
			// for the extra space on the left and right of the content where it can't 
			// be aligned without scrolling past the edge
			float contentWidth = scrollRect.content.rect.width;
			float scrollRectWidth = ((RectTransform)scrollRect.transform).rect.width;
			float newNormalizedPosition = (targetXPosition - (scrollRectWidth * alignment)) / (contentWidth - scrollRectWidth);
			
			// Clamp value between 0 and 1 so it dosen't scroll past the edge of the content
			return Mathf.Clamp01(newNormalizedPosition);
		}
		
		
		/// <summary>
		/// Align the bottom side of the given RectTransform to the bottom side of this ScrollRect
		/// </summary>
		public static void BottomAlign(this ScrollRect scrollRect, RectTransform targetRectTransform) {
			scrollRect.VerticallyAlign(targetRectTransform, 0f);
		}
		
		/// <summary>
		/// Align the top side of the given RectTransform to the top side of this ScrollRect
		/// </summary>
		public static void TopAlign(this ScrollRect scrollRect, RectTransform targetRectTransform) {
			scrollRect.VerticallyAlign(targetRectTransform, 1f);
		}
		
		/// <summary>
		/// Vertically align the center of the given RectTransform to the center side of this ScrollRect
		/// </summary>
		public static void VerticallyCenter(this ScrollRect scrollRect, RectTransform targetRectTransform) {
			scrollRect.VerticallyAlign(targetRectTransform, 0.5f);
		}
		
		/// <summary>
		/// Vertically align the ScrollRect to a RectTransform without scrolling past the edge of the content
		/// <param name="alignment"> 0 = bottom aligned, 1 = top aligned</param>
		/// </summary>
		public static void VerticallyAlign(this ScrollRect scrollRect, RectTransform targetRectTransform, float alignment) {
			
			scrollRect.verticalNormalizedPosition = scrollRect.CalculateVerticalNormalizedPosition(targetRectTransform, alignment);
		}
		
		/// <summary>
		/// Get the normalized position of vertically aligning the ScrollRect to a RectTransform without scrolling past the edge of the content
		/// <param name="alignment"> 0 = bottom aligned, 1 = top aligned</param>
		/// </summary>
		public static float CalculateVerticalNormalizedPosition(this ScrollRect scrollRect, RectTransform targetRectTransform, float alignment) {
			
			// Get world position of the point of the traget transform
			// Alignment of 0 is bottom side of transform, 1 is top side
			Vector2 targetCenterLocalPosition = targetRectTransform.rect.center;
			float targetCenterOffset = targetRectTransform.rect.height * (alignment - 0.5f);
			Vector3 targetWorldPosition = targetRectTransform.TransformPoint(targetCenterLocalPosition + Vector2.up * targetCenterOffset);

			// Convert world position to position relative to scrollRect
			float targetYPosition = scrollRect.content.InverseTransformPoint(targetWorldPosition).y;
			
			// Calculate the new normalized position for the scroll rect by accounting
			// for the extra space on the top and bottom of the content where it can't 
			// be aligned without scrolling past the edge
			float contentHeight = scrollRect.content.rect.height;
			float scrollRectHeight = ((RectTransform)scrollRect.transform).rect.height;
			float newNormalizedPosition = (targetYPosition - (scrollRectHeight * alignment)) / (contentHeight - scrollRectHeight);
			
			// Clamp value between 0 and 1 so it dosen't scroll past the edge of the content
			return Mathf.Clamp01(newNormalizedPosition);
		}

		//////////////////////////////////////////////////////////// For Strings:

		public static string Format(this string str, params object[] args) {
			return string.Format(str, args);
		}

		public static string Join(this string[] strArr, string delim) {
			return string.Join(delim, strArr);
		}

		public static string Times(this string str, int repeatCount) {
			string output = "";
			while(repeatCount>0) {
				output += str;
				repeatCount--;
			}
			return output;
		}

		/* Converts a sentence like "titled case like this" to "Titled Case Like This".
		 * (see: http://stackoverflow.com/a/1206029/468206 for reference)
		 */
		public static string ToTitleCase(this string str) {
			TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
			return textInfo.ToTitleCase(str);
		}

		public static List<T> Clone<T>(this List<T> list) {
			List<T> dup = new List<T>();
			dup.AddRange(list);
			return dup;
		}

		public static string ToBase64(this string str) {
			byte[] plainTextBytes = Encoding.UTF8.GetBytes(str);
			return Convert.ToBase64String(plainTextBytes);
		}

		public static string FromBase64(this string base64str) {
			byte[] base64bytes = Convert.FromBase64String(base64str);
			return Encoding.UTF8.GetString(base64bytes);
		}

		public static string Brackets(this string input, params string[] replaces) {
			return StringUtils.ReplaceBrackets(input, replaces);
		}

		public static string BracketsByName(this string input, object replaces) {
			return StringUtils.ReplaceBracketsByName(input, replaces);
		}

		//public static Color32 ToHexColor32(this string hex, bool isARGB = false) {
		//	int argb = Int32.Parse(hex.Replace("#", ""), NumberStyles.HexNumber);
		//	byte[] channels = new byte[] {
		//		(byte) ((argb >> 24) & 0xff),
		//		(byte) ((argb >> 16) & 0xff),
		//		(byte) ((argb >> 8) & 0xff),
		//		(byte) ((argb >> 0) & 0xff),
		//	};

		//	if (isARGB) return new Color32(channels[1], channels[2], channels[3], channels[0]);
		//	return new Color32(channels[0], channels[1], channels[2], channels[3]);
		//}

		public static string ForEach(this string str, Func<string, string> iterator) {
			string result = "";

			for (int s=0; s<str.Length; s++) {
				result += iterator("" + str[s]);
			}
			
			return result;
		}

		public static Color ToHexColor(this string hex, bool isRGBA=false) {
			hex = hex.Replace("#", "");

			//Convert shorthand hex (ie: #fff or #ffff) to full length hexcodes (6 or 8 hex symbols):
			if(hex.Length<5) hex = hex.ForEach((string ch) => ch + ch);
			if(hex.Length==6) hex = "ff" + hex;

			int argb = Int32.Parse(hex, NumberStyles.HexNumber);

			float[] channels = new float[] {
				(float) ((argb >> 24) & 0xff) / 0xff,
				(float) ((argb >> 16) & 0xff) / 0xff,
				(float) ((argb >> 8) & 0xff) / 0xff,
				(float) ((argb >> 0) & 0xff) / 0xff,
			};

			if (isRGBA) {
				return new Color(channels[0], channels[1], channels[2], channels[3]);
			} else {
				return new Color(channels[1], channels[2], channels[3], channels[0]);
			}
		}

		public static int CountChar(this string str, string sub) {
			int count = 0;
			int lastIndex = 0;
			while(lastIndex!=-1) {
				lastIndex = str.IndexOf(sub, lastIndex);
				if (lastIndex > -1) {
					count++;
					lastIndex += sub.Length;
				}
			}

			return count;
		}

		public static T ToEnum<T>(this string str) {
			return (T) Enum.Parse(typeof(T), str);
		}

		private static Regex _IS_NUMERIC = new Regex("^\\d+$");

		public static bool IsNumeric(this string value) {
			return _IS_NUMERIC.IsMatch(value);
		}

		//////////////////////////////////////////////////////////// For Colors:

		public static Color32 ToColor32(this Color clr) {
			return new Color32((byte)(clr.r * 0xff), (byte)(clr.g * 0xff), (byte)(clr.b * 0xff), (byte)(clr.a * 0xff));
		}

		//////////////////////////////////////////////////////////// For Math:
		
		//Convenience math functions (may be a bit excessive on the convenience... meh!)

		public static float Clamp(this float value, float min, float max) {
			return Mathf.Clamp(value, min, max);
		}

		public static float Lerp(this float from, float to, float interpolate=0.5f) {
			return Mathf.Lerp(from, to, interpolate);
		}

		public static Vector2 Lerp(this Vector2 from, Vector2 to, float interpolate=0.5f) {
			return Vector2.Lerp(from, to, interpolate);
		}

		public static Vector2 CloneOffset(this Vector2 from, float offsetX, float offsetY) {
			return new Vector2(from.x + offsetX, from.y + offsetY);
		}

		public static float AngleRelativeTo(this Vector2 from, Vector2 to, bool flipY) {
			return AngleRelativeTo(from, to, 1, flipY ? -1 : 1);
		}

		public static float AngleRelativeTo(this Vector2 from, Vector2 to, float flipX=1, float flipY=1) {
			return Mathf.Atan2((to.y - from.y) * flipY, (to.x - from.x) * flipX) * Mathf.Rad2Deg;
		}

		public static int Mod(this int value, int n) {
			return ((value % n) + n) % n;
		}

		////////////////////////////////////////////////////////////

		public static float DistanceAll(this Points points) {
			if (points.Count < 2) return 0;
			float dist = 0;
			Vector2 prevPoint = points[0];
			for (int p = 1; p < points.Count; p++) {
				Vector2 point = points[p];
				dist += Vector2.Distance(prevPoint, point);
				prevPoint = point;
			}

			return dist;
		}

		//////////////////////////////////////////////////////////// For Arrays/Lists:

		public static T Last<T>(this T[] arr) {
			return arr.Length==0 ? default(T)  : arr[arr.Length - 1];
		}

		public static T Last<T>(this List<T> list) {
			return list.Count==0 ? default(T) : list[list.Count - 1];
		}

		public static T Pop<T>(this List<T> list) {
			T last = list.Last();
			list.RemoveAt(list.Count - 1);
			return last;
		}

		public static T Shift<T>(this List<T> list) {
			T item = list[0];
			list.RemoveAt(0);
			return item;
		}

		//Returns TRUE if successfully added, FALSE if already in the list:
		public static bool AddUnique<T>(this List<T> list, T other) {
			if (list.Contains(other)) return false;
			list.Add(other);
			return true;
		}

		public static void AddUniques<T>(this List<T> list, List<T> others) {
			foreach (T other in others) {
				if (list.Contains(other)) continue;
				list.Add(other);
			}
		}

		public static void AddUniquesBetween(this List<int> list, int start, int end) {
			for (int i = start; i <= end; i++) {
				if(list.Contains(i)) continue;
				list.Add(i);
			}
		}

		public static void AddMany<T>(this List<T> list, params T[] items) {
			foreach (T item in items) {
				list.Add(item);
			}
		}

		public static List<T> AddRanges<T>(this List<T> list, params List<T>[] otherLists) {
			foreach(List<T> otherList in otherLists) {
				list.AddRange(otherList);
			}

			return list;
		}

		public static bool ContainsAny<T>(this List<T> list, params T[] items) {
			foreach (T item in items) {
				if (list.Contains(item)) return true;
			}
			return false;
		}

		public static bool ContainsAll<T>(this List<T> list, params T[] items) {
			foreach (T item in items) {
				if (!list.Contains(item)) return false;
			}
			return true;
		}

		public static bool ContainsAllBetween(this List<int> list, int start, int end) {
			for(int i=start; i<=end; i++) {
				if(!list.Contains(i)) return false;
			}

			return true;
		}

		public static void ReplaceLast<T>(this T[] arr, T item) {
			arr[arr.Length - 1] = item;
		}

		public static void ReplaceLast<T>(this List<T> list, T item) {
			list[list.Count - 1] = item;
		}

		public static T[] Slice<T>(this T[] data, int start, int end = -1) {
			if (end < 0) end = data.Length - 1;
			int length = 1 + (end - start);
			T[] result = new T[length];
			Array.Copy(data, start, result, 0, length);
			return result;
		}

		public static List<T> Slice<T>(this List<T> data, int start, int end = -1) {
			if (end < 0) end = data.Count - 1;
			int length = 1 + (end - start);
			return data.GetRange(start, length);
		}

		public static T[] UpdateAll<T>(this T[] data, Func<T, T> func) {
			for (int i = 0; i < data.Length; i++) {
				data[i] = func(data[i]);
			}

			return data;
		}

		public static List<T> UpdateAll<T>(this List<T> data, Func<T, T> func) {
			for (int i = 0; i < data.Count; i++) {
				data[i] = func(data[i]);
			}
			
			return data;
		}

		public static int CountUntil<T>(this List<T> list, Func<T, int, bool> boolFunc, int startID=0, int endID=-1) {
			if(endID<0) endID = list.Count - 1;
			int sum = 0;

			for (int i=startID; i<=endID; i++) {
				if (boolFunc(list[i], i)) break;
				sum++;
			}

			return sum;
		}

		public static List<T> RemoveIf<T>(this List<T> list, Func<T, bool> boolFunc) {
			for(int i=list.Count; --i>=0;) {
				T item = list[i];
				if(!boolFunc(item)) continue;
				list.RemoveAt(i);
			}

			return list;
		}

		//////////////////////////////////////////////////////////// For Buttons:

		public static void Show(this GameObject go, float time = 0.0f) {
			CanvasGroup cg = go.GetComponent<CanvasGroup>();
			if (cg == null) return;
			cg.DOFade(1.0f, time);
		}

		public static void Hide(this GameObject go, float time = 0.0f) {
			CanvasGroup cg = go.GetComponent<CanvasGroup>();
			if (cg == null) return;
			cg.DOFade(0.0f, time);
		}

	}//END ExtensionMethods


}//END namespace
