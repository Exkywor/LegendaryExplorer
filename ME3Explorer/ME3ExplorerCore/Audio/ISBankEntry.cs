﻿using System;
using System.ComponentModel;
using System.IO;
using ME3ExplorerCore.Gammtek.IO;

namespace ME3ExplorerCore.Audio
{
    public class ISBankEntry : INotifyPropertyChanged
    {
        public string TLKString { get; set; }

        public string FileName { get; set; }
        public Endian FileEndianness { get; set; }
        public uint numberOfChannels = 0;
        public uint sampleRate = 0;
        public uint DataOffset;
        public int CodecID = -1;
        internal int CodecID2 = -1;
        public int bps;
        public int SmplTitlOffset;
        public uint pcmBytes;
        public byte[] DataAsStored { get; set; }

        public string DisplayString
        {
            get
            {
                string retstr = FileName + " - Data offset: 0x" + DataOffset.ToString("X8");
                var codec = getCodecStr();
                if (codec != null)
                {
                    retstr += " - Codec: " + codec;
                }

                retstr += $"\nSamplerate: {sampleRate} - Bits per sample: {bps}";
                return retstr;
            }
        }

        public byte[] FullData { get; set; }

        /// <summary>
        /// Converts this entry to a standalone RIFF and stores it in the FullData variable. Used for preparing data to feed to VGM stream when this is a subsong in an ISB file
        /// The output of this is NOT a valid ISB file! Only enough to allow VGMStream to parse it.
        /// </summary>
        public void PopulateFakeFullData()
        {
            // This needs further testing. It doesn't seem to be correct for
            // Xenon platform (ME1)
            
            MemoryStream outStream = new MemoryStream();
            EndianWriter writer = new EndianWriter(outStream);
            writer.Endian = FileEndianness;
            writer.WriteStringASCII("RIFF");
            writer.Write(0); //Placeholder for length
            writer.WriteStringASCII("isbf"); //titl is actually a chunk
            writer.WriteStringASCII("LIST");
            var listsizepos = writer.BaseStream.Position;
            writer.Write(0); //list size placeholder
            writer.WriteStringASCII("samp"); //sample ahead

            writer.WriteStringASCII("chnk");
            writer.Write(4);
            writer.Write(numberOfChannels);

            writer.WriteStringASCII("chnk");
            writer.Write(10);
            writer.Write(sampleRate);
            writer.Write(pcmBytes);
            writer.Write(bps);

            writer.WriteStringASCII("cpmi");
            writer.Write(8);
            writer.Write(CodecID);
            writer.Write(CodecID2);

            writer.WriteStringASCII("data");
            writer.Write(DataAsStored.Length);
            writer.Write(DataAsStored);

            //Correct headers
            writer.BaseStream.Position = listsizepos;
            writer.Write((uint)writer.BaseStream.Length - (uint)listsizepos);

            writer.BaseStream.Position = 0x4;
            writer.Write((uint)writer.BaseStream.Length - 0x8);
            FullData = outStream.ToArray();
        }

        internal string GetTextSummary()
        {
            string str = "";
            //Debug.WriteLine("Sound #" + currentCounter);
            str += FileName + "\n";
            str += "Sample Rate: " + sampleRate + "\n";
            str += "Channels: " + numberOfChannels + "\n";
            var codec = getCodecStr();
            if (codec != null)
            {
                str += $"Codec: {codec}\n";
            }
            str += "Has Data: " + (DataAsStored != null);
            return str;

        }

        public string getCodecStr()
        {
            switch (CodecID)
            {
                case -1: return null;
                case 0: return $"{bps}-bit PCM";
                case 1: return "Xbox IMA";
                case 2: return "Vorbis";
                case 4: return "XMA";
                case 5: return "Sony MSF container"; //only for PS3 files, but we'll just document it here anyways
                default: return $"Unknown codec ID ({CodecID})";
            }
        }

        public TimeSpan? GetLength()
        {
            if (CodecID == 0x0)
            {
                //PCM
            }
            else if (CodecID == 0x1)
            {

            }
            else if (CodecID == 0x2)
            {
                //vorbis - based on VGMStream
                var samplecount = pcmBytes / numberOfChannels / (bps / 8);
                var seconds = (double)samplecount / sampleRate;
                return TimeSpan.FromSeconds(seconds);
            }
            else if (CodecID == 0x4)
            {
                //XMA
            }
            else if (CodecID == 0x5)
            {
                //Sony MSF (PS3 ME1)
                // Get actual samplerate (stored in audio container)
                //var datasize = EndianReader.ToUInt32(DataAsStored, 0x0C, FileEndianness);
                var actualSampleRate = EndianReader.ToUInt32(DataAsStored, 0x10, FileEndianness);


                var seconds = (double)pcmBytes / actualSampleRate / (bps / 8);
                //var seconds = (double)samplecount / actualSampleRate;
                return TimeSpan.FromSeconds(seconds);
            }
            return new TimeSpan(0);
        }

#pragma warning disable
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore
    }
}
