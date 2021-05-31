﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ME3ExplorerCore.Gammtek.Extensions;
using ME3ExplorerCore.Gammtek.IO;
using ME3ExplorerCore.Helpers;
using ME3ExplorerCore.Memory;
using ME3ExplorerCore.Misc;
using ME3ExplorerCore.Packages;

namespace ME3ExplorerCore.Unreal.BinaryConverters
{
    public class WwiseBank : ObjectBinary
    {
        public uint Unk1;//ME2
        public uint Unk2;//ME2
        public uint Unk3;

        private uint[] bkhdUnks;
        public uint Version; //If 0, this Bank is serialized empty. When creating a bank, make sure to set this!
        public uint ID;

        public OrderedMultiValueDictionary<uint, byte[]> EmbeddedFiles = new OrderedMultiValueDictionary<uint, byte[]>();
        public OrderedMultiValueDictionary<uint, HIRCObject> HIRCObjects = new OrderedMultiValueDictionary<uint, HIRCObject>();
        public OrderedMultiValueDictionary<uint, string> ReferencedBanks = new OrderedMultiValueDictionary<uint, string>();

        public WwiseStateManagement InitStateManagement;//Only present in Init bank. ME3 version
        private byte[] ME2STMGFallback; //STMG chunk for ME2 isn't decoded yet
        private byte[] ENVS_Chunk;//Unparsed
        private byte[] FXPR_Chunk;//Unparsed, ME2 only

        #region Serialization

        private static readonly uint bkhd = BitConverter.ToUInt32(Encoding.ASCII.GetBytes("BKHD"), 0);
        private static readonly uint stmg = BitConverter.ToUInt32(Encoding.ASCII.GetBytes("STMG"), 0);
        private static readonly uint didx = BitConverter.ToUInt32(Encoding.ASCII.GetBytes("DIDX"), 0);
        private static readonly uint data = BitConverter.ToUInt32(Encoding.ASCII.GetBytes("DATA"), 0);
        private static readonly uint hirc = BitConverter.ToUInt32(Encoding.ASCII.GetBytes("HIRC"), 0);
        private static readonly uint stid = BitConverter.ToUInt32(Encoding.ASCII.GetBytes("STID"), 0);
        private static readonly uint envs = BitConverter.ToUInt32(Encoding.ASCII.GetBytes("ENVS"), 0);
        private static readonly uint fxpr = BitConverter.ToUInt32(Encoding.ASCII.GetBytes("FXPR"), 0);

        protected override void Serialize(SerializingContainer2 sc)
        {
            if (sc.Game != MEGame.ME3 && sc.Game != MEGame.ME2)
            {
                throw new Exception($"WwiseBank is not a valid class for {sc.Game}!");
            }
            if (sc.Game == MEGame.ME2)
            {
                sc.Serialize(ref Unk1);
                sc.Serialize(ref Unk2);
                if (Unk1 == 0 && Unk2 == 0)
                {
                    return; //not sure what's going on here
                }
            }
            sc.Serialize(ref Unk3);
            var dataSizePos = sc.ms.Position; //come back to write size at the end
            int dataSize = 0;
            sc.Serialize(ref dataSize);
            sc.Serialize(ref dataSize);
            sc.SerializeFileOffset();
            if (sc.IsLoading && dataSize == 0 || sc.IsSaving && Version == 0)
            {
                return;
            }

            if (sc.IsLoading)
            {
                sc.ms.Skip(4);
            }
            else
            {
                sc.ms.Writer.WriteUInt32(bkhd);
            }

            int bkhdLen = 8 + (bkhdUnks?.Length ?? 0) * 4;
            sc.Serialize(ref bkhdLen);
            sc.Serialize(ref Version);
            sc.Serialize(ref ID);
            if (sc.IsLoading)
            {
                bkhdUnks = new uint[(bkhdLen - 8) / 4];
            }

            for (int i = 0; i < bkhdUnks.Length; i++)
            {
                sc.Serialize(ref bkhdUnks[i]);
            }

            if (sc.IsLoading)
            {
                ReadChunks(sc);
            }
            else
            {
                WriteChunks(sc);
                var endPos = sc.ms.Position;
                sc.ms.JumpTo(dataSizePos);
                sc.ms.Writer.WriteInt32((int)(endPos - dataSizePos - 12));
                sc.ms.Writer.WriteInt32((int)(endPos - dataSizePos - 12));
                sc.ms.JumpTo(endPos);
            }
        }

