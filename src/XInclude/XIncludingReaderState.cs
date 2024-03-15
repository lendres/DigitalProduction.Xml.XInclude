namespace GotDotNet.XInclude {
    
    /// <summary>
    /// XIncludingReader state machine.    
    /// </summary>
    /// <author>Oleg Tkachenko, oleg@tkachenko.com</author>
    internal enum XIncludingReaderState {
        //Default state
        Default,
        //xml:base attribute is being exposed
        ExposingXmlBaseAttr,
        //xml:base attribute value is being exposed
        ExposingXmlBaseAttrValue
    }    
}
