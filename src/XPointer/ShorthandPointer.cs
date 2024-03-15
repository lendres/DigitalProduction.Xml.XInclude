﻿// Decompiled with JetBrains decompiler
// Type: GotDotNet.XPointer.ShorthandPointer
// Assembly: XInclude, Version=1.0.5338.18190, Culture=neutral, PublicKeyToken=b0a92c4c738ff598
// MVID: A508C8EB-5BD8-4635-B171-E7AA907C25ED
// Assembly location: R:\Programming\XInclude Decompile\XInclude.dll

using System.Xml;

#nullable disable
namespace GotDotNet.XPointer
{
  internal class ShorthandPointer : Pointer
  {
    private string _NCName;

    public string NCName => this._NCName;

    public ShorthandPointer(string n) => this._NCName = n;

    public override XmlNodeList Evaluate(XmlDocument doc)
    {
      XmlNodeList xmlNodeList = doc.SelectNodes("id('" + this._NCName + "')");
      return xmlNodeList != null && xmlNodeList.Count > 0 ? xmlNodeList : throw new NotFoundException("XPointer doesn't identify any subresource");
    }
  }
}
