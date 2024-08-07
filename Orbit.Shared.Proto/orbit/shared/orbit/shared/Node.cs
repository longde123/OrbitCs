// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: orbit/shared/node.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Orbit.Shared.Proto {

  /// <summary>Holder for reflection information generated from orbit/shared/node.proto</summary>
  public static partial class NodeReflection {

    #region Descriptor
    /// <summary>File descriptor for orbit/shared/node.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static NodeReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChdvcmJpdC9zaGFyZWQvbm9kZS5wcm90bxIMT3JiaXQuU2hhcmVkGh9nb29n",
            "bGUvcHJvdG9idWYvdGltZXN0YW1wLnByb3RvIi0KC05vZGVJZFByb3RvEgsK",
            "A2tleRgBIAEoCRIRCgluYW1lc3BhY2UYAiABKAkihwIKDU5vZGVJbmZvUHJv",
            "dG8SJQoCaWQYASABKAsyGS5PcmJpdC5TaGFyZWQuTm9kZUlkUHJvdG8SNQoM",
            "Y2FwYWJpbGl0aWVzGAIgASgLMh8uT3JiaXQuU2hhcmVkLkNhcGFiaWxpdGll",
            "c1Byb3RvEi8KDHZpc2libGVOb2RlcxgDIAMoCzIZLk9yYml0LlNoYXJlZC5O",
            "b2RlSWRQcm90bxIrCgVsZWFzZRgEIAEoCzIcLk9yYml0LlNoYXJlZC5Ob2Rl",
            "TGVhc2VQcm90bxILCgN1cmwYBSABKAkSLQoGc3RhdHVzGAYgASgOMh0uT3Ji",
            "aXQuU2hhcmVkLk5vZGVTdGF0dXNQcm90byItChFDYXBhYmlsaXRpZXNQcm90",
            "bxIYChBhZGRyZXNzYWJsZVR5cGVzGAEgAygJIocBCg5Ob2RlTGVhc2VQcm90",
            "bxIXCg9jaGFsbGVuZ2VfdG9rZW4YASABKAkSLAoIcmVuZXdfYXQYAiABKAsy",
            "Gi5nb29nbGUucHJvdG9idWYuVGltZXN0YW1wEi4KCmV4cGlyZXNfYXQYAyAB",
            "KAsyGi5nb29nbGUucHJvdG9idWYuVGltZXN0YW1wKkYKD05vZGVTdGF0dXNQ",
            "cm90bxILCgdTVE9QUEVEEAASDAoIU1RBUlRJTkcQARIKCgZBQ1RJVkUQAhIM",
            "CghEUkFJTklORxADQhWqAhJPcmJpdC5TaGFyZWQuUHJvdG9iBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::Google.Protobuf.WellKnownTypes.TimestampReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::Orbit.Shared.Proto.NodeStatusProto), }, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Orbit.Shared.Proto.NodeIdProto), global::Orbit.Shared.Proto.NodeIdProto.Parser, new[]{ "Key", "Namespace" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Orbit.Shared.Proto.NodeInfoProto), global::Orbit.Shared.Proto.NodeInfoProto.Parser, new[]{ "Id", "Capabilities", "VisibleNodes", "Lease", "Url", "Status" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Orbit.Shared.Proto.CapabilitiesProto), global::Orbit.Shared.Proto.CapabilitiesProto.Parser, new[]{ "AddressableTypes" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::Orbit.Shared.Proto.NodeLeaseProto), global::Orbit.Shared.Proto.NodeLeaseProto.Parser, new[]{ "ChallengeToken", "RenewAt", "ExpiresAt" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Enums
  public enum NodeStatusProto {
    [pbr::OriginalName("STOPPED")] Stopped = 0,
    [pbr::OriginalName("STARTING")] Starting = 1,
    [pbr::OriginalName("ACTIVE")] Active = 2,
    [pbr::OriginalName("DRAINING")] Draining = 3,
  }

  #endregion

  #region Messages
  public sealed partial class NodeIdProto : pb::IMessage<NodeIdProto>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<NodeIdProto> _parser = new pb::MessageParser<NodeIdProto>(() => new NodeIdProto());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<NodeIdProto> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Orbit.Shared.Proto.NodeReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NodeIdProto() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NodeIdProto(NodeIdProto other) : this() {
      key_ = other.key_;
      namespace_ = other.namespace_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NodeIdProto Clone() {
      return new NodeIdProto(this);
    }

    /// <summary>Field number for the "key" field.</summary>
    public const int KeyFieldNumber = 1;
    private string key_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Key {
      get { return key_; }
      set {
        key_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "namespace" field.</summary>
    public const int NamespaceFieldNumber = 2;
    private string namespace_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Namespace {
      get { return namespace_; }
      set {
        namespace_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as NodeIdProto);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(NodeIdProto other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Key != other.Key) return false;
      if (Namespace != other.Namespace) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (Key.Length != 0) hash ^= Key.GetHashCode();
      if (Namespace.Length != 0) hash ^= Namespace.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (Key.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Key);
      }
      if (Namespace.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Namespace);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (Key.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Key);
      }
      if (Namespace.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Namespace);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (Key.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Key);
      }
      if (Namespace.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Namespace);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(NodeIdProto other) {
      if (other == null) {
        return;
      }
      if (other.Key.Length != 0) {
        Key = other.Key;
      }
      if (other.Namespace.Length != 0) {
        Namespace = other.Namespace;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Key = input.ReadString();
            break;
          }
          case 18: {
            Namespace = input.ReadString();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            Key = input.ReadString();
            break;
          }
          case 18: {
            Namespace = input.ReadString();
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class NodeInfoProto : pb::IMessage<NodeInfoProto>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<NodeInfoProto> _parser = new pb::MessageParser<NodeInfoProto>(() => new NodeInfoProto());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<NodeInfoProto> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Orbit.Shared.Proto.NodeReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NodeInfoProto() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NodeInfoProto(NodeInfoProto other) : this() {
      id_ = other.id_ != null ? other.id_.Clone() : null;
      capabilities_ = other.capabilities_ != null ? other.capabilities_.Clone() : null;
      visibleNodes_ = other.visibleNodes_.Clone();
      lease_ = other.lease_ != null ? other.lease_.Clone() : null;
      url_ = other.url_;
      status_ = other.status_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NodeInfoProto Clone() {
      return new NodeInfoProto(this);
    }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private global::Orbit.Shared.Proto.NodeIdProto id_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Orbit.Shared.Proto.NodeIdProto Id {
      get { return id_; }
      set {
        id_ = value;
      }
    }

    /// <summary>Field number for the "capabilities" field.</summary>
    public const int CapabilitiesFieldNumber = 2;
    private global::Orbit.Shared.Proto.CapabilitiesProto capabilities_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Orbit.Shared.Proto.CapabilitiesProto Capabilities {
      get { return capabilities_; }
      set {
        capabilities_ = value;
      }
    }

    /// <summary>Field number for the "visibleNodes" field.</summary>
    public const int VisibleNodesFieldNumber = 3;
    private static readonly pb::FieldCodec<global::Orbit.Shared.Proto.NodeIdProto> _repeated_visibleNodes_codec
        = pb::FieldCodec.ForMessage(26, global::Orbit.Shared.Proto.NodeIdProto.Parser);
    private readonly pbc::RepeatedField<global::Orbit.Shared.Proto.NodeIdProto> visibleNodes_ = new pbc::RepeatedField<global::Orbit.Shared.Proto.NodeIdProto>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<global::Orbit.Shared.Proto.NodeIdProto> VisibleNodes {
      get { return visibleNodes_; }
    }

    /// <summary>Field number for the "lease" field.</summary>
    public const int LeaseFieldNumber = 4;
    private global::Orbit.Shared.Proto.NodeLeaseProto lease_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Orbit.Shared.Proto.NodeLeaseProto Lease {
      get { return lease_; }
      set {
        lease_ = value;
      }
    }

    /// <summary>Field number for the "url" field.</summary>
    public const int UrlFieldNumber = 5;
    private string url_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Url {
      get { return url_; }
      set {
        url_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "status" field.</summary>
    public const int StatusFieldNumber = 6;
    private global::Orbit.Shared.Proto.NodeStatusProto status_ = global::Orbit.Shared.Proto.NodeStatusProto.Stopped;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Orbit.Shared.Proto.NodeStatusProto Status {
      get { return status_; }
      set {
        status_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as NodeInfoProto);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(NodeInfoProto other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (!object.Equals(Id, other.Id)) return false;
      if (!object.Equals(Capabilities, other.Capabilities)) return false;
      if(!visibleNodes_.Equals(other.visibleNodes_)) return false;
      if (!object.Equals(Lease, other.Lease)) return false;
      if (Url != other.Url) return false;
      if (Status != other.Status) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (id_ != null) hash ^= Id.GetHashCode();
      if (capabilities_ != null) hash ^= Capabilities.GetHashCode();
      hash ^= visibleNodes_.GetHashCode();
      if (lease_ != null) hash ^= Lease.GetHashCode();
      if (Url.Length != 0) hash ^= Url.GetHashCode();
      if (Status != global::Orbit.Shared.Proto.NodeStatusProto.Stopped) hash ^= Status.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (id_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Id);
      }
      if (capabilities_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Capabilities);
      }
      visibleNodes_.WriteTo(output, _repeated_visibleNodes_codec);
      if (lease_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(Lease);
      }
      if (Url.Length != 0) {
        output.WriteRawTag(42);
        output.WriteString(Url);
      }
      if (Status != global::Orbit.Shared.Proto.NodeStatusProto.Stopped) {
        output.WriteRawTag(48);
        output.WriteEnum((int) Status);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (id_ != null) {
        output.WriteRawTag(10);
        output.WriteMessage(Id);
      }
      if (capabilities_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(Capabilities);
      }
      visibleNodes_.WriteTo(ref output, _repeated_visibleNodes_codec);
      if (lease_ != null) {
        output.WriteRawTag(34);
        output.WriteMessage(Lease);
      }
      if (Url.Length != 0) {
        output.WriteRawTag(42);
        output.WriteString(Url);
      }
      if (Status != global::Orbit.Shared.Proto.NodeStatusProto.Stopped) {
        output.WriteRawTag(48);
        output.WriteEnum((int) Status);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (id_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Id);
      }
      if (capabilities_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Capabilities);
      }
      size += visibleNodes_.CalculateSize(_repeated_visibleNodes_codec);
      if (lease_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(Lease);
      }
      if (Url.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Url);
      }
      if (Status != global::Orbit.Shared.Proto.NodeStatusProto.Stopped) {
        size += 1 + pb::CodedOutputStream.ComputeEnumSize((int) Status);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(NodeInfoProto other) {
      if (other == null) {
        return;
      }
      if (other.id_ != null) {
        if (id_ == null) {
          Id = new global::Orbit.Shared.Proto.NodeIdProto();
        }
        Id.MergeFrom(other.Id);
      }
      if (other.capabilities_ != null) {
        if (capabilities_ == null) {
          Capabilities = new global::Orbit.Shared.Proto.CapabilitiesProto();
        }
        Capabilities.MergeFrom(other.Capabilities);
      }
      visibleNodes_.Add(other.visibleNodes_);
      if (other.lease_ != null) {
        if (lease_ == null) {
          Lease = new global::Orbit.Shared.Proto.NodeLeaseProto();
        }
        Lease.MergeFrom(other.Lease);
      }
      if (other.Url.Length != 0) {
        Url = other.Url;
      }
      if (other.Status != global::Orbit.Shared.Proto.NodeStatusProto.Stopped) {
        Status = other.Status;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            if (id_ == null) {
              Id = new global::Orbit.Shared.Proto.NodeIdProto();
            }
            input.ReadMessage(Id);
            break;
          }
          case 18: {
            if (capabilities_ == null) {
              Capabilities = new global::Orbit.Shared.Proto.CapabilitiesProto();
            }
            input.ReadMessage(Capabilities);
            break;
          }
          case 26: {
            visibleNodes_.AddEntriesFrom(input, _repeated_visibleNodes_codec);
            break;
          }
          case 34: {
            if (lease_ == null) {
              Lease = new global::Orbit.Shared.Proto.NodeLeaseProto();
            }
            input.ReadMessage(Lease);
            break;
          }
          case 42: {
            Url = input.ReadString();
            break;
          }
          case 48: {
            Status = (global::Orbit.Shared.Proto.NodeStatusProto) input.ReadEnum();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            if (id_ == null) {
              Id = new global::Orbit.Shared.Proto.NodeIdProto();
            }
            input.ReadMessage(Id);
            break;
          }
          case 18: {
            if (capabilities_ == null) {
              Capabilities = new global::Orbit.Shared.Proto.CapabilitiesProto();
            }
            input.ReadMessage(Capabilities);
            break;
          }
          case 26: {
            visibleNodes_.AddEntriesFrom(ref input, _repeated_visibleNodes_codec);
            break;
          }
          case 34: {
            if (lease_ == null) {
              Lease = new global::Orbit.Shared.Proto.NodeLeaseProto();
            }
            input.ReadMessage(Lease);
            break;
          }
          case 42: {
            Url = input.ReadString();
            break;
          }
          case 48: {
            Status = (global::Orbit.Shared.Proto.NodeStatusProto) input.ReadEnum();
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class CapabilitiesProto : pb::IMessage<CapabilitiesProto>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<CapabilitiesProto> _parser = new pb::MessageParser<CapabilitiesProto>(() => new CapabilitiesProto());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<CapabilitiesProto> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Orbit.Shared.Proto.NodeReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CapabilitiesProto() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CapabilitiesProto(CapabilitiesProto other) : this() {
      addressableTypes_ = other.addressableTypes_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public CapabilitiesProto Clone() {
      return new CapabilitiesProto(this);
    }

    /// <summary>Field number for the "addressableTypes" field.</summary>
    public const int AddressableTypesFieldNumber = 1;
    private static readonly pb::FieldCodec<string> _repeated_addressableTypes_codec
        = pb::FieldCodec.ForString(10);
    private readonly pbc::RepeatedField<string> addressableTypes_ = new pbc::RepeatedField<string>();
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public pbc::RepeatedField<string> AddressableTypes {
      get { return addressableTypes_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as CapabilitiesProto);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(CapabilitiesProto other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!addressableTypes_.Equals(other.addressableTypes_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= addressableTypes_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      addressableTypes_.WriteTo(output, _repeated_addressableTypes_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      addressableTypes_.WriteTo(ref output, _repeated_addressableTypes_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      size += addressableTypes_.CalculateSize(_repeated_addressableTypes_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(CapabilitiesProto other) {
      if (other == null) {
        return;
      }
      addressableTypes_.Add(other.addressableTypes_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            addressableTypes_.AddEntriesFrom(input, _repeated_addressableTypes_codec);
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            addressableTypes_.AddEntriesFrom(ref input, _repeated_addressableTypes_codec);
            break;
          }
        }
      }
    }
    #endif

  }

  public sealed partial class NodeLeaseProto : pb::IMessage<NodeLeaseProto>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<NodeLeaseProto> _parser = new pb::MessageParser<NodeLeaseProto>(() => new NodeLeaseProto());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<NodeLeaseProto> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Orbit.Shared.Proto.NodeReflection.Descriptor.MessageTypes[3]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NodeLeaseProto() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NodeLeaseProto(NodeLeaseProto other) : this() {
      challengeToken_ = other.challengeToken_;
      renewAt_ = other.renewAt_ != null ? other.renewAt_.Clone() : null;
      expiresAt_ = other.expiresAt_ != null ? other.expiresAt_.Clone() : null;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public NodeLeaseProto Clone() {
      return new NodeLeaseProto(this);
    }

    /// <summary>Field number for the "challenge_token" field.</summary>
    public const int ChallengeTokenFieldNumber = 1;
    private string challengeToken_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string ChallengeToken {
      get { return challengeToken_; }
      set {
        challengeToken_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "renew_at" field.</summary>
    public const int RenewAtFieldNumber = 2;
    private global::Google.Protobuf.WellKnownTypes.Timestamp renewAt_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Google.Protobuf.WellKnownTypes.Timestamp RenewAt {
      get { return renewAt_; }
      set {
        renewAt_ = value;
      }
    }

    /// <summary>Field number for the "expires_at" field.</summary>
    public const int ExpiresAtFieldNumber = 3;
    private global::Google.Protobuf.WellKnownTypes.Timestamp expiresAt_;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public global::Google.Protobuf.WellKnownTypes.Timestamp ExpiresAt {
      get { return expiresAt_; }
      set {
        expiresAt_ = value;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as NodeLeaseProto);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(NodeLeaseProto other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (ChallengeToken != other.ChallengeToken) return false;
      if (!object.Equals(RenewAt, other.RenewAt)) return false;
      if (!object.Equals(ExpiresAt, other.ExpiresAt)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (ChallengeToken.Length != 0) hash ^= ChallengeToken.GetHashCode();
      if (renewAt_ != null) hash ^= RenewAt.GetHashCode();
      if (expiresAt_ != null) hash ^= ExpiresAt.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (ChallengeToken.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(ChallengeToken);
      }
      if (renewAt_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(RenewAt);
      }
      if (expiresAt_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(ExpiresAt);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (ChallengeToken.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(ChallengeToken);
      }
      if (renewAt_ != null) {
        output.WriteRawTag(18);
        output.WriteMessage(RenewAt);
      }
      if (expiresAt_ != null) {
        output.WriteRawTag(26);
        output.WriteMessage(ExpiresAt);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (ChallengeToken.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(ChallengeToken);
      }
      if (renewAt_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(RenewAt);
      }
      if (expiresAt_ != null) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(ExpiresAt);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(NodeLeaseProto other) {
      if (other == null) {
        return;
      }
      if (other.ChallengeToken.Length != 0) {
        ChallengeToken = other.ChallengeToken;
      }
      if (other.renewAt_ != null) {
        if (renewAt_ == null) {
          RenewAt = new global::Google.Protobuf.WellKnownTypes.Timestamp();
        }
        RenewAt.MergeFrom(other.RenewAt);
      }
      if (other.expiresAt_ != null) {
        if (expiresAt_ == null) {
          ExpiresAt = new global::Google.Protobuf.WellKnownTypes.Timestamp();
        }
        ExpiresAt.MergeFrom(other.ExpiresAt);
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            ChallengeToken = input.ReadString();
            break;
          }
          case 18: {
            if (renewAt_ == null) {
              RenewAt = new global::Google.Protobuf.WellKnownTypes.Timestamp();
            }
            input.ReadMessage(RenewAt);
            break;
          }
          case 26: {
            if (expiresAt_ == null) {
              ExpiresAt = new global::Google.Protobuf.WellKnownTypes.Timestamp();
            }
            input.ReadMessage(ExpiresAt);
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            ChallengeToken = input.ReadString();
            break;
          }
          case 18: {
            if (renewAt_ == null) {
              RenewAt = new global::Google.Protobuf.WellKnownTypes.Timestamp();
            }
            input.ReadMessage(RenewAt);
            break;
          }
          case 26: {
            if (expiresAt_ == null) {
              ExpiresAt = new global::Google.Protobuf.WellKnownTypes.Timestamp();
            }
            input.ReadMessage(ExpiresAt);
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code