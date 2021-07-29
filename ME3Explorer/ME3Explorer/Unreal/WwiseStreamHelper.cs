﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ME3Explorer.Soundplorer;
using ME3ExplorerCore.Helpers;
using ME3ExplorerCore.Unreal.BinaryConverters;

namespace ME3Explorer.Unreal
{
    /// <summary>
    /// Helper class for audio stuff
    /// </summary>
    public class WwiseStreamHelper
    {
        public static bool ExtractRawFromSourceToFile(string outfile, string afcPath, int dataSize, int dataOffset)
        {
            var ms = ExternalFileHelper.ReadExternalData(afcPath, dataOffset, dataSize);
            if (ms is null)
            {
                return false;
            }
            if (File.Exists(outfile)) File.Delete(outfile);
            ms.WriteToFile(outfile);
            return true;
        }

        /// <summary>
        /// Creates wav stream from this WwiseStream
        /// </summary>
        /// <param name="afcPath"></param>
        /// <returns></returns>
        public static Stream CreateWaveStreamFromRaw(string afcPath, int offset, int datasize, bool ME2)
        {
            string basePath = GetATempSoundPath();
            if (ExtractRawFromSourceToFile(basePath + ".wem", afcPath, datasize, offset))
            {
                return ISBankEntry.ConvertAudioToWave(basePath + ".wem");
            }
            return null;
        }

        public static string GetATempSoundPath() => $"{Path.GetTempPath()}ME3EXP_SOUND_{Guid.NewGuid()}";

        /// <summary>
        /// Converts a RAW RIFF from game data to a playable WAV stream. This can be written to disk as a playable WAV file.
        /// </summary>
        /// <param name="riffPath">Path to RIFF RAW data</param>
        /// <param name="fullSetup">Full setup flag - use for ME2</param>
        public static MemoryStream ConvertRiffToWav(string riffPath, bool fullSetup)
        {
            Stream oggStream = ConvertRIFFToWWwiseOGG(riffPath, fullSetup);
            if (oggStream != null)// && File.Exists(outputOggPath))
            {
                oggStream.Seek(0, SeekOrigin.Begin);
                string oggPath = Path.Combine(Directory.GetParent(riffPath).FullName, Path.GetFileNameWithoutExtension(riffPath)) + ".ogg";

                using (FileStream fs = new FileStream(oggPath, FileMode.OpenOrCreate))
                {
                    oggStream.CopyTo(fs);
                    fs.Flush();
                }
                File.Delete(riffPath); //raw
                return ConvertOggToWave(oggPath);
            }
            return null;
        }

