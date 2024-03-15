namespace GotDotNet.XInclude {
    using System;    
    
	/// <summary>
	/// Text inclusion related utility methods.	
	/// </summary>
	/// <author>Oleg Tkachenko, oleg@tkachenko.com</author>
	internal class TextUtils {
	            
        /// <summary>
        /// Checks string for a presense of characters, 
        /// not permitted in XML 1.0 documents.
        /// </summary>
        /// <param name="str">Input string to check.</param>
        /// <exception cref="NonXmlCharacterInDocument">Given string contains a character,
        /// forbidden in XML 1.0.</exception>        
        public static void CheckForNonXmlChars(string str) {
            int i = 0;
            while (i < str.Length) {
                char c = str[i];
                //Allowed unicode XML characters
                if (c >= 0x0020 && c <= 0xD7FF || c >= 0xE000 && c <= 0xFFFD ||
                    c == 0xA || c == 0xD || c == 0x9) {
                    //Ok, approved.
                    i++;
                    continue;                    
                //Check then surrogate pair
                } else if (c >= 0xd800 && c <=0xdbff) {                    
                    //Looks like first char in a surrogate pair, check second one
                    if (++i < str.Length) {
                        if (str[i] >=0xdc00 && str[i] <= 0xdfff)
                            //Ok, valid surrogate pair
                            i++;
                            continue;
                    }                    
                }
                throw new NonXmlCharacterException(str[i]);
            }
        }
	}
}
