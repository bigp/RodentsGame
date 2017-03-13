using UnityEngine;
using System;
using System.Collections;

public class DateTimeUtils {

	public static string GetTime() {
		DateTime st = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		TimeSpan t = (DateTime.Now.ToUniversalTime() - st);
		return ((Int64) t.TotalMilliseconds).ToString();
	}


	
	/**~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
	/// 
	///     TimeSpanToString
	/// ----------------------------------------
	/// <summary>
	///    
	/// </summary>
	/**~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/     
	public static string TimeSpanToString(System.TimeSpan pTimeSpan){
		
		string retString = "";

		if (pTimeSpan.Days > 0) {
			retString = pTimeSpan.Days.ToString() + " Day" + (pTimeSpan.Days > 1 ? "s" : "");
		} else {

			// Translate DateTime to date string
			string[] dateValArray = new string[3];
			dateValArray[0] = pTimeSpan.Hours < 10 ? ("0" + pTimeSpan.Hours.ToString()) : pTimeSpan.Hours.ToString();
			dateValArray[1] = pTimeSpan.Minutes < 10 ? ("0" + pTimeSpan.Minutes.ToString()) : pTimeSpan.Minutes.ToString();
			dateValArray[2] = pTimeSpan.Seconds < 10 ? ("0" + pTimeSpan.Seconds.ToString()) : pTimeSpan.Seconds.ToString();

			retString = StringUtils.ImplodeArray(dateValArray, ":");
		}

		
		return retString;           
		
	}//END TimeSpanToTimeString      
	
	/**~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
	/// 
	///     DateTimeToString
	/// ----------------------------------------
	/// <summary>
	///    
	/// </summary>
	/**~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/     
	public static string DateTimeToString(System.DateTime pDateTime){
		
		string retString = "";
		
		// Translate DateTime to date string
		int[] dateValArray  = new int[6];
		dateValArray[0]     = pDateTime.Year;
		dateValArray[1]     = pDateTime.Month; 
		dateValArray[2]     = pDateTime.Day;
		dateValArray[3]     = pDateTime.Hour;
		dateValArray[4]     = pDateTime.Minute;
		dateValArray[5]     = pDateTime.Second;
		
		retString = StringUtils.ImplodeArray( dateValArray, ":" );
		
		return retString;           
		
	}//END DateTimeToString    


	/**~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
	/// 
	///     StringToDateTime
	/// ----------------------------------------
	/// <summary>
	///    
	/// </summary>
	/**~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
	public static System.DateTime StringToDateTime(string pStringTime, bool pIsMinValue = false)
	{
		// Try to use normal date formats.
		try {
			return (pStringTime == "" || pStringTime == "N/A" || pStringTime == "0000-00-00 00:00:00") ? (pIsMinValue ? System.DateTime.MinValue : new System.DateTime()) : System.Convert.ToDateTime(pStringTime);
		}
		catch(System.FormatException pE) {
			// continue
		}

		// Use our format.
		int[] dateValArray = StringUtils.ExplodeArray(pStringTime, ":");

		System.DateTime retDateTime;

		if (dateValArray.Length > 5)
			retDateTime = new System.DateTime(dateValArray[0], dateValArray[1], dateValArray[2], dateValArray[3], dateValArray[4], dateValArray[5]);

		else
			retDateTime = new System.DateTime();

		return retDateTime;

	}//END StringToDateTime 
	
	/**~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/
	/// 
	///     StringToTimeSpan
	/// ----------------------------------------
	/// <summary>
	///    
	/// </summary>
	/**~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/         
	public static System.TimeSpan StringToTimeSpan(string pStringTime){
		
		int[]  dateValArray = StringUtils.ExplodeArray( pStringTime, ":" );
		
		System.TimeSpan retDateTime;
		
		if( dateValArray.Length >= 3 )       
			retDateTime = new System.TimeSpan( dateValArray[0], dateValArray[1], dateValArray[2] );
		
		else
			retDateTime = new System.TimeSpan();
		
		return retDateTime;
		
	}//END StringToTimeSpan	


}
