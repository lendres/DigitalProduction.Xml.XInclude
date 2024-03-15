// Decompiled with JetBrains decompiler
// Type: GotDotNet.XPointer.XmlnsSchemaPointerPart
// Assembly: XInclude, Version=1.0.5338.18190, Culture=neutral, PublicKeyToken=b0a92c4c738ff598
// MVID: A508C8EB-5BD8-4635-B171-E7AA907C25ED
// Assembly location: R:\Programming\XInclude Decompile\XInclude.dll

using System;
using System.Xml;

#nullable disable
namespace GotDotNet.XPointer
{
  internal class XmlnsSchemaPointerPart : PointerPart
  {
    private string _prefix;
    private string _uri;

    public XmlnsSchemaPointerPart(string prefix, string uri)
    {
      this._prefix = prefix;
      this._uri = uri;
    }

    public string Prefix => this._prefix;

    public string Uri => this._uri;

    public override XmlNodeList Evaluate(XmlDocument doc, XmlNamespaceManager nm)
    {
      nm.AddNamespace(this._prefix, this._uri);
      return (XmlNodeList) null;
    }

    public static XmlnsSchemaPointerPart ParseSchemaData(XPointerLexer lexer)
    {
      lexer.NextLexeme();
      if (lexer.Kind != XPointerLexer.LexKind.NCName)
      {
        Console.Error.WriteLine("Syntax error in xmlns() schema data: Invalid token in XmlnsSchemaData");
        return (XmlnsSchemaPointerPart) null;
      }
      string ncName = lexer.NCName;
      lexer.SkipWhiteSpace();
      lexer.NextLexeme();
      if (lexer.Kind != XPointerLexer.LexKind.Eq)
      {
        Console.Error.WriteLine("Syntax error in xmlns() schema data: Invalid token in XmlnsSchemaData");
        return (XmlnsSchemaPointerPart) null;
      }
      lexer.SkipWhiteSpace();
      string escapedData;
      try
      {
        escapedData = lexer.ParseEscapedData();
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine("Syntax error in xmlns() schema data: " + ex.Message);
        return (XmlnsSchemaPointerPart) null;
      }
      return new XmlnsSchemaPointerPart(ncName, escapedData);
    }
  }
}
