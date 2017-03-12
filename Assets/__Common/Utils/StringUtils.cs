using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using ExtensionMethods;

/**
 * Pierre: Moved from GameManager, and erased copy in Collection.cs
 */
public class StringUtils {
	#region Implode & Explode Methods
	
	/**~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
		      *	 Implode & Explode Array
	          * -----------------------------------------	
	          * ~ Joins array indices into one string, 
	          *     delimited by specified character.
	          * - TODO: Add these functions to MathExLib
		      *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
	public static string ImplodeArray( Vector3 pVector, string pDelimiter ){
		return ImplodeArray( new float[]{pVector.x, pVector.y, pVector.z}, pDelimiter );
	}

	public static string ImplodeArray( int[] pIntArray, string pDelimiter ){
		float[] tempArray = new float[0];
		if( pIntArray == null )     return "";
		if( pIntArray.Length <= 0 ) return "";
		
		tempArray = new float[pIntArray.Length];   
		for( int i = 0; i < pIntArray.Length; i++ )
			tempArray[i] = pIntArray[i];
		
		return ImplodeArray( tempArray, pDelimiter ); 
		
	}//END ImplodeArray

	public static string ImplodeArray( float[] pIntArray, string pDelimiter ){
		
		string retVal = "";
		if( pIntArray == null )     return retVal;
		if( pIntArray.Length <= 0 ) return retVal;
		
		retVal = pIntArray[0].ToString();
		
		for( int i = 1; i < pIntArray.Length; i++ ){
			retVal += pDelimiter;
			retVal += pIntArray[i].ToString();            
		}//END for
		
		return retVal;
		
	}//END ImplodeArray

	public static string ImplodeArray( string[] pStrArray, string pDelimiter ){
		
		string retVal = "";
		if( pStrArray == null )     return retVal;
		if( pStrArray.Length <= 0 ) return retVal;
		
		retVal = pStrArray[0].ToString();
		
		for( int i = 1; i < pStrArray.Length; i++ ){
			retVal += pDelimiter;
			retVal += pStrArray[i].ToString();            
		}//END for
		
		return retVal;
		
	}//END ImplodeArray


	public static int[] ExplodeArray( string pString, string pDelimiter ){
		
		return ExplodeArrayAsList( pString, pDelimiter ).ToArray();
		
	}//END IntArrayToSingleString

	public static List<int> ExplodeArrayAsList( string pString, string pDelimiter ){
		List<int>   listRetVal          = new List<int>(0);
		string   tempStr             = "";
		int         tempInt             = 0;
		int         nextDelimiterIndex  = 0;
		
		// While there are still delimiters...
		while( pString.Contains( pDelimiter ) ){
			
			// Find the next delimiter
			nextDelimiterIndex  = pString.IndexOf( pDelimiter );
			
			// Convert and store as int            
			tempStr             = pString.Substring( 0, nextDelimiterIndex );
			tempInt             = System.Convert.ToInt32( tempStr );
			listRetVal.Add( tempInt );
			
			// Erase from the main array
			pString          = pString.Remove( 0, nextDelimiterIndex+1 );
			
		}//END while
		
		// Check for last number
		if( pString.Length > 0  ){  
			
			nextDelimiterIndex = pString.Length;
			// Convert and store as int            
			tempStr             = pString.Substring( 0, nextDelimiterIndex );
			tempInt             = System.Convert.ToInt32( tempStr );
			listRetVal.Add( tempInt );
			
			// Erase from the main array
			pString          = pString.Remove( 0, nextDelimiterIndex );
			
		}//END if
		
		// Return the List<int>
		return listRetVal;
	}//END ExplodeArrayAsList
	
	public static string[] ExplodeArrayAsString( string pString, string pDelimiter ){
		return ExplodeArrayAsStringList( pString, pDelimiter ).ToArray();
	}//END 

	public static List<string> ExplodeArrayAsStringList( string pString, string pDelimiter ){
		
		List<string>listRetVal          = new List<string>(10);
		string   tempStr             = "";
		string   tempStr2            = "";
		int         nextDelimiterIndex  = 0;
		
		// While there are still delimiters...
		while( pString.Contains( pDelimiter ) ){
			
			// Find the next delimiter
			nextDelimiterIndex  = pString.IndexOf( pDelimiter );
			
			// Convert and store as int            
			tempStr             = pString.Substring( 0, nextDelimiterIndex );
			tempStr2            = tempStr;
			listRetVal.Add( tempStr2 );
			
			// Erase from the main array
			pString          = pString.Remove( 0, nextDelimiterIndex + pDelimiter.Length );
			
		}//END while
		
		// Check for last number
		if( pString.Length > 0  ){  
			
			nextDelimiterIndex = pString.Length;
			// Convert and store as int            
			tempStr             = pString.Substring( 0, nextDelimiterIndex );
			tempStr2            = tempStr;
			listRetVal.Add( tempStr2 );
			
			// Erase from the main array
			pString          = pString.Remove( 0, nextDelimiterIndex );
			
		}//END if
		
		// Return the int[]
		return listRetVal;
		
	}//END IntArrayToSingleString

	public static float[] ExplodeArrayAsFloat( string pString, string pDelimiter ){
		
		List<float> listRetVal          = new List<float>(10);
		string   tempStr             = "";
		float       tempFlt             = 0f;
		int         nextDelimiterIndex  = 0;
		
		// While there are still delimiters...
		while( pString.Contains( pDelimiter ) ){
			
			// Find the next delimiter
			nextDelimiterIndex  = pString.IndexOf( pDelimiter );
			
			// Convert and store as int            
			tempStr             = pString.Substring( 0, nextDelimiterIndex );
			tempFlt             = float.Parse(tempStr);
			listRetVal.Add( tempFlt );
			
			// Erase from the main array
			pString          = pString.Remove( 0, nextDelimiterIndex+1 );
			
		}//END while
		
		// Check for last number
		if( pString.Length > 0  ){  
			
			nextDelimiterIndex = pString.Length;
			// Convert and store as int            
			tempStr             = pString.Substring( 0, nextDelimiterIndex );
			tempFlt             = float.Parse(tempStr);
			listRetVal.Add( tempFlt );
			
			// Erase from the main array
			pString          = pString.Remove( 0, nextDelimiterIndex );
			
		}//END if
		
		// Return the int[]
		return listRetVal.ToArray();
		
	}//END IntArrayToSingleString
	
	#endregion

	public static string SlashesInBetween(params string[] paths) {
		for(int p=0, pLen=paths.Length; p<pLen; p++) {
			paths[p] = paths[p].Trim ('/');
		}
		return String.Join("/", paths);
	}

	public static string CreateMD5(string input) {
		// Use input string to calculate MD5 hash
		using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create()) {
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hashBytes = md5.ComputeHash(inputBytes);

			// Convert the byte array to hexadecimal string
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hashBytes.Length; i++) {
				sb.Append(hashBytes[i].ToString("X2"));
			}
			return sb.ToString();
		}
	}

	/**
	 * (Snagged from: http://stackoverflow.com/a/463668/468206)
	 */
	public static string ToTimeFormat(float secs) {
		TimeSpan t = TimeSpan.FromSeconds(secs);

		string[] timeParts = null;
		if (t.Hours > 0) {
			timeParts = new string[] {
				t.Hours.ToString("0"),
				t.Minutes.ToString("00"),
				t.Seconds.ToString("00"),
			};
		} else {
			timeParts = new string[] {
				t.Minutes.ToString("0"),
				t.Seconds.ToString("00"),
			};
		}

		return timeParts.Join(":");
	}