        private void ReadChunks(SerializingContainer2 sc)
        {
            while (sc.ms.Position < sc.ms.Length)
            {
                // It looks like on consoles this is not endian
                string chunkID = sc.ms.BaseStream.ReadStringASCII(4);

                int chunkSize = sc.ms.ReadInt32();
                switch (chunkID)
                {
                    case "STMG":
                        {
                            if (sc.Game == MEGame.ME2)
                            {
                                ME2STMGFallback = sc.ms.ReadBytes(chunkSize);
                                break;
                            }
                            InitStateManagement = new WwiseStateManagement
                            {
                                VolumeThreshold = sc.ms.ReadFloat(),
                                MaxVoiceInstances = sc.ms.ReadUInt16()
                            };
                            int stateGroupCount = sc.ms.ReadInt32();
                            InitStateManagement.StateGroups = new OrderedMultiValueDictionary<uint, WwiseStateManagement.StateGroup>();
                            for (int i = 0; i < stateGroupCount; i++)
                            {
                                uint id = sc.ms.ReadUInt32();
                                var stateGroup = new WwiseStateManagement.StateGroup
                                {
                                    ID = id,
                                    DefaultTransitionTime = sc.ms.ReadUInt32(),
                                    CustomTransitionTimes = new List<WwiseStateManagement.CustomTransitionTime>()
                                };
                                int transTimesCount = sc.ms.ReadInt32();
                                for (int j = 0; j < transTimesCount; j++)
                                {
                                    stateGroup.CustomTransitionTimes.Add(new WwiseStateManagement.CustomTransitionTime
                                    {
                                        FromStateID = sc.ms.ReadUInt32(),
                                        ToStateID = sc.ms.ReadUInt32(),
                                        TransitionTime = sc.ms.ReadUInt32(),
                                    });
                                }
                                InitStateManagement.StateGroups.Add(id, stateGroup);
                            }

                            int switchGroupCount = sc.ms.ReadInt32();
                            InitStateManagement.SwitchGroups = new OrderedMultiValueDictionary<uint, WwiseStateManagement.SwitchGroup>();
                            for (int i = 0; i < switchGroupCount; i++)
                            {
                                uint id = sc.ms.ReadUInt32();
                                var switchGroup = new WwiseStateManagement.SwitchGroup
                                {
                                    ID = id,
                                    GameParamID = sc.ms.ReadUInt32(),
                                    Points = new List<WwiseStateManagement.SwitchPoint>()
                                };
                                int pointsCount = sc.ms.ReadInt32();
                                for (int j = 0; j < pointsCount; j++)
                                {
                                    switchGroup.Points.Add(new WwiseStateManagement.SwitchPoint
                                    {
                                        GameParamValue = sc.ms.ReadFloat(),
                                        SwitchID = sc.ms.ReadUInt32(),
                                        CurveShape = sc.ms.ReadUInt32()
                                    });
                                }
                                InitStateManagement.SwitchGroups.Add(id, switchGroup);
                            }

                            int gameParamsCount = sc.ms.ReadInt32();
                            InitStateManagement.GameParameterDefaultValues = new OrderedMultiValueDictionary<uint, float>();
                            for (int i = 0; i < gameParamsCount; i++)
                            {
                                InitStateManagement.GameParameterDefaultValues.Add(sc.ms.ReadUInt32(), sc.ms.ReadFloat());
                            }
                            break;
                        }
                    case "DIDX":
                        int numFiles = chunkSize / 12;
                        var infoPos = sc.ms.Position;
                        sc.ms.Skip(chunkSize);
                        if (sc.ms.BaseStream.ReadUInt32() != data) //not endian swapped
                        {
                            throw new Exception("DIDX chunk is not followed by DATA chunk in WwiseBank!");
                        }

                        var dataBytes = sc.ms.ReadBytes(sc.ms.ReadInt32());
                        var dataEndPos = sc.ms.Position;
                        sc.ms.JumpTo(infoPos);

                        for (int i = 0; i < numFiles; i++)
                        {
                            EmbeddedFiles.Add(sc.ms.ReadUInt32(), dataBytes.Slice(sc.ms.ReadInt32(), sc.ms.ReadInt32()));
                        }
                        sc.ms.JumpTo(dataEndPos);
                        break;
                    case "HIRC":
                        int count = sc.ms.ReadInt32();
                        for (int i = 0; i < count; i++)
                        {
                            var ho = HIRCObject.Create(sc);
                            HIRCObjects.Add(ho.ID, ho);
                        }
                        break;
                    case "STID":
                        sc.ms.Skip(4);//unknown uint (always 1)
                        int numBanks = sc.ms.ReadInt32();
                        for (int i = 0; i < numBanks; i++)
                        {
                            uint id = sc.ms.ReadUInt32();
                            byte strLen = sc.ms.ReadByte();
                            ReferencedBanks.Add(id, sc.ms.ReadStringASCII(strLen));
                        }
                        break;
                    case "FXPR":
                        //no idea what's in this chunk
                        FXPR_Chunk = sc.ms.ReadBytes(chunkSize);
                        break;
                    case "ENVS":
                        //no idea what's in this chunk
                        ENVS_Chunk = sc.ms.ReadBytes(chunkSize);
                        break;
                    default:
                        throw new Exception($"Unknown Chunk: {sc.ms.ReadEndianASCIIString(4)} at {sc.ms.Position - 4}");
                }
            }
        }

