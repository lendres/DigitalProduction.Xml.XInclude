// Decompiled with JetBrains decompiler
// Type: GotDotNet.XPointer.XPointerSchema
// Assembly: XInclude, Version=1.0.5338.18190, Culture=neutral, PublicKeyToken=b0a92c4c738ff598
// MVID: A508C8EB-5BD8-4635-B171-E7AA907C25ED
// Assembly location: R:\Programming\XInclude Decompile\XInclude.dll

using System.Collections;

#nullable disable
namespace GotDotNet.XPointer
{
  public class XPointerSchema
  {
    internal static Hashtable Schemas = XPointerSchema.CreateSchemasTable();

    internal static Hashtable CreateSchemasTable()
    {
      return new Hashtable()
      {
        {
          (object) ":element",
          (object) XPointerSchema.SchemaType.Element
        },
        {
          (object) ":xmlns",
          (object) XPointerSchema.SchemaType.Xmlns
        },
        {
          (object) ":xpath1",
          (object) XPointerSchema.SchemaType.XPath1
        },
        {
          (object) ":xpointer",
          (object) XPointerSchema.SchemaType.XPointer
        }
      };
    }

    internal enum SchemaType
    {
      Element,
      Xmlns,
      XPath1,
      XPointer,
      Unknown,
    }
  }
}