	public static string ReplaceBrackets(string input, params string[] replaces) {
		return ReplaceBrackets(input, replaces, "[]");
	}

	public static string ReplaceBrackets(string input, string[] replaces, string brackets) {
		if(brackets==null || brackets.Length!=2) {
			throw new Exception("ReplaceBrackets NEEDS 2 characters to verify which substrings are wrapped.");
		}

		string output = input;
		string escapedA = "\\" + brackets[0];
		string escapedB = "\\" + brackets[1];
		string pattern = escapedA + "([^" + escapedA + escapedB + "]*)" + escapedB;
		Regex regex = new Regex(pattern);

		int found = 0;
		MatchEvaluator matchEval = new MatchEvaluator((Match m) => {
			if(found<replaces.Length) {
				return replaces[found++];
			} else return "";
		});

		output = regex.Replace(output, matchEval);

		return output;
	}

	/*
	public static string ReplaceBracketsByName(string input, object dict) {
		return ReplaceBracketsByName(input, dict, "[]");
	}
	*/

	public static string ReplaceBracketsByName(string input, object dict, string brackets="[]") {
		if (brackets == null || brackets.Length != 2) {
			throw new Exception("ReplaceBrackets NEEDS 2 characters to verify which substrings are wrapped.");
		}

		string output = input;
		string escapedA = "\\" + brackets[0];
		string escapedB = "\\" + brackets[1];
		string pattern = escapedA + "([^" + escapedA + escapedB + "]*)" + escapedB;
		Regex regex = new Regex(pattern);
		Type dictType = dict.GetType();

		MatchEvaluator matchEval = new MatchEvaluator((Match m) => {
			//Debug.Log("Found: " + m.Groups.Count + " : " + m.Captures.Count);
			string propName = m.Groups[1].Value;
			PropertyInfo propInfo = dictType.GetProperty(propName);
			//Debug.Log("propName: " + propName);

			if (propInfo!=null) {
				return propInfo.GetValue(dict, null).ToString();
			} else return "";
		});

		output = regex.Replace(output, matchEval);

		return output;
	}

