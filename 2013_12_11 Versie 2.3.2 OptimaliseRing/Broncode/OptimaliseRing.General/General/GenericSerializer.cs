#region Copyright -------------------------------------------------------
// Copyright © 2007 HKV lijn in water, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Project    : OptimaliseRing.General
//
// Author(s)  : Rolf Waterman, HKV lijn in water
//
#endregion

#region History ---------------------------------------------------------
// $Header: /Applicaties/Batch OptimaliseRing.root/OptimaliseRing/OptimaliseRing.General/General/GenericSerializer.cs 1     16/06/08 10:24 Ansink $
// $NoKeywords: $
#endregion


using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;

namespace OptimaliseRing.General
{
  public static class GenericSerializer
  {
    public static void SaveCompressedXml<T>(T data, string pathName)
    {
      WriteFileStream(pathName, delegate(Stream file)
          {
            WriteCompressedStream(file, delegate(Stream f2)
            {
              WriteToXml(data, f2);
            });
          });
    }

    public static Byte[] SaveCompressedXml<T>(T data)
    {
      return WriteMemoryStream(delegate(Stream s)
      {
        WriteCompressedStream(s, delegate(Stream f2)
        {
          WriteToXml(data, f2);
        });
      });

    }

    public delegate void WriteToStream(Stream s);
    public static void WriteFileStream(string pathName, WriteToStream func)
    {
      using (FileStream file = new FileStream(pathName,
          FileMode.Create, FileAccess.Write))
      {
        func(file);
      }
    }

    public static Byte[] WriteMemoryStream(WriteToStream func)
    {
      using (MemoryStream storage = new MemoryStream())
      {
        func(storage);
        return storage.ToArray();
      }
    }

    public static void WriteCompressedStream(Stream target, WriteToStream func)
    {
      using (GZipStream compressed = new GZipStream(target, CompressionMode.Compress))
      {
        func(compressed);
      }
    }

    public static void WriteToXml<T>(T storage, Stream file)
    {
      XmlSerializer s = new XmlSerializer(typeof(T));
      s.Serialize(file, storage);
    }

    public static T ReadCompressedXml<T>(string pathName)
    {
      T s2 = ReadFileStream<T>(pathName, delegate(Stream s)
      {
        return ReadCompressedStream<T>(s, delegate(Stream compressedStream)
            {
              return ReadFromXml<T>(compressedStream);
            });
      });
      return s2;
    }

    public static T ReadCompressedXml<T>(Byte[] contents)
    {
      T s2 = ReadMemoryStream<T>(contents, delegate(Stream s)
      {
        return ReadCompressedStream<T>(s, delegate(Stream compressedStream)
            {
              return ReadFromXml<T>(compressedStream);
            });
      });
      return s2;
    }

    public delegate T ReadFromStream<T>(Stream s);
    public static T ReadFileStream<T>(string pathName, ReadFromStream<T> func)
    {
      using (FileStream file = new FileStream(pathName, FileMode.Open, FileAccess.Read))
      {
        return func(file);
      }
    }

    public static T ReadMemoryStream<T>(Byte[] buffer, ReadFromStream<T> func)
    {
      using (MemoryStream reader = new MemoryStream(buffer))
      {
        return func(reader);
      }
    }

    public static T ReadCompressedStream<T>(Stream target, ReadFromStream<T> func)
    {
      using (GZipStream compressed = new GZipStream(target, CompressionMode.Decompress))
      {
        return func(compressed);
      }
    }

    public static T ReadFromXml<T>(Stream inStream)
    {
      XmlSerializer ser = new XmlSerializer(typeof(T));
      return (T)ser.Deserialize(inStream);
    }

    static public void SaveToXml<T>(T storage, string pathName)
    {
      WriteFileStream(pathName, delegate(Stream file)
          {
            WriteToXml(storage, file);
          });
    }

    static public T FromXmlFile<T>(string pathName)
    {
      T s2 = ReadFileStream<T>(pathName, delegate(Stream s)
      {
        return ReadFromXml<T>(s);
      });
      return s2;
    }
  }
}
