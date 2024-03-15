// Decompiled with JetBrains decompiler
// Type: GotDotNet.XPointer.XPointerSchemaPointerPart
// Assembly: XInclude, Version=1.0.5338.18190, Culture=neutral, PublicKeyToken=b0a92c4c738ff598
// MVID: A508C8EB-5BD8-4635-B171-E7AA907C25ED
// Assembly location: R:\Programming\XInclude Decompile\XInclude.dll

using System;
using System.Xml;

#nullable disable
namespace GotDotNet.XPointer
{
  internal class XPointerSchemaPointerPart : PointerPart
  {
    private string _xpath;

    public string XPath
    {
      get => this._xpath;
      set => this._xpath = value;
    }

    public override XmlNodeList Evaluate(XmlDocument doc, XmlNamespaceManager nm)
    {
      try
      {
        return doc.SelectNodes(this._xpath, nm);
      }
      catch
      {
        return (XmlNodeList) null;
      }
    }

    public static XPointerSchemaPointerPart ParseSchemaData(XPointerLexer lexer)
    {
      XPointerSchemaPointerPart schemaData = new XPointerSchemaPointerPart();
      try
      {
        schemaData.XPath = lexer.ParseEscapedData();
      }
      catch (Exception ex)
      {
        Console.Error.WriteLine("Syntax error in xpointer() schema data: " + ex.Message);
        return (XPointerSchemaPointerPart) null;
      }
      return schemaData;
    }
  }
}
