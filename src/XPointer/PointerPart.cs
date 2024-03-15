// Decompiled with JetBrains decompiler
// Type: GotDotNet.XPointer.PointerPart
// Assembly: XInclude, Version=1.0.5338.18190, Culture=neutral, PublicKeyToken=b0a92c4c738ff598
// MVID: A508C8EB-5BD8-4635-B171-E7AA907C25ED
// Assembly location: R:\Programming\XInclude Decompile\XInclude.dll

using System.Xml;

#nullable disable
namespace GotDotNet.XPointer
{
  internal abstract class PointerPart
  {
    public abstract XmlNodeList Evaluate(XmlDocument doc, XmlNamespaceManager nm);
  }
}