	public static DistanceUnits ToDistanceUnits(float kilometers) {
		return new DistanceUnits(kilometers);
	}
}

public class DistanceUnits {
	private static readonly float KM_TO_MILES = 0.621371f;
	public static string PREF_UNITS = "DistanceUnits.isUsingMiles";
	public static string PREF_TOTAL_DISTANCE = "DistanceUnits.totalDistance";
	public static float totalDistanceKM = 0.0f;
	public static Action OnUnitsChanged;
	private static bool _isUsingMiles = false;

	private float _kilometers;

	public static void LoadPrefs() {
		IsUsingMiles = PlayerPrefs.HasKey(PREF_UNITS) && PlayerPrefs.GetInt(PREF_UNITS) == 1;
		totalDistanceKM = PlayerPrefs.HasKey(PREF_TOTAL_DISTANCE) ? PlayerPrefs.GetFloat(PREF_TOTAL_DISTANCE) : 0f;
	}

	public static void SavePrefs() {
		PlayerPrefs.SetInt(PREF_UNITS, IsUsingMiles ? 1 : 0);
		PlayerPrefs.SetFloat(PREF_TOTAL_DISTANCE, totalDistanceKM);
	}

	public DistanceUnits(float kilometers) {
		this._kilometers = kilometers;
	}

	public float kilometers { get { return _kilometers; } set { _kilometers = value; } }

	public float value {
		get { return _isUsingMiles ? _kilometers * KM_TO_MILES : _kilometers; }
	}

	public string valueTruncated {
		get { return value.ToString("n1"); }
	}

	public static string unitShort {
		get { return IsUsingMiles ? "MI" : "km"; }
	}

	public static string unitLongName {
		get { return _isUsingMiles ? "miles" : "kilometers"; }
	}

	public static bool IsUsingMiles {
		get { return _isUsingMiles; }
		set {
			if(_isUsingMiles==value) return;

			_isUsingMiles = value;
			if(OnUnitsChanged!=null) OnUnitsChanged();
		}
	}

	public override string ToString() {
		return valueTruncated + " " + unitShort;
	}
}