        private void WriteChunks(SerializingContainer2 sc)
        {
            EndianWriter writer = sc.ms.Writer;
            if (EmbeddedFiles.Count > 0)
            {
                writer.WriteUInt32(didx);
                writer.WriteInt32(EmbeddedFiles.Count * 12);
                using var dataChunk = MemoryManager.GetMemoryStream();
                foreach ((uint id, byte[] bytes) in EmbeddedFiles)
                {
                    dataChunk.WriteZeros((int)(dataChunk.Position.Align(16) - dataChunk.Position)); //files must be 16-byte aligned in the data chunk
                    writer.WriteUInt32(id); //writing to DIDX
                    writer.WriteInt32((int)dataChunk.Position); //Writing to DIDX
                    writer.WriteInt32(bytes.Length); //Writing to DIDX
                    dataChunk.WriteFromBuffer(bytes); //Writing to DATA
                }

                writer.WriteUInt32(data);
                writer.WriteInt32((int)dataChunk.Length);
                writer.WriteFromBuffer(dataChunk.ToArray());
            }

            if (sc.Game == MEGame.ME2 && ME2STMGFallback != null)
            {
                writer.WriteUInt32(stmg);
                writer.WriteInt32(ME2STMGFallback.Length);
                writer.WriteFromBuffer(ME2STMGFallback);
            }

            if (sc.Game == MEGame.ME3 && InitStateManagement != null)
            {
                writer.WriteUInt32(stmg);
                var lenPos = sc.ms.Position;
                writer.WriteUInt32(0);
                writer.WriteFloat(InitStateManagement.VolumeThreshold);
                writer.WriteUInt16(InitStateManagement.MaxVoiceInstances);
                writer.WriteInt32(InitStateManagement.StateGroups.Count);
                foreach ((uint _, WwiseStateManagement.StateGroup stateGroup) in InitStateManagement.StateGroups)
                {
                    writer.WriteUInt32(stateGroup.ID);
                    writer.WriteUInt32(stateGroup.DefaultTransitionTime);
                    writer.WriteInt32(stateGroup.CustomTransitionTimes.Count);
                    foreach (var transTime in stateGroup.CustomTransitionTimes)
                    {
                        writer.WriteUInt32(transTime.FromStateID);
                        writer.WriteUInt32(transTime.ToStateID);
                        writer.WriteUInt32(transTime.TransitionTime);
                    }
                }
                writer.WriteInt32(InitStateManagement.SwitchGroups.Count);
                foreach ((uint _, WwiseStateManagement.SwitchGroup switchGroup) in InitStateManagement.SwitchGroups)
                {
                    writer.WriteUInt32(switchGroup.ID);
                    writer.WriteUInt32(switchGroup.GameParamID);
                    writer.WriteInt32(switchGroup.Points.Count);
                    foreach (var point in switchGroup.Points)
                    {
                        writer.WriteFloat(point.GameParamValue);
                        writer.WriteUInt32(point.SwitchID);
                        writer.WriteUInt32(point.CurveShape);
                    }
                }
                writer.WriteInt32(InitStateManagement.GameParameterDefaultValues.Count);
                foreach ((uint id, float defaultValue) in InitStateManagement.GameParameterDefaultValues)
                {
                    writer.WriteUInt32(id);
                    writer.WriteFloat(defaultValue);
                }
                var endPos = sc.ms.Position;
                sc.ms.JumpTo(lenPos);
                writer.WriteInt32((int)(endPos - lenPos - 4));
                sc.ms.JumpTo(endPos);
            }

            if (HIRCObjects.Count > 0)
            {
                writer.WriteUInt32(hirc);
                var lengthPos = sc.ms.Position;
                writer.WriteUInt32(0);
                writer.WriteInt32(HIRCObjects.Count);
                foreach ((uint _, HIRCObject h) in HIRCObjects)
                {
                    writer.WriteFromBuffer(h.ToBytes(sc.Game));
                }

                var endPos = sc.ms.Position;
                sc.ms.JumpTo(lengthPos);
                writer.WriteInt32((int)(endPos - lengthPos - 4));
                sc.ms.JumpTo(endPos);
            }

            if (ReferencedBanks.Count > 0)
            {
                writer.WriteUInt32(stid);
                var lengthPos = sc.ms.Position;
                writer.WriteUInt32(0);
                writer.WriteUInt32(1);
                writer.WriteInt32(ReferencedBanks.Count);
                foreach ((uint id, string name) in ReferencedBanks)
                {
                    writer.WriteUInt32(id);
                    writer.WriteByte((byte)name.Length);
                    writer.WriteStringASCII(name);
                }

                var endPos = sc.ms.Position;
                sc.ms.JumpTo(lengthPos);
                writer.WriteInt32((int)(endPos - lengthPos - 4));
                sc.ms.JumpTo(endPos);
            }

            if (FXPR_Chunk != null)
            {
                writer.WriteUInt32(fxpr);
                writer.WriteInt32(FXPR_Chunk.Length);
                writer.WriteFromBuffer(FXPR_Chunk);
            }

            if (ENVS_Chunk != null)
            {
                writer.WriteUInt32(envs);
                writer.WriteInt32(ENVS_Chunk.Length);
                writer.WriteFromBuffer(ENVS_Chunk);
            }
        }

