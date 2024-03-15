// Decompiled with JetBrains decompiler
// Type: GotDotNet.XPointer.SchemaBasedPointer
// Assembly: XInclude, Version=1.0.5338.18190, Culture=neutral, PublicKeyToken=b0a92c4c738ff598
// MVID: A508C8EB-5BD8-4635-B171-E7AA907C25ED
// Assembly location: R:\Programming\XInclude Decompile\XInclude.dll

using System.Collections;
using System.Xml;

#nullable disable
namespace GotDotNet.XPointer
{
  internal class SchemaBasedPointer : Pointer
  {
    private ArrayList _parts;

    public ArrayList Parts => this._parts;

    public SchemaBasedPointer(ArrayList parts) => this._parts = parts;

    public override XmlNodeList Evaluate(XmlDocument doc)
    {
      XmlNamespaceManager nm = new XmlNamespaceManager(doc.NameTable);
      for (int index = 0; index < this._parts.Count; ++index)
      {
        XmlNodeList xmlNodeList = ((PointerPart) this._parts[index]).Evaluate(doc, nm);
        if (xmlNodeList != null && xmlNodeList.Count > 0)
          return xmlNodeList;
      }
      throw new NotFoundException("XPointer doesn't identify any subresource");
    }
  }
}