        /// <summary>
        /// Converts an ogg file to a wav file using oggdec
        /// </summary>
        /// <param name="oggPath">Path to ogg file</param>
        /// <returns></returns>
        public static MemoryStream ConvertOggToWave(string oggPath)
        {
            //convert OGG to WAV
            MemoryStream outputData = new MemoryStream();

            ProcessStartInfo procStartInfo = new ProcessStartInfo(Path.Combine(App.ExecFolder, "oggdec.exe"), $"--stdout \"{oggPath}\"")
            {
                WorkingDirectory = App.ExecFolder,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            //procStartInfo.StandardOutputEncoding = Encoding.GetEncoding(850); //standard cmd-page
            Process proc = new Process
            {
                StartInfo = procStartInfo
            };

            // Set our event handler to asynchronously read the sort output.
            proc.Start();
            //proc.BeginOutputReadLine();
            var outputTask = Task.Run(() =>
            {
                proc.StandardOutput.BaseStream.CopyTo(outputData);

                /*using (var output = new FileStream(outputFile, FileMode.Create))
                {
                    process.StandardOutput.BaseStream.CopyTo(output);
                }*/
            });
            Task.WaitAll(outputTask);

            proc.WaitForExit();
            File.Delete(oggPath); //intermediate

            //Fix headers as they are not correct when output from oggdec over stdout - no idea what it is outputting.
            outputData.Position = 0x4;
            outputData.Write(BitConverter.GetBytes(((int)outputData.Length) - 0x8), 0, 4); //filesize
            outputData.Position = 0x28;
            outputData.Write(BitConverter.GetBytes(((int)outputData.Length) - 0x24), 0, 4); //datasize
            outputData.Position = 0;
            return outputData;
        }

        /// <summary>
        /// Converts a RAW RIFF from game data to a Wwise-based Ogg Vorbis stream
        /// </summary>
        /// <param name="riffPath">Path to RIFF RAW data</param>
        /// <param name="fullSetup">Full setup flag - use for ME2</param>
        public static MemoryStream ConvertRIFFToWWwiseOGG(string riffPath, bool fullSetup)
        {
            //convert RIFF to WwiseOGG
            // Is this useful?
            //System.Diagnostics.Debug.WriteLine("ww2ogg: " + riffPath);
            if (!File.Exists(riffPath))
            {
                Debug.WriteLine("Error: input file does not exist");
            }

            ProcessStartInfo procStartInfo = null;
            if (!fullSetup)
            {
                procStartInfo = new ProcessStartInfo(Path.Combine(App.ExecFolder, "ww2ogg.exe"), "--stdout \"" + riffPath + "\"");
            }
            else
            {
                procStartInfo = new ProcessStartInfo(Path.Combine(App.ExecFolder, "ww2ogg.exe"), "--stdout --full-setup \"" + riffPath + "\"");
            }
            procStartInfo.WorkingDirectory = App.ExecFolder;
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.RedirectStandardError = true;

            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            Process proc = new Process { StartInfo = procStartInfo };
            proc.Start();

            MemoryStream outputData = new MemoryStream();
            MemoryStream outputErrorData = new MemoryStream();
            var outputTask = Task.Run(() =>
            {
                proc.StandardOutput.BaseStream.CopyTo(outputData);
                proc.StandardError.BaseStream.CopyTo(outputErrorData);

                /*using (var output = new FileStream(outputFile, FileMode.Create))
                {
                    process.StandardOutput.BaseStream.CopyTo(output);
                }*/
            });
            Task.WaitAll(outputTask);

            proc.WaitForExit();
            proc.Close();
            //File.WriteAllBytes(System.IO.Path.Combine(loc, "testingoggerr.txt"), outputErrorData.ToArray());

            //Debug.WriteLine("Done");
            return outputData;
            //            return Path.Combine(Directory.GetParent(riffPath).FullName, Path.GetFileNameWithoutExtension(riffPath)) + ".ogg";
        }

        /// <summary>
        /// Converts a Wwise-genreated ogg to the format usable by ME3.
        /// This effectively replaces the need for afc_creator.exe
        /// </summary>
        /// <param name="stream">Stream containing wwiseogg</param>
        /// <returns>ME3 AFC ready stream, at position 0</returns>
        public static MemoryStream ConvertWwiseOggToME3Ogg(Stream stream)
        {
            stream.Position = 0;
            MemoryStream convertedStream = new MemoryStream();
            stream.CopyToEx(convertedStream, 4);
            convertedStream.Write(BitConverter.GetBytes((int)stream.Length - 16), 0, 4);
            stream.Position += 4; //skip over size
            stream.CopyToEx(convertedStream, 0x24); //up to VORB
            stream.Position += 8; //skip vorb
            stream.CopyTo(convertedStream); //copy remaining data

            //update format bytes
            convertedStream.Seek(0x10, SeekOrigin.Begin);
            byte[] firstFmtBytes = { 0x42, 0x00, 0x00, 0x00, 0xFF, 0xFF };
            convertedStream.Write(firstFmtBytes, 0x0, firstFmtBytes.Length);

            //Update second format bytes
            convertedStream.Seek(0x20, SeekOrigin.Begin);
            byte[] secondFmtBytes = { 0x00, 0x00, 0x00, 0x00, 0x30, 0x00, 0x18, 0x00 };
            convertedStream.Write(secondFmtBytes, 0x0, secondFmtBytes.Length);

            convertedStream.Position = 0;
            return convertedStream;
        }

        public static string GetHircObjTypeString(byte b) => GetHircObjTypeString((HIRCType)b);

        public static string GetHircObjTypeString(HIRCType ht) =>
            ht switch
            {
                HIRCType.SoundSXFSoundVoice => "Sound SFX/Sound Voice",
                HIRCType.EventAction => "Event Action",
                HIRCType.Event => "Event",
                HIRCType.RandomOrSequenceContainer => "Random Container or Sequence Container",
                HIRCType.ActorMixer => "Actor-Mixer",
                HIRCType.MusicSegment => "Music Segment",
                HIRCType.MusicTrack => "Music Track",
                HIRCType.MusicSwitchContainer => "Music Switch Container",
                HIRCType.MusicPlaylistContainer => "Music Playlist Container",
                HIRCType.Attenuation => "Attenuation",
                HIRCType.Effect => "Effect",
                HIRCType.AuxiliaryBus => "Auxiliary Bus",
                HIRCType.Settings => "Settings",
                HIRCType.SwitchContainer => "Switch Container",
                HIRCType.AudioBus => "Audio Bus",
                HIRCType.BlendContainer => "Blend Container",
                HIRCType.DialogueEvent => "Dialogue Event",
                HIRCType.MotionBus => "Motion Bus",
                HIRCType.MotionFX => "Motion FX",
                _ => "UNKNOWN HIRCOBJECT TYPE!"
            };

        public static string GetEventActionTypeString(WwiseBank.EventActionType actionType) =>
            actionType switch
            {
                WwiseBank.EventActionType.Play => "Play",
                WwiseBank.EventActionType.Stop => "Stop",
                _ => "Unknown Action"
            };
    }
}