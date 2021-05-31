﻿/*
 * This file is from the UnHood Project
 * https://github.com/yole/unhood
 * Modified by Mgamerz (2019)
 */

using System.IO;
using ME3ExplorerCore.Gammtek.IO;
using ME3ExplorerCore.Helpers;
using ME3ExplorerCore.Packages;

namespace ME3ExplorerCore.ME1.Unreal.UnhoodBytecode
{
    public class UE3FunctionReader
    {
        public static readonly FlagSet _flagSet = new FlagSet("Final", "Defined", "Iterator", "Latent",
            "PreOperator", "Singular", "Net", "NetReliable",
            "Simulated", "Exec", "Native", "Event",
            "Operator", "Static", "Const", null,
            null, "Public", "Private", "Protected",
            "Delegate", "NetServer", "HasOutParms", "HasDefaults",
            "NetClient", "FuncInherit", "FuncOverrideMatch");

        //public static string ReadFunction(ExportEntry export)
        //{
        //    UnFunction func = ReadInstance(export.FileRef, new BinaryReader(new MemoryStream(export.Data)), export);
        //    TextBuilder tb = new TextBuilder();
        //    try
        //    {
        //        func.Decompile(tb);
        //    }
        //    catch (Exception e)
        //    {
        //        return "Error reading function: " + e.ToString();
        //    }
        //    return tb.ToString();
        //}

        public static UnFunction ReadState(ExportEntry export, byte[] dataOverride = null)
        {
            if (dataOverride == null) dataOverride = export.Data;
            using (BinaryReader reader = new BinaryReader(new MemoryStream(dataOverride)))
            {
                reader.ReadBytes(12); //netindex?, none - ASSUMES INCOMING DATA IS FROM .DATA
                int super = reader.ReadInt32();
                int nextCompilingChainItem = reader.ReadInt32();
                reader.ReadBytes(12);
                int line = reader.ReadInt32(); //??
                int textPos = reader.ReadInt32(); //??
                int scriptSize = reader.ReadInt32();
                byte[] bytecode = reader.BaseStream.ReadFully(); //read the rest of the state
                return new UnFunction(export, "STATE", new FlagValues(0, _flagSet), bytecode, 0, 0);
            }
        }

        /// <summary>
        /// Reads the function and returns a parsed object contianing information about the function
        /// Ported from Unhood (Modified by Mgamerz)
        /// </summary>
        /// <param name="export"></param>
        /// <returns></returns>
        public static UnFunction ReadFunction(ExportEntry export, byte[] dataOverride = null)
        {
            // Should dataOverride be GetBinaryData()? Since reader shouldn't care at all about data offset, only binary?

            if (dataOverride == null) dataOverride = export.Data; // Is there a way we could improve the performance of this?
            using var reader = new EndianReader(dataOverride) { Endian = export.FileRef.Endian };
            reader.ReadBytes(12); //netindex?, none - ASSUMES INCOMING DATA IS FROM .DATA

            int super = reader.ReadInt32();
            int nextCompilingChainItem = reader.ReadInt32();
            reader.ReadBytes(12);
            int line = reader.ReadInt32(); //??
            int textPos = reader.ReadInt32(); //??
            int scriptSize = reader.ReadInt32();
            byte[] bytecode = reader.ReadBytes(scriptSize);
            int nativeIndex = reader.ReadInt16();
            int operatorPrecedence = reader.ReadByte();
            int functionFlags = reader.ReadInt32();
            if ((functionFlags & _flagSet.GetMask("Net")) != 0)
            {
                reader.ReadInt16(); // repOffset
            }

            int friendlyNameIndex = reader.ReadInt32();
            reader.ReadInt32();
            return new UnFunction(export, export.FileRef.GetNameEntry(friendlyNameIndex),
                                  new FlagValues(functionFlags, _flagSet), bytecode, nativeIndex, operatorPrecedence);
        }
    }
}