        #endregion

        public class HIRCObject
        {
            public HIRCType Type;
            public uint ID;
            public virtual int DataLength => unparsed.Length + 4;
            protected byte[] unparsed;

            public static HIRCObject Create(SerializingContainer2 sc)
            {
                HIRCType type = (HIRCType)(sc.Game == MEGame.ME3 ? sc.ms.ReadByte() : (byte)sc.ms.ReadInt32());
                int len = sc.ms.ReadInt32();
                uint id = sc.ms.ReadUInt32();
                return type switch
                {
                    HIRCType.SoundSXFSoundVoice => SoundSFXVoice.Create(sc, id, len),
                    HIRCType.Event => Event.Create(sc, id),
                    HIRCType.EventAction => EventAction.Create(sc, id, len),
                    _ => new HIRCObject
                    {
                        Type = type,
                        ID = id,
                        unparsed = sc.ms.ReadBytes(len - 4)
                    }
                };
            }

            public virtual byte[] ToBytes(MEGame game)
            {
                using MemoryStream ms = WriteHIRCObjectHeader(game);
                ms.WriteFromBuffer(unparsed);
                return ms.ToArray();
            }

            protected MemoryStream WriteHIRCObjectHeader(MEGame game)
            {
                var ms = MemoryManager.GetMemoryStream();
                if (game == MEGame.ME3)
                {
                    ms.WriteByte((byte)Type);
                }
                else
                {
                    ms.WriteInt32((byte)Type);
                }

                ms.WriteInt32(DataLength);
                ms.WriteUInt32(ID);
                return ms;
            }

