using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bcl2.PositionalNotation
{
  /// <summary>
  /// A bit group is first-in, first-out queue of bits.
  /// </summary>
  public struct BitGroup
  {
    public ulong Value;

    private readonly int _memberBitCount;
    private readonly uint _memberMask;

    public BitGroup(int memberBitCount)
    {
      System.Diagnostics.Debug.Assert(memberBitCount > 0);
      Value = 0;
      _memberBitCount = memberBitCount;
      _memberMask = (1U << memberBitCount) - 1;
    }

    public void Reset()
    {
      Value = 0;
    }

    public void LoadFrom(byte[] buffer, int offset, int count)
    {
      System.Diagnostics.Debug.Assert(count <= 8);
      switch (count)
      {
        case 8:
          {
            Value = BigEndian.ToUInt64(buffer, offset);
            break;
          }
        case 7:
          {
            Value = ((ulong)BigEndian.ToUInt32(buffer, offset) << 24)
              | ((ulong)BigEndian.ToUInt16(buffer, offset + 4) << 8)
              | (ulong)buffer[offset + 6];
            break;
          }
        case 6:
          {
            Value = ((ulong)BigEndian.ToUInt32(buffer, offset) << 16)
              | (ulong)BigEndian.ToUInt16(buffer, offset + 4);
            break;
          }
        case 5:
          {
            Value = ((ulong)BigEndian.ToUInt32(buffer, offset) << 8)
              | (ulong)buffer[offset + 4];
            break;
          }
        case 4:
          {
            Value = BigEndian.ToUInt32(buffer, offset);
            break;
          }
        case 3:
          {
            Value = ((ulong)BigEndian.ToUInt16(buffer, offset) << 8)
              | (ulong)buffer[offset + 2];
            break;
          }
        case 2:
          {
            Value = BigEndian.ToUInt16(buffer, offset);
            break;
          }
        case 1:
          {
            Value = buffer[offset];
            break;
          }
      }
    }

    /// <summary>
    /// Load a member into the least significant part of the bit group.
    /// </summary>
    public void Write(uint x)
    {
      var v = Value;
      v <<= _memberBitCount;
      v |= (x & _memberMask);
      Value = v;
    }

    public void Read(byte[] buffer, int offset, int count)
    {
      var v = Value;
      Unpack(v, buffer, offset, count);
    }

    public static void Unpack(ulong v, byte[] buffer, int offset, int count)
    {
      // the first byte is in the most significant bits
      switch (count)
      {
        case 1:
          {
            buffer[offset + 0] = (byte)v;
            break;
          }
        case 2:
          {
            buffer[offset + 0] = (byte)(v >> 8);
            buffer[offset + 1] = (byte)v;
            break;
          }
        case 3:
          {
            buffer[offset + 0] = (byte)(v >> 16);
            buffer[offset + 1] = (byte)(v >> 8);
            buffer[offset + 2] = (byte)v;
            break;
          }
        case 4:
          {
            buffer[offset + 0] = (byte)(v >> 24);
            buffer[offset + 1] = (byte)(v >> 16);
            buffer[offset + 2] = (byte)(v >> 8);
            buffer[offset + 3] = (byte)v;
            break;
          }
        case 5:
          {
            buffer[offset + 0] = (byte)(v >> 32);
            buffer[offset + 1] = (byte)(v >> 24);
            buffer[offset + 2] = (byte)(v >> 16);
            buffer[offset + 3] = (byte)(v >> 8);
            buffer[offset + 4] = (byte)v;
            break;
          }
        case 6:
          {
            buffer[offset + 0] = (byte)(v >> 40);
            buffer[offset + 1] = (byte)(v >> 32);
            buffer[offset + 2] = (byte)(v >> 24);
            buffer[offset + 3] = (byte)(v >> 16);
            buffer[offset + 4] = (byte)(v >> 8);
            buffer[offset + 5] = (byte)v;
            break;
          }
        case 7:
          {
            buffer[offset + 0] = (byte)(v >> 48);
            buffer[offset + 1] = (byte)(v >> 40);
            buffer[offset + 2] = (byte)(v >> 32);
            buffer[offset + 3] = (byte)(v >> 24);
            buffer[offset + 4] = (byte)(v >> 16);
            buffer[offset + 5] = (byte)(v >> 8);
            buffer[offset + 6] = (byte)v;
            break;
          }
        case 8:
          {
            buffer[offset + 0] = (byte)(v >> 56);
            buffer[offset + 1] = (byte)(v >> 48);
            buffer[offset + 2] = (byte)(v >> 40);
            buffer[offset + 3] = (byte)(v >> 32);
            buffer[offset + 4] = (byte)(v >> 24);
            buffer[offset + 5] = (byte)(v >> 16);
            buffer[offset + 6] = (byte)(v >> 8);
            buffer[offset + 7] = (byte)v;
            break;
          }
      }
    }

    public override string ToString()
    {
      return string.Format("{0:X16}", Value);
    }
  }
}
