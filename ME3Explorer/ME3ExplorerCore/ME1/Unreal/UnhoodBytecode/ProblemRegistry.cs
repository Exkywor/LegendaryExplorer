﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ME3ExplorerCore.Packages;
#if AZURE
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace ME3ExplorerCore.ME1.Unreal.UnhoodBytecode
{
    public class ProblemRegistry
    {
        private class UnknownBytecodeOccurrence
        {
            public UnknownBytecodeOccurrence(UnBytecodeOwner function, byte[] subsequentBytes)
            {
                Function = function;
                SubsequentBytes = subsequentBytes;
            }

            public UnBytecodeOwner Function { get; }
            public byte[] SubsequentBytes { get; }
        }

        private class BytecodeError
        {
            public BytecodeError(UnBytecodeOwner function, string message)
            {
                Function = function;
                Message = message;
            }

            public UnBytecodeOwner Function { get; }
            public string Message { get; }
        }

        private static readonly Dictionary<byte, List<UnknownBytecodeOccurrence>> _unknownBytecodes = new Dictionary<byte, List<UnknownBytecodeOccurrence>>();
        private static readonly List<BytecodeError> _bytecodeErrors = new List<BytecodeError>();
        private static readonly List<UnBytecodeOwner> _incompleteControlFlow = new List<UnBytecodeOwner>();

        public static void LogProblems(string path)
        {
            if (_unknownBytecodes.Count > 0 || _incompleteControlFlow.Count > 0 || _bytecodeErrors.Count > 0)
            {
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    using (var writer = new StreamWriter(fs, Encoding.ASCII))
                    {
                        DumpUnknownBytecodes(writer);
                        writer.WriteLine("Bytecode errors:");
                        foreach (BytecodeError e in _bytecodeErrors)
                        {
                            WriteFunctionName(writer, e.Function);
                            writer.Write("  ");
                            writer.WriteLine(e.Message);
                        }
                        writer.WriteLine("Incomplete control flow:");
                        _incompleteControlFlow.ForEach(f => WriteFunctionName(writer, f));
                    }
                }
            }
            _unknownBytecodes.Clear();
            _incompleteControlFlow.Clear();
            _bytecodeErrors.Clear();
        }

        public static void RegisterBytecodeError(UnBytecodeOwner function, string message)
        {
            _bytecodeErrors.Add(new BytecodeError(function, message));
#if AZURE
            Assert.Fail($"There was a problem parsing ME1/ME2 bytecode! Issue was in {function.Export.FileRef.FilePath}, export {function.Export.UIndex} {function.Export.InstancedFullPath}");
#endif
        }

        public static void RegisterIncompleteControlFlow(UnBytecodeOwner function)
        {
            _incompleteControlFlow.Add(function);
        }

        public static void RegisterUnknownBytecode(byte b, UnBytecodeOwner function, byte[] subsequentBytes)
        {
            if (subsequentBytes.Length < 4) return;   // most likely bogus bytes after 'return'
            if (!_unknownBytecodes.TryGetValue(b, out List<UnknownBytecodeOccurrence> occurrences))
            {
                occurrences = new List<UnknownBytecodeOccurrence>();
                _unknownBytecodes[b] = occurrences;
            }
            occurrences.Add(new UnknownBytecodeOccurrence(function, subsequentBytes));
        }

        public static void DumpUnknownBytecodes(StreamWriter writer)
        {
            foreach (byte b in _unknownBytecodes.Keys)
            {
                writer.WriteLine(b.ToString("X2"));
                var occurrences = _unknownBytecodes[b];
                foreach (var occurrence in occurrences)
                {
                    var function = occurrence.Function;
                    writer.Write("  ");
                    WriteFunctionName(writer, function);

                    var bytes = occurrence.SubsequentBytes;
                    int skip = 0;
                    if (b == 0x0E)
                    {
                        skip = DumpNextBytecodes(writer, function, bytes, 4, 1);
                    }
                    writer.WriteLine($"      {BytecodeReader.DumpBytes(bytes, skip, 32)}");
                }
            }
        }

        private static void WriteFunctionName(StreamWriter writer, UnBytecodeOwner function)
        {
            if (function.Export.SuperClass is IEntry parent)
            {
                if (parent.ClassName == "State")
                {
                    writer.Write("!!!STATE PARENT -- FIX ME IN CODE!!!.");
                    //writer.Write(parent.Parent.ObjectName + ".");
                }
                writer.Write($"{parent.ObjectName.Instanced}.");
            }
            writer.WriteLine(function.Export.ObjectName.Instanced);
        }

        private static void DumpNextName(StreamWriter writer, UnBytecodeOwner function, byte[] bytes, int startOffset)
        {
            using (var stream = new MemoryStream(bytes))
            {
                stream.Position = startOffset;
                using (var reader = new BinaryReader(stream))
                {
                    var name = function.Package.GetNameEntry(reader.ReadInt32());
                    writer.WriteLine($"      {name}");
                }
            }
        }

        private static int DumpNextBytecodes(StreamWriter writer, UnBytecodeOwner function, byte[] bytes, int startOffset, int nextBytecodes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                stream.Position = startOffset;
                using (var reader = new BinaryReader(stream))
                {
                    var bytecodeReader = new BytecodeReader(function.Package, reader);
                    for (int i = 0; i < nextBytecodes; i++)
                    {
                        try
                        {
                            var bytecodeToken = bytecodeReader.ReadNext();
                            writer.WriteLine($"      {bytecodeToken}");
                        }
                        catch (Exception)
                        {
                            writer.WriteLine("      error reading next bytecode");
                            break;
                        }
                    }
                    return (int)reader.BaseStream.Position;
                }
            }
        }
    }
}
