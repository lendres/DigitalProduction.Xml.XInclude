// Decompiled with JetBrains decompiler
// Type: GotDotNet.XPointer.XPointerException
// Assembly: XInclude, Version=1.0.5338.18190, Culture=neutral, PublicKeyToken=b0a92c4c738ff598
// MVID: A508C8EB-5BD8-4635-B171-E7AA907C25ED
// Assembly location: R:\Programming\XInclude Decompile\XInclude.dll

using System;

#nullable disable
namespace GotDotNet.XPointer
{
  public abstract class XPointerException : ApplicationException
  {
    public XPointerException(string message)
      : base(message)
    {
    }

    public XPointerException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