            public virtual HIRCObject Clone()
            {
                HIRCObject clone = (HIRCObject)MemberwiseClone();
                clone.unparsed = unparsed?.TypedClone();
                return clone;
            }
        }

        public class SoundSFXVoice : HIRCObject
        {
            public uint Unk1;
            public SoundState State;
            public uint AudioID;
            public uint SourceID;
            public int UnkType;
            public int UnkPrefetchLength;
            public SoundType SoundType; //0=SFX, 1=Voice

            private int ParsedLength => 21 + (State == SoundState.Streamed ? 0 : 8);
            public override int DataLength => unparsed.Length + ParsedLength;

            public override byte[] ToBytes(MEGame game)
            {
                using MemoryStream ms = WriteHIRCObjectHeader(game);
                ms.WriteUInt32(Unk1);
                ms.WriteUInt32((uint)State);
                ms.WriteUInt32(AudioID);
                ms.WriteUInt32(SourceID);
                if (State != SoundState.Streamed)
                {
                    ms.WriteInt32(UnkType);
                    ms.WriteInt32(UnkPrefetchLength);
                }
                ms.WriteByte((byte)SoundType);
                ms.WriteFromBuffer(unparsed);
                return ms.ToArray();
            }

            public static SoundSFXVoice Create(SerializingContainer2 sc, uint id, int len)
            {
                SoundSFXVoice sfxVoice = new SoundSFXVoice
                {
                    Type = HIRCType.SoundSXFSoundVoice,
                    ID = id,
                    Unk1 = sc.ms.ReadUInt32(),
                    State = (SoundState)sc.ms.ReadUInt32(),
                    AudioID = sc.ms.ReadUInt32(),
                    SourceID = sc.ms.ReadUInt32()
                };
                if (sfxVoice.State != SoundState.Streamed)
                {
                    sfxVoice.UnkType = sc.ms.ReadInt32();
                    sfxVoice.UnkPrefetchLength = sc.ms.ReadInt32();
                }
                sfxVoice.SoundType = (SoundType)sc.ms.ReadByte();
                sfxVoice.unparsed = sc.ms.ReadBytes(len - sfxVoice.ParsedLength);
                return sfxVoice;
            }
        }

        public enum SoundType : byte
        {
            SFX = 0,
            Voice = 1
        }

        public enum SoundState : uint
        {
            Embed = 0,
            Streamed = 1,
            StreamPrefetched = 2
        }

        //public string[] ActionTypes = {"Stop", "Pause", "Resume", "Play", "Trigger", "Mute", "UnMute", "Set Voice Pitch", "Reset Voice Pitch", "Set Voice Volume", "Reset Voice Volume", "Set Bus Volume", "Reset Bus Volume", "Set Voice Low-pass Filter", "Reset Voice Low-pass Filter", "Enable State" , "Disable State", "Set State", "Set Game Parameter", "Reset Game Parameter", "Set Switch", "Enable Bypass or Disable Bypass", "Reset Bypass Effect", "Break", "Seek"};
        //public string[] EventScopes = { "Game object: Switch or Trigger", "Global", "Game object: by ID", "Game object: State", "All", "All Except ID" };
        public class Event : HIRCObject
        {
            public List<uint> EventActions;

            public override int DataLength => 8 + EventActions.Count * 4;

