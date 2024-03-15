// Decompiled with JetBrains decompiler
// Type: GotDotNet.XPointer.ElementSchemaPointerPart
// Assembly: XInclude, Version=1.0.5338.18190, Culture=neutral, PublicKeyToken=b0a92c4c738ff598
// MVID: A508C8EB-5BD8-4635-B171-E7AA907C25ED
// Assembly location: R:\Programming\XInclude Decompile\XInclude.dll

using System;
using System.Collections;
using System.Text;
using System.Xml;

#nullable disable
namespace GotDotNet.XPointer
{
  internal class ElementSchemaPointerPart : PointerPart
  {
    private string _NCName;
    private string _xpath;
    private ArrayList _childSequence;

    public string NCName
    {
      get => this._NCName;
      set => this._NCName = value;
    }

    public string XPath
    {
      get => this._xpath;
      set => this._xpath = value;
    }

    public ArrayList ChildSequence
    {
      get => this._childSequence;
      set => this._childSequence = value;
    }

    public override XmlNodeList Evaluate(XmlDocument doc, XmlNamespaceManager nm)
    {
      return doc.SelectNodes(this._xpath, nm);
    }

    public static ElementSchemaPointerPart ParseSchemaData(XPointerLexer lexer)
    {
      StringBuilder stringBuilder = new StringBuilder();
      ElementSchemaPointerPart schemaData = new ElementSchemaPointerPart();
      lexer.NextLexeme();
      if (lexer.Kind == XPointerLexer.LexKind.NCName)
      {
        schemaData.NCName = lexer.NCName;
        stringBuilder.Append("id('");
        stringBuilder.Append(lexer.NCName);
        stringBuilder.Append("')");
        lexer.NextLexeme();
      }
      schemaData.ChildSequence = new ArrayList();
      while (lexer.Kind == XPointerLexer.LexKind.Slash)
      {
        lexer.NextLexeme();
        if (lexer.Kind != XPointerLexer.LexKind.Number)
        {
          Console.Error.WriteLine("Syntax error in element() schema data: Invalid token in ChildSequence");
          return (ElementSchemaPointerPart) null;
        }
        if (lexer.Number == 0)
        {
          Console.Error.WriteLine("Syntax error in element() schema data: 0 index ChildSequence");
          return (ElementSchemaPointerPart) null;
        }
        schemaData.ChildSequence.Add((object) lexer.Number);
        stringBuilder.Append("/*[");
        stringBuilder.Append(lexer.Number);
        stringBuilder.Append("]");
        lexer.NextLexeme();
      }
      if (lexer.Kind != XPointerLexer.LexKind.RRBracket)
      {
        Console.Error.WriteLine("Syntax error in element() schema data: Invalid token in ChildSequence");
        return (ElementSchemaPointerPart) null;
      }
      if (schemaData.NCName == null && schemaData.ChildSequence.Count == 0)
      {
        Console.Error.WriteLine("Syntax error in element() schema data: empty XPointer");
        return (ElementSchemaPointerPart) null;
      }
      schemaData.XPath = stringBuilder.ToString();
      return schemaData;
    }
  }
}
