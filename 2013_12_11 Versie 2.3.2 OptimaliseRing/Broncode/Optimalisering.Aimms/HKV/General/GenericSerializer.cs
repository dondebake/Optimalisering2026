#region Copyright -------------------------------------------------------
// Copyright © 2007, ®HKV Consultants, All Rights Reserved.
// Deze software blijft intellectueel eigendom van ®HKV lijn in water.
//
// Project    :
//
//
// Author(s)  : Rolf Waterman HKV lijn in water
//
// Filename   : GenericSerializer.cs
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;

namespace Hkv.GenericSerializer
{

  /// <summary>
  /// GenericSerializer
  /// </summary>
  public class GenericSerializer
  {
    /// <summary>
    /// Saves the compressed XML.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">The data.</param>
    /// <param name="pathName">Name of the path.</param>
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

    /// <summary>
    /// Saves the compressed XML.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data">The data.</param>
    /// <returns></returns>
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

    /// <summary>
    /// WriteToStream
    /// </summary>
    public delegate void WriteToStream(Stream s);
    /// <summary>
    /// Writes the file stream.
    /// </summary>
    /// <param name="pathName">Name of the path.</param>
    /// <param name="func">The func.</param>
    public static void WriteFileStream(string pathName, WriteToStream func)
    {
      using (FileStream file = new FileStream(pathName,
          FileMode.Create, FileAccess.Write))
      {
        func(file);
      }
    }

    /// <summary>
    /// Writes the memory stream.
    /// </summary>
    /// <param name="func">The func.</param>
    /// <returns></returns>
    public static Byte[] WriteMemoryStream(WriteToStream func)
    {
      using (MemoryStream storage = new MemoryStream())
      {
        func(storage);
        return storage.ToArray();
      }
    }

    /// <summary>
    /// Writes the compressed stream.
    /// </summary>
    /// <param name="target">The target.</param>
    /// <param name="func">The func.</param>
    public static void WriteCompressedStream(Stream target, WriteToStream func)
    {
      using (GZipStream compressed = new GZipStream(target, CompressionMode.Compress))
      {
        func(compressed);
      }
    }

    /// <summary>
    /// Writes to XML.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="storage">The storage.</param>
    /// <param name="file">The file.</param>
    public static void WriteToXml<T>(T storage, Stream file)
    {
      XmlSerializer s = new XmlSerializer(typeof(T));
      s.Serialize(file, storage);
    }

    /// <summary>
    /// Reads the compressed XML.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pathName">Name of the path.</param>
    /// <returns></returns>
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

    /// <summary>
    /// Reads the compressed XML.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="contents">The contents.</param>
    /// <returns></returns>
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

    /// <summary>
    /// ReadFromStream
    /// </summary>
    public delegate T ReadFromStream<T>(Stream s);
    /// <summary>
    /// Reads the file stream.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pathName">Name of the path.</param>
    /// <param name="func">The func.</param>
    /// <returns></returns>
    public static T ReadFileStream<T>(string pathName, ReadFromStream<T> func)
    {
      using (FileStream file = new FileStream(pathName, FileMode.Open, FileAccess.Read))
      {
        return func(file);
      }
    }

    /// <summary>
    /// Reads the memory stream.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="buffer">The buffer.</param>
    /// <param name="func">The func.</param>
    /// <returns></returns>
    public static T ReadMemoryStream<T>(Byte[] buffer, ReadFromStream<T> func)
    {
      using (MemoryStream reader = new MemoryStream(buffer))
      {
        return func(reader);
      }
    }

    /// <summary>
    /// Reads the compressed stream.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target">The target.</param>
    /// <param name="func">The func.</param>
    /// <returns></returns>
    public static T ReadCompressedStream<T>(Stream target, ReadFromStream<T> func)
    {
      using (GZipStream compressed = new GZipStream(target, CompressionMode.Decompress))
      {
        return func(compressed);
      }
    }

    /// <summary>
    /// Reads from XML.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="inStream">The in stream.</param>
    /// <returns></returns>
    public static T ReadFromXml<T>(Stream inStream)
    {
      XmlSerializer ser = new XmlSerializer(typeof(T));
      return (T)ser.Deserialize(inStream);
    }

    /// <summary>
    /// Saves to XML.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="storage">The storage.</param>
    /// <param name="pathName">Name of the path.</param>
    static public void SaveToXml<T>(T storage, string pathName)
    {
      WriteFileStream(pathName, delegate(Stream file)
          {
            WriteToXml(storage, file);
          });
    }

    /// <summary>
    /// Froms the XML file.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pathName">Name of the path.</param>
    /// <returns></returns>
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