            public override byte[] ToBytes(MEGame game)
            {
                using MemoryStream ms = WriteHIRCObjectHeader(game);
                ms.WriteInt32(EventActions.Count);
                foreach (uint eventAction in EventActions)
                {
                    ms.WriteUInt32(eventAction);
                }
                return ms.ToArray();
            }

            public override HIRCObject Clone()
            {
                Event clone = (Event)MemberwiseClone();
                clone.EventActions = EventActions.Clone();
                return clone;
            }
            public static Event Create(SerializingContainer2 sc, uint id) =>
                new Event
                {
                    Type = HIRCType.Event,
                    ID = id,
                    // Just call .ToList()?
                    EventActions = Enumerable.Range(0, sc.ms.ReadInt32()).Select(i => (uint)sc.ms.ReadUInt32()).ToList()
                };
        }

        public enum EventActionScope : byte
        {
            Global = 0x10,
            GameAction = 0x11
        }

        public enum EventActionType : byte
        {
            Play = 0x40,
            Stop = 0x10
        }

        public class EventAction : HIRCObject
        {
            public EventActionScope Scope;
            public EventActionType ActionType;
            public ushort Unk1;
            public uint ReferencedObjectID;

            public override int DataLength => 12 + unparsed.Length;

            public override byte[] ToBytes(MEGame game)
            {
                using MemoryStream ms = WriteHIRCObjectHeader(game);
                ms.WriteByte((byte)Scope);
                ms.WriteByte((byte)ActionType);
                ms.WriteUInt16(Unk1);
                ms.WriteUInt32(ReferencedObjectID);
                ms.WriteFromBuffer(unparsed);
                return ms.ToArray();
            }
            public static EventAction Create(SerializingContainer2 sc, uint id, int len) =>
                new EventAction
                {
                    Type = HIRCType.EventAction,
                    ID = id,
                    Scope = (EventActionScope)sc.ms.ReadByte(),
                    ActionType = (EventActionType)sc.ms.ReadByte(),
                    Unk1 = sc.ms.ReadUInt16(),
                    ReferencedObjectID = sc.ms.ReadUInt32(),
                    unparsed = sc.ms.ReadBytes(len - 12)
                };
        }
    }

    public enum HIRCType : byte
    {
        Settings = 0x1,
        SoundSXFSoundVoice = 0x2,
        EventAction = 0x3,
        Event = 0x4,
        RandomOrSequenceContainer = 0x5,
        SwitchContainer = 0x6,
        ActorMixer = 0x7,
        AudioBus = 0x8,
        BlendContainer = 0x9,
        MusicSegment = 0xA,
        MusicTrack = 0xB,
        MusicSwitchContainer = 0xC,
        MusicPlaylistContainer = 0xD,
        Attenuation = 0xE,
        DialogueEvent = 0xF,
        MotionBus = 0x10,
        MotionFX = 0x11,
        Effect = 0x12,
        AuxiliaryBus = 0x13
    }

    public class WwiseStateManagement
    {
        public float VolumeThreshold;
        public ushort MaxVoiceInstances;
        public OrderedMultiValueDictionary<uint, StateGroup> StateGroups;
        public OrderedMultiValueDictionary<uint, SwitchGroup> SwitchGroups;
        public OrderedMultiValueDictionary<uint, float> GameParameterDefaultValues;

        public class CustomTransitionTime
        {
            public uint FromStateID;
            public uint ToStateID;
            public uint TransitionTime; //in milliseconds
        }

        public class StateGroup
        {
            public uint ID;
            public uint DefaultTransitionTime;
            public List<CustomTransitionTime> CustomTransitionTimes;
        }

        public class SwitchPoint
        {
            public float GameParamValue;
            public uint SwitchID; //id of Switch  set when Game Parameter >= GameParamValue
            public uint CurveShape; //Always 9? 9 = constant
        }

        public class SwitchGroup
        {
            public uint ID;
            public uint GameParamID;
            public List<SwitchPoint> Points;
        }
    }
}
